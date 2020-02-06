using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenVASP.Messaging;
using OpenVASP.Messaging.Messages;

namespace OpenVASP.Tests.Client
{
    //TODO: add retry policies

    public static class RetryPolicy
    {
        public static async Task<T> ExecuteAsync<T>(Func<Task<T>> func, int retriesCount = 4)
        {
            var counter = 0;
            Exception lastException = null;

            do
            {
                try
                {
                    return await func();
                }
                catch (Exception e)
                {
                    lastException = e;
                }

                var delay = 100 * (int)Math.Pow(2, counter);
                await Task.Delay(delay);
                counter++;
            } while (counter < retriesCount);

            throw lastException;
        }
    }

    public class TransportClient : ITransportClient
    {
        private readonly IMessageFormatter _messageFormatter;
        private readonly IWhisperRpc _whisper;
        private readonly ISignService _signService;

        public TransportClient(IWhisperRpc whisper, ISignService signService, IMessageFormatter messageFormatter)
        {
            _whisper = whisper;
            _signService = signService;
            _messageFormatter = messageFormatter;
        }

        public async Task<string> SendAsync(MessageEnvelope messageEnvelope, MessageBase message)
        {
            var payload = _messageFormatter.GetPayload(message);
            var sign = _signService.SignPayload(payload, messageEnvelope.SigningKey);

            return await RetryPolicy.ExecuteAsync(async () =>
            {
                return await _whisper.SendMessageAsync(
                    messageEnvelope.Topic,
                    messageEnvelope.EncryptionKey,
                    messageEnvelope.EncryptionType,
                    payload + sign);
            });
        }

        public async Task<IReadOnlyCollection<TransportMessage>> GetSessionMessagesAsync(string messageFilter)
        {
            var messages = await RetryPolicy.ExecuteAsync(async () =>
            {
                return await _whisper.GetMessagesAsync(messageFilter);
            });

            if (messages == null || messages.Count == 0)
            {
                return new TransportMessage[] { };
            }

            var serializedMessages = messages.Select(x =>
            {
                var payload = x.Payload.Substring(0, x.Payload.Length - 130);
                var sign = x.Payload.Substring(x.Payload.Length - 130, 130);
                var message = _messageFormatter.Deserialize(payload);

                return TransportMessage.CreateMessage(message, payload, sign);
            }).ToArray();

            return serializedMessages;
        }
    }
}