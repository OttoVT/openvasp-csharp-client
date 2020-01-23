namespace OpenVASP.Messaging.Messages
{
    public abstract class MessageBase
    {
        public MessageType MessageType { get; protected set; }

        public MessageEnvelope MessageEnvelope { get; set; }

        public string Comment { get; set; } = string.Empty;
    }
}
