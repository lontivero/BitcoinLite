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
		ExtPrivateKey,
		EncryptedKeyEC,
		EncryptedKeyNoEC,
		PassphraseCode,
		ConfirmationCode
	}

	public class Address : IBinarySerializable
	{
		protected readonly byte[] Bytes;
		public Network Network { get; }

		public static Address FromString(string wif)
		{
			Network network;
			DataTypePrefix type;
			var bytes = Base58Data.FromString(wif, out network, out type);

			return new Address(network, type, bytes);
		}

		public Address(Network network, DataTypePrefix type, byte[] hash)
		{
			Ensure.NotNull(nameof(network), network);
			Ensure.NotNull(nameof(hash), hash);
			if (type != DataTypePrefix.PublicKeyHash && type != DataTypePrefix.ScriptHash)
				throw new NotSupportedException("not supported address type");
			//if (pubKeyHash.Length != 32)
			//	throw new ArgumentException("pubKeyHash must be a 32 byte length array");

			Network = network;
			Bytes = hash;
			Type = type;
		}

		public byte[] ToByteArray()
		{
			return Network.GetPrefixBytes(Type).Concat(Bytes);
		}

		public Script ScriptPubKey => Destination.ScriptPubKey;

		public DataTypePrefix Type
		{
			get;
		}

		public ITxDestination Destination
		{
			get
			{
				if (Type == DataTypePrefix.PublicKeyHash)
					return new KeyId(Bytes);
				if (Type == DataTypePrefix.ScriptHash)
					return new ScriptId(Bytes);

				return (ITxDestination)null;
			}
		}

		public override string ToString()
		{
			return Encoders.Base58Check.GetString(ToByteArray());
		}
	}

	//public class PubKeyHashAddress : Address
	//{
	//	public PubKeyHashAddress(Network network, byte[] hash) 
	//		: base(network, hash)
	//	{
	//	}

	//	public PubKeyId PubKeyHash => new PubKeyId(Bytes);
	//	public override Script ScriptPubKey() => Script.FromAddress(this);
	//	public override DataTypePrefix Type => DataTypePrefix.PublicKeyHash;
	//}

	//public class ScriptHashAddress : Address
	//{
	//	public ScriptHashAddress(Network network, byte[] hash)
	//		: base(network, hash)
	//	{
	//	}

	//	public ScriptId ScriptHash => new ScriptId(Bytes);
	//	public override Script ScriptPubKey() => Script.FromAddress(this);
	//	public override DataTypePrefix Type => DataTypePrefix.ScriptHash;
	//}
}