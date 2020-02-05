using System.Threading.Tasks;
using OpenVASP.Messaging.Messages;
using OpenVASP.Messaging.Messages.Entities;
using OpenVASP.Tests.Client.Sessions;

namespace OpenVASP.Tests.Client
{
    public interface IVaspMessageHandler
    {
        Task<bool> AuthorizeSessionRequestAsync(VaspInformation request);

        Task<TransferReplyMessage> TransferRequestHandlerAsync(TransferRequestMessage request, VaspSession vaspSession);

        Task<TransferConfirmationMessage> TransferDispatchHandlerAsync(TransferDispatchMessage request, VaspSession vaspSession);
    }
}