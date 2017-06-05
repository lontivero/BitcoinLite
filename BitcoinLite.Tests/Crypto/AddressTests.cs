using System;
using System.Linq;
using System.Numerics;
using BitcoinLite.Crypto;
using BitcoinLite.Encoding;
using NUnit.Framework;

namespace BitcoinLite.Tests.Crypto
{
	[TestFixture(Category = "Address,Bitcoin,Crypto")]
	public class AddressTests
	{
		[Test]
		public void CannotCreateInvalidAddress()
		{
			Assert.Throws<FormatException>(
				()=>Base58Data.FromString("8ApqRi7vWrh3gR4jJ6LXhAXRKWTu55mrqP"));
		}

		[Test]
		public void CannotCreateAddressxxxx()
		{
			Network network;
			DataTypePrefix type;
			var data = Base58Data.FromString("1ApqRi7vWrh3gR4jJ6LXhAXRKWTu55mrqP", out network, out type);
			var bytes = new byte[] { 0xff, 0xff}.Concat(data).ToArray();
			Assert.Throws<FormatException>(() => Address.FromString(Encoders.Base58Check.GetString(bytes)));
		}

		[Test]
		public void CannotCreateAddressEmpty()
		{
			Network network;
			DataTypePrefix type;
			Assert.Throws<FormatException>(()=>Base58Data.FromString("", out network, out type));
		}

		[Test]
		public void CannotCreateAddressFromUnsupportedDataType()
		{
			Network network;
			DataTypePrefix type;
			var data = Base58Data.FromString("1ApqRi7vWrh3gR4jJ6LXhAXRKWTu55mrqP", out network, out type);
			var bytes = network.GetPrefixBytes(DataTypePrefix.ExtPrivateKey).Concat(data).ToArray();
			Assert.Throws<NotSupportedException>(() => Address.FromString(Encoders.Base58Check.GetString(bytes)));
		}
	}

	[TestFixture(Category = "Address,Bitcoin,Crypto")]
	public class PubKeyHashAddressTests
	{
		[Test(Description = "Create PubKey Address: key.PubKey.ToAddress")]
		public void CanCreateAddress()
		{
			var hash = Hashes.SHA256(new byte[] { 0x01 });
			var privateKey = new Key(hash);
			var publicKey = privateKey.PubKey;
			var address = publicKey.ToAddress(Network.BitcoinMain);
			var str = address.ToString();
			Assert.AreEqual("1ApqRi7vWrh3gR4jJ6LXhAXRKWTu55mrqP", str);
		}

		[Test(Description = "Create PubKey Address: Address.FromString(wif)")]
		public void CanCreateAddress2()
		{
			var address = Address.FromString("1ApqRi7vWrh3gR4jJ6LXhAXRKWTu55mrqP");
			Assert.AreEqual(Network.BitcoinMain, address.Network);
			Assert.AreEqual("1ApqRi7vWrh3gR4jJ6LXhAXRKWTu55mrqP", address.ToString());
		}

		[Test(Description = "Create PubKey Address: key.PubKey.ToAddress")]
		public void CanCreateAddress3()
		{
			var priv = Key.Parse("KzmL119KUWwg7XkeznvFwHo1qnyLZ4Ap9S326HsomBq9H3PmicRN");
			var addr = priv.PubKey.ToAddress(Network.BitcoinMain);
			Assert.AreEqual("1NHAEjTe38z73XKJBndNd4Y2s3m1WTFe2V", addr.ToString());
		}
	}

	[TestFixture(Category = "Address,Bitcoin,Crypto")]
	public class ScriptHashAddressTests
	{
		[Test(Description = "Create Script Address: key.PubKey.ScriptPubKey.Hash.GetAddress")]
		public void CanCreateAddress()
		{
			var hash = Hashes.SHA256(new byte[] { 0x01 });
			var privateKey = new Key(hash);
			var address = privateKey.PubKey.ScriptPubKey.Hash.GetAddress(Network.BitcoinMain);
			Assert.AreEqual("31xfAbnULrmQH5wyjtgjbrTP47PJ48MY3T", address.ToString());
		}

		[Test(Description = "Create Script Address: Address.FromString(wif)")]
		public void CanCreateAddress2()
		{
			var address = Address.FromString("1ApqRi7vWrh3gR4jJ6LXhAXRKWTu55mrqP");
			Assert.AreEqual(Network.BitcoinMain, address.Network);
			Assert.AreEqual("1ApqRi7vWrh3gR4jJ6LXhAXRKWTu55mrqP", address.ToString());
		}

		[Test(Description = "Create Script Address: key.PubKey.ToAddress")]
		public void CanCreateAddress3()
		{
			var priv = Key.Parse("KzmL119KUWwg7XkeznvFwHo1qnyLZ4Ap9S326HsomBq9H3PmicRN");
			var addr = priv.PubKey.ToAddress(Network.BitcoinMain);
			Assert.AreEqual("1NHAEjTe38z73XKJBndNd4Y2s3m1WTFe2V", addr.ToString());
		}
	}

}