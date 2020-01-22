using System;
using System.Linq;
using System.Threading.Tasks;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.RPC.Shh.DTOs;
using Nethereum.Web3;
using OpenVASP.Messaging;
using OpenVASP.Messaging.Messages;
using OpenVASP.Whisper;
using Xunit;
using System.Security.Cryptography;
using Nethereum.Signer;
using OpenVASP.ProtocolMessages.Messages;

namespace OpenVASP.Tests
{
    public class WhisperClientTest
    {
        //This test proves that whisper works correctly for geth
        [Fact]
        public async Task WhisperFlowTestAsync()
        {
            var gethUrl = "";
            var web3 = new Web3(gethUrl);
            var whisperClient = new WhisperClient(gethUrl);

            //Should be a contract
            var vaspKey = EthECKey.GenerateKey();
            var kId = await web3.Shh.KeyPair.NewKeyPair.SendRequestAsync();

            //4Bytes 
            var topic = "0x12345678"; //"0x" + "My Topic".GetHashCode().ToString("x");

            var receiverPubKey = await web3.Shh.KeyPair.GetPublicKey.SendRequestAsync(kId);

            var messageFilter = await web3.Shh.MessageFilter.NewMessageFilter.SendRequestAsync(new MessageFilterInput()
            {
                PrivateKeyID = kId,
                Topics = new object[] { topic }
            });

            string ecdhPubKey;
            using (ECDiffieHellman ecdh = ECDiffieHellman.Create())
            {
                var byteArray = ecdh.PublicKey.ToByteArray();
                ecdhPubKey = byteArray.ToHex(prefix: false);
            }


            var message = new Message(
                MessageType.SessionRequest,
                Guid.NewGuid().ToByteArray().ToHex(prefix: false),
                Guid.NewGuid().ToByteArray().ToHex(prefix: false),
                "1");
            var handshake = new HandShake(topic, null, ecdhPubKey);
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
                "Test test",
                vaspKey.GetPublicAddress(),
                vaspKey.GetPubKey().ToHex(prefix: false),
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

            var request = new SessionRequestMessage(message, handshake, vaspInformation)
            {
                MessageEnvelope = new MessageEnvelope()
                {
                    EncryptionType = EncryptionType.Assymetric,
                    EncryptionKey = receiverPubKey,
                    Topic = topic,
                    Signature = kId
                },
                Comment = "This is test message",
            };

            var whisperMessageFormatter = new WhisperMessageFormatter();
            var payload = whisperMessageFormatter.GetPayload(request);
            var envelope = whisperMessageFormatter.GetEnvelope(request);

            var messageHash = await whisperClient.SendMessageAsync(envelope, payload);

            ShhMessage shhMessage = null;
            for (int i = 0; i < 10; i++)
            {
                var messages = await web3.Shh.MessageFilter.GetFilterMessages.SendRequestAsync(messageFilter);

                if (messages != null && messages.Length != 0)
                {
                    shhMessage = messages.First();
                    break;
                }

                await Task.Delay(10000);
            }

            Assert.NotNull(shhMessage);

            var messageEnvelope = new MessageEnvelope()
            {
                Topic = shhMessage.Topic,
                EncryptionType = EncryptionType.Assymetric,
                EncryptionKey = shhMessage.RecipientPublicKey,
                Signature = shhMessage.Sig
            };

            var response = (SessionRequestMessage)whisperMessageFormatter.Deserialize(shhMessage.Payload, messageEnvelope);


            Assert.NotNull(response);

            Assert.Equal(request.MessageEnvelope.Topic, response.MessageEnvelope.Topic);
            //Assert.Equal(request.MessageEnvelope.Signature, response.MessageEnvelope.Signature);
            Assert.Equal(request.MessageEnvelope.EncryptionType, response.MessageEnvelope.EncryptionType);
            Assert.Equal(request.MessageEnvelope.EncryptionKey, response.MessageEnvelope.EncryptionKey);

            Assert.Equal(request.HandShake.TopicA, response.HandShake.TopicA);
            Assert.Equal(request.HandShake.ECDHPublicKey, response.HandShake.ECDHPublicKey);

            Assert.Equal(request.Comment, response.Comment);

            Assert.Equal(request.Message.SessionId, response.Message.SessionId);
            Assert.Equal(request.Message.MessageType, response.Message.MessageType);
            Assert.Equal(request.Message.MessageCode, response.Message.MessageCode);
            Assert.Equal(request.Message.MessageId, response.Message.MessageId);

            Assert.Equal(request.VASP.PlaceOfBirth.DateOfBirth.Date, response.VASP.PlaceOfBirth.DateOfBirth.Date);
            Assert.Equal(request.VASP.PlaceOfBirth.CountryOfBirth, response.VASP.PlaceOfBirth.CountryOfBirth);
            Assert.Equal(request.VASP.PlaceOfBirth.CityOfBirth, response.VASP.PlaceOfBirth.CityOfBirth);

            Assert.Equal(request.VASP.BIC, response.VASP.BIC);
            Assert.Equal(request.VASP.Name, response.VASP.Name);
            Assert.Equal(request.VASP.VaspPublickKey, response.VASP.VaspPublickKey);
            Assert.Equal(request.VASP.VaspIdentity, response.VASP.VaspIdentity);
            
            Assert.Equal(request.VASP.PostalAddress.StreetName, response.VASP.PostalAddress.StreetName);
            Assert.Equal(request.VASP.PostalAddress.AddressLine, response.VASP.PostalAddress.AddressLine);
            Assert.Equal(request.VASP.PostalAddress.BuildingNumber, response.VASP.PostalAddress.BuildingNumber); 
            Assert.Equal(request.VASP.PostalAddress.Country, response.VASP.PostalAddress.Country);
            Assert.Equal(request.VASP.PostalAddress.PostCode, response.VASP.PostalAddress.PostCode);
            
            Assert.Equal(request.VASP.JuridicalPersonIds.Count(), response.VASP.JuridicalPersonIds.Count());

            for (int i = 0; i < request.VASP.JuridicalPersonIds.Count(); i++)
            {
                var expected = request.VASP.JuridicalPersonIds[i];
                var actual = response.VASP.JuridicalPersonIds[i];

                Assert.Equal(expected.IssuingCountry, actual.IssuingCountry);
                Assert.Equal(expected.IdentificationType, actual.IdentificationType);
                Assert.Equal(expected.Identifier, actual.Identifier);
                Assert.Equal(expected.NonStateIssuer, actual.NonStateIssuer);
            }

            Assert.Equal(request.VASP.NaturalPersonIds.Count(), response.VASP.NaturalPersonIds.Count());

            for (int i = 0; i < request.VASP.NaturalPersonIds.Count(); i++)
            {
                var expected = request.VASP.NaturalPersonIds[i];
                var actual = response.VASP.NaturalPersonIds[i];

                Assert.Equal(expected.IssuingCountry, actual.IssuingCountry);
                Assert.Equal(expected.IdentificationType, actual.IdentificationType);
                Assert.Equal(expected.Identifier, actual.Identifier);
                Assert.Equal(expected.NonStateIssuer, actual.NonStateIssuer);
            }
        }
    }
}
