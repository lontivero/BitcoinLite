using BitcoinLite.Crypto;
using BitcoinLite.Encoding;
//using BigInteger = NBitcoin.BouncyCastle.Math.BigInteger;
using BigInteger = BitcoinLite.Math.BigInteger;
using NetBigInteger = System.Numerics.BigInteger;

using NUnit.Framework;

namespace BitcoinLite.Tests.Math
{
	[TestFixture]
	public class BigIntegerTests
	{
		private static IPRNGenerator rng = new WinCryptoPrng();
#if false
		[Test]
		public void ToByteArray()
		{
			var src = new byte[] {0x01};
			var b1 = new BigInteger(src);
			var b2 = src.ToBigIntegerUnsigned(true);
			CollectionAssert.AreEqual(b2.ToByteArray(), b1.ToByteArray());

			src = new byte[] { 0x01, 0x00 };
			b1 = new BigInteger(src);
			b2 = src.ToBigIntegerUnsigned(true);
			CollectionAssert.AreEqual(b2.ToByteArray().ToBigEndian(), b1.ToByteArray());

			src = new byte[] { 0x01, 0x00, 0x00 };
			b1 = new BigInteger(src);
			b2 = src.ToBigIntegerUnsigned(true);
			CollectionAssert.AreEqual(b2.ToByteArray().ToBigEndian(), b1.ToByteArray());

			src = new byte[] { 0x01, 0x00, 0x00, 0x00 };
			b1 = new BigInteger(src);
			b2 = src.ToBigIntegerUnsigned(true);
			CollectionAssert.AreEqual(b2.ToByteArray().ToBigEndian(), b1.ToByteArray());

			src = new byte[] { 0x01, 0x00, 0x00, 0x00, 0x00 };
			b1 = new BigInteger(src);
			b2 = src.ToBigIntegerUnsigned(true);
			CollectionAssert.AreEqual(b2.ToByteArray().ToBigEndian(), b1.ToByteArray());


			src = new byte[] { 0x00, 0x01 };
			b1 = new BigInteger(src);
			b2 = src.ToBigIntegerUnsigned(true);
			CollectionAssert.AreEqual(b2.ToByteArray().ToBigEndian(), b1.ToByteArray());

			src = new byte[] { 0x00, 0x01, 0x00 };
			b1 = new BigInteger(src);
			b2 = src.ToBigIntegerUnsigned(true);
			CollectionAssert.AreEqual(b2.ToByteArray().ToBigEndian(), b1.ToByteArray());

			src = new byte[] { 0x00, 0x00, 0x01, 0x00, 0x00 };
			b1 = new BigInteger(src);
			b2 = src.ToBigIntegerUnsigned(true);
			CollectionAssert.AreEqual(b2.ToByteArray().ToBigEndian(), b1.ToByteArray());

			src = new byte[] { 0x00, 0x00, 0x00, 0x01, 0x00 };
			b1 = new BigInteger(src);
			b2 = src.ToBigIntegerUnsigned(true);
			CollectionAssert.AreEqual(b2.ToByteArray().ToBigEndian(), b1.ToByteArray());

			src = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x01, 0x00 };
			b1 = new BigInteger(src);
			b2 = src.ToBigIntegerUnsigned(true);
			CollectionAssert.AreEqual(b2.ToByteArray().ToBigEndian(), b1.ToByteArray());
		}

		[Test]
		public void WithZeros()
		{
			var bytes = new byte[]
			{
				54,
				0,
				26,
				84,
				89,
				207,
				182,
				77,
				51,
				36
			};
			var bi1 = new BigInteger(bytes);
			CollectionAssert.AreEqual(bytes, bi1.ToByteArray());
		}

		[Test]
		public void Fuzz()
		{
			for (int i = 0; i < 10000; i++)
			{
				var bytes = GenerateRandoBytes();
				var bi1 = new BigInteger(bytes);
				var bi2 = bytes.ToBigIntegerUnsigned(true);

				CollectionAssert.AreEqual(bi1.ToByteArray(), bi2.ToByteArray().ToBigEndian());
			}
		}

		[Test]
		public void Roll()
		{
			var bytes = Encoders.Hex.GetBytes("7cf5f6fcd68fb74dc496e99a20531f8c723e5e98b62fe2ad28de23b13605085c7f4adb5c2659ca62ec14fabbc14f528ab295a9");
			var bi1 = new BigInteger(bytes);
			CollectionAssert.AreEqual(bytes, bi1.ToByteArray());
		}


		[Test]
		public void SumWithCarry()
		{
			var bytes1 = Encoders.Hex.GetBytes("7cf5f6fcd68fb74dc496e99a20531f8c723e5e98b62fe2ad28de22de2605c977bbc396c5752ee6a306960b47a9e71c72cac518");
			var bytes2 = Encoders.Hex.GetBytes("d30fff3ee4c3874496b12ae3bfe57eef7417683617e7d091");
			var expected = Encoders.Hex.GetBytes("7cf5f6fcd68fb74dc496e99a20531f8c723e5e98b62fe2ad28de23b13605085c7f4adb5c2659ca62ec14fabbc14f528ab295a9");
			var bi11 = new BigInteger(bytes1);
			var bi12 = new BigInteger(bytes2);

			var sum = bi11 + bi12;
			CollectionAssert.AreEqual(expected, sum.ToByteArray());
		}

		[Test]
		public void Additions()
		{
			for (int i = 0; i < 10000; i++)
			{
				var bytes1 = GenerateRandoBytes();
				var bytes2 = GenerateRandoBytes();

				var bi11 = new BigInteger(bytes1);
				var bi12 = new BigInteger(bytes2);

				var bi21 = bytes1.ToBigIntegerUnsigned(true);
				var bi22 = bytes2.ToBigIntegerUnsigned(true);

				CollectionAssert.AreEqual((bi11 + bi12).ToByteArray(), (bi21 + bi22).ToByteArray().ToBigEndian());
			}
		}

		[Test]
		public void SubstractionBorrowed()
		{
			var bytes1 = Encoders.Hex.GetBytes("5d679b70a6a1856fc041d58b55a6f005d36fb140");
			var bytes2 = Encoders.Hex.GetBytes("90a246bf30f807bc");

			var bi11 = new BigInteger(bytes1);
			var bi12 = new BigInteger(bytes2);

			var bi21 = bytes1.ToBigIntegerUnsigned(true);
			var bi22 = bytes2.ToBigIntegerUnsigned(true);

			var expected = (bi21 - bi22).ToByteArray().ToBigEndian();
			var actual = (bi11 - bi12).ToByteArray();

			CollectionAssert.AreEqual(expected, actual);
		}

		[Test]
		public void Substractionx()
		{
			var bytes1 = Encoders.Hex.GetBytes("3fa8ec74c35de5390d8b59");
			var bytes2 = Encoders.Hex.GetBytes("bf62768311a2bed09b4ee1fa8b817f7b4f0be4d831b2f87fce5fe152ffbc3747f99ad655090da147b4a8c78ffac087ba00");

			var bi11 = new BigInteger(bytes1);
			var bi12 = new BigInteger(bytes2);

			var bi21 = bytes1.ToBigIntegerUnsigned(true);
			var bi22 = bytes2.ToBigIntegerUnsigned(true);


			var expected = (bi21 - bi22);
			var actual = (bi11 - bi12);

			CollectionAssert.AreEqual(expected.ToByteArray().ToBigEndian(), actual.ToByteArray());
		}

		[Test]
		public void Substractions()
		{
			for (int i = 0; i < 10000; i++)
			{
				var bytes1 = GenerateRandoBytes();
				var bytes2 = GenerateRandoBytes();

				var bi11 = new BigInteger(bytes1);
				var bi12 = new BigInteger(bytes2);

				var bi21 = bytes1.ToBigIntegerUnsigned(true);
				var bi22 = bytes2.ToBigIntegerUnsigned(true);

				var expected = (bi21 - bi22).ToByteArray().ToBigEndian();
				var actual = (bi11 - bi12).ToByteArray();

				CollectionAssert.AreEqual(expected, actual);
			}
		}
#endif
		private byte[] GenerateRandoBytes()
		{
			var len = (int)(((float)(rng.GetBytes(1)[0]) / 256) * 70);
			var bytes = rng.GetBytes(len + 1);
			return bytes;
		}

		[Test]
		public void Substractionx()
		{
			var bytes1 = Encoders.Hex.GetBytes("3fa8ec74c35de5390d8b59");
			var bytes2 = Encoders.Hex.GetBytes("bf62768311a2bed09b4ee1fa8b817f7b4f0be4d831b2f87fce5fe152ffbc3747f99ad655090da147b4a8c78ffac087ba");

			var bi11 = new BigInteger(1, bytes1);
			var bi12 = new BigInteger(1, bytes2);

			var bi21 = bytes1.ToBigIntegerUnsigned(true);
			var bi22 = bytes2.ToBigIntegerUnsigned(true);


			var expected = (bi21 - bi22);
			var actual = (bi11- bi12);

			CollectionAssert.AreEqual(expected.ToByteArray().ToBigEndian(), actual.ToByteArray());
		}

	}
}
