using OpenVASP.Messaging.Messages;

namespace OpenVASP.Messaging
{
    public interface IMessageFormatter
    {
        MessageEnvelope GetEnvelope(MessageBase messageBase);

        string GetPayload(SessionRequestMessage sessionRequestMessage);

        MessageBase Deserialize(string payload, MessageEnvelope messageEnvelope);
    }
}