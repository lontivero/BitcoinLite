using BitcoinLite.Crypto;
using BitcoinLite.Utils;

namespace BitcoinLite
{
	public class ScriptId : ITxDestination, IBinarySerializable
	{
		private readonly byte[] _hash;

		public ScriptId(byte[] hash)
		{
			Ensure.NotNull(nameof(hash), hash);
			Ensure.That(nameof(hash), ()=>hash.Length == uint160.Size);
			_hash = hash;
		}

		public ScriptId(Script script)
			: this(Hashes.RIPEMD160(Hashes.SHA256(script.ToByteArray())))
		{
		}

		public Script ScriptPubKey => Script.FromScriptId(this);

		public Address GetAddress(Network network)
		{
			return new Address(network, DataTypePrefix.ScriptHash, ToByteArray());
		}

		public byte[] ToByteArray()
		{
			return _hash.CloneByteArray();
		}

		public override bool Equals(object obj)
		{
			return ReferenceEquals(this, obj) || Equals(obj as ScriptId);
		}

		protected bool Equals(ScriptId other)
		{
			return other != null && _hash.IsEqualTo(other._hash);
		}

		public override int GetHashCode()
		{
			return _hash.GetHashCode();
		}

	}
}