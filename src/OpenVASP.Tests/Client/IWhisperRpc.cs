using System.Collections.Generic;
using System.Threading.Tasks;
using OpenVASP.Messaging;
using OpenVASP.Messaging.Messages;

namespace OpenVASP.Tests.Client
{
    public interface IWhisperRpc
    {
        Task<string> RegisterSymKeyAsync(string privateKey);

        Task<string> RegisterKeyPairAsync(string privateKey);

        Task<string> CreateMessageFilterAsync(string topic, string privateKeyId = null, string symKeyId = null, string signingKey = null);

        Task<string> SendMessageAsync(MessageEnvelope messageEnvelope, MessageBase message);

        Task<IReadOnlyCollection<SessionRequestMessage>> GetSessionRequestMessages(string messageFilter);
        
        Task<IReadOnlyCollection<MessageBase>> GetSessionMessagesAsync(string topic);
    }
}