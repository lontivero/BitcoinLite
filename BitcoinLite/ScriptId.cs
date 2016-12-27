using BitcoinLite.Crypto;
using BitcoinLite.Utils;

namespace BitcoinLite
{
	public class ScriptId : TxDestination
	{
		public ScriptId(byte[] hash)
			: base(hash)
		{
		}

		public ScriptId(Script script)
			: this(Hashes.RIPEMD160(Hashes.SHA256(script.ToByteArray())))
		{
		}

		public override Script ScriptPubKey => Script.FromScriptId(this);

		public Address GetAddress(Network network)
		{
			return new ScriptHashAddress(network, Bytes);
		}

		public override bool Equals(object obj)
		{
			return ReferenceEquals(this, obj) || Equals(obj as ScriptId);
		}

		protected bool Equals(ScriptId other)
		{
			return other != null && Bytes.IsEqualTo(other.Bytes);
		}
	}
}