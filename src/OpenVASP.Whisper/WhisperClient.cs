using System;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.RPC.Shh.DTOs;
using Nethereum.Web3;
using OpenVASP.Messaging;
using OpenVASP.Messaging.Messages;

namespace OpenVASP.Whisper
{
    public class WhisperClient : ITransportClient
    {
        private readonly Web3 web3;

        public WhisperClient(string gethUrl)
        {
            this.web3 = new Web3(gethUrl);
        }

        public async Task<string> SendMessageAsync(MessageEnvelope messageEnvelope, string payload)
        {
            var messageInput = new MessageInput()
            {
                Topic = messageEnvelope.Topic,
                Sig = messageEnvelope.Signature,
                Payload = payload,
                //Find a way to calculate it
                PowTime = 12,
                PowTarget = 0.4,
                Ttl = 300,
            };

            switch (messageEnvelope.EncryptionType)
            {
                case EncryptionType.Assymetric:
                    messageInput.PubKey = messageEnvelope.EncryptionKey;
                    break;
                case EncryptionType.Symmetric:
                    messageInput.SymKeyID = messageEnvelope.EncryptionKey;
                    break;
                default:
                    throw new ArgumentException(
                        $"Current Encryption type {messageEnvelope.EncryptionType} is not supported.",
                        nameof(messageEnvelope.EncryptionType));
            }

            var messageHash = await web3.Shh.Post.SendRequestAsync(messageInput);

            return messageHash;
        }

        public async Task<ReceivedMessage[]> GetMessagesAsync(string source)
        {
            var messages = await web3.Shh.MessageFilter.GetFilterMessages.SendRequestAsync(source);

            if (messages == null || messages.Length == 0)
            {
                return new ReceivedMessage[]{};
            }

            var receivedMessages = messages.Select(x => new ReceivedMessage()
            {
                MessageEnvelope = new MessageEnvelope()
                {
                    Topic = x.Topic,
                    EncryptionType = EncryptionType.Assymetric, // -_- Find a way to determine Symmetric encryption
                    EncryptionKey = x.RecipientPublicKey,
                    Signature = x.Sig
                },
                Payload = x.Payload
            }).ToArray();

            return receivedMessages;
        }
    }
}
