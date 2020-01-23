using System.Threading.Tasks;
using OpenVASP.Messaging.Messages;

namespace OpenVASP.Messaging
{
    public class MessageSender
    {
        private readonly IMessageFormatter messageFormatter;
        private readonly ITransportClient transportClient;

        public MessageSender(
            IMessageFormatter messageFormatter, 
            ITransportClient transportClient)
        {
            this.messageFormatter = messageFormatter;
            this.transportClient = transportClient;
        }

        public async Task<string> SendMessageAsync(MessageBase message)
        {
            var payload = messageFormatter.GetPayload(message);
            var messageEnvelope = messageFormatter.GetEnvelope(message);
            var messageId = await transportClient.SendMessageAsync(messageEnvelope, payload);

            return messageId;
        }

        public async Task<string> SendSessionRequestAsync(SessionRequestMessage sessionRequestMessage)
        {
            var payload = messageFormatter.GetPayload(sessionRequestMessage);
            var messageEnvelope = messageFormatter.GetEnvelope(sessionRequestMessage);
            var messageId = await transportClient.SendMessageAsync(messageEnvelope, payload);

            return messageId;
        }
    }
}