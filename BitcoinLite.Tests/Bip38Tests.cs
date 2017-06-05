using System;
using System.Linq;
using BitcoinLite.Bip38;
using BitcoinLite.Crypto;
using NUnit.Framework;

namespace BitcoinLite.Tests
{
	[TestFixture(Category = "Crypto, Bip32")]
	public class Bip38Tests
	{
		[Test(Description = "Encrypt and Decrypt Private Key")]
		public void CanEncryptAndDecrypt()
		{
			var key = Key.Parse("5Hx15HFGyep2CfPxsJKe2fXJsCVn5DEiyoeGGF6JZjGbTRnqfiD").ToByteArray();
			var derived = Enumerable.Range(1, 64).Select(x=>(byte)x).ToArray();
			var encryptedKey = EncryptedKey.EncryptKey(key, derived);
			CollectionAssert.AreEqual(key, EncryptedKey.DecryptKey(encryptedKey, derived));
		}


		[TestCase(
			"TestingOneTwoThree", 
			"6PRVWUbkzzsbcVac2qwfssoUJAN1Xhrg6bNk8J7Nzm5H7kxEbn2Nh2ZoGg",
			"5KN7MzqK5wt2TP1fQCYyHBtDrXdJuXbUzm4A9rKAteGu3Qi5CVR",
			false
			)]
		[TestCase(
			"Satoshi",
			"6PRNFFkZc2NZ6dJqFfhRoFNMR9Lnyj7dYGrzdgXXVMXcxoKTePPX1dWByq",
			"5HtasZ6ofTHP6HCwTqTkLDuLQisYPah7aUnSKfC7h4hMUVw2gi5",
			false
			)]
		[TestCase(
			"TestingOneTwoThree",
			"6PYNKZ1EAgYgmQfmNVamxyXVWHzK5s6DGhwP4J5o44cvXdoY7sRzhtpUeo",
			"L44B5gGEpqEDRS9vVPz7QT35jcBG2r3CZwSwQ4fCewXAhAhqGVpP",
			true
			)]
		[TestCase(
			"Satoshi",
			"6PYLtMnXvfG3oJde97zRyLYFZCYizPU5T3LwgdYJz1fRhh16bU7u6PPmY7",
			"KwYgW8gcxj1JWJXhPSu4Fqwzfhp5Yfi42mdYmMa4XqK7NJxXUSK7",
			true
			)]
		[Parallelizable(ParallelScope.Self)]
		public void EncryptedSecretNoECmultiply(string passphrase, string encrypted, string unencrypted, bool compressed)
		{
			var key = Key.Parse(unencrypted);
			var encryptedKey = new EncryptedKey(key, passphrase, Network.BitcoinMain);
			Assert.AreEqual(encrypted, encryptedKey.ToString());

			var actualKey = encryptedKey.GetKey(passphrase);
			Assert.AreEqual(unencrypted, actualKey.ToString(Network.BitcoinMain));

			Assert.AreEqual(compressed, actualKey.IsCompressed);
		}

		[TestCase(
			"Satoshi",
			"6PYLtMnXvfG3oJde97zRyLYFZCYizPU5T3LwgdYJz1fRhh16bU7u6PPmY7",
			"KwYgW8gcxj1JWJXhPSu4Fqwzfhp5Yfi42mdYmMa4XqK7NJxXUSK7",
			true
			)]
		public void EncryptedKeyNoEC(string passphrase, string encrypted, string unencrypted, bool compressed)
		{
			var key = Key.Parse(unencrypted);
			var encryptedKey = new EncryptedKey(key, passphrase, Network.BitcoinMain);
			Assert.AreEqual(encrypted, encryptedKey.ToString());

			Assert.Throws<InvalidOperationException>(() => encryptedKey.GetKey(passphrase + "wrong"));
		}
	}
}
