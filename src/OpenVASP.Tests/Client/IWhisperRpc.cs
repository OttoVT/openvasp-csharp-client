using System.Collections.Generic;
using System.Threading.Tasks;
using OpenVASP.Messaging;
using OpenVASP.Messaging.Messages;
using OpenVASP.Messaging.Messages.Entities;

namespace OpenVASP.Tests.Client
{
    public interface IWhisperRpc
    {
        Task<string> RegisterSymKeyAsync(string privateKey);

        Task<string> RegisterKeyPairAsync(string privateKey);

        Task<string> CreateMessageFilterAsync(string topic, string privateKeyId = null, string symKeyId = null, string signingKey = null);

        Task<string> SendMessageAsync(string topic, string encryptionKey, EncryptionType encryptionType,
            string payload);

        Task<IReadOnlyCollection<SessionRequestMessage>> GetSessionRequestMessages(string messageFilter);
        
        Task<IReadOnlyCollection<MessageBase>> GetSessionMessagesAsync(string messageFilter);

        Task<IReadOnlyCollection<ReceivedMessage>> GetMessagesAsync(string messageFilter);
    }
}