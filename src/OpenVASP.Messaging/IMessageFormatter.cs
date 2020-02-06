using OpenVASP.Messaging.Messages;

namespace OpenVASP.Messaging
{
    public interface IMessageFormatter
    {
        MessageEnvelope GetEnvelope(MessageBase messageBase);

        string GetPayload(MessageBase sessionRequestMessage);

        MessageBase Deserialize(string payload);
    }
}