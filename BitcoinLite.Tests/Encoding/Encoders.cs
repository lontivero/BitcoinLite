using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitcoinLite.Encoding;
using BitcoinLite.Utils;
using NUnit.Framework;

namespace BitcoinLite.Tests.Encoding
{
	[TestFixture(Category = "Encoders")]
	public class HexEncoder
	{
		[Test]
		public void Decode()
		{
			Assert.Throws<ArgumentNullException>(()=> Encoders.Hex.GetBytes(null));
			Assert.Throws<FormatException>(() => Encoders.Hex.GetBytes(" "));
			Assert.Throws<FormatException>(() => Encoders.Hex.GetBytes("AAA"));
			Assert.Throws<FormatException>(() => Encoders.Hex.GetBytes("AADDGG"));
			Assert.Throws<FormatException>(() => Encoders.Hex.GetBytes("AA DD"));

			CollectionAssert.AreEqual(ByteArray.Empty, Encoders.Hex.GetBytes(string.Empty));
			CollectionAssert.AreEqual(new byte[] { 0xff, 0x00, 0xaa }, Encoders.Hex.GetBytes("FF00AA"));
		}

		[Test]
		public void Encode()
		{
			Assert.Throws<ArgumentNullException>(() => Encoders.Hex.GetString(null));
			Assert.AreEqual(string.Empty, Encoders.Hex.GetString(ByteArray.Empty));
			Assert.AreEqual("ffaa", Encoders.Hex.GetString(new byte[] {0xff, 0xaa}));
		}
	}

	[TestFixture(Category = "Encoders")]
	public class ASCIIEncoder
	{
		[Test]
		public void Decode()
		{
			CollectionAssert.AreEqual(ByteArray.Empty, Encoders.ASCII.GetBytes(null));
			CollectionAssert.AreEqual(ByteArray.Empty, Encoders.ASCII.GetBytes(string.Empty));
			CollectionAssert.AreEqual(new byte[] { 0x20 }, Encoders.ASCII.GetBytes(" "));
			CollectionAssert.AreEqual(new byte[] { 0x41, 0x41, 0x41 }, Encoders.ASCII.GetBytes("AAA"));
		}

		[Test]
		public void Encode()
		{
			Assert.Throws<ArgumentNullException>(() => Encoders.ASCII.GetString(null));
			Assert.AreEqual(string.Empty, Encoders.ASCII.GetString(ByteArray.Empty));
			Assert.AreEqual("ABC", Encoders.ASCII.GetString(new byte[] { 0x41, 0x42, 0x43 }));
		}
	}

	[TestFixture(Category = "Encoders")]
	public class Base64Encoder
	{
		[Test]
		public void Decode()
		{
			Assert.Throws<ArgumentNullException>(() => Encoders.Base64.GetBytes(null));
			CollectionAssert.AreEqual(ByteArray.Empty, Encoders.Base64.GetBytes(string.Empty));
			CollectionAssert.AreEqual(ByteArray.Empty, Encoders.Base64.GetBytes(" "));
			CollectionAssert.AreEqual(new byte[] { 0x41, 0x42, 0x43 }, Encoders.Base64.GetBytes("QUJD"));
		}

		[Test]
		public void Encode()
		{
			Assert.Throws<ArgumentNullException>(() => Encoders.Base64.GetString(null));
			Assert.AreEqual(string.Empty, Encoders.Base64.GetString(ByteArray.Empty));
			Assert.AreEqual("QUJD", Encoders.Base64.GetString(new byte[] { 0x41, 0x42, 0x43 }));
		}
	}
}
