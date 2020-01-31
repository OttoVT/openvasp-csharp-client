using System.Collections.Generic;
using System.Threading.Tasks;
using OpenVASP.Messaging.Messages;

namespace OpenVASP.Tests.Client
{
    public interface IWhisperRpc
    {
        Task<string> RegisterSymKeyAsync(string privateKey);

        Task<string> RegisterKeyPairAsync(string privateKey);

        Task<string> CreateMessageFilterAsync(string topic, string privateKey = null, string signingKey = null);

        Task<IReadOnlyCollection<SessionRequestMessage>> GetSessionRequestMessages(string messageFilter);
    }
}