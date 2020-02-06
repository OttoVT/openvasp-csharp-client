using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.ObjectModel;
using OpenVASP.Messaging;
using OpenVASP.Messaging.Messages;

namespace OpenVASP.Tests.Client
{
    public class TransportClient : ITransportClient
    {
        private readonly IMessageFormatter _messageFormatter;
        private readonly IWhisperRpc _whisper;
        private readonly ISignService _signService;

        public TransportClient(IWhisperRpc whisper, ISignService signService, IMessageFormatter messageFormatter)
        {
            _whisper = whisper;
            _signService = signService;
            _messageFormatter = messageFormatter;
        }

        public async Task<string> SendAsync(MessageEnvelope messageEnvelope, MessageBase message)
        {
            var payload = _messageFormatter.GetPayload(message);
            var sign = _signService.SignPayload(payload, messageEnvelope.Signature);

            return await _whisper.SendMessageAsync(
                messageEnvelope.Topic, 
                messageEnvelope.EncryptionKey,
                messageEnvelope.EncryptionType,
                payload + sign);
        }

        public async Task<IReadOnlyCollection<(MessageBase Message, string Payload, string Signature)>> GetSessionMessagesAsync(string messageFilter)
        {
            var messages = await _whisper.GetMessagesAsync(messageFilter);

            if (messages == null || messages.Count == 0)
            {
                return new (MessageBase, string, string)[] { };
            }

            var serializedMessages = messages.Select(x =>
            {
                var payload = x.Payload.Substring(0, x.Payload.Length - 130);
                var sign = x.Payload.Substring(x.Payload.Length - 130, 130);
                x.MessageEnvelope.Signature = sign;
                var message = _messageFormatter.Deserialize(payload, x.MessageEnvelope);

                return (message, payload, sign);
            }).ToArray();

            return serializedMessages;
        }
    }
}