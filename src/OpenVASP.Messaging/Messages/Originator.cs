namespace OpenVASP.Messaging.Messages
{
    public class Originator
    {
        public string Name { get; set; }

        public string VAAN { get; set; }

        public PostalAddress PostalAddress { get; set; }

        public PlaceOfBirth PlaceOfBirth { get; set; }

        public NaturalPersonId NaturalPersonId { get; set; }

        public JuridicalPersonId JuridicalPersonId { get; set; }

        public string BIC { get; set; }
    }
}