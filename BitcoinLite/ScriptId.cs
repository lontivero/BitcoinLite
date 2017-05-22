using BitcoinLite.Crypto;
using BitcoinLite.Utils;

namespace BitcoinLite
{
	public class ScriptId : TxDestination
	{
		public ScriptId(byte[] hash)
			: base(hash)
		{
			Ensure.That(nameof(hash), () => hash.Length == uint160.Size);
		}

		public ScriptId(Script script)
			: this(Hashes.Hash160(script.ToByteArray()))
		{
		}

		public override Script ScriptPubKey => Script.FromScriptId(this);

		public Address GetAddress(Network network)
		{
			return new ScriptHashAddress(network, Bytes);
		}
	}

	public class SegWitScriptId : TxDestination
	{
		public SegWitScriptId(byte[] hash)
			: base(hash)
		{
			Ensure.That(nameof(hash), () => hash.Length == uint256.Size);
		}

		public SegWitScriptId(Script script)
			: this(Hashes.Hash160(script.ToByteArray()))
		{
		}

		public override Script ScriptPubKey => Script.FromSegWitScriptHash(this);

		public Address GetAddress(Network network)
		{
			return new SegWitScriptHashAddress(network, Bytes);
		}
	}
}