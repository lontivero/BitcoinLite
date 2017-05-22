using System;
using System.Linq;
using System.Numerics;
using BitcoinLite.Utils;

namespace BitcoinLite.Crypto
{
	public class SchnorrSigner
	{
		private readonly IKProvider _kprovider;
		private readonly Key _privateKey;

		public SchnorrSigner(Key key)
			: this(new HmackKProvider(), key)
		{
		}

		public SchnorrSigner(IKProvider kprovider, Key key)
		{
			_kprovider = kprovider;
			_privateKey = key;
		}

		public BigInteger[] GenerateSignature(byte[] message)
		{
			BigInteger k = new BigInteger(123); //_kprovider.GetNextK();
			ECPoint r = k * Secp256k1.G; //R
			BigInteger e = CalculateHash(r.Encode(true), message);
			BigInteger x = new BigInteger(_privateKey.ToByteArray());
			BigInteger s = (k - x * e) % Secp256k1.P;
			
			return new [] {s, e};
		}

		//public bool VerifySignature(byte[] message, BigInteger s, BigInteger e)
		//{
		//	BigInteger rv1 = (Secp256k1.G * s).Encode(true).ToBigInteger();
		//	BigInteger rv2 = (_privateKey.PublicPoint * e).Encode(true).ToBigInteger();
		//	BigInteger rv =   rv1 + rv2;
		//	BigInteger ev = CalculateHash(rv.ToByteArray(), message);
		//	return ev.Equals(e);
		//}

		public bool VerifySignature(byte[] message, BigInteger s, BigInteger e)
		{
			var rv1 = (Secp256k1.G * s);
			var rv2 = (_privateKey.PublicPoint * e);
			var rv = (rv1 + rv2);
			BigInteger ev = CalculateHash(rv.Encode(true), message);
			return ev.Equals(e);
		}

		private BigInteger CalculateHash(byte[] r, byte[] message)
		{
			var hash = Hashes.SHA256(message.Concat(r));
			return hash.ToBigInteger();
		}
	}

	class MyClass
	{
		public void t()
		{
			var message = System.Text.Encoding.ASCII.GetBytes("Hello");
			var keyRaw = Enumerable.Range(1, 32).Select(x=> (byte)x).ToArray();
			var signer = new SchnorrSigner(new Key(keyRaw));
			var sig = signer.GenerateSignature(message);
			var valid = signer.VerifySignature(message, sig[0], sig[1]);
			if(!valid) throw new Exception("Error");
		}
	}
}