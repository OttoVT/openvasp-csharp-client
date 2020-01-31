using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OpenVASP.CSharpClient;
using OpenVASP.CSharpClient.Cryptography;
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

        public VaspInformation VaspInfo { get; private set; }

        public event Func<TransferDispatchMessage, TransferConfirmationMessage> TransferDispatch;

        public event Func<TransferRequestMessage, TransferReplyMessage> TransferRequest;

        public event Action<VaspInformation> SessionTerminated;

        public event Action<VaspInformation, VaspSession> SessionCreated;

        public VaspClient(
            IEthereumRpc nodeClientEthereumRpc,
            IWhisperRpc nodeClientWhisperRpc)
        {
            this._ethereumRpc = nodeClientEthereumRpc;
            this._whisperRpc = nodeClientWhisperRpc;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public void RunListener(IVaspMessageHandler messageHandler)
        {
            var token = _cancellationTokenSource.Token;

            this._listener = Task.Run(async () =>
            {
                string messageFilter = await _whisperRpc.CreateMessageFilterAsync(topic: _vaspContractInfo.VaspCode.Code);

                do
                {
                    var sessionRequestMessages = await _whisperRpc.GetSessionRequestMessages(messageFilter);

                    if (sessionRequestMessages != null &&
                        sessionRequestMessages.Count != 0)
                    {
                        foreach (var sessionRequestMessage in sessionRequestMessages)
                        {
                            var reply = await messageHandler.SessionRequestHandlerAsync(sessionRequestMessage);

                            var session = new VaspSession()
                            {
                                Counterparty = new VaspSessionCounterparty()
                                {
                                    VaspInfo = reply.VASP,
                                }
                            };
                            session.IsBeneficiary = true;
                        }
                    }

                    await Task.Delay(5000, token);
                } while (!token.IsCancellationRequested);
            }, token);
        }

        public async Task<VaspSession> CreateSessionAsync(VirtualAssetssAccountNumber beneficiaryVaan)
        {
            var session = new VaspSession();

            return session;
        }

        public static VaspClient Create(
            VaspInformation vaspInfo,
            VaspContractInfo vaspContractInfo,
            string handshakePrivateKeyHex,
            string signaturePrivateKeyHex,
            IEthereumRpc nodeClientEthereumRpc,
            IWhisperRpc nodeClientWhisperRpc)
        {
            var vaspClient = new VaspClient(nodeClientEthereumRpc, nodeClientWhisperRpc);

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
            throw new NotImplementedException();
        }
    }
}