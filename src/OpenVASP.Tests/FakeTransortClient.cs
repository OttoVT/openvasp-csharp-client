using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenVASP.Messaging;
using OpenVASP.Messaging.Messages;
using OpenVASP.Messaging.Messages.Entities;
using OpenVASP.Tests.Client;

namespace OpenVASP.Tests
{

    public class TransportMessage
    {
        private TransportMessage(MessageBase message, string payload, string signature)
        {
            this.Message = message;
            this.Payload = payload;
            this.Signature = signature;
        }

        public string Payload { get; }

        public string Signature { get; }

        public MessageBase Message { get; }

        public static TransportMessage CreateMessage(MessageBase messageBase, string payload, string signature)
        {
            var message = new TransportMessage(messageBase, payload, signature);

            return message;
        }
    }

    public class FakeTransportClient : ITransportClient
    {
        private readonly ConcurrentQueue<TransportMessage> _queue =
            new ConcurrentQueue<TransportMessage>();
        private readonly IMessageFormatter _messageFormatter;
        private readonly ISignService _signService;

        public FakeTransportClient(IMessageFormatter messageFormatter, ISignService signService)
        {
            this._messageFormatter = messageFormatter;
            this._signService = signService;
        }
        
        public Task<string> SendAsync(MessageEnvelope messageEnvelope, MessageBase message)
        {
            var payload = _messageFormatter.GetPayload(message);
            var sign = _signService.SignPayload(payload, messageEnvelope.SigningKey);

            _queue.Enqueue(TransportMessage.CreateMessage(message, payload, sign));

            return Task.FromResult(_queue.Count.ToString());
        }

        public Task<IReadOnlyCollection<TransportMessage>> GetSessionMessagesAsync(string messageFilter)
        {
            var messages = new List<TransportMessage>();
            while (_queue.TryDequeue(out var received))
            {
                messages.Add(received);
            }

            var result = messages.ToArray();

            return Task.FromResult((IReadOnlyCollection<TransportMessage>)result);
        }
    }
}