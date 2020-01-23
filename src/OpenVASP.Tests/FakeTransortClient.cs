using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenVASP.Messaging;

namespace OpenVASP.Tests
{
    public class FakeTransortClient : ITransportClient
    {
        private ConcurrentQueue<ReceivedMessage> queue = new ConcurrentQueue<ReceivedMessage>();

        public FakeTransortClient()
        {

        }

        public Task<string> SendMessageAsync(MessageEnvelope messageEnvelope, string payload)
        {
            queue.Enqueue(new ReceivedMessage()
            {
                MessageEnvelope = messageEnvelope,
                Payload = payload
            });

            return Task.FromResult(queue.Count.ToString());
        }

        public Task<ReceivedMessage[]> GetMessagesAsync(string source)
        {
            List<ReceivedMessage> messages = new List<ReceivedMessage>();
            while (queue.TryDequeue(out var received))
            {
                messages.Add(received);
            }

            return Task.FromResult(messages.ToArray());
        }
    }
}