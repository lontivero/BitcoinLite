using System;
using System.Collections;
using System.Linq;
using System.Numerics;
using System.Text;
using BitcoinLite.Encoding;
using NUnit.Framework;

namespace BitcoinLite.Tests
{
	internal static class TestUtils
	{
		internal static byte[] HexToBytes(string hex)
		{
			if (hex.Length%2 == 1)
				hex = "0" + hex;

			var bytes = Encoders.Hex.Decode(hex);
			Array.Reverse(bytes);
			return bytes;
		}

		internal static string ToHex(this byte[] arr)
		{
			return Encoders.Hex.Encode(arr);
		}

		internal static BigInteger HexToBigInteger(string hex)
		{
			var bytes = HexToBytes(hex);
			if (bytes[bytes.Length - 1] > 0x7F)
			{
				Array.Resize(ref bytes, bytes.Length + 1);
				bytes[bytes.Length - 1] = 0x00;
			}
			return new BigInteger(bytes);
		}
	}

	public class TestDataFactory
	{
		public static IEnumerable ec_points_mul
		{
			get
			{
				foreach (var line in JsonFile.GetData("ec_points_mul.json"))
				{
					var tc = new TestCaseData(
						TestUtils.HexToBigInteger((string)line[0]),
						TestUtils.HexToBigInteger((string)line[1]),
						TestUtils.HexToBigInteger((string)line[2]));

					tc.SetName("ECPoint - " + (string)line[3]);
					yield return tc;
				}
			}
		}

		public static IEnumerable ec_points_dbl
		{
			get
			{
				foreach (var line in JsonFile.GetData("ec_points_dbl.json"))
				{
					var tc = new TestCaseData(
						TestUtils.HexToBigInteger((string)line[0]),
						TestUtils.HexToBigInteger((string)line[1]),
						TestUtils.HexToBigInteger((string)line[2]),
						TestUtils.HexToBigInteger((string)line[3]));

					tc.SetName("ECPoint - " + (string)line[4]);
					yield return tc;
				}
			}
		}

		public static IEnumerable ec_points_add
		{
			get
			{
				foreach (var line in JsonFile.GetData("ec_points_add.json"))
				{
					var tc = new TestCaseData(
						TestUtils.HexToBigInteger((string) line[0]),
						TestUtils.HexToBigInteger((string) line[1]),
						TestUtils.HexToBigInteger((string) line[2]),
						TestUtils.HexToBigInteger((string) line[3]),
						TestUtils.HexToBigInteger((string) line[4]),
						TestUtils.HexToBigInteger((string) line[5])
						);
					tc.SetName("ECPoint - " + (string) line[6]);
					yield return tc;
				}
			}
		}

		public static IEnumerable signatures
		{
			get 
			{
				return JsonFile.GetData("signatures.json").Select(line =>
				{
					var tc = new TestCaseData(line);
					tc.SetName("Signature - " + (string)line[0]);
					return tc;
				});
			}
		}

		public static IEnumerable targets
		{
			get
			{
				return JsonFile.GetData("targets.json").Select(line => {
					var tc = new TestCaseData(line);
					tc.SetName("Targets - " + (string) line[0]);
					return tc;
				});
			}
		}
	}
}