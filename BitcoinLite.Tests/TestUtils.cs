using System;
using System.Collections;
using System.Linq;
using System.Numerics;
using BitcoinLite.Crypto;
using BitcoinLite.Encoding;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace BitcoinLite.Tests
{
	internal static class TestUtils
	{
		internal static byte[] HexToBytes(string hex)
		{
			if (hex.Length%2 == 1)
				hex = "0" + hex;

			var bytes = Encoders.Hex.GetBytes(hex);
			Array.Reverse(bytes);
			return bytes;
		}

		internal static string ToHex(this byte[] arr)
		{
			return Encoders.Hex.GetString(arr);
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

		public static IEnumerable ec_points_comparison
		{
			get
			{
				foreach (var line in JsonFile.GetData("ec_points_comparison.json"))
				{
					ECPoint p1 = null;
					var l0 = (string)line[0];
					var l1 = (string)line[1];
					if (!string.IsNullOrWhiteSpace(l0) && !string.IsNullOrWhiteSpace(l1))
					{
						var x = TestUtils.HexToBigInteger(l0);
						var y = TestUtils.HexToBigInteger(l1);
						p1 = new ECPoint(x, y);
					}

					ECPoint p2 = null;
					var l2 = (string)line[2];
					var l3 = (string)line[3];
					if (!string.IsNullOrWhiteSpace(l2) && !string.IsNullOrWhiteSpace(l3))
					{
						var x = TestUtils.HexToBigInteger(l2);
						var y = TestUtils.HexToBigInteger(l3);
						p2 = new ECPoint(x, y);
					}

					var tc = new TestCaseData(
						p1,
						p2,
						(bool)line[4]
						);
					tc.SetName("ECPoint - " + (string)line[5]);
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

		public static IEnumerable base58_keys_invalid
		{
			get
			{
				return JsonFile.GetData("base58_keys_invalid.json").Select(line => {
					var tc = new TestCaseData(line);
					tc.SetName("Invalid Key - " + (string)line[0]);
					return tc;
				});
			}
		}

		public static IEnumerable base58_keys_valid
		{
			get
			{
				foreach (var line in JsonFile.GetData("base58_keys_valid.json"))
				{
					var wif = (string)line[0];
					var bytes = Encoders.Hex.GetBytes((string)line[1]);
					var metadata = (JObject)line[2];
					var tc = new TestCaseData(
						wif, bytes, 
						(string)metadata["isPrivkey"] == "True", 
						(string)metadata["isTestnet"] == "True", 
						(string)metadata["addrType"],
						(string)metadata["isCompressed"] == "True");
					tc.SetName("Valid Key - " + wif);
					yield return tc;
				}
			}
		}

	}
}