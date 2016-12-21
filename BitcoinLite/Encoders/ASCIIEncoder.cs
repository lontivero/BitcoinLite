using System;
using System.Linq;
using BitcoinLite.Utils;

namespace BitcoinLite.Encoding
{
	public class ASCIIEncoder : Encoder
	{
		public override byte[] GetBytes(string encoded)
		{
			if(string.IsNullOrEmpty(encoded))
				return ByteArray.Empty;
			return encoded.ToCharArray().Select(o => (byte)o).ToArray();
		}

		public override string GetString(byte[] data, int offset, int count)
		{
			return new string(data.Skip(offset).Take(count).Select(o => (char)o).ToArray()).Replace("\0", "");
		}
	}
}
