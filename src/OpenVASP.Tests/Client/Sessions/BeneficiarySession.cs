using System.Threading.Tasks;
using OpenVASP.CSharpClient;
using OpenVASP.CSharpClient.Cryptography;
using OpenVASP.Messaging;
using OpenVASP.Messaging.Messages;
using OpenVASP.Messaging.Messages.Entities;
using OpenVASP.Messaging.MessagingEngine;

namespace OpenVASP.Tests.Client.Sessions
{
    public class BeneficiarySession : VaspSession
    {
        private readonly IVaspMessageHandler _vaspMessageHandler;
        private readonly IEthereumRpc _ethereumRpc;

        public BeneficiarySession(
            VaspContractInfo beneficiaryVaspContractInfo,
            VaspInformation beneficiaryVasp,
            string sessionId,
            string counterpartyTopic,
            string counterPartyPubSigningKey,
            string sharedKey,
            string privateSigningKey,
            IWhisperRpc whisperRpc,
            IEthereumRpc ethereumRpc,
            IVaspMessageHandler vaspMessageHandler,
            ITransportClient transportClient,
            ISignService signService)
            : base(
                beneficiaryVaspContractInfo,
                beneficiaryVasp,
                counterPartyPubSigningKey, 
                sharedKey, 
                privateSigningKey, 
                whisperRpc,
                transportClient, 
                signService)
        {
            this.SessionId = sessionId;
            this.CounterPartyTopic = counterpartyTopic;
            this._vaspMessageHandler = vaspMessageHandler;
            this._ethereumRpc = ethereumRpc;

            _messageHandlerResolverBuilder.AddHandler(typeof(TransferRequestMessage), new TransferRequestMessageHandler(
                async (message, cancellationToken) =>
                {
                    var reply = await _vaspMessageHandler.TransferRequestHandlerAsync(message, this);

                    await _transportClient.SendAsync(new MessageEnvelope()
                    {
                        Topic = this.CounterPartyTopic,
                        Signature = _privateSigningKey,
                        EncryptionType = EncryptionType.Symmetric,
                        EncryptionKey = _sharedSymKeyId
                    }, reply);
                }));

            _messageHandlerResolverBuilder.AddHandler(typeof(TransferDispatchMessage), new TransferDispatchMessageHandler(
                async (message, cancellationToken) =>
                {
                    var reply = await _vaspMessageHandler.TransferDispatchHandlerAsync(message, this);

                    await _transportClient.SendAsync(new MessageEnvelope()
                    {
                        Topic = this.CounterPartyTopic,
                        Signature = _privateSigningKey,
                        EncryptionType = EncryptionType.Symmetric,
                        EncryptionKey = _sharedSymKeyId
                    }, reply);
                }));

            _messageHandlerResolverBuilder.AddHandler(typeof(TerminationMessage),
                new TerminationMessageHandler(async (message, token) =>
                {
                    _hasReceivedTerminationMessage = true;
                    await TerminateAsync(message.GetMessageCode());
                }));
        }

        public override async Task StartAsync()
        {
            var reply = new SessionReplyMessage(this.SessionId, new HandShakeResponse(this.SessionTopic), this._vaspInfo);
            this.CounterParty.VaspInfo = reply.VASP;
            _sharedSymKeyId = await _whisperRpc.RegisterSymKeyAsync(_sharedKey);

            await _transportClient.SendAsync(new MessageEnvelope()
            {
                EncryptionKey = _sharedSymKeyId,
                EncryptionType = EncryptionType.Symmetric,
                Topic = this.CounterPartyTopic,
                Signature = this._sharedSymKeyId
            }, reply);

            await base.StartAsync();
        }
    }
}