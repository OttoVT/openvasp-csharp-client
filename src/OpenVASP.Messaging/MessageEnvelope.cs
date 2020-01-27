using OpenVASP.Messaging.Messages.Entities;

namespace OpenVASP.Messaging
{
    public class MessageEnvelope
    {
        public MessageEnvelope(string topic, EncryptionType encryptionType, string encryptionKey)
        {
            Topic = topic;
            EncryptionType = encryptionType;
            EncryptionKey = encryptionKey;
        }

        public string Signature { get; set; }
        public string Topic { get; }
        public EncryptionType EncryptionType { get; }
        public string EncryptionKey { get; }
    }

    public class MessageEnvelope<T> : MessageEnvelope
    {
        public MessageEnvelope(string topic, EncryptionType encryptionType, string encryptionKey, T message) 
            : base(topic, encryptionType, encryptionKey)
        {
            Message = message;
        }
        public T Message { get; }
    }
}