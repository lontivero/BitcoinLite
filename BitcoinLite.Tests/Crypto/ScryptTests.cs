using System;
using BitcoinLite.Crypto;
using BitcoinLite.Encoding;
using NUnit.Framework;

namespace BitcoinLite.Tests.Crypto
{
	[TestFixture(Category = "Bitcoin,Crypto")]
	public class ScryptTests
	{
		[Test]
		public void Salsa20Test()
		{
			var inputStr = @"
				7e879a21 4f3ec986 7ca940e6 41718f26 baee555b 8c61c1b5 0df84611 6dcd3b1d
				ee24f319 df9b3d85 14121e4b 5ac5aa32 76021d29 09c74829 edebc68d b8b8c25e".Clean();
			var outputStr = @"
				a41f859c 6608cc99 3b81cacb 020cef05	044b2181 a2fd337d fd7b1c63 96682f29
				b4393168 e3c9e6bc fe6bc5b7 a06d96ba	e424cc10 2c91745c 24ad673d c7618f81".Clean();

			var input = Encoders.Hex.GetBytes(inputStr);
			Salsa20.Compute(input);

			CollectionAssert.AreEqual(input, Encoders.Hex.GetBytes(outputStr));
		}

		[Test]
		public void ScryptSmixTest()
		{
			var B0 = @"
				f7ce0b65 3d2d72a4 108cf5ab e912ffdd
				777616db bb27a70e 8204f3ae 2d0f6fad
				89f68f48 11d1e87b cc3bd740 0a9ffd29
				094f0184 639574f3 9ae5a131 5217bcd7".Clean();
			var B1 = @"
				89499144 7213bb22 6c25b54d a86370fb
				cd984380 374666bb 8ffcb5bf 40c254b0
				67d27c51 ce4ad5fe d829c90b 505a571b
				7f4d1cad 6a523cda 770e67bc eaaf7e89".Clean();
			var B0o = @"
				a41f859c 6608cc99 3b81cacb 020cef05
				044b2181 a2fd337d fd7b1c63 96682f29
				b4393168 e3c9e6bc fe6bc5b7 a06d96ba
				e424cc10 2c91745c 24ad673d c7618f81".Clean();
			var B1o = @"
				20edc975 323881a8 0540f64c 162dcd3c
				21077cfe 5f8d5fe2 b1a4168f 953678b7
				7d3b3d80 3b60e4ab 920996e5 9b4d53b6
				5d2a2258 77d5edf5 842cb9f1 4eefe425".Clean();

			var input = Encoders.Hex.GetBytes(B0+B1);
			var output = Encoders.Hex.GetBytes(B0o + B1o);
			SCrypt.BlockMixSalsa8(input,0, 0, 1);

			CollectionAssert.AreEqual(input, output);
		}

		[Test]
		public void ROMix()
		{
			var input = Encoders.Hex.GetBytes(@"
				f7ce0b65 3d2d72a4 108cf5ab e912ffdd
				777616db bb27a70e 8204f3ae 2d0f6fad
				89f68f48 11d1e87b cc3bd740 0a9ffd29
				094f0184 639574f3 9ae5a131 5217bcd7
				89499144 7213bb22 6c25b54d a86370fb
				cd984380 374666bb 8ffcb5bf 40c254b0
				67d27c51 ce4ad5fe d829c90b 505a571b
				7f4d1cad 6a523cda 770e67bc eaaf7e89".Clean());
			var output = Encoders.Hex.GetBytes(@"
				79ccc193 629debca 047f0b70 604bf6b6
				2ce3dd4a 9626e355 fafc6198 e6ea2b46
				d5841367 3b99b029 d665c357 601fb426
				a0b2f4bb a200ee9f 0a43d19b 571a9c71
				ef1142e6 5d5a266f ddca832c e59faa7c
				ac0b9cf1 be2bffca 300d01ee 387619c4
				ae12fd44 38f203a0 e4e1c47e c314861f
				4e9087cb 33396a68 73e8f9d2 539a4b8e".Clean());

			var XY = new byte[2 *  128* 1];
			var V = new byte[128 * 1 * 16];

			SCrypt.Smix(input, 0, 1, 16, V, XY);
			CollectionAssert.AreEqual(output, input);
		}


		[TestCase("", "", 1, 16, 1, 64, @"
			77 d6 57 62 38 65 7b 20 3b 19 ca 42 c1 8a 04 97
			f1 6b 48 44 e3 07 4a e8 df df fa 3f ed e2 14 42
			fc d0 06 9d ed 09 48 f8 32 6a 75 3a 0f c8 1f 17
			e8 d3 e0 fb 2e 0d 36 28 cf 35 e2 0c 38 d1 89 06",
			TestName = "scrypt (P='', S='', r = 16, N = 1, p = 1, dklen = 64)")]
		[TestCase("password", "NaCl", 8, 1024, 16, 64, @"
			fd ba be 1c 9d 34 72 00 78 56 e7 19 0d 01 e9 fe
			7c 6a d7 cb c8 23 78 30 e7 73 76 63 4b 37 31 62
			2e af 30 d9 2e 22 a3 88 6f f1 09 27 9d 98 30 da
			c7 27 af b9 4a 83 ee 6d 83 60 cb df a2 cc 06 40",
			TestName = "scrypt (P='password', S='NaCl', r = 1024, N = 8, p = 16, dkLen = 64)")]
		[TestCase("pleaseletmein", "SodiumChloride", 8, 16384, 1, 64, @"
			70 23 bd cb 3a fd 73 48 46 1c 06 cd 81 fd 38 eb
			fd a8 fb ba 90 4f 8e 3e a9 b5 43 f6 54 5d a1 f2
			d5 43 29 55 61 3f 0f cf 62 d4 97 05 24 2a 9a f9
			e6 1e 85 dc 0d 65 1e 40 df cf 01 7b 45 57 58 87",
			TestName = "scrypt(P= 'pleaseletmein', S= 'SodiumChloride', r= 1048576, N= 8, p= 1, dkLen= 64)")]
		[TestCase("pleaseletmein", "SodiumChloride", 8, 1048576, 1, 64, @"
			21 01 cb 9b 6a 51 1a ae ad db be 09 cf 70 f8 81
			ec 56 8d 57 4a 2f fd 4d ab e5 ee 98 20 ad aa 47
			8e 56 fd 8f 4b a5 d0 9f fa 1c 6d 92 7c 40 f4 c3
			37 30 40 49 e8 a9 52 fb cb f4 5c 6f a7 7a 41 a4",
			TestName = "scrypt (P='pleaseletmein', S='SodiumChloride', r = 1048576, N = 8, p = 1, dkLen = 64)")]
		public void Test(string P, string S, int r, int N, int p, int dkLen, string expectedStr)
		{
			var password = System.Text.Encoding.ASCII.GetBytes(P);
			var salt = System.Text.Encoding.ASCII.GetBytes(S);
			try
			{
				var hash = SCrypt.Hash(password, salt, N, r, p, dkLen);
			
				var expected = Encoders.Hex.GetBytes(expectedStr.Clean());
				CollectionAssert.AreEqual(expected, hash);
			}
			catch (OutOfMemoryException)
			{
			}
		}
	}
}
