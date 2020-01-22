namespace OpenVASP.Messaging
{
    public class MessageEnvelope
    {
        public string Topic { get; set; }
        public string Signature { get; set; }
        public EncryptionType EncryptionType { get; set; }
        public string EncryptionKey { get; set; }
    }
}