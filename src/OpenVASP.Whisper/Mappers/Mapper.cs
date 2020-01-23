using System;
using System.Globalization;
using System.Linq;
using OpenVASP.Messaging.Messages;
using OpenVASP.ProtocolMessages.Messages;

namespace OpenVASP.Whisper.Mappers
{
    public static class Mapper
    {
        #region TO_PROTO

        public static ProtoMessage MapMessageToProto(MessageType messageType, Message message)
        {
            var proto = new ProtoMessage()
            {
                MessageCode = message.MessageCode,
                MessageId = message.MessageId,
                MessageType = (int)messageType,
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

            proto.JuridicalPersonId.Add(vaspInfo.JuridicalPersonIds.Select<JuridicalPersonId, ProtoJuridicalPersonId>(x => MapJuridicalPersonIdToProto(x)));
            proto.NaturalPersonId.Add(vaspInfo.NaturalPersonIds.Select<NaturalPersonId, ProtoNaturalPersonId>(x => MapNaturalPersonIdToProto(x)));

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

        #endregion

        #region FROM_PROTO

        public static VaspInformation MapVaspInformationFromProto(ProtoVaspInfo vaspInfo)
        {
            var proto = new VaspInformation(
                vaspInfo.Name,
                vaspInfo.VaspIdentity,
                vaspInfo.VaspPubkey,
                MapPostalAddressFromProto(vaspInfo.PostalAddress),
                MapPlaceOfBirthFromProto(vaspInfo.PlaceOfBirth),
                vaspInfo.NaturalPersonId?.Select<ProtoNaturalPersonId, NaturalPersonId>(x => MapNaturalPersonIdFromProto(x)).ToArray(),
                vaspInfo.JuridicalPersonId?.Select<ProtoJuridicalPersonId, JuridicalPersonId>(x => MapJuridicalPersonIdFromProto(x)).ToArray(),
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

        #endregion
    }
}