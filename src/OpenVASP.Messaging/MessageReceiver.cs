using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenVASP.Messaging
{
    public class MessageReceiver
    {
        private readonly IMessageFormatter messageFormatter;
        private readonly ITransportClient transportClient;
        private readonly MessageHandlerResolver messageHandlerResolver;

        public MessageReceiver(
            IMessageFormatter messageFormatter,
            ITransportClient transportClient,
            MessageHandlerResolver messageHandlerResolver)
        {
            this.messageFormatter = messageFormatter;
            this.transportClient = transportClient;
            this.messageHandlerResolver = messageHandlerResolver;
        }

        public async Task ReceiveMessagesAsync(string source, CancellationToken cancellationToken = default)
        {
            var receivedMessages = await transportClient.GetMessagesAsync(source);

            foreach (var message in receivedMessages)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                var messageDeserialized = messageFormatter.Deserialize(message.Payload, message.MessageEnvelope);
                var handlers = messageHandlerResolver.ResolveMessageHandlers(messageDeserialized.GetType());
                var tasks = handlers.Select(handler => handler.HandleMessageAsync(messageDeserialized, cancellationToken)).ToArray();

                Task.WaitAll(tasks, cancellationToken);
            }
        }
    }

    public class MessageHandlerResolver
    {
        public MessageHandlerResolver(params (Type type, MessageHandlerBase handler)[] handlers)
        {
            handlersDict = handlers
                .GroupBy(x => x.type, y => y.handler)
                .ToDictionary(group => group.Key, group => group.ToArray());
        }

        public readonly Dictionary<Type, MessageHandlerBase[]> handlersDict;

        public MessageHandlerBase[] ResolveMessageHandlers(Type type)
        {
            handlersDict.TryGetValue(type, out var handlers);

            return handlers;
        }
    }
}