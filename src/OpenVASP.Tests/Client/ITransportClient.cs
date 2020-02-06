using System.Collections.Generic;
using System.Threading.Tasks;
using OpenVASP.Messaging;
using OpenVASP.Messaging.Messages;

namespace OpenVASP.Tests.Client
{
    public interface ITransportClient
    {
        Task<string> SendAsync(MessageEnvelope messageEnvelope, MessageBase message);
        Task<IReadOnlyCollection<TransportMessage>> GetSessionMessagesAsync(string messageFilter);
    }
}