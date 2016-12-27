using BitcoinLite.Crypto;
using BitcoinLite.Encoding;
using BitcoinLite.Utils;

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
		}

		public KeyId(PubKey pubKey)
			: this(Hashes.RIPEMD160(Hashes.SHA256(pubKey.ToByteArray())))
		{
		}

		public override Script ScriptPubKey => Script.FromPubKeyHash(this);

		public Address GetAddress(Network network)
		{
			return new PubKeyHashAddress(network, Bytes);
		}

		protected bool Equals(KeyId other)
		{
			return other != null && Bytes.IsEqualTo(other.Bytes);
		}

		public override bool Equals(object obj)
		{
			return ReferenceEquals(this, obj) || Equals(obj as KeyId);
		}
	}
}