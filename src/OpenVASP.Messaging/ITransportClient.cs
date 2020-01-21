using System.Threading.Tasks;

namespace OpenVASP.Messaging
{
    public interface ITransportClient
    {
        Task<string> SendMessageAsync(MessageEnvelope messageEnvelope, string payload);

        Task<ReceivedMessage[]> GetMessagesAsync(string source);
    }
}