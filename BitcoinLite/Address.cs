using System;
using BitcoinLite.Crypto;
using BitcoinLite.Encoding;
using BitcoinLite.Utils;

namespace BitcoinLite
{
	public enum DataTypePrefix
	{
		PublicKeyHash,
		ScriptHash,
		PrivateKey,
		ExtPublicKey,
		ExtPrivateKey
	}

	public class Address
	{
		public Network Network { get; private set; }
		private readonly byte[] _pubKeyHash;

		public static Address FromString(string wif)
		{
			Network network;
			var bytes = Base58Data.FromString(wif, out network);
			return new Address(network, bytes);
		}

		public Address(Network network, byte[] pubKeyHash)
		{
			if(network == null )
				throw new ArgumentNullException("network");
			if(pubKeyHash == null)
				throw new ArgumentNullException("pubKeyHash");
			//if (pubKeyHash.Length != 32)
			//	throw new ArgumentException("pubKeyHash must be a 32 byte length array");

			Network = network;
			_pubKeyHash = pubKeyHash;
		}

		public Script ScriptPubKey
		{
			get { return Script.FromAddress(this); }
		}

		public byte[] PubKeyHash
		{
			get { return _pubKeyHash; }
		}

		public byte[] ToByteArray()
		{
			return Network.GetPrefixBytes(DataTypePrefix.PublicKeyHash).Concat(PubKeyHash);
		}

		public override string ToString()
		{
			return Encoders.Base58Check.Encode(ToByteArray());
		}
	}
}