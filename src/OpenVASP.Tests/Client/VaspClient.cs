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

namespace OpenVASP.Tests.Client
{
    public class VaspClient : IDisposable
    {
        private readonly IEthereumRpc _ethereumRpc;
        private readonly IWhisperRpc _whisperRpc;
        private VaspContractInfo _vaspContractInfo;
        private CancellationTokenSource _cancellationTokenSource;
        private Task _listener;
        private readonly MessageHandlerResolverBuilder _messageHandlerResolverBuilder;
        private readonly IEnsProvider _ensProvider;

        public VaspInformation VaspInfo { get; private set; }

        public event Func<TransferDispatchMessage, TransferConfirmationMessage> TransferDispatch;

        public event Func<TransferRequestMessage, TransferReplyMessage> TransferRequest;

        public event Action<VaspInformation> SessionTerminated;

        public event Action<VaspInformation, VaspSession> SessionCreated;

        public VaspClient(
            IEthereumRpc nodeClientEthereumRpc,
            IWhisperRpc nodeClientWhisperRpc,
            IEnsProvider ensProvider,
            MessageHandlerResolverBuilder messageHandlerResolverBuilder)
        {
            this._ethereumRpc = nodeClientEthereumRpc;
            this._whisperRpc = nodeClientWhisperRpc;
            this._cancellationTokenSource = new CancellationTokenSource();
            this._messageHandlerResolverBuilder = messageHandlerResolverBuilder;
            this._ensProvider = ensProvider;
        }

        public void RunListener(IVaspMessageHandler messageHandler)
        {
            var token = _cancellationTokenSource.Token;

            this._listener = Task.Run(async () =>
            {
                var privateKeyId = await _whisperRpc.RegisterKeyPairAsync(this.HandshakeKey.PrivateKey);
                string messageFilter = await _whisperRpc.CreateMessageFilterAsync(topic: _vaspContractInfo.VaspCode.Code, privateKeyId);

                do
                {
                    var sessionRequestMessages = await _whisperRpc.GetSessionRequestMessages(messageFilter);

                    if (sessionRequestMessages != null &&
                        sessionRequestMessages.Count != 0)
                    {
                        foreach (var sessionRequestMessage in sessionRequestMessages)
                        {
                            var reply = await messageHandler.SessionRequestHandlerAsync(sessionRequestMessage);
                            var sharedKey = this.HandshakeKey.GenerateSharedSecretHex(sessionRequestMessage.HandShake.EcdhPubKey);
                            var originatorVaspContractInfo = await _ethereumRpc.GetVaspContractInfoAync(sessionRequestMessage.VASP.VaspIdentity);

                            var session = new BeneficiarySession(
                                sessionRequestMessage.Message.SessionId,
                                sessionRequestMessage.HandShake.TopicA,
                                originatorVaspContractInfo.SigningKey,
                                sharedKey,
                                this.SignatureKey,
                                this._whisperRpc,
                                this._messageHandlerResolverBuilder
                                )
                            {
                                CounterParty = new VaspSessionCounterparty()
                                {
                                    VaspInfo = reply.VASP,
                                    // ?? Vaan = 
                                }
                            };

                            if (BeneficiarySessionsDict.TryAdd(session.SessionId, session))
                            {
                                var sharedSymKeyId = await _whisperRpc.RegisterSymKeyAsync(sharedKey);

                                await _whisperRpc.SendMessageAsync(new MessageEnvelope()
                                {
                                    EncryptionKey = sharedSymKeyId,
                                    EncryptionType = EncryptionType.Symmetric,
                                    Topic = sessionRequestMessage.HandShake.TopicA,
                                    Signature = this.SignatureKey
                                }, reply);

                                await session.StartAsync();
                            }
                            else
                            {
                                await session.TerminateAsync();
                            }
                        }

                        continue;
                    }

                    await Task.Delay(5000, token);
                } while (!token.IsCancellationRequested);
            }, token);
        }

        public ConcurrentDictionary<string, BeneficiarySession> BeneficiarySessionsDict { get; } = new ConcurrentDictionary<string, BeneficiarySession>();

        public ConcurrentDictionary<string, OriginatorSession> OriginatorSessionsDict { get; } = new ConcurrentDictionary<string, OriginatorSession>();

        public async Task<OriginatorSession> CreateSessionAsync(VirtualAssetssAccountNumber beneficiaryVaan)
        {
            string counterPartyVaspContractAddress = await _ensProvider.GetContractAddressByVaspCodeAsync(beneficiaryVaan.VaspCode);
            var contractInfo = await _ethereumRpc.GetVaspContractInfoAync(counterPartyVaspContractAddress);
            var sessionX25519Key = X25519Key.GenerateKey();
            var sharedKey = sessionX25519Key.GenerateSharedSecretHex(contractInfo.HandshakeKey);
            
            var session = new OriginatorSession(
                this.VaspInfo,
                beneficiaryVaan,
                contractInfo.SigningKey,
                contractInfo.HandshakeKey,
                sharedKey,
                sessionX25519Key.PublicKey,
                this.SignatureKey,
                _whisperRpc,
                _ethereumRpc,
                _messageHandlerResolverBuilder);

            if (OriginatorSessionsDict.TryAdd(session.SessionId, session))
            {
                await session.StartAsync();
                return session;
            }

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
            MessageHandlerResolverBuilder messageHandlerResolverBuilder)
        {
            var vaspClient = new VaspClient(nodeClientEthereumRpc, nodeClientWhisperRpc, ensProvider, messageHandlerResolverBuilder);

            vaspClient._vaspContractInfo = vaspContractInfo;
            vaspClient.VaspInfo = vaspInfo;
            vaspClient.HandshakeKey = X25519Key.ImportKey(handshakePrivateKeyHex);
            vaspClient.SignatureKey = signaturePrivateKeyHex;

            return vaspClient;
        }

        public string SignatureKey { get; private set; }

        public X25519Key HandshakeKey { get; private set; }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();

            if (TransferDispatch != null)
                foreach (var d in TransferDispatch.GetInvocationList())
                    TransferDispatch -= (d as Func<TransferDispatchMessage, TransferConfirmationMessage>);

            if (TransferRequest != null)
                foreach (var d in TransferRequest.GetInvocationList())
                    TransferRequest -= (d as Func<TransferRequestMessage, TransferReplyMessage>);

            if (SessionTerminated != null)
                foreach (var d in SessionTerminated.GetInvocationList())
                    SessionTerminated -= (d as Action<VaspInformation>);

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
    }
}