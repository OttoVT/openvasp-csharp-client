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
using OpenVASP.Whisper;
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
            var geth = "http://144.76.25.187:8025";
            var web3 = new Web3(geth);

            var senderKId = await web3.Shh.KeyPair.NewKeyPair.SendRequestAsync();
            var senderSignId = await web3.Shh.KeyPair.NewKeyPair.SendRequestAsync();

            var receiverKId = await web3.Shh.KeyPair.NewKeyPair.SendRequestAsync();
            var receiverSignId = await web3.Shh.KeyPair.NewKeyPair.SendRequestAsync();

            var receiverHandshakePubKey = await web3.Shh.KeyPair.GetPublicKey.SendRequestAsync(receiverKId);
            var receiverSignPubKey = await web3.Shh.KeyPair.GetPublicKey.SendRequestAsync(receiverSignId);

            var senderHandshakePubKey = await web3.Shh.KeyPair.GetPublicKey.SendRequestAsync(senderKId);
            var senderSignPubKey = await web3.Shh.KeyPair.GetPublicKey.SendRequestAsync(senderSignId);

            var originatorTopicPubkey = await web3.Shh.SymKey.NewSymKey.SendRequestAsync();
            //4Bytes 
            var senderTopic = "0x12345678";
            var beneficiaryVaspCodeTopic = "0x12345677";//"0x" + "My Topic".GetHashCode().ToString("x");

            #region MessageToSender

            SessionRequestMessage requestToReceiver = null;
            var sessionId = Guid.NewGuid().ToByteArray().ToHex(prefix: false);
            var originatorTopicA = "0x01020304";
            {
                string ecdhPubKey;
                using (ECDiffieHellman ecdh = ECDiffieHellman.Create())
                {
                    var byteArray = ecdh.PublicKey.ToByteArray();
                    ecdhPubKey = byteArray.ToHex(prefix: false);
                }

                var handshake = new HandShakeRequest(originatorTopicA, ecdhPubKey);
                var vaspSender = GetSenderVaspInformation("0x...01", senderSignPubKey);
                var vaspReceiver = GetReceiverVaspInformation("0x...02", receiverSignPubKey);
                var messageHeader = new Message(
                    Guid.NewGuid().ToByteArray().ToHex(prefix: false), sessionId, "1");
                requestToReceiver = new SessionRequestMessage(
                    messageHeader,
                    handshake,
                    vaspSender)
                {
                    MessageEnvelope = new MessageEnvelope()
                    {
                        Topic = beneficiaryVaspCodeTopic,
                        EncryptionKey = receiverHandshakePubKey,
                        EncryptionType = EncryptionType.Assymetric,
                        Signature = receiverSignPubKey
                    },
                    Comment = "This is test to receiver message"
                };
            }


            #endregion

            #region MessageToSender

            SessionReplyMessage sessionReplyMessage = null;

            {
                var topicB = "0x01020305";
                var handshake = new HandShakeResponse(topicB);
                var vaspReceiver = GetReceiverVaspInformation("0x...02", receiverSignPubKey);
                var messageHeader = new Message(
                    Guid.NewGuid().ToByteArray().ToHex(prefix: false), sessionId, "1");
                sessionReplyMessage = new SessionReplyMessage(
                    messageHeader,
                    handshake,
                    vaspReceiver)
                {
                    MessageEnvelope = new MessageEnvelope()
                    {
                        Topic = beneficiaryVaspCodeTopic,
                        EncryptionKey = receiverHandshakePubKey,
                        EncryptionType = EncryptionType.Assymetric,
                        Signature = receiverSignPubKey
                    },
                    Comment = "This is test to receiver message"
                };
            }


            #endregion

            var whisperFormatter = new WhisperMessageFormatter();
            var whisperClient = new WhisperClient(geth);

            var originatorMessageFilter = await web3.Shh.MessageFilter.NewMessageFilter.SendRequestAsync(new MessageFilterInput()
            {
                //Sig = senderSignPubKey,
                PrivateKeyID = senderKId,
                Topics = new object[] { originatorTopicA }
            });

            var beneficiaryMessageFilter = await web3.Shh.MessageFilter.NewMessageFilter.SendRequestAsync(new MessageFilterInput()
            {
                //Sig = receiverSignPubKey,
                PrivateKeyID = receiverKId,
                Topics = new object[] { beneficiaryVaspCodeTopic }
            });

            var payload = whisperFormatter.GetPayload(requestToReceiver);
            var sentHash1 = await whisperClient.SendMessageAsync(requestToReceiver.MessageEnvelope, payload);

            ReceivedMessage shhMessage = null;
            for (int i = 0; i < 100; i++)
            {
                var messages = await whisperClient.GetMessagesAsync(beneficiaryMessageFilter);

                if (messages != null && messages.Length != 0)
                {
                    shhMessage = messages.First();
                    break;
                }

                await Task.Delay(2000);
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

            shhMessage = null;
            for (int i = 0; i < 100; i++)
            {
                var messages = await whisperClient.GetMessagesAsync(originatorMessageFilter);

                if (messages != null && messages.Length != 0)
                {
                    shhMessage = messages.First();
                    break;
                }

                await Task.Delay(2000);
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

        [Fact]
        public async Task WhisperSessionCreationTestAsync()
        {
            var geth = "";
            var web3 = new Web3(geth);
            var whisperClient = new WhisperClient(geth);

            VaspInformation originatorVaspInfo = GenerateOriginatorVaspInfo();
            VaspInformation beneficiaryVaspInfo = GenerateBeneficiaryVaspInfo();
            string sessionId = GenerateSessionId();

            (string originatorSigningPubKey, string originatorSigningPrivateKey) = await GenerateSigningKey();
            (string beneficiarySigningPubKey, string beneficiarySigningPrivateKey) = await GenerateSigningKey();

            (string beneficiaryVaspCode, 
             string beneficiaryHandshakePubKey, 
             string beneficiaryHandshakeKeyId,
             string beneficiaryTopicFilter) = await CreateBeneficiaryGenericTopic();
            
            (string originatorSessionTopic, 
             string sharedKey, 
             string sharedKeyId,
             string originatorSessionTopicFilter) = await CreateOriginatorSessionTopic();
            string sessionRequestMessagePayload = GenerateSessionRequest(
                sessionId,
                originatorSessionTopic, 
                sharedKey, 
                originatorVaspInfo, 
                originatorSigningPrivateKey);

            string messageHash1 = await SendMessage(
                payload: sessionRequestMessagePayload,
                receiverTopic: beneficiaryVaspCode,
                encryptionType: EncryptionType.Assymetric,
                encryptionKey: beneficiaryHandshakePubKey);

            string[] receivedPayloads = await ReceiveMessage(filter: beneficiaryTopicFilter);

            Assert.NotNull(receivedPayloads);
            Assert.Single(receivedPayloads);

            string payload = receivedPayloads.First();

            Assert.Equal(sessionRequestMessagePayload, payload);

            SessionRequestMessage sessionRequestMessage = ParseSessionRequestMessage(payload);
            Assert.True(VerifySignature(sessionRequestMessage, originatorSigningPubKey));

            (string beneficiarySessionTopic, string beneficiarySessionTopicFilter) = 
                await CreateBeneficiarySessionTopic(sessionRequestMessage.HandShake.AesGsmSharedKey);
            string sessionReplyMessagePayload = GenerateSessionReply(
                sessionId,
                SessionReplyMessage.GetMessageCode(SessionReplyMessage.SessionReplyMessageCode.SessionAccepted),
                beneficiarySessionTopic, 
                beneficiaryVaspInfo, 
                beneficiarySigningPrivateKey);

            string messageHash2 = await SendMessage(
                payload: sessionReplyMessagePayload,
                receiverTopic: sessionRequestMessage.HandShake.TopicA,
                encryptionType: EncryptionType.Symmetric,
                encryptionKey: sharedKey);

            string[] receivedPayloads2 = await ReceiveMessage(filter: originatorSessionTopicFilter);
            //asserts
            SessionReplyMessage sessionReplyMessage = ParseSessionReplyMessage(receivedPayloads2.First());
            Assert.True(ValidateSignature(sessionReplyMessage, beneficiarySigningPubKey));

            //string sessionReplyMessage = GenerateSessionReply(beTopicB, sharedKey, originatorVaspInfo, originatorSigningKey);
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
