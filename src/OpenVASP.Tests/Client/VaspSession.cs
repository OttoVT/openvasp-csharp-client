using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OpenVASP.Messaging;
using OpenVASP.Messaging.Messages;
using OpenVASP.Messaging.Messages.Entities;
using OpenVASP.Messaging.MessagingEngine;

namespace OpenVASP.Tests.Client
{
    public abstract class VaspSession
    {
        // ReSharper disable once NotAccessedField.Global
        protected Task _task;
        protected readonly IWhisperRpc _whisperRpc;
        protected readonly MessageHandlerResolverBuilder _messageHandlerResolverBuilder;
        protected readonly CancellationTokenSource _cancellationTokenSource;
        protected readonly string _sessionTopic;
        protected readonly string _sharedKey;
        protected readonly string _privateSigningKey;
        protected readonly string _counterPartyPubSigningKey;
        private readonly object _lock = new object();

        private bool _isActivated;
        private string _sharedSymKeyId;

        public VaspSession(
            string counterPartyPubSigningKey,
            string sharedEncryptionKey,
            string privateSigningKey,
            IWhisperRpc whisperRpc,
            MessageHandlerResolverBuilder messageHandlerResolverBuilder)
        {
            this._sessionTopic = TopicGenerator.GenerateSessionTopic();
            this._cancellationTokenSource = new CancellationTokenSource();
            this._whisperRpc = whisperRpc;
            this._messageHandlerResolverBuilder = messageHandlerResolverBuilder;
            this._sharedKey = sharedEncryptionKey;
            this._privateSigningKey = privateSigningKey;
            this._counterPartyPubSigningKey = counterPartyPubSigningKey;
        }

        public string SessionId { get; protected set; }

        public string CounterPartyTopic { get; protected set; }
        public VaspSessionCounterparty CounterParty { get; set; }

        protected void StartTopicMonitoring()
        {
            lock (_lock)
            {
                if (!this._isActivated)
                {
                    this._isActivated = true;
                    var cancellationToken = _cancellationTokenSource.Token;

                    _task = Task.Run(async () =>
                    {
                        _sharedSymKeyId = await _whisperRpc.RegisterSymKeyAsync(_sharedKey);
                        string messageFilter = await _whisperRpc.CreateMessageFilterAsync(topic: _sessionTopic, symKeyId: _sharedSymKeyId);
                        var messageHandlerResolver = _messageHandlerResolverBuilder.Build();

                        do
                        {
                            var messages = await _whisperRpc.GetSessionMessagesAsync(messageFilter);

                            if (messages != null &&
                                messages.Count != 0)
                            {
                                List<Task> currentlyProcessing = new List<Task>(messages.Count);

                                foreach (var message in messages)
                                {
                                    var handlers = messageHandlerResolver.ResolveMessageHandlers(message.GetType());
                                    var tasks = handlers.Select(handler =>
                                        handler.HandleMessageAsync(message, cancellationToken));
                                    currentlyProcessing.AddRange(tasks);
                                }

                                Task.WaitAll(currentlyProcessing.ToArray(), cancellationToken);

                                continue;
                            }

                            //Poll whisper each 5 sec for new messages
                            await Task.Delay(5000, cancellationToken);

                        } while (!cancellationToken.IsCancellationRequested);
                    }, cancellationToken);
                }
                else
                {
                    throw new Exception("Session was already started");
                }
            }
        }

        public virtual Task StartAsync()
        {
            StartTopicMonitoring();

            return Task.CompletedTask;
        }

        public abstract Task TerminateAsync();
    }

    public class OriginatorSession : VaspSession
    {
        private readonly VirtualAssetssAccountNumber _vaan;
        private readonly IEthereumRpc _ethereumRpc;
        private string _beneficiaryPubHandshakeKey;
        private string _pubEncryptionKey;
        private readonly VaspInformation _originatorVasp;

        public OriginatorSession(
            VaspInformation originatorVasp,
            VirtualAssetssAccountNumber vaan,
            string beneficiaryPubSigningKey,
            string beneficiaryPubHandshakeKey,
            string sharedEncryptionKey,
            string pubEncryptionKey,
            string privateSigningKey,
            IWhisperRpc whisperRpc,
            IEthereumRpc ethereumRpc,
            MessageHandlerResolverBuilder messageHandlerResolverBuilder)
            : base(beneficiaryPubSigningKey, sharedEncryptionKey, privateSigningKey, whisperRpc, messageHandlerResolverBuilder)
        {
            this._originatorVasp = originatorVasp;
            this._vaan = vaan;
            this.SessionId = Guid.NewGuid().ToString();
            this._ethereumRpc = ethereumRpc;
            this._beneficiaryPubHandshakeKey = beneficiaryPubHandshakeKey;
            this._pubEncryptionKey = pubEncryptionKey;
            messageHandlerResolverBuilder.AddHandler(typeof(SessionReplyMessage),
                new SessionReplyMessageHandler((sessionReplyMessage, token) =>
            {
                //TODO: Set once
                this.CounterPartyTopic = sessionReplyMessage.HandShake.TopicB;

                return Task.CompletedTask;
            }));
        }

        public override async Task StartAsync()
        {
            await base.StartAsync();

            var sessionRequestMessage = new SessionRequestMessage(
                this.SessionId,
                new HandShakeRequest(this._sessionTopic, this._pubEncryptionKey),
                this._originatorVasp);

            await _whisperRpc.SendMessageAsync(new MessageEnvelope()
            {
                Topic = _vaan.VaspCode.Code,
                Signature = _privateSigningKey,
                EncryptionType = EncryptionType.Assymetric,
                EncryptionKey = _beneficiaryPubHandshakeKey
            }, sessionRequestMessage);
        }

        public async Task<TransferReplyMessage> TransferRequestAsync()
        {
            throw new NotImplementedException();
        }
        public async Task<TransferConfirmationMessage> TransferDispatchAsync()
        {
            throw new NotImplementedException();
        }

        public override async Task TerminateAsync()
        {
            var terminationMessage = new TerminationMessage(
                this.SessionId,
                TerminationMessage.TerminationMessageCode.SessionClosedTransferCancelledByOriginator,
                this.CounterParty.VaspInfo);

            await _whisperRpc.SendMessageAsync(new MessageEnvelope()
            {
                Topic = this.CounterPartyTopic,
                Signature = _privateSigningKey,
                EncryptionType = EncryptionType.Symmetric,
                EncryptionKey = _sharedKey
            }, terminationMessage);

            this._cancellationTokenSource.Cancel();
        }
    }

    public class BeneficiarySession : VaspSession
    {
        public BeneficiarySession(
            string sessionId,
            string counterpartyTopic,
            string counterPartyPubSigningKey,
            string sharedEncryptionKey,
            string privateSigningKey,
            IWhisperRpc whisperRpc,
            MessageHandlerResolverBuilder messageHandlerResolverBuilder)
            : base(counterPartyPubSigningKey, sharedEncryptionKey, privateSigningKey, whisperRpc, messageHandlerResolverBuilder)
        {
            this.SessionId = sessionId;
            this.CounterPartyTopic = counterpartyTopic;
        }

        public override async Task TerminateAsync()
        {
            var terminationMessage = new TerminationMessage(
                this.SessionId,
                TerminationMessage.TerminationMessageCode.SessionClosedTransferCancelledByOriginator,
                this.CounterParty.VaspInfo);

            await _whisperRpc.SendMessageAsync(new MessageEnvelope()
            {
                Topic = this.CounterPartyTopic,
                Signature = _privateSigningKey,
                EncryptionType = EncryptionType.Symmetric,
                EncryptionKey = _sharedKey
            }, terminationMessage);

            this._cancellationTokenSource.Cancel();
        }
    }
}