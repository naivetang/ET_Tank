using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace ETModel
{
	public static class StringHelper
	{
		public static IEnumerable<byte> ToBytes(this string str)
		{
			byte[] byteArray = Encoding.Default.GetBytes(str);
			return byteArray;
		}

		public static byte[] ToByteArray(this string str)
		{
			byte[] byteArray = Encoding.Default.GetBytes(str);
			return byteArray;
		}

	    public static byte[] ToUtf8(this string str)
	    {
            byte[] byteArray = Encoding.UTF8.GetBytes(str);
            return byteArray;
        }

		public static byte[] HexToBytes(this string hexString)
		{
			if (hexString.Length % 2 != 0)
			{
				throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "The binary key cannot have an odd number of digits: {0}", hexString));
			}

			var hexAsBytes = new byte[hexString.Length / 2];
			for (int index = 0; index < hexAsBytes.Length; index++)
			{
				string byteValue = "";
				byteValue += hexString[index * 2];
				byteValue += hexString[index * 2 + 1];
				hexAsBytes[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
			}
			return hexAsBytes;
		}

		public static string Fmt(this string text, params object[] args)
		{
			return string.Format(text, args);
		}

		public static string ListToString<T>(this List<T> list)
		{
			StringBuilder sb = new StringBuilder();
			foreach (T t in list)
			{
				sb.Append(t);
				sb.Append(",");
			}
			return sb.ToString();
		}

        public static bool IsPhoneNum(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return false;
            }

            if (str.Length != 11)
                return false;

            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }
        public static bool IsUserName(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return false;
            }

            if (str.Contains(" "))
            {
                return false;
            }
                


            if (str.Length < 4 || str.Length > 9)
                return false;

            return true;
        }

        public static bool IsPassword(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return false;
            }

            if (str.Contains(" "))
            {
                return false;
            }

            if (str.Length < 6 || str.Length > 11)
                return false;


            return true;
        }

        public static UInt64 ToNum(this string str)
        {
            UInt64 num = 0;

            if (string.IsNullOrWhiteSpace(str))
                return num;

            foreach (char c in str)
            {
                num = num * 10 + (UInt64)(c - '0');
            }

            return num;
        }


	}
}