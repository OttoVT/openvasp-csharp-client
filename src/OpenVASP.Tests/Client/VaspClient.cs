using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OpenVASP.CSharpClient;
using OpenVASP.CSharpClient.Cryptography;
using OpenVASP.Messaging;
using OpenVASP.Messaging.Messages;
using OpenVASP.Messaging.Messages.Entities;
using OpenVASP.Tests.Client.Sessions;

namespace OpenVASP.Tests.Client
{
    //TODO: Add thread safety + state machine
    public class VaspClient : IDisposable
    {
        private readonly IEthereumRpc _ethereumRpc;
        private readonly IWhisperRpc _whisperRpc;
        private VaspContractInfo _vaspContractInfo;
        private CancellationTokenSource _cancellationTokenSource;
        private Task _listener;
        private readonly IEnsProvider _ensProvider;
        private readonly ITransportClient _transportClient;
        private readonly ISignService _signService;

        public VaspInformation VaspInfo { get; private set; }

        public event Func<TransferDispatchMessage, TransferConfirmationMessage> TransferDispatch;

        public event Func<TransferRequestMessage, TransferReplyMessage> TransferRequest;

        public event SessionTermination SessionTerminated;

        public event Action<VaspInformation, VaspSession> SessionCreated;

        public VaspClient(
            IEthereumRpc nodeClientEthereumRpc,
            IWhisperRpc nodeClientWhisperRpc,
            IEnsProvider ensProvider,
            ITransportClient transportClient,
            ISignService signService)
        {
            this._ethereumRpc = nodeClientEthereumRpc;
            this._whisperRpc = nodeClientWhisperRpc;
            this._cancellationTokenSource = new CancellationTokenSource();
            this._ensProvider = ensProvider;
            this._transportClient = transportClient;
            this._signService = signService;
        }

        public void RunListener(IVaspMessageHandler messageHandler)
        {
            var token = _cancellationTokenSource.Token;
            var taskFactory = new TaskFactory(_cancellationTokenSource.Token);

            this._listener = taskFactory.StartNew(async (_) =>
            {
                var privateKeyId = await _whisperRpc.RegisterKeyPairAsync(this.HandshakeKey.PrivateKey);
                string messageFilter = await _whisperRpc.CreateMessageFilterAsync(topic: _vaspContractInfo.VaspCode.Code, privateKeyId);

                do
                {
                    var sessionRequestMessages = await _transportClient.GetSessionMessagesAsync(messageFilter);

                    if (sessionRequestMessages != null &&
                        sessionRequestMessages.Count != 0)
                    {
                        foreach (var message in sessionRequestMessages)
                        {
                            var sessionRequestMessage = message.Message as SessionRequestMessage;

                            if (sessionRequestMessage == null)
                                continue;

                            var isAuthorized = await messageHandler.AuthorizeSessionRequestAsync(sessionRequestMessage.VASP);

                            if (!isAuthorized)
                                continue;

                            var originatorVaspContractInfo = await _ethereumRpc.GetVaspContractInfoAync(sessionRequestMessage.VASP.VaspIdentity);

                            if (!_signService.VerifySign(message.Payload, message.Signature, originatorVaspContractInfo.SigningKey))
                                continue;

                            var sharedSecret = this.HandshakeKey.GenerateSharedSecretHex(sessionRequestMessage.HandShake.EcdhPubKey);

                            var session = new BeneficiarySession(
                                originatorVaspContractInfo,
                                this.VaspInfo,
                                sessionRequestMessage.Message.SessionId,
                                sessionRequestMessage.HandShake.TopicA,
                                originatorVaspContractInfo.SigningKey,
                                sharedSecret,
                                this.SignatureKey,
                                this._whisperRpc,
                                this._ethereumRpc,
                                messageHandler,
                                _transportClient,
                                _signService);

                            session.OnSessionTermination += this.ProcessSessionTermination;

                            if (BeneficiarySessionsDict.TryAdd(session.SessionId, session))
                            {
                                await session.StartAsync();
                            }
                            else
                            {
                                await session.TerminateAsync(TerminationMessage.TerminationMessageCode.SessionClosedTransferDeclinedByBeneficiaryVasp);
                            }
                        }

                        continue;
                    }

                    await Task.Delay(5000, token);
                } while (!token.IsCancellationRequested);
            }, token, TaskCreationOptions.LongRunning);
        }

        public void Wait()
        {
            try
            {
                _listener.Wait();
            }
            catch (Exception e)
            {
            }
        }

        public ConcurrentDictionary<string, BeneficiarySession> BeneficiarySessionsDict { get; } = new ConcurrentDictionary<string, BeneficiarySession>();

        public ConcurrentDictionary<string, OriginatorSession> OriginatorSessionsDict { get; } = new ConcurrentDictionary<string, OriginatorSession>();

        public async Task<OriginatorSession> CreateSessionAsync(
            Originator originator,
            VirtualAssetssAccountNumber originatorVaan,
            VirtualAssetssAccountNumber beneficiaryVaan)
        {
            string counterPartyVaspContractAddress = await _ensProvider.GetContractAddressByVaspCodeAsync(beneficiaryVaan.VaspCode);
            var contractInfo = await _ethereumRpc.GetVaspContractInfoAync(counterPartyVaspContractAddress);
            var sessionKey = ECDH_Key.GenerateKey();
            var sharedKey = sessionKey.GenerateSharedSecretHex(contractInfo.HandshakeKey);

            var session = new OriginatorSession(
                originator,
                this._vaspContractInfo,
                this.VaspInfo,
                originatorVaan,
                beneficiaryVaan,
                contractInfo.SigningKey,
                contractInfo.HandshakeKey,
                sharedKey,
                sessionKey.PublicKey,
                this.SignatureKey,
                _whisperRpc,
                _ethereumRpc,
                _transportClient,
                _signService);

            if (OriginatorSessionsDict.TryAdd(session.SessionId, session))
            {
                session.OnSessionTermination += this.ProcessSessionTermination;
                await session.StartAsync();
                return session;
            }

            //TODO: process it as exception or retry
            return null;
        }

        public static VaspClient Create(
            VaspInformation vaspInfo,
            VaspContractInfo vaspContractInfo,
            string handshakePrivateKeyHex,
            string signaturePrivateKeyHex,
            IEthereumRpc nodeClientEthereumRpc,
            IWhisperRpc nodeClientWhisperRpc,
            IEnsProvider ensProvider,
            ISignService signService,
            ITransportClient transportClient)
        {
            var vaspClient = new VaspClient(
                nodeClientEthereumRpc,
                nodeClientWhisperRpc,
                ensProvider,
                transportClient,
                signService);

            vaspClient._vaspContractInfo = vaspContractInfo;
            vaspClient.VaspInfo = vaspInfo;
            vaspClient.HandshakeKey = ECDH_Key.ImportKey(handshakePrivateKeyHex);
            vaspClient.SignatureKey = signaturePrivateKeyHex;

            return vaspClient;
        }

        public string SignatureKey { get; private set; }

        public ECDH_Key HandshakeKey { get; private set; }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();

            //TODO: Work on session life cycle

            var sessions = BeneficiarySessionsDict.Values.Cast<VaspSession>().Concat(OriginatorSessionsDict.Values);

            foreach (var session in sessions)
            {
                bool isOriginator = session is OriginatorSession;
                var runningSession = session;
                runningSession
                    .TerminateAsync(isOriginator ? TerminationMessage.TerminationMessageCode.SessionClosedTransferCancelledByOriginator
                        : TerminationMessage.TerminationMessageCode.SessionClosedTransferDeclinedByBeneficiaryVasp).Wait();
                runningSession.Wait();
            }

            if (TransferDispatch != null)
                foreach (var d in TransferDispatch.GetInvocationList())
                    TransferDispatch -= (d as Func<TransferDispatchMessage, TransferConfirmationMessage>);

            if (TransferRequest != null)
                foreach (var d in TransferRequest.GetInvocationList())
                    TransferRequest -= (d as Func<TransferRequestMessage, TransferReplyMessage>);

            if (SessionTerminated != null)
                foreach (var d in SessionTerminated.GetInvocationList())
                    SessionTerminated -= (d as SessionTermination);

            if (SessionCreated != null)
                foreach (var d in SessionCreated.GetInvocationList())
                    SessionCreated -= (d as Action<VaspInformation, VaspSession>);
        }

        public IReadOnlyList<VaspSession> GetActiveSessions()
        {
            var beneficiarySessions = BeneficiarySessionsDict.Values.Cast<VaspSession>();
            var originatorSessions = OriginatorSessionsDict.Values.Cast<VaspSession>();

            return beneficiarySessions.Concat(originatorSessions).ToImmutableArray();
        }

        private void ProcessSessionTermination(SessionTerminationEvent @event)
        {
            SessionTerminated?.Invoke(@event);

            string sessionId = @event.SessionId;
            VaspSession vaspSession;

            if (!BeneficiarySessionsDict.TryGetValue(sessionId, out var benSession))
            {
                if (!OriginatorSessionsDict.TryGetValue(sessionId, out var origSession))
                {
                    return;
                }

                vaspSession = origSession;
                OriginatorSessionsDict.TryRemove(sessionId, out _);
            }
            else
            {
                BeneficiarySessionsDict.TryRemove(sessionId, out _);
                vaspSession = benSession;
            }

            //TODO: Work on session life cycle
            try
            {
                vaspSession.Dispose();
            }
            finally
            {
            }
        }
    }
}