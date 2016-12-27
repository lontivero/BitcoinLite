using System.Security.Cryptography;
using BitcoinLite.Crypto;
using BitcoinLite.Encoding;
using NUnit.Framework;

namespace BitcoinLite.Tests.Crypto
{
	[TestFixture(Category = "Crypto,Pbkdf2")]
	public class Pbkdf2Tests
	{
		[TestCase("admin",    "palomaherrera",  1000, 10, TestName = "Pbkdf2-Rfc2898 admin:palomaherrera:1000:10")]
		[TestCase("admin",    "palomaherrera",  2000, 13, TestName = "Pbkdf2-Rfc2898 admin:palomaherrera:1000:13")]
		[TestCase("admin",    "palomaherrera", 10000, 20, TestName = "Pbkdf2-Rfc2898 admin:palomaherrera:10000:20")]
		[TestCase("password", "saltlakecity",   1000, 21, TestName = "Pbkdf2-Rfc2898 password:saltlakecity:1000:21")]
		[TestCase("password", "saltlakecity",   2000, 57, TestName = "Pbkdf2-Rfc2898 password:saltlakecity:2000:57")]
		[TestCase("password", "saltlakecity",  10000, 500, TestName= "Pbkdf2-Rfc2898 password:saltlakecity:10000:500")]
		public void ComparisonTest(string password, string salt, int iterations, int bytes)
		{
			var _pass = System.Text.Encoding.UTF8.GetBytes(password);
			var _salt = System.Text.Encoding.UTF8.GetBytes(salt);
			var algo1 = new Rfc2898DeriveBytes(_pass, _salt, iterations);
			var algo2 = new Pbkdf2(new HMACSHA1(_pass), _salt, iterations);
			
			var h1 = algo1.GetBytes(bytes).ToHex();
			var h2 = algo2.GetBytes(bytes).ToHex();
			Assert.AreEqual(h1, h2);

			var h3 = algo1.GetBytes(bytes + 7).ToHex();
			var h4 = algo2.GetBytes(bytes + 7).ToHex();
			Assert.AreEqual(h3, h4);

			var h5 = algo1.GetBytes(1).ToHex();
			var h6 = algo2.GetBytes(1).ToHex();
			Assert.AreEqual(h5, h6);

			h5 = algo1.GetBytes(1).ToHex();
			h6 = algo2.GetBytes(1).ToHex();
			Assert.AreEqual(h5, h6);

			h5 = algo1.GetBytes(1).ToHex();
			h6 = algo2.GetBytes(1).ToHex();
			Assert.AreEqual(h5, h6);
		}

		[Test]
		public void TestVector_PBKDF2_HMAC_SHA_256()
		{
			var p = System.Text.Encoding.ASCII.GetBytes("passwd");
			var s = System.Text.Encoding.ASCII.GetBytes("salt");
			var pbkdf = new Pbkdf2(new HMACSHA256(p), s, 1);

			var dk = pbkdf.GetBytes(64);
			var expected = Encoders.Hex.GetBytes(@"
				55 ac 04 6e 56 e3 08 9f ec 16 91 c2 25 44 b6 05
				f9 41 85 21 6d de 04 65 e6 8b 9d 57 c2 0d ac bc
				49 ca 9c cc f1 79 b6 45 99 16 64 b3 9d 77 ef 31
				7c 71 b8 45 b1 e3 0b d5 09 11 20 41 d3 a1 97 83".Clean());

			CollectionAssert.AreEqual(expected, dk);

			p = System.Text.Encoding.ASCII.GetBytes("Password");
			s = System.Text.Encoding.ASCII.GetBytes("NaCl");
			pbkdf = new Pbkdf2(new HMACSHA256(p), s, 80000);

			dk = pbkdf.GetBytes(64);
			expected = Encoders.Hex.GetBytes(@"
				4d dc d8 f6 0b 98 be 21 83 0c ee 5e f2 27 01 f9
				64 1a 44 18 d0 4c 04 14 ae ff 08 87 6b 34 ab 56
				a1 d4 25 a1 22 58 33 54 9a db 84 1b 51 c9 b3 17
				6a 27 2b de bb a1 d0 78 47 8f 62 b3 97 f3 3c 8d".Clean());

			CollectionAssert.AreEqual(expected, dk);
		}
	}
}
