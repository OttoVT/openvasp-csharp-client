using System;
using System.Globalization;
using System.Linq;
using OpenVASP.Messaging.Messages;
using OpenVASP.ProtocolMessages.Messages;

namespace OpenVASP.Whisper
{
    public static class Mapper
    {
        #region TO_PROTO
        public static ProtoSessionRequestMessage MapSessionRequestMessageToProto(SessionRequestMessage message)
        {
            var proto = new ProtoSessionRequestMessage()
            {
                Comment = message.Comment,
                EcdshPubKey = message.HandShake.ECDHPublicKey,
                Message = MapMessageToProto(message.Message),
                TopicA = message.HandShake.TopicA,
                VaspInfo = MapVaspInformationToProto(message.VASP)
            };

            return proto;
        }

        public static ProtoMessage MapMessageToProto(Message message)
        {
            var proto = new ProtoMessage()
            {
                MessageCode = message.MessageCode,
                MessageId = message.MessageId,
                MessageType = (int)message.MessageType,
                SessionId = message.SessionId
            };

            return proto;
        }

        public static ProtoVaspInfo MapVaspInformationToProto(VaspInformation vaspInfo)
        {
            var proto = new ProtoVaspInfo()
            {
                Name = vaspInfo.Name,
                PlaceOfBirth = MapPlaceOfBirthToProto(vaspInfo.PlaceOfBirth),
                PostalAddress = MapPostalAddressToProto(vaspInfo.PostalAddress),
                VaspIdentity = vaspInfo.VaspIdentity,
                VaspPubkey = vaspInfo.VaspPublickKey,
                Bic = vaspInfo.BIC
            };

            proto.JuridicalPersonId.Add(vaspInfo.JuridicalPersonIds.Select(x => MapJuridicalPersonIdToProto(x)));
            proto.NaturalPersonId.Add(vaspInfo.NaturalPersonIds.Select(x => MapNaturalPersonIdToProto(x)));

            return proto;
        }

        public static ProtoNaturalPersonId MapNaturalPersonIdToProto(NaturalPersonId naturalPersonId)
        {
            if (naturalPersonId == null)
            {
                return null;
            }

            var proto = new ProtoNaturalPersonId()
            {
               IdentificationType = (int)naturalPersonId.IdentificationType,
               Identifier = naturalPersonId.Identifier,
               IssuingCountry = naturalPersonId.IssuingCountry.TwoLetterCode,
               NonstateIssuer = naturalPersonId.NonStateIssuer ?? string.Empty
            };

            return proto;
        }

        public static ProtoJuridicalPersonId MapJuridicalPersonIdToProto(JuridicalPersonId juridicalPersonId)
        {
            if (juridicalPersonId == null)
            {
                return null;
            }

            var proto = new ProtoJuridicalPersonId()
            {
                IdentificationType = (int)juridicalPersonId.IdentificationType,
                IssuingCountry = juridicalPersonId.IssuingCountry.TwoLetterCode,
                Identifier = juridicalPersonId.Identifier,
                NonstateIssuer = juridicalPersonId.NonStateIssuer
            };

            return proto;
        }

        public static ProtoPlaceOfBirth MapPlaceOfBirthToProto(PlaceOfBirth placeOfBirth)
        {
            if (placeOfBirth == null)
            {
                return null;
            }

            var proto = new ProtoPlaceOfBirth()
            {
                CityOfBirth = placeOfBirth.CityOfBirth,
                CountryOfBirth = placeOfBirth.CountryOfBirth.TwoLetterCode,
                Date = placeOfBirth.DateOfBirth.ToString("yyyyMMdd", CultureInfo.InvariantCulture)
            };

            return proto;
        }

        public static ProtoPostalAddress MapPostalAddressToProto(PostalAddress postalAddress)
        {
            if (postalAddress == null)
            {
                return null;
            }

            var proto = new ProtoPostalAddress()
            {
                AddressLine = postalAddress.AddressLine,
                BuildingNumber = postalAddress.BuildingNumber,
                Country = postalAddress.Country.TwoLetterCode,
                PostCode = postalAddress.PostCode,
                StreetName = postalAddress.StreetName,
                TownName = postalAddress.TownName
            };

            return proto;
        }

        #endregion TO_PROTO

        #region FROM_PROTO

        public static SessionRequestMessage MapSessionRequestMessageFromProto(ProtoSessionRequestMessage message)
        {
            var messageIn = new Message(
                MessageType.SessionRequest, 
                message.Message.MessageId,
                message.Message.SessionId,
                message.Message.MessageCode);
            var handshake = new HandShake(message.TopicA, null, message.EcdshPubKey);
            var vasp = MapVaspInformationFromProto(message.VaspInfo);

            var proto = new SessionRequestMessage(messageIn, handshake, vasp)
            {
                Comment = message.Comment,
            };

            return proto;
        }

        public static VaspInformation MapVaspInformationFromProto(ProtoVaspInfo vaspInfo)
        {
            var proto = new VaspInformation(
                vaspInfo.Name,
                vaspInfo.VaspIdentity,
                vaspInfo.VaspPubkey,
                MapPostalAddressFromProto(vaspInfo.PostalAddress),
                MapPlaceOfBirthFromProto(vaspInfo.PlaceOfBirth),
                vaspInfo.NaturalPersonId?.Select(x => MapNaturalPersonIdFromProto(x)).ToArray(),
                vaspInfo.JuridicalPersonId?.Select(x => MapJuridicalPersonIdFromProto(x)).ToArray(),
                vaspInfo.Bic);

            return proto;
        }

        public static NaturalPersonId MapNaturalPersonIdFromProto(ProtoNaturalPersonId naturalPersonId)
        {
            if (naturalPersonId == null)
            {
                return null;
            }

            Country.List.TryGetValue(naturalPersonId.IssuingCountry, out var country);
            var proto = new NaturalPersonId(
                naturalPersonId.Identifier,
                (NaturalIdentificationType) naturalPersonId.IdentificationType,
                country,
                naturalPersonId.NonstateIssuer);

            return proto;
        }

        public static JuridicalPersonId MapJuridicalPersonIdFromProto(ProtoJuridicalPersonId juridicalPersonId)
        {
            if (juridicalPersonId == null)
            {
                return null;
            }

            Country.List.TryGetValue(juridicalPersonId.IssuingCountry, out var country);
            var proto = new JuridicalPersonId(
                juridicalPersonId.Identifier, 
                (JuridicalIdentificationType)juridicalPersonId.IdentificationType, 
                country,
                juridicalPersonId.NonstateIssuer);

            return proto;
        }

        public static PlaceOfBirth MapPlaceOfBirthFromProto(ProtoPlaceOfBirth placeOfBirth)
        {
            if (placeOfBirth == null)
            {
                return null;
            }

            Country.List.TryGetValue(placeOfBirth.CountryOfBirth, out var country);
            DateTime.TryParseExact(placeOfBirth.Date, "yyyyMMdd",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateOfBirth);
            var proto = new PlaceOfBirth(dateOfBirth, placeOfBirth.CityOfBirth, country);

            return proto;
        }

        public static PostalAddress MapPostalAddressFromProto(ProtoPostalAddress postalAddress)
        {
            if (postalAddress == null)
            {
                return null;
            }

            Country.List.TryGetValue(postalAddress.Country, out var country);
            var proto = new PostalAddress(
                postalAddress.StreetName,
                postalAddress.BuildingNumber,
                postalAddress.AddressLine,
                postalAddress.PostCode,
                postalAddress.TownName,
                country);

            return proto;
        }

        #endregion FROM_PROTO
    }
}
