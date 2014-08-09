using System;
using System.Text;

namespace Prototype
{
    public static class CryptoUtils
    {
        public static string XOr(string num1AsHex, string num2AsHex)
        {
            if (num1AsHex.Length < num2AsHex.Length)
            {
                //Pad num1AsHex
                num1AsHex = num1AsHex.PadLeft(num2AsHex.Length, '0');
            }
            else if (num1AsHex.Length > num2AsHex.Length)
            {
                //Pad num2AsHex
                num2AsHex = num2AsHex.PadLeft(num1AsHex.Length, '0');
            }

            StringBuilder sb = new StringBuilder(num1AsHex.Length);

            for (int i = 0; i < num1AsHex.Length; i++)
            {
                var xorResult = Convert.ToInt32(num1AsHex[i].ToString(), 16) ^ Convert.ToInt32(num2AsHex[i].ToString(), 16);
                sb.Append(xorResult.ToString("x"));
            }

            return sb.ToString();
        }

        public static int GCD(int a, int b)
        {
            return b == 0 ? a : GCD(b, a%b);
        }
    }
}
