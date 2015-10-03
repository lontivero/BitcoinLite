using System;
using System.Linq;
using System.Numerics;
using BitcoinLite.Crypto;
using BitcoinLite.Utils;

namespace BitcoinLite.Encoding
{
	public class Base58CheckEncoder : Base58Encoder
	{
		private static readonly Base58Encoder InternalEncoder = new Base58Encoder();

		public override string Encode(byte[] data, int offset, int count)
		{
			var toEncode = new byte[count + 4];
			Buffer.BlockCopy(data, offset, toEncode, 0, count);

			var hash = Hashes.SHA256d(data, offset, count);
			Buffer.BlockCopy(hash, 0, toEncode, count, 4);

			return InternalEncoder.Encode(toEncode, 0, toEncode.Length);
		}

		public override byte[] Decode(string encoded)
		{
			var vchRet = InternalEncoder.Decode(encoded);
			if (vchRet.Length < 4)
				throw new FormatException("Invalid checked base 58 string");
			
			var calculatedHash = Hashes.SHA256d(vchRet, 0, vchRet.Length - 4).SafeSubarray(0, 4);
			var expectedHash = vchRet.SafeSubarray(vchRet.Length - 4, 4);

			if (!calculatedHash.IsEqualTo(expectedHash))
				throw new FormatException("Invalid hash of the base 58 string");
			
			vchRet = vchRet.SafeSubarray(0, vchRet.Length - 4);
			return vchRet;
		}
	}

	public class Base58Encoder : Encoder
	{
		private static readonly char[] Base58Chars = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz".ToCharArray();

		public override string Encode(byte[] data, int offset, int count)
		{
			BigInteger bn58 = 58;
			BigInteger bn0 = 0;

			var t = ByteArray.Zero.Concat(data.SafeSubarray(offset, count));
			var bn = t.ToBigIntegerUnsigned(true);

			var str = "";
			// Expected size increase from base58 conversion is approximately 137%
			// use 138% to be safe

			while(bn > bn0)
			{
				BigInteger rem;
				var dv = BigInteger.DivRem(bn, bn58, out rem);
				bn = dv;
				var c = (int)rem;
				str += Base58Chars[c];
			}

			// Leading zeroes encoded as base58 zeros
			for (int i = offset; i < offset+count && data[i] == 0; i++)
				str += Base58Chars[0];

			// Convert little endian std::string to big endian
			str = new String(str.ToCharArray().Reverse().ToArray()); //keep that way to be portable
			return str;
		}

		public override byte[] Decode(string encoded)
		{
			if (encoded == null)
				throw new ArgumentNullException("encoded");

			var result = new byte[0];
			if(encoded.Length == 0)
				return result;
			BigInteger bn58 = 58;
			BigInteger bn = 0;
			var i = 0;
			while(IsSpace(encoded[i]))
			{
				i++;
				if(i >= encoded.Length)
					return result;
			}

			for(var y = i ; y < encoded.Length ; y++)
			{
				var p1 = Array.IndexOf(Base58Chars, encoded[y]);
				if(p1 == -1)
				{
					while(IsSpace(encoded[y]))
					{
						y++;
						if(y >= encoded.Length)
							break;
					}
					if(y != encoded.Length)
						throw new FormatException("Invalid base 58 string");
					break;
				}
				var bnChar = new BigInteger(p1);
				bn = BigInteger.Multiply(bn, bn58);
				bn += bnChar;
			}

			// Get bignum as little endian data
			var t = bn.ToByteArray();
			if(t.All(b => b == 0))
				t = new byte[0];

			// Trim off sign byte if present
			if(t.Length >= 2 && t[t.Length - 1] == 0 && t[t.Length - 2] >= 0x80)
				t = t.SafeSubarray(0, t.Length - 1);

			// Restore leading zeros
			var nLeadingZeros = 0;
			for(var y = i ; y < encoded.Length && encoded[y] == Base58Chars[0] ; y++)
				nLeadingZeros++;


			result = new byte[nLeadingZeros + t.Length];
			Array.Copy(t.ToBigEndian(), 0, result, nLeadingZeros, t.Length);
			return result;
		}
	}
}
