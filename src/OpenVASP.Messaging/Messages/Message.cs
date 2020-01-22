namespace OpenVASP.Messaging.Messages
{
    public class Message
    {
        public Message(MessageType messageType, string messageId, string sessionId, string messageCode)
        {
            MessageType = messageType;
            MessageId = messageId;
            SessionId = sessionId;
            MessageCode = messageCode;
        }

        public MessageType MessageType { get; private set; }

        public string MessageId { get; private set; }

        public string SessionId { get; private set; }

        public string MessageCode { get; private set; }
    }
}