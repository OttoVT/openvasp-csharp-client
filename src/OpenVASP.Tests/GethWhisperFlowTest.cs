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
    public class WhisperFixture : IDisposable
    {
        public WhisperFixture()
        {
            string gethUrl = "http://144.76.25.187:8025";
            this.Web3 = new Web3(gethUrl);
            this.WhisperMessageFormatter = new WhisperMessageFormatter();
            this.Signer = new EthereumMessageSigner();
        }

        public EthereumMessageSigner Signer { get; }

        public WhisperMessageFormatter WhisperMessageFormatter { get; }

        public Web3 Web3 { get; }

        public void Dispose()
        {
        }
    }
    public class GethWhisperFlowTest : IClassFixture<WhisperFixture>
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly WhisperFixture _whisperFixture;

        public GethWhisperFlowTest(WhisperFixture whisperFixture, ITestOutputHelper testOutputHelper)
        {
            this._whisperFixture = whisperFixture;
            this._testOutputHelper = testOutputHelper;
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

            _testOutputHelper.WriteLine($"{Newtonsoft.Json.JsonConvert.SerializeObject(shhMessage, Formatting.Indented)}");

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

            _testOutputHelper.WriteLine($"{Newtonsoft.Json.JsonConvert.SerializeObject(shhMessage, Formatting.Indented)}");
            //Assert.NotNull(message);
            //Assert.Equal(payload, message.Payload);
            //Assert.Equal(senderTopic, message.Topic);
            //Assert.Equal(messageHash, message.Hash);
            ////Assert.Equal(0.4m, message.Pow);
            //Assert.Equal(700, message.Ttl);
            //Assert.Equal(receiverPubKey, message.RecipientPublicKey);
            //Assert.Null(message.Sig);
        }

        //TODO: SPlit in originator and beneficiary sides
        [Fact]
        public async Task WhisperSessionCreationTestAsync()
        {
            //var geth = "";
            //var web3 = new Web3(geth);
            //var whisperClient = new WhisperClient(geth);
            //var whisperMessageFormatter = new WhisperMessageFormatter();

            //OriginatorSide
            (string originatorSigningPubKey, string originatorSigningPrivateKey, string originatorSigningPrivateKeyId) =
                await GenerateSigningKey();

            //BeneficiarySide
            (string beneficiarySigningPubKey, string beneficiarySigningPrivateKey,
                string beneficiarySigningPrivateKeyId) = await GenerateSigningKey();

            //BeneficiarySide
            (string beneficiaryVaspCode,
                string beneficiaryHandshakePubKey,
                string beneficiaryHandshakeKeyId,
                string beneficiaryTopicFilter) = await CreateBeneficiaryGenericTopic();

            //OriginatorSide
            //(string beneficiaryVaspCode, string beneficiaryVaspPubKey ) = await GetVaspFromIdentityAsync(beneficiaryIdentity);

            //BeneficiarySide
            //(string originatorVaspCode, string originatorVaspPubKey) = await GetVaspFromIdentityAsync(originatorIdentity);

            //OriginatorSide
            (string originatorSessionTopic,
                string sharedKey,
                string sharedKeyId,
                string originatorSessionTopicFilter) = await CreateOriginatorSessionTopic(beneficiarySigningPubKey);

            //OriginatorSide
            string sessionId = GenerateSessionId();
            VaspInformation originatorVaspInfo = GenerateOriginatorVaspInfo("0x..." + originatorSessionTopic, originatorSigningPubKey);

            //BeneficiarySide
            VaspInformation beneficiaryVaspInfo = GenerateBeneficiaryVaspInfo("0x..." + beneficiaryVaspCode, beneficiarySigningPubKey);

            //OriginatorSide
            string sessionRequestMessagePayload = GenerateSessionRequest(
                sessionId,
                originatorSessionTopic,
                sharedKey,
                originatorVaspInfo,
                originatorSigningPrivateKey);

            //OriginatorSide
            string messageHash1 = await SendMessage(
                payload: sessionRequestMessagePayload,
                receiverTopic: beneficiaryVaspCode,
                encryptionType: EncryptionType.Assymetric,
                encryptionKey: beneficiaryHandshakePubKey);

            //BeneficiarySide
            ReceivedMessage[] receivedMessages = await ReceiveMessage(filter: beneficiaryTopicFilter);

            Assert.NotNull(receivedMessages);
            Assert.Single(receivedMessages);

            ReceivedMessage receivedMessage = receivedMessages.First();

            Assert.Equal(sessionRequestMessagePayload, receivedMessage.Payload);

            //BeneficiarySide
            SessionRequestMessage sessionRequestMessage = ParseSessionRequestMessage(receivedMessage.Payload);
            //Assert.True(VerifySignature(sessionRequestMessage, originatorSigningPubKey));

            //BeneficiarySide
            (string beneficiarySessionTopic, string beneficiarySessionTopicFilter) =
                await CreateBeneficiarySessionTopic(sessionRequestMessage.HandShake.EcdhPubKey);

            //BeneficiarySide
            string sessionReplyMessagePayload = GenerateSessionReply(
                sessionId,
                SessionReplyMessage.GetMessageCode(SessionReplyMessage.SessionReplyMessageCode.SessionAccepted),
                beneficiarySessionTopic,
                beneficiaryVaspInfo,
                beneficiarySigningPrivateKey);

            //BeneficiarySide
            string messageHash2 = await SendMessage(
                payload: sessionReplyMessagePayload,
                receiverTopic: sessionRequestMessage.HandShake.TopicA,
                encryptionType: EncryptionType.Symmetric,
                encryptionKey: sharedKeyId,
                signature: beneficiarySigningPrivateKeyId);

            //OriginatorSide
            ReceivedMessage[] receivedMessages2 = await ReceiveMessage(filter: originatorSessionTopicFilter);

            Assert.NotNull(receivedMessages2);
            Assert.Single(receivedMessages2);

            ReceivedMessage receivedMessage2 = receivedMessages2.First();

            Assert.Equal(sessionReplyMessagePayload, receivedMessage2.Payload);

            SessionReplyMessage sessionReplyMessage = ParseSessionReplyMessage(receivedMessages2.First().Payload);

            _testOutputHelper?.WriteLine(sessionRequestMessage.HandShake.EcdhPubKey);
            _testOutputHelper?.WriteLine(sessionReplyMessage.HandShake.TopicB);

        }

        private SessionReplyMessage ParseSessionReplyMessage(string payload)
        {
            var sessionRequestMessage = (SessionReplyMessage)_whisperFixture.WhisperMessageFormatter.Deserialize(payload, null);

            return sessionRequestMessage;
        }

        //private bool VerifySignature(MessageBase sessionRequestMessage, string originatorSigningPubKey)
        //{
        //    var expectedSigner = new EthECKey(originatorSigningPubKey.HexToByteArray(), false);
        //    var payload = _whisperFixture.WhisperMessageFormatter.GetPayload(sessionRequestMessage,);
        //    var signerAddress = _whisperFixture.Signer.EncodeUTF8AndEcRecover(payload, sessionRequestMessage.Signature);

        //    return expectedSigner.GetPublicAddress().Equals(signerAddress, StringComparison.CurrentCultureIgnoreCase);
        //}

        private string GenerateSessionReply(string sessionId,
            string getMessageCode, string beneficiarySessionTopic, VaspInformation beneficiaryVaspInfo, string beneficiarySigningPrivateKey)
        {
            var handshake = new HandShakeResponse(beneficiarySessionTopic);
            var sessionReplyMessage = new SessionReplyMessage(sessionId, handshake, beneficiaryVaspInfo);
            //var payload = _whisperFixture.WhisperMessageFormatter.GetPayload(sessionReplyMessage, withSignature: false);
            //sessionReplyMessage.Signature = _whisperFixture.Signer.EncodeUTF8AndSign(payload, new EthECKey(beneficiarySigningPrivateKey));
            var payload = _whisperFixture.WhisperMessageFormatter.GetPayload(sessionReplyMessage);

            return payload;
        }

        private async Task<(string, string)> CreateBeneficiarySessionTopic(string handShakeAesGsmSharedKey)
        {
            var topicB = "0x04060801";
            var symKeyId = await _whisperFixture.Web3.Shh.SymKey.AddSymKey.SendRequestAsync(handShakeAesGsmSharedKey);

            var filter = await _whisperFixture.Web3.Shh.MessageFilter.NewMessageFilter.SendRequestAsync(new MessageFilterInput()
            {
                Topics = new[] { topicB },
                SymKeyID = symKeyId
            });

            return (topicB, filter);
        }

        private SessionRequestMessage ParseSessionRequestMessage(string payload)
        {
            var sessionRequestMessage = (SessionRequestMessage)_whisperFixture.WhisperMessageFormatter.Deserialize(payload, null);

            return sessionRequestMessage;
        }

        private async Task<ReceivedMessage[]> ReceiveMessage(string filter)
        {
            var messages = await _whisperFixture.Web3.Shh.MessageFilter.GetFilterMessages.SendRequestAsync(filter);

            if (messages == null || messages.Length == 0)
            {
                return new ReceivedMessage[] { };
            }

            var receivedMessages = messages.Select(x => new ReceivedMessage()
            {
                MessageEnvelope = new MessageEnvelope()
                {
                    Topic = x.Topic,
                    EncryptionType = EncryptionType.Assymetric, // -_- Find a way to determine Symmetric encryption
                    EncryptionKey = x.RecipientPublicKey,
                    //Signature = x.Sig
                },
                Payload = x.Payload
            }).ToArray();

            return receivedMessages;
        }

        private async Task<string> SendMessage(string payload, string receiverTopic, EncryptionType encryptionType,
            string encryptionKey, string signature = null)
        {
            var messageInput = new MessageInput()
            {
                Topic = receiverTopic,
                Sig = signature,
                Payload = payload,
                //Find a way to calculate it
                PowTime = 12,
                PowTarget = 0.4,
                Ttl = 300,
            };

            switch (encryptionType)
            {
                case EncryptionType.Assymetric:
                    messageInput.PubKey = encryptionKey;
                    break;
                case EncryptionType.Symmetric:
                    messageInput.SymKeyID = encryptionKey;
                    break;
                default:
                    throw new ArgumentException(
                        $"Current Encryption type {encryptionType} is not supported.",
                        nameof(encryptionType));
            }

            var messageHash = await _whisperFixture.Web3.Shh.Post.SendRequestAsync(messageInput);

            return messageHash;
        }

        private string GenerateSessionRequest(
            string sessionId,
            string originatorSessionTopic,
            string sharedKey,
            VaspInformation originatorVaspInfo,
            string originatorSigningPrivateKey)
        {
            var handshake = new HandShakeRequest(originatorSessionTopic, sharedKey);
            var sessionRequestMessage = new SessionRequestMessage(sessionId, handshake, originatorVaspInfo);

            //TODO: Optimize signing
            //var payload = _whisperFixture.WhisperMessageFormatter.GetPayload(sessionRequestMessage, withSignature:false);
            //sessionRequestMessage.Signature = _whisperFixture.Signer.EncodeUTF8AndSign(payload, new EthECKey(originatorSigningPrivateKey));
            var payload = _whisperFixture.WhisperMessageFormatter.GetPayload(sessionRequestMessage);

            return payload;
        }

        private async Task<(string OriginatorVaspCode, string OriginatorSharedKey, string OriginatorHandshakeKeyId, string OriginatorTopicFilter)>
            CreateOriginatorSessionTopic(string beneficiarySignPubKey)
        {
            var topicA = "0x02030405";
            var symKeyGenerated = (new byte[]
            {
                97,
                191,
                23,
                61,
                91,
                84,
                160,
                22,
                249,
                178,
                199,
                104,
                15,
                142,
                33,
                118,
                205,
                0,
                7,
                204,
                189,
                84,
                81,
                157,
                159,
                173,
                174,
                115,
                154,
                233,
                131,
                117
            }).ToHex(prefix: true);


            var symKeyId = await _whisperFixture.Web3.Shh.SymKey.AddSymKey.SendRequestAsync(symKeyGenerated);
            var symKey = await _whisperFixture.Web3.Shh.SymKey.GetSymKey.SendRequestAsync(symKeyId);
            var filter = await _whisperFixture.Web3.Shh.MessageFilter.NewMessageFilter.SendRequestAsync(new MessageFilterInput()
            {
                Topics = new[] { topicA },
                SymKeyID = symKeyId,
                Sig = beneficiarySignPubKey
            });

            return (topicA, symKey, symKeyId, filter);
        }

        private async Task<(string BeneficiaryVaspCode, string BeneficiaryHandshakePubKey, string BeneficiaryHandshakeKeyId, string BeneficiaryTopicFilter)>
            CreateBeneficiaryGenericTopic()
        {
            var vaspCode = "0x01020304";
            var generatedKey = await GenerateSigningKey();
            var filter = await _whisperFixture.Web3.Shh.MessageFilter.NewMessageFilter.SendRequestAsync(new MessageFilterInput()
            {
                Topics = new[] { vaspCode },
                PrivateKeyID = generatedKey.KeyId
            });

            return (vaspCode, generatedKey.PubKey, generatedKey.KeyId, filter);
        }

        private string GenerateSessionId()
        {
            return Guid.NewGuid().ToString();
        }

        private async Task<(string PubKey, string PrivateKey, string KeyId)> GenerateSigningKey()
        {
            var senderKId = await _whisperFixture.Web3.Shh.KeyPair.NewKeyPair.SendRequestAsync();
            var receiverPubKey = await _whisperFixture.Web3.Shh.KeyPair.GetPublicKey.SendRequestAsync(senderKId);
            var receiverPrivateKey = await _whisperFixture.Web3.Shh.KeyPair.GetPrivateKey.SendRequestAsync(senderKId);

            return (receiverPubKey, receiverPrivateKey, senderKId);
        }

        private VaspInformation GenerateOriginatorVaspInfo(string vaspIdentity, string vaspPubKey)
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

        private VaspInformation GenerateBeneficiaryVaspInfo(string vaspIdentity, string vaspPubKey)
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
