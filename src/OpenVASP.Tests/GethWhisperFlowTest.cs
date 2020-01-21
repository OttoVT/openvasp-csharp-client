using System;
using System.Linq;
using System.Threading.Tasks;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.RPC.Shh.DTOs;
using Nethereum.Web3;
using Xunit;

namespace OpenVASP.Tests
{
    public class GethWhisperFlowTest
    {
        //This test proves that whisper works correctly for geth
        [Fact]
        public async Task WhisperFlowTestAsync()
        {
            var web3 = new Web3("");

            var kId = await web3.Shh.KeyPair.NewKeyPair.SendRequestAsync();

            //4Bytes 
            var topic = "0x12345678"; //"0x" + "My Topic".GetHashCode().ToString("x");
            var payload = "Hello there!".ToHexUTF8();

            var receiverPubKey = await web3.Shh.KeyPair.GetPublicKey.SendRequestAsync(kId);

            var messageFilter = await web3.Shh.MessageFilter.NewMessageFilter.SendRequestAsync(new MessageFilterInput()
            {
                PrivateKeyID = kId,
                Topics = new object[] { topic }
            });

            var messageHash = await web3.Shh.Post.SendRequestAsync(new MessageInput()
            {
                PowTarget = 0.4,
                PowTime = 6,
                Topic = topic,
                Ttl = 700,
                PubKey = receiverPubKey,
                Payload = payload,
            });

            ShhMessage message = null;
            for (int i = 0; i < 100; i++)
            {
                var messages = await web3.Shh.MessageFilter.GetFilterMessages.SendRequestAsync(messageFilter);

                if (messages != null && messages.Length != 0)
                {
                    message = messages.First();
                    break;
                }

                await Task.Delay(10000);
            }

            Assert.NotNull(message);
            Assert.Equal(payload, message.Payload);
            Assert.Equal(topic, message.Topic);
            Assert.Equal(messageHash, message.Hash);
            //Assert.Equal(0.4m, message.Pow);
            Assert.Equal(700, message.Ttl);
            Assert.Equal(receiverPubKey, message.RecipientPublicKey);
            Assert.Null(message.Sig);
        }
    }
}
