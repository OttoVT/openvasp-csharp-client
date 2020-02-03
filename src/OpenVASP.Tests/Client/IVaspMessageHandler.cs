using System.Threading.Tasks;
using OpenVASP.Messaging.Messages;

namespace OpenVASP.Tests.Client
{
    public interface IVaspMessageHandler
    {
        Task<SessionReplyMessage> SessionRequestHandlerAsync(SessionRequestMessage request);

        Task<TransferReplyMessage> TransferRequestHandlerAsync(TransferRequestMessage request);

        Task<TransferConfirmationMessage> TransferDispatchHandlerAsync(TransferDispatchMessage request);
    }
}