using System.Threading.Tasks;
using OpenVASP.Messaging.Messages;

namespace OpenVASP.Messaging
{
    public interface ITransportClient
    {
        Task<string> SendMessageAsync<T>(MessageEnvelope<T> messageEnvelope) where T : MessageBase;

        Task<ReceivedMessage[]> GetMessagesAsync(string source);
    }
}