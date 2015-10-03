namespace BitcoinLite.Encoding
{
	public abstract class Encoder
	{
		// char.IsWhiteSpace fits well but it match other whitespaces 
		// characters too and also works for unicode characters.
		public static bool IsSpace(char c)
		{
			switch(c) {
				case ' ':
				case '\t':
				case '\n':
				case '\v':
				case '\f':
				case '\r':
					return true;
			}
			return false;
		}

		internal Encoder()
		{
		}

		public string Encode(byte[] data)
		{
			return Encode(data, 0, data.Length);
		}

		public abstract string Encode(byte[] data, int offset, int count);

		public abstract byte[] Decode(string encoded);
	}

	public static class Encoders
	{
		public static HexEncoder Hex { get; private set; }
		public static ASCIIEncoder ASCII { get; private set; }
		public static Base58Encoder Base58 { get; private set; }
		public static Base58CheckEncoder Base58Check { get; private set; }
		public static Base64Encoder Base64 { get; private set; }

		static Encoders()
		{
			Base64 = new Base64Encoder();
			Base58Check = new Base58CheckEncoder();
			Base58 = new Base58Encoder();
			Hex = new HexEncoder();
			ASCII = new ASCIIEncoder();
		}
	}
}
