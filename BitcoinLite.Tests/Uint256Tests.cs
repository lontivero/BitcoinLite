using System;
using System.Linq;
using NUnit.Framework;

namespace BitcoinLite
{
	[TestFixture(Category = "Unsigned integers")]
	public class Uint256Tests
	{
		private static uint256 u256_1 = uint256.Parse("9c524adbcf5611122b29125e5d35d2d22281aab533f00832d556b1f9eae51d7d");
		private static uint256 u256_2 = uint256.Parse("70321d7c47a56b40267e0ac3a69cb6bf133047a3192dda71491372f0b4ca81d7");
		private static uint160 u160_1 = uint160.Parse("9c524adbcf5611122b29125e5d35d2d22281aab5");
		private static uint160 u160_2 = uint160.Parse("70321d7c47a56b40267e0ac3a69cb6bf133047a3");

		[Test]
		public void Parsing()
		{
			Assert.AreEqual("9c524adbcf5611122b29125e5d35d2d22281aab533f00832d556b1f9eae51d7d", u256_1.ToString());
			Assert.AreEqual("0000000000000000000000000000000000000000000000000000000000000000", uint256.Zero.ToString());
			Assert.AreEqual("0000000000000000000000000000000000000000000000000000000000000001", uint256.One.ToString());
			Assert.AreEqual("0000000000000000000000000000000000000000000000000000000000000002", uint256.Two.ToString());
			Assert.AreEqual("ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff", uint256.MaxValue.ToString());

			Assert.AreEqual("9c524adbcf5611122b29125e5d35d2d22281aab5", u160_1.ToString());
			Assert.AreEqual("0000000000000000000000000000000000000000", uint160.Zero.ToString());
			Assert.AreEqual("0000000000000000000000000000000000000001", uint160.One.ToString());
			Assert.AreEqual("0000000000000000000000000000000000000002", uint160.Two.ToString());
			Assert.AreEqual("ffffffffffffffffffffffffffffffffffffffff", uint160.MaxValue.ToString());

			Assert.True(uint256.Parse(u256_1.ToString()) == u256_1);
			Assert.True(uint256.Parse(u256_2.ToString()) == u256_2);
			Assert.True(uint160.Parse(u160_1.ToString()) == u160_1);
			Assert.True(uint160.Parse(u160_2.ToString()) == u160_2);

			Assert.True(uint256.Parse(uint256.Zero.ToString()) == uint256.Zero);
			Assert.True(uint256.Parse(uint256.One.ToString()) == uint256.One);
			Assert.True(uint256.Parse(uint256.Two.ToString()) == uint256.Two);
			Assert.True(uint256.Parse(uint256.MaxValue.ToString()) == uint256.MaxValue);

			Assert.True(uint160.Parse(uint160.Zero.ToString()) == uint160.Zero);
			Assert.True(uint160.Parse(uint160.One.ToString()) == uint160.One);
			Assert.True(uint160.Parse(uint160.Two.ToString()) == uint160.Two);
			Assert.True(uint160.Parse(uint160.MaxValue.ToString()) == uint160.MaxValue);

			Assert.True(uint256.Parse(" " + uint256.Two + "  ") == uint256.Two);
			Assert.True(uint160.Parse(" " + uint160.Two + "  ") == uint160.Two);

			uint256 o256;
			Assert.True(uint256.TryParse(u256_1.ToString(), out o256));
			Assert.True(u256_1 == o256);

			uint160 o160;
			Assert.True(uint160.TryParse(u160_1.ToString(), out o160));
			Assert.True(u160_1 == o160);

			Assert.False(uint256.TryParse("Z-" + u256_1, out o256));
			Assert.False(uint160.TryParse("Z-" + u160_1, out o160));
		}


		[Test]
		public void Equality()
		{
			Assert.True(uint256.Zero == (uint256.One << 256));
			Assert.True(uint160.Zero == (uint160.One << 160));

			Assert.True(u256_1 != u256_2);
			Assert.True(u160_1 != u160_2);
			Assert.True(uint256.Zero != uint256.One);
			Assert.True(uint160.Zero != uint160.One);

			Assert.True(u256_1 != u256_2);
			Assert.True(u160_1 != u160_2);

			Assert.True(~uint256.MaxValue == uint256.Zero);
			Assert.True(~uint160.MaxValue == uint160.Zero);
			Assert.True(((u256_1 ^ u256_2) ^ u256_1) == u256_2);
			Assert.True(((u160_1 ^ u160_2) ^ u160_1) == u160_2);
		}

		[Test]
		public void Contrustors()
		{
			Assert.True(new uint256(u256_1) == u256_1);
			Assert.True(new uint160(u160_1) == u160_1);
			Assert.True(new uint256(u256_2) == u256_2);
			Assert.True(new uint160(u160_2) == u160_2);

			Assert.True((u256_1 & uint256.Parse("ffffffffffffffff")) == new uint256(u256_1.GetLow64()));
			Assert.True((u160_1 & uint160.Parse("ffffffffffffffff")) == new uint160(u160_1.GetLow64()));
			Assert.True((u256_1 & uint256.Parse("ffffffff")) == new uint256(u256_1.GetLow32()));
			Assert.True((u160_1 & uint160.Parse("ffffffff")) == new uint160(u160_1.GetLow32()));
			Assert.True(uint256.Parse("ffffffffffffffff") == new uint256(0xffffffffffffffffUL));
			Assert.True(uint160.Parse("ffffffffffffffff") == new uint160(0xffffffffffffffffUL));

			var arr = Enumerable.Range(0, 256).Select(x => (byte) x);
			Assert.Throws<FormatException>(() => new uint256(arr.Take(33).ToArray()));
			Assert.Throws<FormatException>(() => new uint256(arr.Take(17).ToArray()));
			Assert.Throws<FormatException>(() => new uint160(arr.Take(33).ToArray()));
			Assert.Throws<FormatException>(() => new uint160(arr.Take(17).ToArray()));

			Assert.Throws<ArgumentNullException>(() => new uint256((byte[])null));
			Assert.Throws<ArgumentNullException>(() => new uint160((byte[])null));
			Assert.Throws<ArgumentNullException>(() => new uint256((uint256)null));
			Assert.Throws<ArgumentNullException>(() => new uint160((uint160)null));
		}

		[Test]
		public void Comparison()
		{
			for (var i = 0; i < 256; ++i)
			{
				var u256 = uint256.One << i;
				Assert.True(u256 >= uint256.Zero && u256 > uint256.Zero && uint256.Zero < u256 && uint256.Zero <= u256);
				Assert.True(u256 >= 0 && u256 > 0 && 0 < u256 && 0 <= u256);
				u256 |= u256_1;
				Assert.True(u256 >= u256_1);
				Assert.True((u256 == u256_1) != (u256 > u256_1));
				Assert.True((u256 == u256_1) || !(u256 <= u256_1));
				Assert.True(u256_1 <= u256);
				Assert.True((u256_1 == u256) != (u256_1 < u256));
				Assert.True((u256 == u256_1) || !(u256_1 >= u256));
				Assert.True(!(u256 < u256_1));
				Assert.True(!(u256_1 > u256));
			}

			Assert.IsTrue(u256_1.CompareTo(u256_2) > 0);
			Assert.IsTrue(u256_2.CompareTo(u256_1) < 0);
			Assert.IsTrue(u256_1.CompareTo(u256_1) == 0);
			Assert.IsTrue(u256_1.CompareTo((object)u256_2) > 0);
			Assert.IsTrue(u256_2.CompareTo((object)u256_1) < 0);
			Assert.IsTrue(u256_1.CompareTo((object)u256_1) == 0);

			Assert.IsTrue(u160_1.CompareTo(u160_2) > 0);
			Assert.IsTrue(u160_2.CompareTo(u160_1) < 0);
			Assert.IsTrue(u160_1.CompareTo(u160_1) == 0);
			Assert.IsTrue(u160_1.CompareTo((object)u160_2) > 0);
			Assert.IsTrue(u160_2.CompareTo((object)u160_1) < 0);
			Assert.IsTrue(u160_1.CompareTo((object)u160_1) == 0);

			Assert.AreEqual(-2114756539, u256_1.GetHashCode());
			Assert.AreEqual(872570265, u160_1.GetHashCode());

			for (var i = 0; i < 160; ++i)
			{
				var u160 = uint160.One << i;
				Assert.True(u160 >= uint160.Zero && u160 > uint160.Zero && uint160.Zero < u160 && uint160.Zero <= u160);
				Assert.True(u160 >= 0 && u160 > 0 && 0 < u160 && 0 <= u160);
				u160 |= u160_1;
				Assert.True(u160 >= u160_1);
				Assert.True((u160 == u160_1) != (u160 > u160_1));
				Assert.True((u160 == u160_1) || !(u160 <= u160_1));
				Assert.True(u160_1 <= u160);
				Assert.True((u160_1 == u160) != (u160_1 < u160));
				Assert.True((u160 == u160_1) || !(u160_1 >= u160));
				Assert.True(!(u160 < u160_1));
				Assert.True(!(u160_1 > u160));
			}
		}

		[Test]
		public void Addition()
		{
			var half256 = uint256.One << 255;
			uint256 t256 = 0;
			Assert.True(u256_1 + u256_2 == uint256.Parse("0c84685816fb7c5251a71d2203d2899135b1f2584d1de2a41e6a24ea9faf9f54"));
			t256 += u256_1;
			Assert.True(t256 == u256_1);
			t256 += u256_2;
			Assert.True(t256 == u256_1 + u256_2);
			Assert.True(uint256.One + uint256.MaxValue == uint256.Zero);
			Assert.True(uint256.MaxValue + uint256.One == uint256.Zero);
			for (var i = 1; i < 256; ++i)
			{
				Assert.True((uint256.MaxValue >> i) + uint256.One == (half256 >> (i - 1)));
				Assert.True(uint256.One + (uint256.MaxValue >> i) == (half256 >> (i - 1)));
				t256 = (uint256.MaxValue >> i);
				t256 += uint256.One;
				Assert.True(t256 == (half256 >> (i - 1)));
				t256 = (uint256.MaxValue >> i);
				t256 += 1;
				Assert.True(t256 == (half256 >> (i - 1)));
				t256 = (uint256.MaxValue >> i);
				Assert.True(t256++ == (uint256.MaxValue >> i));
				Assert.True(t256 == (half256 >> (i - 1)));
			}
			Assert.True(new uint256(0xbedc77e27940a7UL) + 0xee8d836fce66fbUL == new uint256(0xbedc77e27940a7UL + 0xee8d836fce66fbUL));
			t256 = new uint256(0xbedc77e27940a7UL);
			t256 += 0xee8d836fce66fbUL;
			Assert.True(t256 == new uint256(0xbedc77e27940a7UL + 0xee8d836fce66fbUL));
			t256 -= 0xee8d836fce66fbUL;
			Assert.True(t256 == 0xbedc77e27940a7UL);
			t256 = u256_1;
			Assert.True(++t256 == u256_1 + 1);

			Assert.True(u256_1 - (-u256_2) == u256_1 + u256_2);
			Assert.True(u256_1 - (-uint256.One) == u256_1 + uint256.One);
			Assert.True(u256_1 - uint256.One == u256_1 + (-uint256.One));
			for (int i = 1; i < 256; ++i)
			{
				Assert.True((uint256.MaxValue >> i) - (-uint256.One) == (half256 >> (i - 1)));
				Assert.True((half256 >> (i - 1)) - uint256.One == (uint256.MaxValue >> i));
				t256 = (half256 >> (i - 1));
				Assert.True(t256-- == (half256 >> (i - 1)));
				Assert.True(t256 == (uint256.MaxValue >> i));
				t256 = (half256 >> (i - 1));
				Assert.True(--t256 == (uint256.MaxValue >> i));
			}
			t256 = u256_1;
			Assert.True(--t256 == u256_1 - 1);

			var half160 = uint160.One << 159;
			uint160 t160 = 0;
			Assert.True(u160_1 + u160_2 == uint160.Parse("0c84685816fb7c5251a71d2203d2899135b1f258"));
			t160 += u160_1;
			Assert.True(t160 == u160_1);
			t160 += u160_2;
			Assert.True(t160 == u160_1 + u160_2);
			Assert.True(uint160.One + uint160.MaxValue == uint160.Zero);
			Assert.True(uint160.MaxValue + uint160.One == uint160.Zero);
			for (int i = 1; i < 160; ++i)
			{
				Assert.True((uint160.MaxValue >> i) + uint160.One == (half160 >> (i - 1)));
				Assert.True(uint160.One + (uint160.MaxValue >> i) == (half160 >> (i - 1)));
				t160 = (uint160.MaxValue >> i);
				t160 += uint160.One;
				Assert.True(t160 == (half160 >> (i - 1)));
				t160 = (uint160.MaxValue >> i);
				t160 += 1;
				Assert.True(t160 == (half160 >> (i - 1)));
				t160 = (uint160.MaxValue >> i);
				Assert.True(t160++ == (uint160.MaxValue >> i));
				Assert.True(t160 == (half160 >> (i - 1)));
			}
			Assert.True(new uint160(0xbedc77e27940a7UL) + 0xee8d836fce66fbUL == new uint160(0xbedc77e27940a7UL + 0xee8d836fce66fbUL));
			t160 = new uint160(0xbedc77e27940a7UL);
			t160 += 0xee8d836fce66fbUL;
			Assert.True(t160 == new uint160(0xbedc77e27940a7UL + 0xee8d836fce66fbUL));
			t160 -= 0xee8d836fce66fbUL;
			Assert.True(t160 == 0xbedc77e27940a7UL);
			t160 = u160_1;
			Assert.True(++t160 == u160_1 + 1);

			Assert.True(u160_1 - (-u160_2) == u160_1 + u160_2);
			Assert.True(u160_1 - (-uint160.One) == u160_1 + uint160.One);
			Assert.True(u160_1 - uint160.One == u160_1 + (-uint160.One));
			for (int i = 1; i < 160; ++i)
			{
				Assert.True((uint160.MaxValue >> i) - (-uint160.One) == (half160 >> (i - 1)));
				Assert.True((half160 >> (i - 1)) - uint160.One == (uint160.MaxValue >> i));
				t160 = (half160 >> (i - 1));
				Assert.True(t160-- == (half160 >> (i - 1)));
				Assert.True(t160 == (uint160.MaxValue >> i));
				t160 = (half160 >> (i - 1));
				Assert.True(--t160 == (uint160.MaxValue >> i));
			}
			t160 = u160_1;
			Assert.True(--t160 == u160_1 - 1);
		}


		[Test]
		public void ComplementEquality()
		{
			Assert.True((~~u256_1 >> 10) == (u256_1 >> 10));
			Assert.True((~~u160_1 >> 10) == (u160_1 >> 10));
			Assert.True((~~u256_1 << 10) == (u256_1 << 10));
			Assert.True((~~u160_1 << 10) == (u160_1 << 10));
			Assert.True(!(~~u256_1 < u256_1));
			Assert.True(!(~~u160_1 < u160_1));
			Assert.True(~~u256_1 <= u256_1);
			Assert.True(~~u160_1 <= u160_1);
			Assert.True(!(~~u256_1 > u256_1));
			Assert.True(!(~~u160_1 > u160_1));
			Assert.True(~~u256_1 >= u256_1);
			Assert.True(~~u160_1 >= u160_1);
			Assert.True(!(u256_1 < ~~u256_1));
			Assert.True(!(u160_1 < ~~u160_1));
			Assert.True(u256_1 <= ~~u256_1);
			Assert.True(u160_1 <= ~~u160_1);
			Assert.True(!(u256_1 > ~~u256_1));
			Assert.True(!(u160_1 > ~~u160_1));
			Assert.True(u256_1 >= ~~u256_1);
			Assert.True(u160_1 >= ~~u160_1);

			Assert.True((~~u256_1 + u256_2).Equals(u256_1 + ~~u256_2));
			Assert.True((~~u160_1 + u160_2).Equals(u160_1 + ~~u160_2));
			Assert.True((~~u256_1 - u256_2).Equals(u256_1 - ~~u256_2));
			Assert.True((~~u160_1 - u160_2).Equals(u160_1 - ~~u160_2));

			Assert.True((~~u256_1 + u256_2).Equals((object)(u256_1 + ~~u256_2)));
			Assert.True((~~u160_1 + u160_2).Equals((object)(u160_1 + ~~u160_2)));
			Assert.True((~~u256_1 - u256_2).Equals((object)(u256_1 - ~~u256_2)));
			Assert.True((~~u160_1 - u160_2).Equals((object)(u160_1 - ~~u160_2)));

			Assert.True(!u160_1.Equals((object)null));
			Assert.True(!u256_1.Equals((object)null));
			Assert.True(!u160_1.Equals(null));
			Assert.True(!u256_1.Equals(null));

			Assert.True(~u256_1 != u256_1);
			Assert.True(u256_1 != ~u256_1);
			Assert.True(~u160_1 != u160_1);
			Assert.True(u160_1 != ~u160_1);
			Assert.True(u256_1 != ~u256_1);
			Assert.True(~u160_1 != u160_1);
			Assert.True(u160_1 != ~u160_1);

			Assert.True(uint256.Zero == 0);
			Assert.True(uint256.Zero != 1);
			Assert.True(uint256.Zero != 2);
			Assert.True(uint256.Two != uint256.Parse("04"));

			Assert.True(uint160.Zero == 0);
			Assert.True(uint160.Zero != 1);
			Assert.True(uint160.Zero != 2);
			Assert.True(uint160.Two != uint160.Parse("04"));

			Assert.True(!uint256.Zero);
			Assert.True(!!uint256.One);
			Assert.True(!uint160.Zero);
			Assert.True(!!uint160.One);

			byte b;
			var t256 = uint256.Parse("0403020100");
			Assert.True(t256[0] == 0 && t256[1] == 1 && t256[2] == 2 && t256[3] == 3 && t256[4] == 4 && t256[31] == 0);
			Assert.Throws<IndexOutOfRangeException>(() => b = t256[-10]);
			Assert.Throws<IndexOutOfRangeException>(() => b = t256[32]);

			var t160 = uint160.Parse("0403020100");
			Assert.True(t160[0] == 0 && t160[1] == 1 && t160[2] == 2 && t160[3] == 3 && t160[4] == 4 && t160[19] == 0);
			Assert.Throws<IndexOutOfRangeException>(() => b = t160[-10]);
			Assert.Throws<IndexOutOfRangeException>(() => b = t160[20]);
		}
	}
}
