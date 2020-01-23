﻿namespace OpenVASP.Messaging.Messages.Entities
{
    public class Originator
    {
        public Originator(
            string name, 
            string vaan, 
            PostalAddress postalAddress, 
            PlaceOfBirth placeOfBirth, 
            NaturalPersonId[] naturalPersonId, 
            JuridicalPersonId[] juridicalPersonId, 
            string bic)
        {
            Name = name;
            VAAN = vaan;
            PostalAddress = postalAddress;
            PlaceOfBirth = placeOfBirth;
            NaturalPersonId = naturalPersonId;
            JuridicalPersonId = juridicalPersonId;
            BIC = bic;
        }

        public string Name { get; set; }

        public string VAAN { get; set; }

        public PostalAddress PostalAddress { get; set; }

        public PlaceOfBirth PlaceOfBirth { get; set; }

        public NaturalPersonId[] NaturalPersonId { get; set; }

        public JuridicalPersonId[] JuridicalPersonId { get; set; }

        public string BIC { get; set; }
    }
}