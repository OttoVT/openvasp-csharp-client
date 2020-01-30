using System;

namespace OpenVASP.Messaging
{
    public static class HexExtensions
    {
        private static readonly uint[] _lookup32 = CreateLookup32();

        private static uint[] CreateLookup32()
        {
            var result = new uint[256];
            for (int i = 0; i < 256; i++)
            {
                string s = i.ToString("x2");
                result[i] = ((uint)s[0]) + ((uint)s[1] << 16);
            }
            return result;
        }

        public static string CustomToHex(this byte[] byteArray, bool prefix = false)
        {
            var lookup32 = _lookup32;
            var result = new char[byteArray.Length * 2];
            for (int i = 0; i < byteArray.Length; i++)
            {
                var val = lookup32[byteArray[i]];
                result[2 * i] = (char)val;
                result[2 * i + 1] = (char)(val >> 16);
            }

            return new string(result);
        }

        public static byte[] CustomFromHex(this string hex)
        {
            if (hex == null)
                throw new ArgumentNullException(nameof(hex));
            if (hex.Length % 2 != 0)
                throw new ArgumentException("Hexadecimal value length must be even.", nameof(hex));

            int i = 0;
            if (hex.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase))
            {
                i = 2;
            }

            byte[] arr = new byte[hex.Length >> 1];

            for (; i < hex.Length >> 1; ++i)
            {
                arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + (GetHexVal(hex[(i << 1) + 1])));
            }

            return arr;
        }

        public static int GetHexVal(char hex)
        {
            int val = (int)hex;
            //For uppercase A-F letters:
            //return val - (val < 58 ? 48 : 55);
            //For lowercase a-f letters:
            return val - (val < 58 ? 48 : 87);
            //Or the two combined, but a bit slower:
            //return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
        }
    }
}