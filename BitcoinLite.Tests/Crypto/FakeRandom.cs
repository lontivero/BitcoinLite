using System;
using System.Numerics;
using BitcoinLite.Crypto;

namespace BitcoinLite.Tests.Crypto
{
	class FakeRandom : IKProvider
	{
		private readonly byte[] _n;

		public FakeRandom(byte[] n)
		{
			_n = n;
			Array.Reverse(_n);
			if (_n[_n.Length - 1] > 0x7F)
			{
				Array.Resize(ref _n, _n.Length + 1);
				_n[_n.Length - 1] = 0x00;
			}
		}

		public void Initialize(BigInteger privateKey, byte[] msghash)
		{
		}

		public BigInteger GetNextK()
		{
			return new BigInteger(_n);
		}
	}
}
