﻿using System.Text;

namespace OpenVASP.Messaging.Messages.Entities
{
    /// <summary>
    /// VAAN
    /// </summary>
    public class VirtualAssetssAccountNumber
    {
        private VirtualAssetssAccountNumber(VaspCode vaspCode, string customerSpecificNumberHex)
        {
            VaspCode = vaspCode;
            CustomerNumber = customerSpecificNumberHex;

            string vaan = vaspCode.Code + customerSpecificNumberHex;
            var checksum = GetChecksum8Modulo256(vaan);
            var checkSumStr = (new byte[] { checksum }).CustomToHex();

            Vaan = $"{vaan}{checkSumStr}";
        }

        public VaspCode VaspCode { get;}

        public string CustomerNumber { get; }

        public string Vaan { get; }

        public static VirtualAssetssAccountNumber Create(string vaspCode, string customerSpecificNumberHex)
        {
            var vasp = VaspCode.Create(vaspCode);
            var result = new VirtualAssetssAccountNumber(vasp, customerSpecificNumberHex);

            return result;
        }

        private static byte GetChecksum8Modulo256(string vaan)
        {
            byte[] data = vaan.CustomFromHex();
            byte check = 0;

            for (int i = 0; i < data.Length; i++)
            {
                check += data[i];
            }

            return check;
        }
    }

    public class VaspCode
    {
        private VaspCode(string vaspCodeHex)
        {
            this.Code = vaspCodeHex;
        }
        public string Code { get; }

        public static VaspCode Create(string vaspCodeHex)
        {
            var result = new VaspCode(vaspCodeHex);

            return result;
        }
    }
}
