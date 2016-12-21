using System.Linq;
using BitcoinLite.Encoding;
using BitcoinLite.Utils;
using NUnit.Framework;

namespace BitcoinLite.Tests.Utils
{
	[TestFixture(Category = "Helpers,Packer")]
	public class PackerTests
	{
		[Test]
		public void LittleEndian()
		{
			var little = Packer.LittleEndian;
			var arr = new byte[] { 0x01, 0x02, 0x03, 0x04, 0xff };
			CollectionAssert.AreEqual(arr, little.GetBytes(arr));

			// booleans
			CollectionAssert.AreEqual(Hex2Bytes("01"), little.GetBytes(true));
			CollectionAssert.AreEqual(Hex2Bytes("00"), little.GetBytes(false));
			Assert.AreEqual(false, little.ToBoolean(Hex2Bytes("00"), 0));
			Assert.AreEqual(true, little.ToBoolean(Hex2Bytes("01"), 0));

			// chars
			CollectionAssert.AreEqual(Hex2Bytes("0061"), little.GetBytes('a'));
			Assert.AreEqual('a', little.ToChar(Hex2Bytes("0061"), 0));

			// shorts
			CollectionAssert.AreEqual(Hex2Bytes("3039"), little.GetBytes((ushort)12345));
			CollectionAssert.AreEqual(Hex2Bytes("3039"), little.GetBytes((short)12345));
			CollectionAssert.AreEqual(Hex2Bytes("CFC7"), little.GetBytes((short)-12345));
			Assert.AreEqual(12345, little.ToUInt16(Hex2Bytes("3039"), 0));
			Assert.AreEqual(12345, little.ToInt16(Hex2Bytes("3039"), 0));
			Assert.AreEqual(-12345, little.ToInt16(Hex2Bytes("CFC7"), 0));

			// ints
			CollectionAssert.AreEqual(Hex2Bytes("0034B59F"), little.GetBytes((uint)3454367));
			CollectionAssert.AreEqual(Hex2Bytes("0034B59F"), little.GetBytes(3454367));
			CollectionAssert.AreEqual(Hex2Bytes("FFCB4A61"), little.GetBytes(-3454367));
			Assert.AreEqual(3454367, little.ToUInt32(Hex2Bytes("0034B59F"), 0));
			Assert.AreEqual(3454367, little.ToInt32(Hex2Bytes("0034B59F"), 0));
			Assert.AreEqual(-3454367, little.ToInt32(Hex2Bytes("FFCB4A61"), 0));

			// long
			CollectionAssert.AreEqual(Hex2Bytes("000000506DA33CD5"), little.GetBytes((ulong)345436798165));
			CollectionAssert.AreEqual(Hex2Bytes("000000506DA33CD5"), little.GetBytes(345436798165));
			CollectionAssert.AreEqual(Hex2Bytes("FFFFFFAF925CC32B"), little.GetBytes(-345436798165));
			Assert.AreEqual(345436798165, little.ToUInt64(Hex2Bytes("000000506DA33CD5"), 0));
			Assert.AreEqual(345436798165, little.ToInt64(Hex2Bytes("000000506DA33CD5"), 0));
			Assert.AreEqual(-345436798165, little.ToInt64(Hex2Bytes("FFFFFFAF925CC32B"), 0));

			var u256 = new uint256(345436798165);
			var u256b = Hex2Bytes("000000000000000000000000000000000000000000000000000000506da33cd5");
			CollectionAssert.AreEqual(u256b, little.GetBytes(u256));
			Assert.AreEqual(u256, little.ToUInt256(u256b, 0));

			var u160 = new uint160(345436798165);
			var u160b = Hex2Bytes("000000000000000000000000000000506da33cd5");
			CollectionAssert.AreEqual(u160b, little.GetBytes(u160));
			Assert.AreEqual(u160, little.ToUInt160(u160b, 0));
		}

		[Test]
		public void BigEndian()
		{
			var big = Packer.BigEndian;
			var arr = new byte[] { 0x01, 0x02, 0x03, 0x04, 0xff };
			CollectionAssert.AreEqual(arr.Reverse(), big.GetBytes(arr));

			// booleans
			CollectionAssert.AreEqual(Hex2Bytes("01"), big.GetBytes(true));
			CollectionAssert.AreEqual(Hex2Bytes("00"), big.GetBytes(false));
			Assert.AreEqual(false, big.ToBoolean(Hex2Bytes("00"), 0));
			Assert.AreEqual(true, big.ToBoolean(Hex2Bytes("01"), 0));

			// chars
			CollectionAssert.AreEqual(Hex2Bytes("0061").Reverse(), big.GetBytes('a'));
			Assert.AreEqual('a', big.ToChar(Hex2Bytes("0061").Reverse().ToArray(), 0));

			// shorts
			CollectionAssert.AreEqual(Hex2Bytes("3039").Reverse(), big.GetBytes((ushort)12345));
			CollectionAssert.AreEqual(Hex2Bytes("3039").Reverse(), big.GetBytes((short)12345));
			CollectionAssert.AreEqual(Hex2Bytes("CFC7").Reverse(), big.GetBytes((short)-12345));
			Assert.AreEqual(12345, big.ToUInt16(Hex2Bytes("3039").Reverse().ToArray(), 0));
			Assert.AreEqual(12345, big.ToInt16(Hex2Bytes("3039").Reverse().ToArray(), 0));
			Assert.AreEqual(-12345, big.ToInt16(Hex2Bytes("CFC7").Reverse().ToArray(), 0));

			// ints
			CollectionAssert.AreEqual(Hex2Bytes("0034B59F").Reverse(), big.GetBytes((uint)3454367));
			CollectionAssert.AreEqual(Hex2Bytes("0034B59F").Reverse(), big.GetBytes(3454367));
			CollectionAssert.AreEqual(Hex2Bytes("FFCB4A61").Reverse(), big.GetBytes(-3454367));
			Assert.AreEqual(3454367, big.ToUInt32(Hex2Bytes("0034B59F").Reverse().ToArray(), 0));
			Assert.AreEqual(3454367, big.ToInt32(Hex2Bytes("0034B59F").Reverse().ToArray(), 0));
			Assert.AreEqual(-3454367, big.ToInt32(Hex2Bytes("FFCB4A61").Reverse().ToArray(), 0));

			// long
			CollectionAssert.AreEqual(Hex2Bytes("000000506DA33CD5").Reverse(), big.GetBytes((ulong)345436798165));
			CollectionAssert.AreEqual(Hex2Bytes("000000506DA33CD5").Reverse(), big.GetBytes(345436798165));
			CollectionAssert.AreEqual(Hex2Bytes("FFFFFFAF925CC32B").Reverse(), big.GetBytes(-345436798165));
			Assert.AreEqual(345436798165, big.ToUInt64(Hex2Bytes("000000506DA33CD5").Reverse().ToArray(), 0));
			Assert.AreEqual(345436798165, big.ToInt64(Hex2Bytes("000000506DA33CD5").Reverse().ToArray(), 0));
			Assert.AreEqual(-345436798165, big.ToInt64(Hex2Bytes("FFFFFFAF925CC32B").Reverse().ToArray(), 0));

			var u256 = new uint256(345436798165);
			var u256b = Hex2Bytes("000000000000000000000000000000000000000000000000000000506da33cd5");
			CollectionAssert.AreEqual(u256b.Reverse(), big.GetBytes(u256));
			Assert.AreEqual(u256, big.ToUInt256(u256b.Reverse().ToArray(), 0));

			var u160 = new uint160(345436798165);
			var u160b = Hex2Bytes("000000000000000000000000000000506da33cd5");
			CollectionAssert.AreEqual(u160b.Reverse(), big.GetBytes(u160));
			Assert.AreEqual(u160, big.ToUInt160(u160b.Reverse().ToArray(), 0));
		}

		[Test]
		public void PackerTest()
		{
			var packed = Packer.Pack("_sSiIlLbb", 210, 810, 234560, 7734560, 234560234560, 12234560234560, 7, 9);
			var l = Packer.LittleEndian;
			var arr = l.GetBytes((ushort) 210)
				.Concat(l.GetBytes((short) 810))
				.Concat(l.GetBytes((uint) 234560))
				.Concat(l.GetBytes((int) 7734560))
				.Concat(l.GetBytes((ulong) 234560234560))
				.Concat(l.GetBytes((long) 12234560234560))
				.Concat(l.GetBytes(new [] { (byte)0x07 }))
				.Concat(l.GetBytes(new[] { (byte)0x09 }));
	
			CollectionAssert.AreEqual(packed, arr);

			var c = Packer.Unpack("_sSiIlLbb", packed, 0);
			Assert.AreEqual(210, c[0]);
			Assert.AreEqual(810, c[1]);
			Assert.AreEqual(234560, c[2]);
			Assert.AreEqual(7734560, c[3]);
			Assert.AreEqual(234560234560, c[4]);
			Assert.AreEqual(12234560234560, c[5]);
			Assert.AreEqual(7, c[6]);
			Assert.AreEqual(9, c[7]);
		}

		private static byte[] Hex2Bytes(string str)
		{
			return Encoders.Hex.GetBytes(str).Reverse().ToArray();
		}
	}
}
