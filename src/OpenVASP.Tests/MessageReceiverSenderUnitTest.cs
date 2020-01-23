using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Signer;
using OpenVASP.Messaging;
using OpenVASP.Messaging.Messages;
using OpenVASP.Messaging.Messages.Entities;
using OpenVASP.Messaging.MessagingEngine;
using OpenVASP.Whisper;
using Xunit;

namespace OpenVASP.Tests
{
    public class MessageReceiverSenderUnitTest
    {
        [Fact]
        public async Task TestSendingSessionRequest()
        {
            var fakeTransport = new FakeTransortClient();
            SessionRequestMessage receivedMessage = null;
            var messageSender = new MessageSender(new WhisperMessageFormatter(), fakeTransport);
            var messageHandlerResolver =
                new MessageHandlerResolver(
                    (typeof(SessionRequestMessage), new SessionRequestMessageHandler((messageForProcessing, token) =>
                    {
                        receivedMessage = messageForProcessing;
                        return Task.FromResult(0);
                    })));
            var messageReceiver = new MessageReceiver(new WhisperMessageFormatter(), fakeTransport, messageHandlerResolver);
            var request = GetSessionRequestMessage();

            var messageHash = await messageSender.SendSessionRequestAsync(request);

            await messageReceiver.ReceiveMessagesAsync("");

            var response = receivedMessage;


            Assert.NotNull(response);

            Assert.Equal(request.MessageEnvelope.Topic, response.MessageEnvelope.Topic);
            //Assert.Equal(request.MessageEnvelope.Signature, response.MessageEnvelope.Signature);
            Assert.Equal(request.MessageEnvelope.EncryptionType, response.MessageEnvelope.EncryptionType);
            Assert.Equal(request.MessageEnvelope.EncryptionKey, response.MessageEnvelope.EncryptionKey);

            Assert.Equal(request.HandShake.TopicA, response.HandShake.TopicA);
            Assert.Equal(request.HandShake.ECDHPublicKey, response.HandShake.ECDHPublicKey);

            Assert.Equal(request.Comment, response.Comment);

            Assert.Equal(request.Message.SessionId, response.Message.SessionId);
            Assert.Equal(request.MessageType, response.MessageType);
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

            AssertJuridicalPersonIds(request.VASP.JuridicalPersonIds, response.VASP.JuridicalPersonIds);

            Assert.Equal(request.VASP.NaturalPersonIds.Count(), response.VASP.NaturalPersonIds.Count());

            AssertNaturalPersonIds(request.VASP.NaturalPersonIds, response.VASP.NaturalPersonIds);
        }

        private static void AssertNaturalPersonIds(NaturalPersonId[] request, NaturalPersonId[] response)
        {
            for (int i = 0; i < request.Count(); i++)
            {
                var expected = request[i];
                var actual = response[i];

                Assert.Equal(expected.IssuingCountry, actual.IssuingCountry);
                Assert.Equal(expected.IdentificationType, actual.IdentificationType);
                Assert.Equal(expected.Identifier, actual.Identifier);
                Assert.Equal(expected.NonStateIssuer, actual.NonStateIssuer);
            }
        }

        private static void AssertJuridicalPersonIds(JuridicalPersonId[] request, JuridicalPersonId[] response)
        {
            for (int i = 0; i < request.Count(); i++)
            {
                var expected = request[i];
                var actual = response[i];

                Assert.Equal(expected.IssuingCountry, actual.IssuingCountry);
                Assert.Equal(expected.IdentificationType, actual.IdentificationType);
                Assert.Equal(expected.Identifier, actual.Identifier);
                Assert.Equal(expected.NonStateIssuer, actual.NonStateIssuer);
            }
        }

        private static SessionRequestMessage GetSessionRequestMessage()
        {
            //Should be a contract
            var vaspKey = EthECKey.GenerateKey();

            //4Bytes 
            var topic = "0x12345678"; //"0x" + "My Topic".GetHashCode().ToString("x");

            string ecdhPubKey;
            using (ECDiffieHellman ecdh = ECDiffieHellman.Create())
            {
                var byteArray = ecdh.PublicKey.ToByteArray();
                ecdhPubKey = byteArray.ToHex(prefix: false);
            }

            var message = new Message(
                Guid.NewGuid().ToByteArray().ToHex(prefix: false),
                Guid.NewGuid().ToByteArray().ToHex(prefix: false),
                "1");
            var handshake = new HandShakeRequest(topic, ecdhPubKey);
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
                    EncryptionKey = "123",
                    Topic = topic,
                    Signature = "123"
                },
                Comment = "This is test message",
            };
            return request;
        }
    }
}
