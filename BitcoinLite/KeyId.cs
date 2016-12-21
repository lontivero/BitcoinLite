using BitcoinLite.Crypto;
using BitcoinLite.Encoding;
using BitcoinLite.Utils;

namespace BitcoinLite
{
	public class KeyId : ITxDestination, IBinarySerializable
	{
		private readonly byte[] _hash;

		public static KeyId Parse(string hex)
		{
			return new KeyId(Encoders.Hex.GetBytes(hex));
		}

		public KeyId(byte[] hash)
		{
			Ensure.NotNull(nameof(hash), hash);
			Ensure.That(nameof(hash), () => hash.Length == uint160.Size);
			_hash = hash;
		}

		public KeyId(PubKey pubKey)
			: this(Hashes.RIPEMD160(Hashes.SHA256(pubKey.ToByteArray())))
		{
		}

		public Script ScriptPubKey => Script.FromPubKeyHash(this);

		public Address GetAddress(Network network)
		{
			return new Address(network, DataTypePrefix.PublicKeyHash,  ToByteArray());
		}

		public byte[] ToByteArray()
		{
			return _hash.CloneByteArray();
		}

		public override bool Equals(object obj)
		{
			return ReferenceEquals(this, obj) || Equals(obj as KeyId);
		}

		protected bool Equals(KeyId other)
		{
			return other != null && _hash.IsEqualTo(other._hash);
		}

		public override int GetHashCode()
		{
			return _hash.GetHashCode();
		}

		public override string ToString()
		{
			return Encoders.Hex.GetString(_hash);
		}
	}
}