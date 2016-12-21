using System.Numerics;
using BitcoinLite.Encoding;

namespace BitcoinLite.Crypto
{
	public static class Secp256k1
	{
		public static readonly BigInteger N = "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEBAAEDCE6AF48A03BBFD25E8CD0364141".HexToBigInteger();
		public static readonly BigInteger P = "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFFFFFC2F".HexToBigInteger();
		public static readonly ECPoint G = ECPoint.Decode(Encoders.Hex.GetBytes("0279BE667EF9DCBBAC55A06295CE870B07029BFCDB2DCE28D959F2815B16F81798"));
		public static readonly BigInteger a = "0000000000000000000000000000000000000000000000000000000000000000".HexToBigInteger();
		public static readonly BigInteger b = "0000000000000000000000000000000000000000000000000000000000000007".HexToBigInteger();

		private static BigInteger HexToBigInteger(this string hex)
		{
			return Encoders.Hex.GetBytes(hex).ToBigIntegerUnsigned(true);
		}
	}
}