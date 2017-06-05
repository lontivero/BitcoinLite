using BitcoinLite.Crypto;
using BitcoinLite.Encoding;

namespace BitcoinLite
{
	public class KeyId : TxDestination
	{
		public static KeyId Parse(string hex)
		{
			return new KeyId(Encoders.Hex.GetBytes(hex));
		}

		public KeyId(byte[] hash)
			: base(hash)
		{
			Ensure.That(nameof(hash), () => hash.Length == uint160.Size);
		}

		public KeyId(PubKey pubKey)
			: this(Hashes.Hash160(pubKey.ToByteArray()))
		{
		}

		public override Script ScriptPubKey => Script.FromPubKeyHash(this);

		public override Address GetAddress(Network network)
		{
			return new PubKeyHashAddress(network, Bytes);
		}
	}

	public class SegWitKeyId : TxDestination
	{
		public static SegWitKeyId Parse(string hex)
		{
			return new SegWitKeyId(Encoders.Hex.GetBytes(hex));
		}

		public SegWitKeyId(byte[] hash)
			: base(hash)
		{
			Ensure.That(nameof(hash), () => hash.Length == uint160.Size);
		}

		public override Script ScriptPubKey => Script.FromSegWitHash(this);

		public override Address GetAddress(Network network)
		{
			return new SegWitPubKeyHashAddress(network, Bytes);
		}
	}
}