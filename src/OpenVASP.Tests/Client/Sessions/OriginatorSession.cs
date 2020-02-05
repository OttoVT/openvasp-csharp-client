using System;
using System.Threading.Tasks;
using OpenVASP.CSharpClient;
using OpenVASP.Messaging;
using OpenVASP.Messaging.Messages;
using OpenVASP.Messaging.Messages.Entities;
using OpenVASP.Messaging.MessagingEngine;

namespace OpenVASP.Tests.Client.Sessions
{
    public class OriginatorSession : VaspSession
    {
        private readonly VirtualAssetssAccountNumber _beneficiaryVaan;
        private readonly IEthereumRpc _ethereumRpc;
        private readonly string _beneficiaryPubHandshakeKey;
        private readonly string _pubEncryptionKey;
        private readonly VirtualAssetssAccountNumber _originatorVaan;
        private readonly string _clientName;
        private readonly IEnsProvider _ensProvider;

        private TaskCompletionSource<SessionReplyMessage> _sessionReplyCompletionSource = new TaskCompletionSource<SessionReplyMessage>();
        private TaskCompletionSource<TransferReplyMessage> _transferReplyCompletionSource = new TaskCompletionSource<TransferReplyMessage>();
        private TaskCompletionSource<TransferConfirmationMessage> _transferConfirmationCompletionSource = new TaskCompletionSource<TransferConfirmationMessage>();


        public OriginatorSession(
            string clientName,
            VaspContractInfo originatorVaspContractInfo,
            VaspInformation originatorVasp,
            VirtualAssetssAccountNumber originatorVaan,
            VirtualAssetssAccountNumber beneficiaryVaan,
            string beneficiaryPubSigningKey,
            string beneficiaryPubHandshakeKey,
            string sharedEncryptionKey,
            string pubEncryptionKey,
            string privateSigningKey,
            IWhisperRpc whisperRpc,
            IEthereumRpc ethereumRpc,
            ITransportClient transportClient,
            ISignService signService)
            //IEnsProvider ensProvider)
            : base(
                originatorVaspContractInfo, 
                originatorVasp, 
                beneficiaryPubSigningKey, 
                sharedEncryptionKey, 
                privateSigningKey, 
                whisperRpc, 
                transportClient, 
                signService)
        {
            this._clientName = clientName;
            this._originatorVaan = originatorVaan;
            this._beneficiaryVaan = beneficiaryVaan;
            this.SessionId = Guid.NewGuid().ToString();
            this._ethereumRpc = ethereumRpc;
            this._beneficiaryPubHandshakeKey = beneficiaryPubHandshakeKey;
            this._pubEncryptionKey = pubEncryptionKey;
            //this._ensProvider = ensProvider;

            _messageHandlerResolverBuilder.AddHandler(typeof(SessionReplyMessage),
                new SessionReplyMessageHandler((sessionReplyMessage, token) =>
                {
                    //TODO: Set once
                    this.CounterPartyTopic = sessionReplyMessage.HandShake.TopicB;

                    _sessionReplyCompletionSource.SetResult(sessionReplyMessage);

                    return Task.CompletedTask;
                }));

            _messageHandlerResolverBuilder.AddHandler(typeof(TransferReplyMessage),
                new TransferReplyMessageHandler((transferReplyMessage, token) =>
                {
                    _transferReplyCompletionSource.TrySetResult(transferReplyMessage);

                    return Task.CompletedTask;
                }));

            _messageHandlerResolverBuilder.AddHandler(typeof(TransferConfirmationMessage),
                new TransferConfirmationMessageHandler(async (transferDispatchMessage, token) =>
                {
                    _transferConfirmationCompletionSource.TrySetResult(transferDispatchMessage);
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
            await base.StartAsync();

            //string beneficiaryVaspContractAddress = await _ensProvider.GetContractAddressByVaspCodeAsync(_beneficiaryVaan.VaspCode);
            //await _ethereumRpc.GetVaspContractInfoAync()

            var sessionRequestMessage = new SessionRequestMessage(
                this.SessionId,
                new HandShakeRequest(this._sessionTopic, this._pubEncryptionKey),
                this._vaspInfo);

            await _transportClient.SendAsync(new MessageEnvelope()
            {
                Topic = _beneficiaryVaan.VaspCode.Code,
                Signature = _privateSigningKey,
                EncryptionType = EncryptionType.Assymetric,
                EncryptionKey = _beneficiaryPubHandshakeKey
            }, sessionRequestMessage);

            await _sessionReplyCompletionSource.Task;
        }

        public async Task<TransferReplyMessage> TransferRequestAsync(TransferInstruction instruction)
        {
            var transferRequest = new TransferRequestMessage(
                this.SessionId,
                new Originator(
                    _vaspContractInfo.Name,
                    _originatorVaan.Vaan,
                    _vaspContractInfo.Address,
                    null,
                    null,
                    null,
                    null
                ),
                new Beneficiary(_clientName, _beneficiaryVaan.Vaan), 
                new TransferRequest(
                    instruction.VirtualAssetTransfer.VirtualAssetType, 
                    instruction.VirtualAssetTransfer.TransferType, 
                    instruction.VirtualAssetTransfer.TransferAmount), 
                _vaspInfo
            );

            await _transportClient.SendAsync(new MessageEnvelope()
            {
                Topic = this.CounterPartyTopic,
                Signature = _privateSigningKey,
                EncryptionType = EncryptionType.Symmetric,
                EncryptionKey = _sharedSymKeyId
            }, transferRequest);

            return await _transferReplyCompletionSource.Task;
        }
        public async Task<TransferConfirmationMessage> TransferDispatchAsync(TransferReply transferReply, Transaction transaction)
        {
            var transferRequest = new TransferDispatchMessage(
                this.SessionId,
                new Originator(
                    _vaspContractInfo.Name,
                    _originatorVaan.Vaan,
                    _vaspContractInfo.Address,
                    null,
                    null,
                    null,
                    null
                ),
                new Beneficiary(_clientName, _beneficiaryVaan.Vaan),
                transferReply,
                transaction,
                _vaspInfo
            );

            await _transportClient.SendAsync(new MessageEnvelope()
            {
                Topic = this.CounterPartyTopic,
                Signature = _privateSigningKey,
                EncryptionType = EncryptionType.Symmetric,
                EncryptionKey = _sharedSymKeyId
            }, transferRequest);

            return await _transferConfirmationCompletionSource.Task;
        }
    }
}