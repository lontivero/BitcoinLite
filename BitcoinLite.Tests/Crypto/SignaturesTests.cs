using System;
using System.IO;
using System.Linq;
using BitcoinLite.Crypto;
using BitcoinLite.Encoding;
using NUnit.Framework;
using Newtonsoft.Json.Linq;

namespace BitcoinLite.Tests.Crypto
{
	[TestFixture(Category = "Crypto")]
	public class SignaturesTests
	{
		[Test, TestCaseSource(typeof(TestDataFactory), nameof(TestDataFactory.signatures))]
		public void Signatures(string name, JArray sig, bool isDER, bool isValid)
		{
			try
			{
				var derSig = sig.Select(x => (byte) x).ToArray();
				var s = ECDSASignature.FromDER(derSig);
				Assert.IsTrue(isValid);
			}
			catch (Exception e)
			{
				if (e is FormatException || e is EndOfStreamException)
					Assert.False(isValid);
				else
					Assert.Fail(name);
			}
		}

		[TestCase(TestName = "Signer - Generate and verify message")]
		public void CanSignMessage()
		{
			var dA = Hashes.SHA256(new byte[] { 0x01 });
			var k = Encoders.Hex.GetBytes("0098e9c07e8e6adb97b77d85b0c10a265e11737a89a3e37b");
			var rnd = new FakeRandom(k);
			var key = new Key(dA);

			var signer = new ECDsaSigner(rnd, key);

			var message = Encoders.Hex.GetBytes("66e98a165854cd07989b1ee0ec3f8dbe0ee3c2fb0051ef53a0be03457c4f21bc");
			var s1 = signer.GenerateSignature(message);
			Assert.IsTrue(signer.VerifySignature(message, s1));
			Assert.IsTrue(s1.R == (rnd.GetNextK()*Secp256k1.G).X%Secp256k1.N);

			Assert.IsTrue(ECDsaSigner.VerifySignature(message, s1.R, s1.S, key.PubKey));
		}

		[TestCase(TestName = "Signer - Verify message with bad signature")]
		public void InvalidData()
		{
			var message = Encoders.ASCII.GetBytes("hola");
			var isValid = ECDsaSigner.VerifySignature(message, 0, 0, ECPoint.Infinity);
			Assert.IsFalse(isValid);

			isValid = ECDsaSigner.VerifySignature(message, Secp256k1.N + 1, Secp256k1.N+1, ECPoint.Infinity);
			Assert.IsFalse(isValid);
		}

		[TestCase(TestName = "Signer - Sign with private key and verify with public key")]
		public void CanVerifyWithPublickKey()
		{
			var privateKey = Key.Create("hello world!");
			var message = Encoders.ASCII.GetBytes("hola");
			var signature = privateKey.Sign(message);
			var isValid = privateKey.PubKey.Verify(message, signature);
			Assert.IsTrue(isValid);
		}

		[TestCase(TestName = "Signer - Using deterministic k value")]
		public void DeterministicECDSA()
		{
			var privateKey = Hashes.SHA256(new byte[]{ 0x01 });
			var signer = new ECDsaSigner(new Key(privateKey));
			var signature = signer.GenerateSignature(Encoders.ASCII.GetBytes("hello word"));

			Assert.IsTrue(signature.R == TestUtils.HexToBigInteger("8804e75cbdac8ab296df53eaf8f64cee01c283aa4a18f3f5853317faa282d4cd"));
			Assert.IsTrue(signature.S == TestUtils.HexToBigInteger("42706d708c4fbf0dd075f3d3152872c7bd7e5ec9e2f691d33ce040d448f42002"));
		}

		[Test]
		public void ToFromDer()
		{
			var signature = new ECDSASignature(
				TestUtils.HexToBigInteger("657912a72d3ac8169fe8eaecd5ab401c94fc9981717e3e6dd4971889f785790c"),
				TestUtils.HexToBigInteger("00ed3bf3456eb76677fd899c8ccd1cc6d1ebc631b94c42f7c4578f28590d651c6e"));

			var expected = Encoders.Hex.GetBytes("30450220657912a72d3ac8169fe8eaecd5ab401c94fc9981717e3e6dd4971889f785790c022100ed3bf3456eb76677fd899c8ccd1cc6d1ebc631b94c42f7c4578f28590d651c6e");
			CollectionAssert.AreEqual(expected, signature.ToDER());

			var signature1 = ECDSASignature.FromDER(signature.ToDER());
			Assert.AreEqual(signature.R, signature1.R);
			Assert.AreEqual(signature.S, signature1.S);
		}
	}
}