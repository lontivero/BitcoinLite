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
		ConfirmationCode,
		SegWitPublicKeyHash,
		SegWitScriptHash,
	}

	public abstract class Address : IBinarySerializable
	{
		protected readonly byte[] Bytes;
		public Network Network { get; }

		public static Address FromString(string wif)
		{
			Network network;
			DataTypePrefix type;
			var bytes = Base58Data.FromString(wif, out network, out type);
			if(bytes.Length != 20)
				throw new FormatException("An address has to have 20 bytes");

			switch (type)
			{
				case DataTypePrefix.PublicKeyHash:
					return new PubKeyHashAddress(network, bytes);
				case DataTypePrefix.ScriptHash:
					return new ScriptHashAddress(network, bytes);
				case DataTypePrefix.SegWitPublicKeyHash:
					return new SegWitPubKeyHashAddress(network, bytes);
				case DataTypePrefix.SegWitScriptHash:
					return new SegWitScriptHashAddress(network, bytes);
			}
			throw new NotSupportedException("not supported address type");
		}

		protected Address(Network network, byte[] hash)
		{
			Ensure.NotNull(nameof(network), network);
			Ensure.NotNull(nameof(hash), hash);
			//Ensure.That(nameof(hash), ()=>hash.Length == 20, "An address has to have 20 bytes");
			Network = network;
			Bytes = hash;
		}

		public byte[] ToByteArray()
		{
			return Network.GetPrefixBytes(Type).Concat(Bytes);
		}

		public Script ScriptPubKey => Script.FromAddress(this);

		public abstract DataTypePrefix Type { get; }

		public abstract TxDestination Destination { get; }

		public override string ToString()
		{
			return Encoders.Base58Check.GetString(ToByteArray());
		}
	}

	public class PubKeyHashAddress : Address
	{
		public PubKeyHashAddress(Network network, byte[] hash)
			: base(network, hash)
		{
		}

		public KeyId PubKeyHash => new KeyId(Bytes);
		public override DataTypePrefix Type => DataTypePrefix.PublicKeyHash;
		public override TxDestination Destination => PubKeyHash;
	}

	public class ScriptHashAddress : Address
	{
		public ScriptHashAddress(Network network, byte[] hash)
			: base(network, hash)
		{
		}

		public ScriptId ScriptHash => new ScriptId(Bytes);
		public override DataTypePrefix Type => DataTypePrefix.ScriptHash;
		public override TxDestination Destination => ScriptHash;
	}

	public class SegWitPubKeyHashAddress : Address
	{
		public SegWitPubKeyHashAddress(Network network, byte[] hash)
			: base(network, new []{ (byte)Opcode.OP_0, (byte)0x00 }.Concat(hash))
		{
		}

		public SegWitKeyId SegWitPubKeyHash => new SegWitKeyId(Bytes.Slice(2, 20));
		public override DataTypePrefix Type => DataTypePrefix.SegWitPublicKeyHash;
		public override TxDestination Destination => SegWitPubKeyHash;
	}

	public class SegWitScriptHashAddress : Address
	{
		public SegWitScriptHashAddress(Network network, byte[] hash)
			: base(network, new[] { (byte)Opcode.OP_0, (byte)0x00 }.Concat(hash))
		{
		}

		public SegWitScriptId SegWitScriptHash => new SegWitScriptId(Bytes.Slice(2, 32));
		public override DataTypePrefix Type => DataTypePrefix.SegWitScriptHash;
		public override TxDestination Destination => SegWitScriptHash;
	}
}