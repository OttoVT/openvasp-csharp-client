using System;
using System.Threading.Tasks;
using OpenVASP.Messaging.Messages;

namespace OpenVASP.Tests.Client
{
    public class VaspMessageHandlerCallbacks : IVaspMessageHandler
    {
        private readonly Func<TransferRequestMessage, Task<TransferReplyMessage>> _transferRequest;
        private readonly Func<TransferDispatchMessage, Task<TransferConfirmationMessage>> _transferDispatch;
        private readonly Func<SessionRequestMessage, Task<SessionReplyMessage>> _sessionRequest;

        public VaspMessageHandlerCallbacks(
            Func<SessionRequestMessage, Task<SessionReplyMessage>> sessionRequest,
            Func<TransferRequestMessage, Task<TransferReplyMessage>> transferRequest,
            Func<TransferDispatchMessage, Task<TransferConfirmationMessage>> transferDispatch)
        {
            _sessionRequest= sessionRequest ?? throw new ArgumentNullException(nameof(sessionRequest));
            _transferRequest = transferRequest ?? throw new ArgumentNullException(nameof(transferRequest));
            _transferDispatch = transferDispatch ?? throw new ArgumentNullException(nameof(transferDispatch));
        }

        public Task<SessionReplyMessage> SessionRequestHandlerAsync(SessionRequestMessage request)
        {
            return _sessionRequest?.Invoke(request);
        }

        public Task<TransferReplyMessage> TransferRequestHandlerAsync(TransferRequestMessage request)
        {
            return _transferRequest?.Invoke(request);
        }

        public Task<TransferConfirmationMessage> TransferDispatchHandlerAsync(TransferDispatchMessage request)
        {
            return _transferDispatch?.Invoke(request);
        }
    }
}
