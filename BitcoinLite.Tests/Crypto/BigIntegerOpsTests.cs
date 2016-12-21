using System;
using System.Numerics;
using BitcoinLite.Crypto;
using NUnit.Framework;

namespace BitcoinLite.Tests.Crypto
{
	[TestFixture(Category = "Crypto")]
	public class BigIntegerOpsTests
	{
		[Test]
		public void BigIntegerShanksSqrt()
		{
			var tests = new []
			{
				new Tuple<BigInteger, BigInteger>(10, 13),
				new Tuple<BigInteger, BigInteger>(56, 101),
				new Tuple<BigInteger, BigInteger>(1030, 10009),
				new Tuple<BigInteger, BigInteger>(44402, 100049),
				new Tuple<BigInteger, BigInteger>(665820697, 1000000009),
				new Tuple<BigInteger, BigInteger>(881398088036, 1000000000039),
			};

			foreach (var tuple in tests)
			{
				var n = tuple.Item1;
				var p = tuple.Item2;

				var r = n.ShanksSqrt(p);
				Assert.True(((BigInteger.Pow(r, 2) - n) % p).IsZero );
			}
		}
	}
}
