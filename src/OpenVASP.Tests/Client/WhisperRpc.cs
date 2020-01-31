using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nethereum.Web3;
using OpenVASP.Messaging;
using OpenVASP.Messaging.Messages;
using OpenVASP.Messaging.Messages.Entities;

namespace OpenVASP.Tests.Client
{
    public class WhisperRpc : IWhisperRpc
    {
        private readonly IWeb3 _web3;
        private readonly IMessageFormatter _messageFormatter;

        public WhisperRpc(IWeb3 web3, IMessageFormatter messageFormatter)
        {
            this._web3 = web3;
            this._messageFormatter = messageFormatter;
        }
        public Task<string> RegisterSymKeyAsync(string privateKey)
        {
            throw new NotImplementedException();
        }

        public Task<string> RegisterKeyPairAsync(string privateKey)
        {
            throw new NotImplementedException();
        }

        public Task<string> CreateMessageFilterAsync(string topic, string privateKey = null, string signingKey = null)
        {
            throw new NotImplementedException();
        }

        public async Task<IReadOnlyCollection<SessionRequestMessage>> GetSessionRequestMessages(string messageFilter)
        {
            var messages = await GetMessagesAsync(messageFilter);

            if (messages == null || messages.Length == 0)
            {
                return new SessionRequestMessage[] {};
            }

            var sessionRequests = messages.Select(x =>
            {
                var message = _messageFormatter.Deserialize(x.Payload, x.MessageEnvelope) as SessionRequestMessage;

                return message;
            }).Where(x => x != null);

            return sessionRequests.ToArray();
        }

        private async Task<ReceivedMessage[]> GetMessagesAsync(string source)
        {
            var messages = await _web3.Shh.MessageFilter.GetFilterMessages.SendRequestAsync(source);

            if (messages == null || messages.Length == 0)
            {
                return new ReceivedMessage[] { };
            }

            var receivedMessages = messages.Select(x => new ReceivedMessage()
            {
                MessageEnvelope = new MessageEnvelope()
                {
                    Topic = x.Topic,
                    EncryptionType = EncryptionType.Assymetric, // -_- Find a way to determine Symmetric encryption, do we even need it?
                    EncryptionKey = x.RecipientPublicKey,
                    Signature = x.Sig
                },
                Payload = x.Payload
            }).ToArray();

            return receivedMessages;
        }
    }
}