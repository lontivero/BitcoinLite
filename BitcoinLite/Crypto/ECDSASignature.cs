using System.Numerics;
using System;
using System.IO;
using BitcoinLite.Utils;

namespace BitcoinLite.Crypto
{
	public class ECDSASignature
	{
		private readonly static BigInteger HalfCurveOrder = (Secp256k1.N >> 1);

		public BigInteger R { get; private set; }
		public BigInteger S { get; private set; }

		public ECDSASignature(BigInteger r, BigInteger s)
		{
			R = r;
			S = s;
		}

		public byte[] ToDER()
		{
			var r = Packer.BigEndian.GetBytes(R.ToByteArray());
			var s = Packer.BigEndian.GetBytes(S.ToByteArray());
			var lenght = r.Length + s.Length + 4;

			return Packer.Pack("bbbbAbbA", 0x30, lenght, 0x02, r.Length, r, 0x02, s.Length, s);
		}

		public static ECDSASignature FromDER(byte[] sig)
		{
			if(sig.Length < 70)
				throw new FormatException("Signature is not DER formatted. " + "Signature too large or too short");
			if(sig[0] != 0x30)
				throw new FormatException("Signature is not DER formatted. " + "Header byte should be 0x30");
			if(sig[1] < 68)
				throw new FormatException("Signature is not DER formatted. " + "Wrong length byte value");

			if(sig[2] != 0x02)
				throw new FormatException("Signature is not DER formatted. " + "Integer byte for R should be 0x02");
			var rlength = sig[3];
			if(rlength != 0x20 && rlength != 0x21)
				throw new FormatException("Signature is not DER formatted. " + "Length of R incorrect");
			if(sig[4] >= 0x80  || (sig[4] == 0x00 && sig[5] < 0x80))
				throw new FormatException("Signature is not DER formatted. " + "R is not valid");

			if(sig[4 + rlength] != 0x02)
				throw new FormatException("Signature is not DER formatted. " + "Integer byte for S should be 0x02");
			var slength = sig[5 + rlength];
			if(slength != 0x20 && slength != 0x21)
				throw new FormatException("Signature is not DER formatted. " + "Length of S incorrect");
			if(sig[6 + rlength] >= 0x80 || (sig[6 + rlength] == 0x00 && sig[7 + rlength] < 0x80))
				throw new FormatException("Signature is not DER formatted. " + "R is not valid");

			if(rlength + slength + 4 != sig[1])
				throw new FormatException("Signature is not DER formatted. " + "Lenght is incorrect");

			var r = new BigInteger(sig.SafeSubarray(4, rlength).ToBigEndian());
			var s = new BigInteger(sig.SafeSubarray(4 + rlength + 2, slength).ToBigEndian());
			return new ECDSASignature(r, s);
		}

		public ECDSASignature MakeCanonical()
		{
			var isLowS = S <= HalfCurveOrder;
			return isLowS
				? this 
				: new ECDSASignature(R, Secp256k1.N - S); 
		}
	}
}
