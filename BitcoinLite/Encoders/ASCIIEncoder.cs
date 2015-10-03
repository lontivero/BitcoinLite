using System;
using System.Linq;

namespace BitcoinLite.Encoding
{
	public class ASCIIEncoder : Encoder
	{
		//Do not using Encoding.ASCII (not portable)
		public override byte[] Decode(string encoded)
		{
			if(String.IsNullOrEmpty(encoded))
				return new byte[0];
			return encoded.ToCharArray().Select(o => (byte)o).ToArray();
		}

		public override string Encode(byte[] data, int offset, int count)
		{
			return new String(data.Skip(offset).Take(count).Select(o => (char)o).ToArray()).Replace("\0", "");
		}
	}
}
