using System;
using System.Linq;
using System.Numerics;
using BitcoinLite.Crypto;
using BitcoinLite.Encoding;
using NUnit.Framework;

namespace BitcoinLite.Tests.Crypto
{
	[TestFixture(Category = "Crypto")]
	public class Secp256k1Tests
	{
		[Test]
		[TestCase("00", ExpectedResult = true, TestName = "ECPoint - Infinity point encoding")]
		[TestCase("0279BE667EF9DCBBAC55A06295CE870B07029BFCDB2DCE28D959F2815B16F81798", ExpectedResult = true, TestName = "ECPoint - compressed Secp256k1 G point")]
		[TestCase("0479BE667EF9DCBBAC55A06295CE870B07029BFCDB2DCE28D959F2815B16F81798483ADA7726A3C4655DA4FBFC0E1108A8FD17B448A68554199C47D08FFB10D4B8", ExpectedResult = true, TestName = "ECPoint - uncompressed Secp256k1 G point")]
		[TestCase("0000", ExpectedResult = false, TestName = "ECPoint - Infinity encoded point is 00")]
		[TestCase("0279BE667EF9DCBBAC55A06295CE870B07029BFCDB2DCE28D959F2815B16", ExpectedResult = false, TestName = "ECPoint - Too short compressed")]
		[TestCase("0479BE667EF9DCBBAC55A06295CE870B07029BFCDB2DCE28D959F2815B16F81798483ADA7726A3C4655DA4FBFC0E1108A8FD17", ExpectedResult = false, TestName = "ECPoint - Too short uncompressed")]
		[TestCase("0279BE667EF9DCBBAC55A06295CE870B07029BFCDB2DCE28D959F2815B16F81798987654", ExpectedResult = false, TestName = "ECPoint - Too long compressed")]
		[TestCase("0479BE667EF9DCBBAC55A06295CE870B07029BFCDB2DCE28D959F2815B16F81798483ADA7726A3C4655DA4FBFC0E1108A8FD17B448A68554199C47D08FFB10D4B81798987654", ExpectedResult = false, TestName = "ECPoint - Too long uncompressed")]
		public bool Points(string encoded)
		{
			try
			{
				var p = ECPoint.Decode(Encoders.Hex.GetBytes(encoded));
				return true;
			}
			catch (FormatException)
			{
				return false;
			}
		}

		[Test]
		public void PointCompress()
		{
			Assert.That(ECPoint.Decode(new byte[1]), Is.EqualTo(ECPoint.Infinity));
			Assert.That(ECPoint.Infinity.Encode(true), Is.EquivalentTo(new byte[1]));
			Assert.That(ECPoint.Infinity.Encode(false), Is.EquivalentTo(new byte[1]));

			Assert.Throws<FormatException>(()=>ECPoint.Decode(new byte[]{0x00, 0x00}));

			var p1 = ECPoint.Decode(Encoders.Hex.GetBytes("0279BE667EF9DCBBAC55A06295CE870B07029BFCDB2DCE28D959F2815B16F81798"));
			var p2 = ECPoint.Decode(Encoders.Hex.GetBytes("0479BE667EF9DCBBAC55A06295CE870B07029BFCDB2DCE28D959F2815B16F81798483ADA7726A3C4655DA4FBFC0E1108A8FD17B448A68554199C47D08FFB10D4B8"));
			Assert.AreEqual(p1, p2);

			Assert.That(
				Encoders.Hex.GetString(p1.Encode(true)),
				Does.Contain("0279BE667EF9DCBBAC55A06295CE870B07029BFCDB2DCE28D959F2815B16F81798").IgnoreCase);

			Assert.That(
				Encoders.Hex.GetString(p1.Encode(false)),
				Does.Contain("0479BE667EF9DCBBAC55A06295CE870B07029BFCDB2DCE28D959F2815B16F81798483ADA7726A3C4655DA4FBFC0E1108A8FD17B448A68554199C47D08FFB10D4B8").IgnoreCase);

			Assert.That(
				Encoders.Hex.GetString(p2.Encode(true)),
				Does.Contain("0279BE667EF9DCBBAC55A06295CE870B07029BFCDB2DCE28D959F2815B16F81798").IgnoreCase);

			Assert.That(
				Encoders.Hex.GetString(p2.Encode(false)),
				Does.Contain("0479BE667EF9DCBBAC55A06295CE870B07029BFCDB2DCE28D959F2815B16F81798483ADA7726A3C4655DA4FBFC0E1108A8FD17B448A68554199C47D08FFB10D4B8").IgnoreCase);
		}

		[Test, TestCaseSource(typeof (TestDataFactory), nameof(TestDataFactory.ec_points_mul))]
		public void PointMultiplication(BigInteger m, BigInteger x, BigInteger y)
		{
			var expected = new ECPoint(x, y);
			var point = m*Secp256k1.G;
			Assert.True(expected == point);
		}

		[Test, TestCaseSource(typeof (TestDataFactory), nameof(TestDataFactory.ec_points_add))]
		public void PointAddition(
			BigInteger x1, BigInteger y1,
			BigInteger x2, BigInteger y2,
			BigInteger x3, BigInteger y3)
		{
			var p1 = new ECPoint(x1, y1);
			var p2 = new ECPoint(x2, y2);
			var expected = new ECPoint(x3, y3);

			Assert.True(expected == p1 + p2);
		}

		[Test, TestCaseSource(typeof (TestDataFactory), nameof(TestDataFactory.ec_points_dbl))]
		public void PointDoubling(
			BigInteger x1, BigInteger y1,
			BigInteger x2, BigInteger y2)
		{
			var p = new ECPoint(x1, y1);
			var expected = new ECPoint(x2, y2);

			Assert.True(expected == 2*p);
		}

		[TestCase(     1, TestName = "ECPoint - kG is in curve for all k")]
		[TestCase(    13, TestName = "ECPoint - kG is in curve for all k")]
		[TestCase(    31, TestName = "ECPoint - kG is in curve for all k")]
		[TestCase(  4521, TestName = "ECPoint - kG is in curve for all k")]
		[TestCase(735241, TestName = "ECPoint - kG is in curve for all k")]
		public void PointsAreInCurve(int factor)
		{
			Assert.IsTrue((factor * Secp256k1.G).IsInCurve());
		}

		[Test, TestCaseSource(typeof(TestDataFactory), nameof(TestDataFactory.ec_points_comparison))]
		public void Comparisons(ECPoint point, object other, bool expected)
		{
			Assert.AreEqual(expected, point.Equals(other));
		}

		[Test]
		public void Comparisons()
		{
			var point = 13*Secp256k1.G;
			Assert.True(point.Equals(point));
			Assert.False(point.Equals(null));
			Assert.True(point == point);
			Assert.True(point != (ECPoint)null);
		}

		[Test]
		public void ToStrinf()
		{
			Assert.AreEqual("X=79be667ef9dcbbac55a06295ce870b07029bfcdb2dce28d959f2815b16f81798, Y=483ada7726a3c4655da4fbfc0e1108a8fd17b448a68554199c47d08ffb10d4b8", Secp256k1.G.ToString());
		}

		[TestCase(1,   337176337, TestName = "ECPoint - (1*G).GetHashCode() == 337176337")]
		[TestCase(2, -2009981190, TestName = "ECPoint - (1*G).GetHashCode() == -2009981190")]
		[TestCase(5,   253952930, TestName = "ECPoint - (1*G).GetHashCode() == 253952930")]
		public void HashCode(int times, int expected)
		{
			var hashCode = (times * Secp256k1.G).GetHashCode();
			Assert.AreEqual(expected, hashCode);
		}


		[TestCase(1, TestName = "ECPoint - 1P = P")]
		[TestCase(2, TestName = "ECPoint - 2P = P + P")]
		[TestCase(5, TestName = "ECPoint - 5P = P + P + P + P + P")]
		public void ProductDistribution(int times)
		{
			var sum = Enumerable.Repeat(Secp256k1.G, times).Aggregate((s1, s2) => s1+s2);
			Assert.IsTrue( times * Secp256k1.G == sum);
		}

		[Test]
		public void ProductDistribution()
		{
			Assert.IsTrue(Secp256k1.G.IsInCurve());
			Assert.IsTrue((Secp256k1.G*Secp256k1.N).IsInfinity);
			Assert.IsTrue((00*Secp256k1.G).IsInfinity);
			Assert.IsTrue((10*ECPoint.Infinity).IsInfinity);

			Assert.IsTrue((100*Secp256k1.G) == (50*Secp256k1.G + 50*Secp256k1.G));
		}

		[TestCase(TestName = "ECPoint - P + Q == R")]
		public void SelfTest()
		{
			var rnd = new Random();
			var buf = new byte[32];

			rnd.NextBytes(buf);
			buf[31] = 0;
			var a = new BigInteger(buf);

			rnd.NextBytes(buf);
			buf[31] = 0;
			var b = new BigInteger(buf);
			var c = a + b;

			var P = a*Secp256k1.G;
			var Q = b*Secp256k1.G;
			var R = c*Secp256k1.G;

			Assert.True(P.IsInCurve());
			Assert.True(Q.IsInCurve());
			Assert.True(R.IsInCurve());
			Assert.True(P + Q == R);
			Assert.True(Q + P == R);
		}
	}
}