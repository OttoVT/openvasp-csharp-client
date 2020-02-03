using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OpenVASP.Messaging.MessagingEngine;

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

    public class MessageHandlerResolverBuilder
    {
        private List<(Type type, MessageHandlerBase handler)> _registeredHandlers;
        public MessageHandlerResolverBuilder()
        {
            _registeredHandlers = new List<(Type type, MessageHandlerBase handler)>();
        }

        public MessageHandlerResolverBuilder AddHandler(Type type, MessageHandlerBase handler)
        {
            _registeredHandlers.Add((type, handler));

            return this;
        }

        public MessageHandlerResolver Build()
        {
            var messageHandlerResolver = new MessageHandlerResolver(_registeredHandlers.ToArray());

            return messageHandlerResolver;
        }
    }

    public class MessageHandlerResolver
    {
        private readonly Dictionary<Type, MessageHandlerBase[]> handlersDict;

        public MessageHandlerResolver()
        {
            handlersDict = new Dictionary<Type, MessageHandlerBase[]>();
        }

        internal MessageHandlerResolver(params (Type type, MessageHandlerBase handler)[] handlers)
        {
            handlersDict = handlers
                .GroupBy(x => x.type, y => y.handler)
                .ToDictionary(group => group.Key, group => group.ToArray());
        }

        public MessageHandlerBase[] ResolveMessageHandlers(Type type)
        {
            handlersDict.TryGetValue(type, out var handlers);

            return handlers;
        }
    }
}