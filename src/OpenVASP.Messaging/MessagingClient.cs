using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OpenVASP.Messaging.Messages;

namespace OpenVASP.Messaging
{
    public static class MessageTypeMapping
    {
        public static Dictionary<MessageType, Type> Mapping = new Dictionary<MessageType, Type>()
        {
            {MessageType.SessionRequest, typeof(SessionRequestMessage) }
        };
    }

    public abstract class MessageHandlerBase
    {
        public abstract Task HandleMessageAsync(MessageBase message, CancellationToken cancellationToken);
    }
    
    public class SessionRequestMessageHandler : MessageHandlerBase
    {
        private readonly Func<SessionRequestMessage, CancellationToken, Task> processFunc;

        public SessionRequestMessageHandler(Func<SessionRequestMessage, CancellationToken, Task> processFunc)
        {
            this.processFunc = processFunc;
        }

        public override async Task HandleMessageAsync(MessageBase message, CancellationToken cancellationToken)
        {
            if (message is SessionRequestMessage sessionRequestMessage)
            {
                await processFunc(sessionRequestMessage, cancellationToken);
            }
        }
    }
}