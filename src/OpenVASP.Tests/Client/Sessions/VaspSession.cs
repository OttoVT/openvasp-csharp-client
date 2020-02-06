using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OpenVASP.CSharpClient;
using OpenVASP.Messaging;
using OpenVASP.Messaging.Messages;
using OpenVASP.Messaging.Messages.Entities;

namespace OpenVASP.Tests.Client.Sessions
{
    //TODO: Add thread safety + state machine
    public abstract class VaspSession : IDisposable
    {
        // ReSharper disable once NotAccessedField.Global
        protected Task _task;
        protected readonly IWhisperRpc _whisperRpc;
        protected readonly CancellationTokenSource _cancellationTokenSource;
        protected readonly string _sessionTopic;
        protected readonly string _sharedKey;
        protected readonly string _privateSigningKey;
        protected readonly string _counterPartyPubSigningKey;
        protected readonly MessageHandlerResolverBuilder _messageHandlerResolverBuilder;
        protected readonly VaspInformation _vaspInfo;
        protected readonly VaspContractInfo _vaspContractInfo;

        private readonly object _lock = new object();

        protected bool _hasReceivedTerminationMessage = false;
        private bool _isActivated;
        protected string _sharedSymKeyId;
        protected readonly ITransportClient _transportClient;
        protected readonly ISignService _signService;

        public event SessionTermination OnSessionTermination;

        public VaspSession(
            VaspContractInfo vaspContractInfo,
            VaspInformation vaspInfo,
            string counterPartyPubSigningKey,
            string sharedEncryptionKey,
            string privateSigningKey,
            IWhisperRpc whisperRpc,
            ITransportClient transportClient,
            ISignService signService)
        {
            this._vaspInfo = vaspInfo;
            this._vaspContractInfo = vaspContractInfo;
            this._sessionTopic = TopicGenerator.GenerateSessionTopic();
            this._cancellationTokenSource = new CancellationTokenSource();
            this._whisperRpc = whisperRpc;
            this._sharedKey = sharedEncryptionKey;
            this._privateSigningKey = privateSigningKey;
            this._counterPartyPubSigningKey = counterPartyPubSigningKey;
            this._messageHandlerResolverBuilder = new MessageHandlerResolverBuilder();
            this._transportClient = transportClient;
            this._signService = signService;
        }

        public string SessionId { get; protected set; }

        public string CounterPartyTopic { get; protected set; }
        public VaspSessionCounterparty CounterParty { get; protected set; } = new VaspSessionCounterparty();

        public string SessionTopic
        {
            get { return _sessionTopic; }
        }

        protected void StartTopicMonitoring()
        {
            lock (_lock)
            {
                if (!this._isActivated)
                {
                    var taskFactory = new TaskFactory(_cancellationTokenSource.Token);
                    this._isActivated = true;
                    var cancellationToken = _cancellationTokenSource.Token;

                    _task = taskFactory.StartNew(async (_) =>
                    {
                        _sharedSymKeyId = await _whisperRpc.RegisterSymKeyAsync(_sharedKey);
                        string messageFilter = await _whisperRpc.CreateMessageFilterAsync(topic: _sessionTopic, symKeyId: _sharedSymKeyId);
                        var messageHandlerResolver = _messageHandlerResolverBuilder.Build();

                        do
                        {
                            var messages = await _transportClient.GetSessionMessagesAsync(messageFilter);

                            if (messages != null &&
                                messages.Count != 0)
                            {
                                List<Task> currentlyProcessing = new List<Task>(messages.Count);

                                foreach (var message in messages)
                                {
                                    if (!_signService.VerifySign(message.Payload, message.Signature,
                                        _counterPartyPubSigningKey))
                                        continue;

                                    var handlers = messageHandlerResolver.ResolveMessageHandlers(message.Message.GetType());

                                    if (handlers == null || handlers.Length == 0)
                                        continue;

                                    var tasks = handlers.Select(handler =>
                                        handler.HandleMessageAsync(message.Message, cancellationToken));
                                    currentlyProcessing.AddRange(tasks);
                                }

                                Task.WaitAll(currentlyProcessing.ToArray(), cancellationToken);

                                continue;
                            }

                            //Poll whisper each 5 sec for new messages
                            await Task.Delay(5000, cancellationToken);

                        } while (!cancellationToken.IsCancellationRequested);
                    }, cancellationToken, TaskCreationOptions.LongRunning);
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

        public void Wait()
        {
            try
            {
                _task.Wait();
            }
            catch (Exception e)
            {
            }
        }


        public virtual async Task TerminateAsync(TerminationMessage.TerminationMessageCode terminationMessageCode)
        {
            if (!_hasReceivedTerminationMessage)
            {
                await TerminateStrategyAsync(terminationMessageCode);
            }

            NotifyAboutTermination();
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();

            if (OnSessionTermination != null)
            {
                var invocationList = OnSessionTermination?.GetInvocationList();
                
                foreach (var item in invocationList)
                {
                    OnSessionTermination -= (SessionTermination)item;
                }
            }

            _task?.Dispose();
            _cancellationTokenSource?.Dispose();
        }

        protected virtual async Task TerminateStrategyAsync(TerminationMessage.TerminationMessageCode terminationMessageCode)
        {
            var terminationMessage = new TerminationMessage(
                this.SessionId,
                terminationMessageCode,
                _vaspInfo);

            await _transportClient.SendAsync(new MessageEnvelope()
            {
                Topic = this.CounterPartyTopic,
                SigningKey = _privateSigningKey,
                EncryptionType = EncryptionType.Symmetric,
                EncryptionKey = _sharedSymKeyId
            }, terminationMessage);
        }

        private void NotifyAboutTermination()
        {
            var @event = new SessionTerminationEvent(this.SessionId);

            OnSessionTermination?.Invoke(@event);
        }
    }
}