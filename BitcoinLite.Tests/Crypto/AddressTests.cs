using BitcoinLite.Crypto;
using NUnit.Framework;

namespace BitcoinLite.Tests.Crypto
{
	[TestFixture(Category = "Address,Bitcoin,Crypto")]
	public class AddressTests
	{
		[Test]
		public void CanCreateAddress()
		{
			var hash = Hashes.SHA256(new byte[] { 0x01 });
			var privateKey = new Key(hash);
			var publicKey = privateKey.PublicKey;
			var address = publicKey.ToAddress(Network.Main);
			var str = address.ToString();
			Assert.AreEqual("1ApqRi7vWrh3gR4jJ6LXhAXRKWTu55mrqP", str);
		}

		[Test]
		public void CanCreateAddress2()
		{
			var address = Address.FromString("1ApqRi7vWrh3gR4jJ6LXhAXRKWTu55mrqP");
			Assert.AreEqual(Network.Main, address.Network);
			Assert.AreEqual("1ApqRi7vWrh3gR4jJ6LXhAXRKWTu55mrqP", address.ToString());
		}

		[Test]
		public void CanCreateAddress3()
		{
			var priv = Key.Parse("KzmL119KUWwg7XkeznvFwHo1qnyLZ4Ap9S326HsomBq9H3PmicRN");
			var addr = priv.PublicKey.ToAddress(Network.Main);
			Assert.AreEqual("1NHAEjTe38z73XKJBndNd4Y2s3m1WTFe2V", addr.ToString());
		}
	}
}