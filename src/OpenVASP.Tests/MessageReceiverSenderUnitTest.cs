using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Signer;
using OpenVASP.Messaging;
using OpenVASP.Messaging.Messages;
using OpenVASP.Messaging.Messages.Entities;
using OpenVASP.Messaging.MessagingEngine;
using OpenVASP.Whisper;
using Xunit;
using Transaction = OpenVASP.Messaging.Messages.Entities.Transaction;

namespace OpenVASP.Tests
{
    public class MessageReceiverSenderUnitTest
    {
        private static MessageEnvelope _envelope = new MessageEnvelope()
        {
            SigningKey = "0x74152a90669ef4c166a1d2b140d307181f262142486881f91a9277ee370960d9"
        };

        [Fact]
        public async Task TestSendingSessionRequestMessage()
        {
            var request = GetSessionRequestMessage();

            var fakeTransport = new FakeTransportClient(new WhisperMessageFormatter(), new WhisperSignService());
            var messageHash = await fakeTransport.SendAsync(_envelope, request);
            var receivedMessage =  (await fakeTransport.GetSessionMessagesAsync("fake")).First();

            AssertSessionRequest(receivedMessage.Message as SessionRequestMessage, request);
        }

        [Fact]
        public async Task TestSendingSessionReplyMessage()
        {
            var request = GetSessionReplyMessage();

            var fakeTransport = new FakeTransportClient(new WhisperMessageFormatter(), new WhisperSignService());
            var messageHash = await fakeTransport.SendAsync(_envelope, request);
            var response = (await fakeTransport.GetSessionMessagesAsync("fake")).First();

            AssertSessionReply(response.Message as SessionReplyMessage, request);
        }

        [Fact]
        public async Task TestSendingTransferRequestMessage()
        {
            var request = GetTransferRequestMessage();

            var fakeTransport = new FakeTransportClient(new WhisperMessageFormatter(), new WhisperSignService());
            var messageHash = await fakeTransport.SendAsync(_envelope, request);
            var response = (await fakeTransport.GetSessionMessagesAsync("fake")).First();

            AssertTransferRequest(response.Message as TransferRequestMessage, request);
        }

        [Fact]
        public async Task TestSendingTransferReplyMessage()
        {
            var request = GetTransferReplyMessage();

            var fakeTransport = new FakeTransportClient(new WhisperMessageFormatter(), new WhisperSignService());
            var messageHash = await fakeTransport.SendAsync(_envelope, request);
            var response = (await fakeTransport.GetSessionMessagesAsync("fake")).First();

            AssertTransferReply(response.Message as TransferReplyMessage, request);
        }

        [Fact]
        public async Task TestSendingTransferDispatchMessage()
        {
            var request = GetTransferDispatchMessage();

            var fakeTransport = new FakeTransportClient(new WhisperMessageFormatter(), new WhisperSignService());
            var messageHash = await fakeTransport.SendAsync(_envelope, request);
            var response = (await fakeTransport.GetSessionMessagesAsync("fake")).First();

            AssertTransferDispatch(response.Message as TransferDispatchMessage, request);
        }

        [Fact]
        public async Task TestSendingTransferConfirmationMessage()
        {
            var request = GetTransferConfirmationMessage();

            var fakeTransport = new FakeTransportClient(new WhisperMessageFormatter(), new WhisperSignService());
            var messageHash = await fakeTransport.SendAsync(_envelope, request);
            var response = (await fakeTransport.GetSessionMessagesAsync("fake")).First();

            AssertTransferConfirmation(response.Message as TransferConfirmationMessage, request);
        }

        [Fact]
        public async Task TestSendingTerminationMessage()
        {
            var request = GetTerminationMessage();
            var fakeTransport = new FakeTransportClient(new WhisperMessageFormatter(), new WhisperSignService());
            var messageHash = await fakeTransport.SendAsync(_envelope, request);
            var response = (await fakeTransport.GetSessionMessagesAsync("fake")).First();

            AssertTermination(response.Message as TerminationMessage, request);
        }

        private void AssertTermination(TerminationMessage response, TerminationMessage request)
        {
            Assert.NotNull(response);

            Assert.Equal(request.MessageEnvelope.Topic, response.MessageEnvelope.Topic);
            Assert.Equal(request.MessageEnvelope.EncryptionType, response.MessageEnvelope.EncryptionType);
            Assert.Equal(request.MessageEnvelope.EncryptionKey, response.MessageEnvelope.EncryptionKey);

            Assert.Equal(request.Comment, response.Comment);

            Assert.Equal(request.Message.SessionId, response.Message.SessionId);
            Assert.Equal(request.MessageType, response.MessageType);
            Assert.Equal(request.Message.MessageCode, response.Message.MessageCode);
            Assert.Equal(request.Message.MessageId, response.Message.MessageId);

            AssertPlaceOfBirth(request.VASP.PlaceOfBirth, response.VASP.PlaceOfBirth);

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

        private void AssertTransferDispatch(TransferDispatchMessage response, TransferDispatchMessage request)
        {
            Assert.NotNull(response);

            Assert.Equal(request.MessageEnvelope.Topic, response.MessageEnvelope.Topic);
            Assert.Equal(request.MessageEnvelope.EncryptionType, response.MessageEnvelope.EncryptionType);
            Assert.Equal(request.MessageEnvelope.EncryptionKey, response.MessageEnvelope.EncryptionKey);

            Assert.Equal(request.Comment, response.Comment);

            Assert.Equal(request.Message.SessionId, response.Message.SessionId);
            Assert.Equal(request.MessageType, response.MessageType);
            Assert.Equal(request.Message.MessageCode, response.Message.MessageCode);
            Assert.Equal(request.Message.MessageId, response.Message.MessageId);

            AssertPlaceOfBirth(request.VASP.PlaceOfBirth, response.VASP.PlaceOfBirth);

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

            Assert.Equal(request.Transfer.TransferType, response.Transfer.TransferType);
            Assert.Equal(request.Transfer.VirtualAssetType, response.Transfer.VirtualAssetType);
            Assert.Equal(request.Transfer.Amount, response.Transfer.Amount);
            Assert.Equal(request.Transfer.DestinationAddress, response.Transfer.DestinationAddress);

            AssertBeneficiary(request.Beneficiary, response.Beneficiary);

            AssertOriginator(request.Originator, response.Originator);

            AssertTransaction(request.Transaction, response.Transaction);
        }

        private void AssertTransferConfirmation(TransferConfirmationMessage response, TransferConfirmationMessage request)
        {
            Assert.NotNull(response);

            Assert.Equal(request.MessageEnvelope.Topic, response.MessageEnvelope.Topic);
            Assert.Equal(request.MessageEnvelope.EncryptionType, response.MessageEnvelope.EncryptionType);
            Assert.Equal(request.MessageEnvelope.EncryptionKey, response.MessageEnvelope.EncryptionKey);

            Assert.Equal(request.Comment, response.Comment);

            Assert.Equal(request.Message.SessionId, response.Message.SessionId);
            Assert.Equal(request.MessageType, response.MessageType);
            Assert.Equal(request.Message.MessageCode, response.Message.MessageCode);
            Assert.Equal(request.Message.MessageId, response.Message.MessageId);

            AssertPlaceOfBirth(request.VASP.PlaceOfBirth, response.VASP.PlaceOfBirth);

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

            Assert.Equal(request.Transfer.TransferType, response.Transfer.TransferType);
            Assert.Equal(request.Transfer.VirtualAssetType, response.Transfer.VirtualAssetType);
            Assert.Equal(request.Transfer.Amount, response.Transfer.Amount);
            Assert.Equal(request.Transfer.DestinationAddress, response.Transfer.DestinationAddress);

            AssertBeneficiary(request.Beneficiary, response.Beneficiary);

            AssertOriginator(request.Originator, response.Originator);

            AssertTransaction(request.Transaction, response.Transaction);
        }

        private void AssertTransaction(Transaction request, Transaction response)
        {
            Assert.Equal(request.TransactionId, response.TransactionId);
            Assert.Equal(request.DateTime.Date, response.DateTime.Date);
            Assert.Equal(request.DateTime.Hour, response.DateTime.Hour);
            Assert.Equal(request.DateTime.Minute, response.DateTime.Minute);
            Assert.Equal(request.DateTime.Second, response.DateTime.Second);
            Assert.Equal(request.SendingAddress, response.SendingAddress);
        }

        private static void AssertSessionRequest(SessionRequestMessage response, SessionRequestMessage request)
        {
            Assert.NotNull(response);

            Assert.Equal(request.MessageEnvelope.Topic, response.MessageEnvelope.Topic);
            //Assert.Equal(request.MessageEnvelope.Signature, response.MessageEnvelope.Signature);
            Assert.Equal(request.MessageEnvelope.EncryptionType, response.MessageEnvelope.EncryptionType);
            Assert.Equal(request.MessageEnvelope.EncryptionKey, response.MessageEnvelope.EncryptionKey);

            Assert.Equal(request.HandShake.TopicA, response.HandShake.TopicA);
            Assert.Equal(request.HandShake.EcdhPubKey, response.HandShake.EcdhPubKey);

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

        private static void AssertSessionReply(SessionReplyMessage response, SessionReplyMessage request)
        {
            Assert.NotNull(response);

            Assert.Equal(request.MessageEnvelope.Topic, response.MessageEnvelope.Topic);
            Assert.Equal(request.MessageEnvelope.EncryptionType, response.MessageEnvelope.EncryptionType);
            Assert.Equal(request.MessageEnvelope.EncryptionKey, response.MessageEnvelope.EncryptionKey);

            Assert.Equal(request.HandShake.TopicB, response.HandShake.TopicB);

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

        private static void AssertTransferRequest(TransferRequestMessage response, TransferRequestMessage request)
        {
            Assert.NotNull(response);

            Assert.Equal(request.MessageEnvelope.Topic, response.MessageEnvelope.Topic);
            Assert.Equal(request.MessageEnvelope.EncryptionType, response.MessageEnvelope.EncryptionType);
            Assert.Equal(request.MessageEnvelope.EncryptionKey, response.MessageEnvelope.EncryptionKey);

            Assert.Equal(request.Comment, response.Comment);

            Assert.Equal(request.Message.SessionId, response.Message.SessionId);
            Assert.Equal(request.MessageType, response.MessageType);
            Assert.Equal(request.Message.MessageCode, response.Message.MessageCode);
            Assert.Equal(request.Message.MessageId, response.Message.MessageId);

            AssertPlaceOfBirth(request.VASP.PlaceOfBirth, response.VASP.PlaceOfBirth);

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

            Assert.Equal(request.Transfer.TransferType, response.Transfer.TransferType);
            Assert.Equal(request.Transfer.VirtualAssetType, response.Transfer.VirtualAssetType);
            Assert.Equal(request.Transfer.Amount, response.Transfer.Amount);

            AssertBeneficiary(request.Beneficiary, response.Beneficiary);

            AssertOriginator(request.Originator, response.Originator);
        }

        private static void AssertTransferReply(TransferReplyMessage response, TransferReplyMessage request)
        {
            Assert.NotNull(response);

            Assert.Equal(request.MessageEnvelope.Topic, response.MessageEnvelope.Topic);
            Assert.Equal(request.MessageEnvelope.EncryptionType, response.MessageEnvelope.EncryptionType);
            Assert.Equal(request.MessageEnvelope.EncryptionKey, response.MessageEnvelope.EncryptionKey);

            Assert.Equal(request.Comment, response.Comment);

            Assert.Equal(request.Message.SessionId, response.Message.SessionId);
            Assert.Equal(request.MessageType, response.MessageType);
            Assert.Equal(request.Message.MessageCode, response.Message.MessageCode);
            Assert.Equal(request.Message.MessageId, response.Message.MessageId);

            AssertPlaceOfBirth(request.VASP.PlaceOfBirth, response.VASP.PlaceOfBirth);

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

            Assert.Equal(request.Transfer.TransferType, response.Transfer.TransferType);
            Assert.Equal(request.Transfer.VirtualAssetType, response.Transfer.VirtualAssetType);
            Assert.Equal(request.Transfer.Amount, response.Transfer.Amount);
            Assert.Equal(request.Transfer.DestinationAddress, response.Transfer.DestinationAddress);

            AssertBeneficiary(request.Beneficiary, response.Beneficiary);

            AssertOriginator(request.Originator, response.Originator);
        }

        private static void AssertPlaceOfBirth(PlaceOfBirth request, PlaceOfBirth response)
        {
            Assert.Equal(request.DateOfBirth.Date, response.DateOfBirth.Date);
            Assert.Equal(request.CountryOfBirth, response.CountryOfBirth);
            Assert.Equal(request.CityOfBirth, response.CityOfBirth);
        }

        private static void AssertOriginator(Originator requestOriginator, Originator responseOriginator)
        {
            Assert.Equal(requestOriginator.BIC, requestOriginator.BIC);
            Assert.Equal(requestOriginator.VAAN, requestOriginator.VAAN);
            Assert.Equal(requestOriginator.Name, requestOriginator.Name);

            AssertPlaceOfBirth(requestOriginator.PlaceOfBirth, responseOriginator.PlaceOfBirth);
            AssertPostalAddress(requestOriginator.PostalAddress, responseOriginator.PostalAddress);

            AssertJuridicalPersonIds(requestOriginator.JuridicalPersonId, responseOriginator.JuridicalPersonId);
            AssertNaturalPersonIds(requestOriginator.NaturalPersonId, responseOriginator.NaturalPersonId);
        }

        private static void AssertPostalAddress(PostalAddress request, PostalAddress response)
        {
            Assert.Equal(request.StreetName, response.StreetName);
            Assert.Equal(request.AddressLine, response.AddressLine);
            Assert.Equal(request.BuildingNumber, response.BuildingNumber);
            Assert.Equal(request.Country, response.Country);
            Assert.Equal(request.PostCode, response.PostCode);
        }

        private static void AssertBeneficiary(Beneficiary requestBeneficiary, Beneficiary responseBeneficiary)
        {
            Assert.Equal(requestBeneficiary.Name, responseBeneficiary.Name);
            Assert.Equal(requestBeneficiary.VAAN, responseBeneficiary.VAAN);
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
                    SigningKey = "123"
                },
                Comment = "This is test message",
            };
            return request;
        }

        private static SessionReplyMessage GetSessionReplyMessage()
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
            var handshake = new HandShakeResponse(topic);
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

            var request = new SessionReplyMessage(message, handshake, vaspInformation)
            {
                MessageEnvelope = new MessageEnvelope()
                {
                    EncryptionType = EncryptionType.Assymetric,
                    EncryptionKey = "123",
                    Topic = topic,
                    SigningKey = "123"
                },
                Comment = "This is test message",
            };

            return request;
        }

        private static TransferRequestMessage GetTransferRequestMessage()
        {
            //Should be a contract
            var vaspKey = EthECKey.GenerateKey();

            //4Bytes 
            var topic = "0x12345678"; //"0x" + "My Topic".GetHashCode().ToString("x");

            var message = new Message(
                Guid.NewGuid().ToByteArray().ToHex(prefix: false),
                Guid.NewGuid().ToByteArray().ToHex(prefix: false),
                "1");

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

            var originator = new Originator("Originator1", "VaaN", postalAddress, placeOfBirth,
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

            var beneficiary = new Beneficiary("Ben1", "VaaN");

            var transferRequest = new TransferRequest(VirtualAssetType.ETH, TransferType.BlockchainTransfer, "10000000");

            var request = new TransferRequestMessage(message, originator, beneficiary, transferRequest, vaspInformation)
            {
                MessageEnvelope = new MessageEnvelope()
                {
                    EncryptionType = EncryptionType.Assymetric,
                    EncryptionKey = "123",
                    Topic = topic,
                    SigningKey = "123"
                },
                Comment = "This is test message",
            };

            return request;
        }

        private static TransferReplyMessage GetTransferReplyMessage()
        {
            //Should be a contract
            var vaspKey = EthECKey.GenerateKey();

            //4Bytes 
            var topic = "0x12345678"; //"0x" + "My Topic".GetHashCode().ToString("x");

            var message = new Message(
                Guid.NewGuid().ToByteArray().ToHex(prefix: false),
                Guid.NewGuid().ToByteArray().ToHex(prefix: false),
                "1");

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

            var originator = new Originator("Originator1", "VaaN", postalAddress, placeOfBirth,
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

            var beneficiary = new Beneficiary("Ben1", "VaaN");

            var transferReply = new TransferReply(VirtualAssetType.ETH, TransferType.BlockchainTransfer, "10000000", "0x0000001");

            var request = new TransferReplyMessage(message, originator, beneficiary, transferReply, vaspInformation)
            {
                MessageEnvelope = new MessageEnvelope()
                {
                    EncryptionType = EncryptionType.Assymetric,
                    EncryptionKey = "123",
                    Topic = topic,
                    SigningKey = "123"
                },
                Comment = "This is test message",
            };

            return request;
        }

        private static TransferDispatchMessage GetTransferDispatchMessage()
        {
            //Should be a contract
            var vaspKey = EthECKey.GenerateKey();

            //4Bytes 
            var topic = "0x12345678"; //"0x" + "My Topic".GetHashCode().ToString("x");

            var message = new Message(
                Guid.NewGuid().ToByteArray().ToHex(prefix: false),
                Guid.NewGuid().ToByteArray().ToHex(prefix: false),
                "1");

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

            var originator = new Originator("Originator1", "VaaN", postalAddress, placeOfBirth,
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

            var beneficiary = new Beneficiary("Ben1", "VaaN");

            var transferReply = new TransferReply(VirtualAssetType.ETH, TransferType.BlockchainTransfer, "10000000", "0x0000001");
            var transaction = new Transaction("txId", DateTime.UtcNow, "0x0000002");

            var request = new TransferDispatchMessage(message, originator, beneficiary, transferReply, transaction, vaspInformation)
            {
                MessageEnvelope = new MessageEnvelope()
                {
                    EncryptionType = EncryptionType.Assymetric,
                    EncryptionKey = "123",
                    Topic = topic,
                    SigningKey = "123"
                },
                Comment = "This is test message",
            };

            return request;
        }

        private static TransferConfirmationMessage GetTransferConfirmationMessage()
        {
            //Should be a contract
            var vaspKey = EthECKey.GenerateKey();

            //4Bytes 
            var topic = "0x12345678"; //"0x" + "My Topic".GetHashCode().ToString("x");

            var message = new Message(
                Guid.NewGuid().ToByteArray().ToHex(prefix: false),
                Guid.NewGuid().ToByteArray().ToHex(prefix: false),
                "1");

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

            var originator = new Originator("Originator1", "VaaN", postalAddress, placeOfBirth,
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

            var beneficiary = new Beneficiary("Ben1", "VaaN");

            var transferReply = new TransferReply(VirtualAssetType.ETH, TransferType.BlockchainTransfer, "10000000", "0x0000001");
            var transaction = new Transaction("txId", DateTime.UtcNow, "0x0000002");

            var request = new TransferConfirmationMessage(message, originator, beneficiary, transferReply, transaction, vaspInformation)
            {
                MessageEnvelope = new MessageEnvelope()
                {
                    EncryptionType = EncryptionType.Assymetric,
                    EncryptionKey = "123",
                    Topic = topic,
                    SigningKey = "123"
                },
                Comment = "This is test message",
            };

            return request;
        }

        private static TerminationMessage GetTerminationMessage()
        {
            //Should be a contract
            var vaspKey = EthECKey.GenerateKey();

            //4Bytes 
            var topic = "0x12345678"; //"0x" + "My Topic".GetHashCode().ToString("x");

            var message = new Message(
                Guid.NewGuid().ToByteArray().ToHex(prefix: false),
                Guid.NewGuid().ToByteArray().ToHex(prefix: false),
                "1");
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

            var request = new TerminationMessage(message, vaspInformation)
            {
                MessageEnvelope = new MessageEnvelope()
                {
                    EncryptionType = EncryptionType.Assymetric,
                    EncryptionKey = "123",
                    Topic = topic,
                    SigningKey = "123"
                },
                Comment = "This is test message",
            };
            return request;
        }
    }
}
