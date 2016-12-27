using System;
using BitcoinLite.Crypto;
using BitcoinLite.Encoding;
using NUnit.Framework;

namespace BitcoinLite.Tests.Crypto
{
	[TestFixture(Category = "Crypto")]
	public class KeyTests
	{
		[TestCase(    "", TestName = "PrivateKey - Invalid")]
		[TestCase(  "00", TestName = "PrivateKey - Invalid")]
		[TestCase("FFFF", TestName = "PrivateKey - Invalid")]
		public void InvalidKeyLength(string s)
		{
			var arr = TestUtils.HexToBytes(s);
			Assert.Throws<ArgumentException>(() => new Key(arr));
		}

		[Test, TestCaseSource(typeof(TestDataFactory), nameof(TestDataFactory.base58_keys_invalid))]
		public void InvalidKey(string s)
		{
			Assert.Throws<FormatException>(() => Key.Parse(s));
			Assert.Throws<FormatException>(() => Address.FromString(s));
		}

		[Test, TestCaseSource(typeof(TestDataFactory), nameof(TestDataFactory.base58_keys_valid))]
		public void ValidKey(string wif, byte[] bytes, bool isPrivateKey, bool isTestnet, string addrType, bool isCompressed)
		{
			var network = isTestnet ? Network.Test : Network.Main;
			if (isPrivateKey)
			{
				var key = new Key(bytes, isCompressed);
				Assert.AreEqual(wif, key.ToString(network));
			}
			else
			{
				Address addr=null;
				if(addrType=="pubkey")
					addr = new PubKeyHashAddress(network, bytes);
				else if (addrType == "script")
					addr = new ScriptHashAddress(network, bytes);
				else
					Assert.Fail($"unknown address type: '{addrType}' for '{wif}'");

				Assert.AreEqual(wif, addr.ToString());
				Assert.AreEqual(addr.Destination.ToByteArray(), bytes);
			}
		}

		[Test]
		[TestCase("0000000000000000000000000000000000000000000000000000000000000000", TestName = "PrivateKey - Is Zero (out of range)")]
		[TestCase("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEBAAEDCE6AF48A03BBFD25E8CD0364141", TestName = "PrivateKey - Is Greater than Secp256k1 N (out of range)")]
		public void PrivateKeyOutOfRange(string s)
		{
			var arr = TestUtils.HexToBytes(s);
			Assert.Throws<ArgumentException>(()=>new Key(arr));
		}

		[Test]
		public void CompressedPrivateKey()
		{
			var pk = Key.Create();
			var str = pk.ToString(Network.Main);
		}

		[Test]
		public void CanGeneratePubKeysAndAddress()
		{
			//Took from http://brainwallet.org/ and http://procbits.com/2013/08/27/generating-a-bitcoin-address-with-javascript
			var tests = new[]
			{
				new 
				{
					PrivateKeyWIF = "5Hx15HFGyep2CfPxsJKe2fXJsCVn5DEiyoeGGF6JZjGbTRnqfiD",
					CompressedPrivateKeyWIF = "KwomKti1X3tYJUUMb1TGSM2mrZk1wb1aHisUNHCQXTZq5auC2qc3",
					PubKey = "04d0988bfa799f7d7ef9ab3de97ef481cd0f75d2367ad456607647edde665d6f6fbdd594388756a7beaf73b4822bc22d36e9bda7db82df2b8b623673eefc0b7495",
					CompressedPubKey = "03d0988bfa799f7d7ef9ab3de97ef481cd0f75d2367ad456607647edde665d6f6f",
					Address =           "16UjcYNBG9GTK4uq2f7yYEbuifqCzoLMGS",
					CompressedAddress = "1FkKMsKNJqWSDvTvETqcCeHcUQQ64kSC6s",
					Hash160 = "3c176e659bea0f29a3e9bf7880c112b1b31b4dc8",
					CompressedHash160 = "a1c2f92a9dacbd2991c3897724a93f338e44bdc1",
					DER = "3082011302010104201184cd2cdd640ca42cfc3a091c51d549b2f016d454b2774019c2b2d2e08529fda081a53081a2020101302c06072a8648ce3d0101022100fffffffffffffffffffffffffffffffffffffffffffffffffffffffefffffc2f300604010004010704410479be667ef9dcbbac55a06295ce870b07029bfcdb2dce28d959f2815b16f81798483ada7726a3c4655da4fbfc0e1108a8fd17b448a68554199c47d08ffb10d4b8022100fffffffffffffffffffffffffffffffebaaedce6af48a03bbfd25e8cd0364141020101a14403420004d0988bfa799f7d7ef9ab3de97ef481cd0f75d2367ad456607647edde665d6f6fbdd594388756a7beaf73b4822bc22d36e9bda7db82df2b8b623673eefc0b7495",
					CompressedDER = "3081d302010104201184cd2cdd640ca42cfc3a091c51d549b2f016d454b2774019c2b2d2e08529fda08185308182020101302c06072a8648ce3d0101022100fffffffffffffffffffffffffffffffffffffffffffffffffffffffefffffc2f300604010004010704210279be667ef9dcbbac55a06295ce870b07029bfcdb2dce28d959f2815b16f81798022100fffffffffffffffffffffffffffffffebaaedce6af48a03bbfd25e8cd0364141020101a12403220003d0988bfa799f7d7ef9ab3de97ef481cd0f75d2367ad456607647edde665d6f6f"
				},
				new
				{
					PrivateKeyWIF = "5J7WTMRn1vjZ9udUxNCLq7F9DYEJiqRCjstiBrY6mDjnaomd6kZ",
					CompressedPrivateKeyWIF = "KxXj1KAMh6ApvKJ2PNZ4XLZRGLqjDehppFdEnueGSBDrC2Hfe7vt",
					PubKey = "0493e5d305cad2588d5fb254065fe48ce446028ba380e6ee663baea9cd105500897eb030c033cdab160f31c36df0ea38330fdd69677df49cd14826902022d17f3f",
					CompressedPubKey = "0393e5d305cad2588d5fb254065fe48ce446028ba380e6ee663baea9cd10550089",
					Address =           "1MZmwgyMyjM11uA6ZSpgn1uK3LBWCzvV6e",
					CompressedAddress = "1AECNr2TDye8dpC1TeDH3eJpGoZ7dNPy4g",
					Hash160 = "e19557c8f8fb53a964c5dc7bfde86d806709f7c5",
					CompressedHash160 = "6538094af65453ea279f14d1a04b408e3adfebd7",
					DER = "308201130201010420271ac4d7056937c156abd828850d05df0697dd662d3c1b0107f53a387b4c176ca081a53081a2020101302c06072a8648ce3d0101022100fffffffffffffffffffffffffffffffffffffffffffffffffffffffefffffc2f300604010004010704410479be667ef9dcbbac55a06295ce870b07029bfcdb2dce28d959f2815b16f81798483ada7726a3c4655da4fbfc0e1108a8fd17b448a68554199c47d08ffb10d4b8022100fffffffffffffffffffffffffffffffebaaedce6af48a03bbfd25e8cd0364141020101a1440342000493e5d305cad2588d5fb254065fe48ce446028ba380e6ee663baea9cd105500897eb030c033cdab160f31c36df0ea38330fdd69677df49cd14826902022d17f3f",
					CompressedDER = "3081d30201010420271ac4d7056937c156abd828850d05df0697dd662d3c1b0107f53a387b4c176ca08185308182020101302c06072a8648ce3d0101022100fffffffffffffffffffffffffffffffffffffffffffffffffffffffefffffc2f300604010004010704210279be667ef9dcbbac55a06295ce870b07029bfcdb2dce28d959f2815b16f81798022100fffffffffffffffffffffffffffffffebaaedce6af48a03bbfd25e8cd0364141020101a1240322000393e5d305cad2588d5fb254065fe48ce446028ba380e6ee663baea9cd10550089"
				}
			};

			foreach (var test in tests)
			{
				var privateKey = Key.Parse(test.PrivateKeyWIF);
				Assert.AreEqual(test.PubKey, privateKey.PubKey.ToString());
				Assert.AreEqual(Encoders.Hex.GetString(Base58Data.FromString(test.PrivateKeyWIF)), privateKey.ToString());

				var address = Address.FromString(test.Address);
				Assert.AreEqual(KeyId.Parse(test.Hash160), address.Destination);
				Assert.AreEqual(KeyId.Parse(test.Hash160), privateKey.PubKey.Hash);
				Assert.AreEqual(address.ScriptPubKey.ToString(), privateKey.PubKey.ToAddress(Network.Main).ScriptPubKey.ToString());
				Assert.True(privateKey.PubKey.IsCanonical);
				Assert.AreEqual(test.Address, privateKey.PubKey.ToString(Network.Main));
				Assert.AreEqual(address.ScriptPubKey.ToString(), privateKey.PubKey.Hash.ScriptPubKey.ToString());
				Assert.AreEqual(address.ScriptPubKey.ToString(), new KeyId(privateKey.PubKey).ScriptPubKey.ToString());

				var compressedPrivKey = new Key(privateKey.ToByteArray(), true);
				Assert.AreEqual(test.CompressedPrivateKeyWIF, compressedPrivKey.ToString(Network.Main));
				Assert.AreEqual(test.CompressedPubKey, compressedPrivKey.PubKey.ToString());
				// Assert.True(compressedPrivKey.PubKey.IsCompressed);
				Assert.True(compressedPrivKey.PubKey.IsCanonical);
				Assert.AreEqual(test.CompressedAddress, compressedPrivKey.PubKey.ToString(Network.Main));

				var compressedAddr = Address.FromString(test.CompressedAddress);
				Assert.AreEqual(KeyId.Parse(test.CompressedHash160), compressedAddr.Destination);
				Assert.AreEqual(KeyId.Parse(test.CompressedHash160), compressedPrivKey.PubKey.Hash);
				Assert.AreEqual(compressedAddr.ScriptPubKey.ToString(), compressedPrivKey.PubKey.Hash.ScriptPubKey.ToString());
			}
		}
	}

	[TestFixture(Category = "Address")]
	public class DestinationTests
	{
		[Test]
		public void GetHashCodeTest()
		{
			var hash = Key.Create().PubKey.Hash.ToByteArray();
			var id = new KeyId(hash);
			Assert.AreEqual(hash.GetHashCode(), id.GetHashCode());
		}

		[Test]
		public void ParseTest()
		{
			var hash = Encoders.Hex.GetString(Key.Create().PubKey.Hash.ToByteArray());
			var id = KeyId.Parse(hash);
			Assert.AreEqual(hash, id.ToString());
		}

	}
}
