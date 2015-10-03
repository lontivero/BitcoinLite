using System;
using BitcoinLite.Encoding;
using NUnit.Framework;

namespace BitcoinLite.Tests.Encoding
{
	[TestFixture(Category = "Base58")]
	public class Base58Tests
	{
		public static TestCaseData[] DataSet
		{
			get
			{
				return new TestCaseData[] {
					new TestCaseData(string.Empty, ""),
					new TestCaseData("61", "2g"),
					new TestCaseData("626262", "a3gV"),
					new TestCaseData("636363", "aPEr"),
					new TestCaseData("73696d706c792061206c6f6e6720737472696e67", "2cFupjhnEsSn59qHXstmK2ffpLv2"),
					new TestCaseData("00eb15231dfceb60925886b67d065299925915aeb172c06647", "1NS17iag9jJgTHD1VXjvLCEnZuQ3rJDE9L"),
					new TestCaseData("516b6fcd0f", "ABnLTmg"),
					new TestCaseData("bf4f89001e670274dd", "3SEo3LWLoPntC"),
					new TestCaseData("572e4794", "3EFU7m"),
					new TestCaseData("ecac89cad93923c02321", "EJDM8drfXA6uyA"),
					new TestCaseData("10c8511e", "Rt5zm"),
					new TestCaseData("00000000000000000000", "1111111111")
				};
			}
		}

		[Test, TestCaseSource("DataSet")]
		public void Encode(string data, string encoded)
		{
			var testBytes = Encoders.Hex.Decode(data);
			Assert.AreEqual(encoded, Encoders.Base58.Encode(testBytes));
		}

		[Test, TestCaseSource("DataSet")]
		public void Decode(string data, string encoded)
		{
			var testBytes = Encoders.Base58.Decode(encoded);
			CollectionAssert.AreEqual(Encoders.Hex.Decode(data), testBytes);
		}

		[Test]
		public void ShouldThrowFormatExceptionOnInvalidBase58()
		{
			Assert.Throws<FormatException>(() => Encoders.Base58.Decode("invalid"));
			Assert.DoesNotThrow(() => Encoders.Base58.Decode(" "));

			Assert.Throws<FormatException>(() => Encoders.Base58.Decode(" \t\n\v\f\r skip \r\f\v\n\t a"));
			var result = Encoders.Base58.Decode(" \t\n\v\f\r skip \r\f\v\n\t ");
			var expected2 = Encoders.Hex.Decode("971a55");
			CollectionAssert.AreEqual(result, expected2);
		}
	}
}
