namespace OpenVASP.Messaging.Messages
{
    public abstract class MessageBase
    {
        public MessageEnvelope MessageEnvelope { get; set; }

        public string Comment { get; set; }
    }
}
