using System;

namespace BitcoinLite.Encoding
{
	public class Base64Encoder : Encoder
	{
		public override byte[] GetBytes(string encoded)
		{
			return Convert.FromBase64String(encoded);
		}

		public override string GetString(byte[] data, int offset, int count)
		{
			return Convert.ToBase64String(data, offset, count);
		}
	}
}
