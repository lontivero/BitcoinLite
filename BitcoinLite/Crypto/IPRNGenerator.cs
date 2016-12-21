using System;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using BitcoinLite.Utils;

namespace BitcoinLite.Crypto
{
	public interface IPRNGenerator
	{
		byte[] GetBytes(int count);
	}

	public class WinCryptoPrng : IPRNGenerator
	{
		public byte[] GetBytes(int count)
		{
			using(var rnd = new RNGCryptoServiceProvider())
			{
				var buffer = new byte[count];
				rnd.GetBytes(buffer);
				return buffer;
			}
		}
	}

	public interface IKProvider
	{
		void Initialize(BigInteger privateKey, byte[] msghash);
		BigInteger GetNextK();
	}

	public class HmackKProvider : IKProvider
	{
		private byte[] _v;
		private byte[] _k;

		public void Initialize(BigInteger privateKey, byte[] msghash)
		{
			_v = Enumerable.Repeat((byte)0x01, 32).ToArray();
			_k = Enumerable.Repeat((byte)0x00, 32).ToArray();
			var keyBytes = privateKey.ToUByteArray().ToBigEndian();
			var prvKey = new byte[32];
			Array.Copy(keyBytes, 0, prvKey, 32 - keyBytes.Length, keyBytes.Length);
			_k = Hashes.HMACSHA256(_k, _v.Concat(ByteArray.Zero, prvKey, msghash));
			_v = Hashes.HMACSHA256(_k, _v);
			_k = Hashes.HMACSHA256(_k, _v.Concat(ByteArray.One, prvKey, msghash));
			_v = Hashes.HMACSHA256(_k, _v);
		}

		public BigInteger GetNextK()
		{
			do
			{
				_v = Hashes.HMACSHA256(_k, _v);
				var candidateK = _v.ToBigIntegerUnsigned(true);
				if (!candidateK.IsZero && candidateK < Secp256k1.N)
					return candidateK;

				_k = Hashes.HMACSHA256(_k, _v.Concat(ByteArray.Zero));
				_v = Hashes.HMACSHA256(_k, _v);
			} while (true);
		}
	}
}
