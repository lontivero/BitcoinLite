using System.Numerics;

namespace BitcoinLite.Crypto
{
	public class ECDsaSigner
	{
		private readonly IKProvider _kprovider;
		private readonly Key _privateKey;

		public ECDsaSigner(Key key)
			: this(new HmackKProvider(), key)
		{
		}

		public ECDsaSigner(IKProvider kprovider, Key key)
		{
			_kprovider = kprovider;
			_privateKey = key;
		}

		public ECDSASignature GenerateSignature(byte[] hash)
		{
			var n = Secp256k1.N;
			var z = CalculateZ(hash);
			var key = _privateKey.K;

			BigInteger r, s;

			do
			{
				BigInteger k;

				do 
				{
					_kprovider.Initialize(key, z.ToUByteArray().ToBigEndian());
					do
					{
						k = _kprovider.GetNextK();
					}
					while (k.IsZero || k >= n);

					var p = k * Secp256k1.G;

					r = p.X % n;
				}
				while (r.IsZero);

				s = k.ModInverse(n) * (z + key * r) % n;
			}
			while (s.IsZero);

			return new ECDSASignature(r, s).MakeCanonical();
		}

		public bool VerifySignature(byte[] message, ECDSASignature signature)
		{
			return VerifySignature(message, signature.R, signature.S);
		}

		public bool VerifySignature(byte[] message, BigInteger r, BigInteger s)
		{
			return VerifySignature(message, r, s, _privateKey.PublicPoint);
		}

		public static bool VerifySignature(byte[] message, ECDSASignature signature, PublicKey publicKey)
		{
			return VerifySignature(message, signature.R, signature.S, publicKey.Point);
		}

		public static bool VerifySignature(byte[] message, BigInteger r, BigInteger s, PublicKey publicKey)
		{
			return VerifySignature(message, r, s, publicKey.Point);
		}

		public static bool VerifySignature(byte[] message, BigInteger r, BigInteger s, ECPoint publicPoint)
		{
			var n = Secp256k1.N;

			if (r.Sign < 1 || s.Sign < 1 || r >= n || s >= n)
				return false;

			var z = CalculateZ(message);
			var w = s.ModInverse(n);

			var u1 = (z * w) % n;
			var u2 = (r * w) % n;

			var G = Secp256k1.G;
			var Q = publicPoint;

			var C = u1 * G + u2 * Q;

			if(C.IsInfinity) return false;

			var Cmodn = C.X % n;

			return Cmodn == r;
		}

		private static BigInteger CalculateZ(byte[] message)
		{
			return Hashes.SHA256(message).ToBigIntegerUnsigned(true);
		}
	}
}