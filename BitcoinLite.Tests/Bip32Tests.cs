﻿using System;
using System.Collections.Generic;
using System.Linq;
using BitcoinLite.Bip32;
using BitcoinLite.Crypto;
using BitcoinLite.Encoding;
using NUnit.Framework;

namespace BitcoinLite.Tests
{
	[TestFixture]
	public class Bip32Tests
	{
		[Test]
		public void CanRecoverExtKeyFromExtPubKeyAndOneChildExtKey()
		{
			var extkey =
				HdKey.Parse(
					"xprv9s21ZrQH143K3Z9EwCXrA5VbypnvWGiE9z22S1cLLPi7r8DVUkTabBvMjeirS8KCyppw24KoD4sFmja8UDU4VL32SBdip78LY6sz3X2GPju");
			var key = extkey.Derive(1);
			var pubkey = key.Neuter;
			var childKey = key.Derive(1);

			var recovered = childKey.GetParentExtKey(pubkey);
			Assert.AreEqual(recovered.ToString(Network.Main), key.ToString(Network.Main));

			childKey = key.Derive((uint) int.MaxValue + 1);
			Assert.Throws<InvalidOperationException>(() => childKey.GetParentExtKey(pubkey));

			childKey = key.Derive(1).Derive(1);
			Assert.Throws<ArgumentException>(() => childKey.GetParentExtKey(pubkey));
		}

		[Test]
		public void CanRecoverExtKeyFromExtPubKeyAndOneChildExtKey2()
		{
			for (uint i = 0; i < 255; i++)
			{
				var key = new HdKey().Derive(i);
				var childKey = key.Derive(i);
				var recovered = childKey.GetParentExtKey(key.Neuter);
				Assert.AreEqual(recovered.ToString(Network.Main), key.ToString(Network.Main));
			}
		}

		[Test]
		public void FristTestVectorTest()
		{
			var entropy = "000102030405060708090a0b0c0d0e0f";
			var children = new[]
			{
				Tuple.Create(
					"xpub661MyMwAqRbcFtXgS5sYJABqqG9YLmC4Q1Rdap9gSE8NqtwybGhePY2gZ29ESFjqJoCu1Rupje8YtGqsefD265TMg7usUDFdp6W1EGMcet8",
					"xprv9s21ZrQH143K3QTDL4LXw2F7HEK3wJUD2nW2nRk4stbPy6cq3jPPqjiChkVvvNKmPGJxWUtg6LnF5kejMRNNU3TGtRBeJgk33yuGBxrMPHi",
					0x80000000),
				Tuple.Create(
					"xpub68Gmy5EdvgibQVfPdqkBBCHxA5htiqg55crXYuXoQRKfDBFA1WEjWgP6LHhwBZeNK1VTsfTFUHCdrfp1bgwQ9xv5ski8PX9rL2dZXvgGDnw",
					"xprv9uHRZZhk6KAJC1avXpDAp4MDc3sQKNxDiPvvkX8Br5ngLNv1TxvUxt4cV1rGL5hj6KCesnDYUhd7oWgT11eZG7XnxHrnYeSvkzY7d2bhkJ7",
					(uint) 1),
				Tuple.Create(
					"xpub6ASuArnXKPbfEwhqN6e3mwBcDTgzisQN1wXN9BJcM47sSikHjJf3UFHKkNAWbWMiGj7Wf5uMash7SyYq527Hqck2AxYysAA7xmALppuCkwQ",
					"xprv9wTYmMFdV23N2TdNG573QoEsfRrWKQgWeibmLntzniatZvR9BmLnvSxqu53Kw1UmYPxLgboyZQaXwTCg8MSY3H2EU4pWcQDnRnrVA1xe8fs",
					(uint) 0x80000002),
				Tuple.Create(
					"xpub6D4BDPcP2GT577Vvch3R8wDkScZWzQzMMUm3PWbmWvVJrZwQY4VUNgqFJPMM3No2dFDFGTsxxpG5uJh7n7epu4trkrX7x7DogT5Uv6fcLW5",
					"xprv9z4pot5VBttmtdRTWfWQmoH1taj2axGVzFqSb8C9xaxKymcFzXBDptWmT7FwuEzG3ryjH4ktypQSAewRiNMjANTtpgP4mLTj34bhnZX7UiM",
					(uint) 2),
				Tuple.Create(
					"xpub6FHa3pjLCk84BayeJxFW2SP4XRrFd1JYnxeLeU8EqN3vDfZmbqBqaGJAyiLjTAwm6ZLRQUMv1ZACTj37sR62cfN7fe5JnJ7dh8zL4fiyLHV",
					"xprvA2JDeKCSNNZky6uBCviVfJSKyQ1mDYahRjijr5idH2WwLsEd4Hsb2Tyh8RfQMuPh7f7RtyzTtdrbdqqsunu5Mm3wDvUAKRHSC34sJ7in334",
					(uint) 1000000000),
				Tuple.Create(
					"xpub6H1LXWLaKsWFhvm6RVpEL9P4KfRZSW7abD2ttkWP3SSQvnyA8FSVqNTEcYFgJS2UaFcxupHiYkro49S8yGasTvXEYBVPamhGW6cFJodrTHy",
					"xprvA41z7zogVVwxVSgdKUHDy1SKmdb533PjDz7J6N6mV6uS3ze1ai8FHa8kmHScGpWmj4WggLyQjgPie1rFSruoUihUZREPSL39UNdE3BBDu76",
					(uint) 0)
			};
			VerifyTestVector(entropy, children);
		}

		[Test]
		public void SecondTestVectorTest()
		{
			var entropy =
				"fffcf9f6f3f0edeae7e4e1dedbd8d5d2cfccc9c6c3c0bdbab7b4b1aeaba8a5a29f9c999693908d8a8784817e7b7875726f6c696663605d5a5754514e4b484542";
			var children = new[]
			{
				Tuple.Create(
					"xpub661MyMwAqRbcFW31YEwpkMuc5THy2PSt5bDMsktWQcFF8syAmRUapSCGu8ED9W6oDMSgv6Zz8idoc4a6mr8BDzTJY47LJhkJ8UB7WEGuduB",
					"xprv9s21ZrQH143K31xYSDQpPDxsXRTUcvj2iNHm5NUtrGiGG5e2DtALGdso3pGz6ssrdK4PFmM8NSpSBHNqPqm55Qn3LqFtT2emdEXVYsCzC2U",
					0u),
				Tuple.Create(
					"xpub69H7F5d8KSRgmmdJg2KhpAK8SR3DjMwAdkxj3ZuxV27CprR9LgpeyGmXUbC6wb7ERfvrnKZjXoUmmDznezpbZb7ap6r1D3tgFxHmwMkQTPH",
					"xprv9vHkqa6EV4sPZHYqZznhT2NPtPCjKuDKGY38FBWLvgaDx45zo9WQRUT3dKYnjwih2yJD9mkrocEZXo1ex8G81dwSM1fwqWpWkeS3v86pgKt",
					0xFFFFFFFF),
				Tuple.Create(
					"xpub6ASAVgeehLbnwdqV6UKMHVzgqAG8Gr6riv3Fxxpj8ksbH9ebxaEyBLZ85ySDhKiLDBrQSARLq1uNRts8RuJiHjaDMBU4Zn9h8LZNnBC5y4a",
					"xprv9wSp6B7kry3Vj9m1zSnLvN3xH8RdsPP1Mh7fAaR7aRLcQMKTR2vidYEeEg2mUCTAwCd6vnxVrcjfy2kRgVsFawNzmjuHc2YmYRmagcEPdU9",
					1u),
				Tuple.Create(
					"xpub6DF8uhdarytz3FWdA8TvFSvvAh8dP3283MY7p2V4SeE2wyWmG5mg5EwVvmdMVCQcoNJxGoWaU9DCWh89LojfZ537wTfunKau47EL2dhHKon",
					"xprv9zFnWC6h2cLgpmSA46vutJzBcfJ8yaJGg8cX1e5StJh45BBciYTRXSd25UEPVuesF9yog62tGAQtHjXajPPdbRCHuWS6T8XA2ECKADdw4Ef",
					0xFFFFFFFE),
				Tuple.Create(
					"xpub6ERApfZwUNrhLCkDtcHTcxd75RbzS1ed54G1LkBUHQVHQKqhMkhgbmJbZRkrgZw4koxb5JaHWkY4ALHY2grBGRjaDMzQLcgJvLJuZZvRcEL",
					"xprvA1RpRA33e1JQ7ifknakTFpgNXPmW2YvmhqLQYMmrj4xJXXWYpDPS3xz7iAxn8L39njGVyuoseXzU6rcxFLJ8HFsTjSyQbLYnMpCqE2VbFWc",
					2u),
				Tuple.Create(
					"xpub6FnCn6nSzZAw5Tw7cgR9bi15UV96gLZhjDstkXXxvCLsUXBGXPdSnLFbdpq8p9HmGsApME5hQTZ3emM2rnY5agb9rXpVGyy3bdW6EEgAtqt",
					"xprvA2nrNbFZABcdryreWet9Ea4LvTJcGsqrMzxHx98MMrotbir7yrKCEXw7nadnHM8Dq38EGfSh6dqA9QWTyefMLEcBYJUuekgW4BYPJcr9E7j",
					0u)
			};
			VerifyTestVector(entropy, children);
		}

		private static void VerifyTestVector(string entropyStr, IEnumerable<Tuple<string, string, uint>> children)
		{
			var entropy = Encoders.Hex.GetBytes(entropyStr);
			var key = HdKey.FromEntrophy(entropy);
			var pubkey = key.Neuter;

			foreach (var child in children.Select(c => new {Key = c.Item2, PubKey = c.Item1, Child = c.Item3}))
			{
				var data = key.ToByteArray();
				Assert.AreEqual(74, data.Length);
				Assert.AreEqual(child.Key, key.ToString(Network.Main));
				Assert.AreEqual(child.Key, HdKey.Parse(child.Key).ToString(Network.Main));
				data = pubkey.ToByteArray();
				Assert.AreEqual(74, data.Length);
				Assert.AreEqual(child.PubKey, pubkey.ToString(Network.Main));
				Assert.AreEqual(child.PubKey, HdPubKey.Parse(child.PubKey).ToString(Network.Main));

				var keyNew = key.Derive(child.Child);
				var pubkeyNew = keyNew.Neuter;
				if ((child.Child & 0x80000000) == 0)
				{
					// Compare with public derivation
					var pubkeyNew2 = pubkey.Derive(child.Child);
					Assert.AreEqual(pubkeyNew.ToString(Network.Main), pubkeyNew2.ToString(Network.Main));
				}
				key = keyNew;
				pubkey = pubkeyNew;
			}
		}

		[TestCase("m/0/1/2/3/4/5")]
		[TestCase("m/0/10/20/30/40/50")]
		public void CanUseKeyPath(string path)
		{
			var masterkey = HdKey.FromEntrophy(Encoders.ASCII.GetBytes("Hello world!"));
			var masterPubkey = masterkey.Neuter;
			var key = masterkey;
			var pubKey = masterPubkey;
			var children = path.Split('/').Skip(1).Select(uint.Parse);
			foreach (var child in children)
			{
				key = key.Derive(child);
				pubKey = pubKey.Derive(child);
			}

			Assert.AreEqual(key.ToString(Network.Main), masterkey.Derive(KeyPath.Parse(path)).ToString(Network.Main));
			Assert.AreEqual(pubKey.ToString(Network.Main), masterPubkey.Derive(KeyPath.Parse(path)).ToString(Network.Main));
		}
	}
}
