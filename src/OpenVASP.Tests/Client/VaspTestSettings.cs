using System.Collections.Generic;
using OpenVASP.Messaging.Messages.Entities;

namespace OpenVASP.Tests
{
    public class VaspTestSettings
    {
        public object NodeRPC;
        public string VaspSmartContractAddressPerson { get; set; }
        public string VaspSmartContractAddressJuridical { get; set; }
        public string VaspSmartContractAddressBank { get; set; }

        public NaturalPersonId[] NaturalPersonIds { get; set; }
        public PlaceOfBirth PlaceOfBirth { get; set; }
        public JuridicalPersonId[] JuridicalIds { get; set; }
        public IEnumerable<char> Bic { get; set; }

        public string PersonHandshakePrivateKeyHex { get; set; }
        public string PersonSignaturePrivateKeyHex { get; set; }
        public string JuridicalHandshakePrivateKeyHex { get; set; }
        public string JuridicalSignaturePrivateKeyHex { get; set; }
        public string BankHandshakePrivateKeyHex { get; set; }
        public string BankSignaturePrivateKeyHex { get; set; }
    }
}