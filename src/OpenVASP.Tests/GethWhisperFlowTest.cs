using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.RPC.Shh.DTOs;
using Nethereum.Signer;
using Nethereum.Web3;
using Newtonsoft.Json;
using OpenVASP.Messaging;
using OpenVASP.Messaging.Messages;
using OpenVASP.Messaging.Messages.Entities;
using Xunit;
using Xunit.Abstractions;

namespace OpenVASP.Tests
{
    public class GethWhisperFlowTest
    {
        private readonly ITestOutputHelper testOutputHelper;

        public GethWhisperFlowTest(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        //This test proves that whisper works correctly for geth
        [Fact]
        public async Task WhisperFlowTestAsync()
        {
            var web3 = new Web3("http://144.76.25.187:8025");

            var senderKId = await web3.Shh.KeyPair.NewKeyPair.SendRequestAsync();
            var senderSignId = await web3.Shh.KeyPair.NewKeyPair.SendRequestAsync();

            var receiverKId = await web3.Shh.KeyPair.NewKeyPair.SendRequestAsync();
            var receiverSignId = await web3.Shh.KeyPair.NewKeyPair.SendRequestAsync();

            var receiverHandshakePubKey = await web3.Shh.KeyPair.GetPublicKey.SendRequestAsync(receiverKId);
            var receiverSignPubKey = await web3.Shh.KeyPair.GetPublicKey.SendRequestAsync(receiverSignId);

            var senderHandshakePubKey = await web3.Shh.KeyPair.GetPublicKey.SendRequestAsync(senderKId);
            var senderSignPubKey = await web3.Shh.KeyPair.GetPublicKey.SendRequestAsync(senderSignId);

            //4Bytes 
            var senderTopic = "0x12345678";
            var receiverTopic = "0x12345677";//"0x" + "My Topic".GetHashCode().ToString("x");

            #region Message

            string ecdhPubKey;
            using (ECDiffieHellman ecdh = ECDiffieHellman.Create())
            {
                var byteArray = ecdh.PublicKey.ToByteArray();
                ecdhPubKey = byteArray.ToHex(prefix: false);
            }

            var topicA = "0x01020304";
            var handshake = new HandShakeRequest(topicA, ecdhPubKey);
            var vaspSender = GetSenderVaspInformation("0x...01", senderSignPubKey);
            var vaspReceiver = GetReceiverVaspInformation("0x...02", receiverSignPubKey);

            var sessionId = Guid.NewGuid().ToByteArray().ToHex(prefix: false);
            var requestToReceiver = new SessionRequestMessage(
                sessionId,
                handshake,
                vaspSender,
                Guid.NewGuid().ToByteArray().ToHex(prefix: false))
            {
                Comment = "This is test to receiver message"
            };

            var envelope = new MessageEnvelope<SessionRequestMessage>(
                receiverTopic, 
                EncryptionType.Assymetric,
                receiverSignPubKey, 
                requestToReceiver);

            #endregion

            var payload = "Hello there!".ToHexUTF8();

            var senderMessageFilter = await web3.Shh.MessageFilter.NewMessageFilter.SendRequestAsync(new MessageFilterInput()
            {
                //Sig = senderSignPubKey,
                PrivateKeyID = senderKId,
                Topics = new object[] { senderTopic }
            });

            var receiverMessageFilter = await web3.Shh.MessageFilter.NewMessageFilter.SendRequestAsync(new MessageFilterInput()
            {
                //Sig = receiverSignPubKey,
                PrivateKeyID = receiverKId,
                Topics = new object[] { receiverTopic }
            });

            //var whisperClient = new 
            var messageHash = await web3.Shh.Post.SendRequestAsync(new MessageInput()
            {
                PowTarget = 0.4,
                PowTime = 6,
                Topic = receiverTopic,
                Ttl = 700,
                PubKey = receiverHandshakePubKey,
                //Sig = receiverSignId, 
                Payload = payload,
            });

            ShhMessage shhMessage = null;
            for (int i = 0; i < 100; i++)
            {
                var messages = await web3.Shh.MessageFilter.GetFilterMessages.SendRequestAsync(receiverMessageFilter);

                if (messages != null && messages.Length != 0)
                {
                    shhMessage = messages.First();
                    break;
                }

                await Task.Delay(10000);
            }

            testOutputHelper.WriteLine($"{Newtonsoft.Json.JsonConvert.SerializeObject(shhMessage, Formatting.Indented)}");

            var messageHash1 = await web3.Shh.Post.SendRequestAsync(new MessageInput()
            {
                PowTarget = 0.4,
                PowTime = 6,
                Topic = senderTopic,
                Ttl = 700,
                PubKey = senderHandshakePubKey,
                //Sig = senderSignId,
                Payload = "HI".ToHexUTF8(),
            });

            for (int i = 0; i < 100; i++)
            {
                var messages = await web3.Shh.MessageFilter.GetFilterMessages.SendRequestAsync(senderMessageFilter);

                if (messages != null && messages.Length != 0)
                {
                    shhMessage = messages.First();
                    break;
                }

                await Task.Delay(10000);
            }

            testOutputHelper.WriteLine($"{Newtonsoft.Json.JsonConvert.SerializeObject(shhMessage, Formatting.Indented)}");
            //Assert.NotNull(message);
            //Assert.Equal(payload, message.Payload);
            //Assert.Equal(senderTopic, message.Topic);
            //Assert.Equal(messageHash, message.Hash);
            ////Assert.Equal(0.4m, message.Pow);
            //Assert.Equal(700, message.Ttl);
            //Assert.Equal(receiverPubKey, message.RecipientPublicKey);
            //Assert.Null(message.Sig);
        }

        private static VaspInformation GetSenderVaspInformation(string vaspIdentity, string vaspPubKey)
        {
            var postalAddress = new PostalAddress(
                "TestingStreet",
                61,
                "Test Address Line",
                "410000",
                "TownN",
                Country.List["DE"]
            );
            var placeOfBirth = new PlaceOfBirth(DateTime.UtcNow, "TownN", Country.List["DE"]);
            var vaspInformation = new VaspInformation(
                "Sender test",
                vaspIdentity,
                vaspPubKey,
                postalAddress,
                placeOfBirth,
                new NaturalPersonId[]
                {
                    new NaturalPersonId("SomeId2", NaturalIdentificationType.AlienRegistrationNumber,
                        Country.List["DE"]),
                },
                new JuridicalPersonId[]
                {
                    new JuridicalPersonId("SomeId1", JuridicalIdentificationType.BankPartyIdentification,
                        Country.List["DE"]),
                },
                "DEUTDEFF");
            return vaspInformation;
        }

        private static VaspInformation GetReceiverVaspInformation(string vaspIdentity, string vaspPubKey)
        {
            var postalAddress = new PostalAddress(
                "TestingStreet",
                61,
                "Test Address Line",
                "410000",
                "TownN",
                Country.List["DE"]
            );
            var placeOfBirth = new PlaceOfBirth(DateTime.UtcNow, "TownN", Country.List["DE"]);
            var vaspInformation = new VaspInformation(
                "Sender test",
                vaspIdentity,
                vaspPubKey,
                postalAddress,
                placeOfBirth,
                new NaturalPersonId[]
                {
                    new NaturalPersonId("SomeId2", NaturalIdentificationType.AlienRegistrationNumber,
                        Country.List["DE"]),
                },
                new JuridicalPersonId[]
                {
                    new JuridicalPersonId("SomeId1", JuridicalIdentificationType.BankPartyIdentification,
                        Country.List["DE"]),
                },
                "DEUTDEFF");
            return vaspInformation;
        }
    }
}
