using BitcoinLite.Encoding;
using BitcoinLite.Utils;

namespace BitcoinLite.Crypto
{
	public interface ITxDestination
	{
		Script ScriptPubKey { get; }
	}

	public abstract class TxDestination : ITxDestination, IBinarySerializable
	{
		protected readonly byte[] Bytes;

		protected TxDestination(byte[] bytes)
		{
			Ensure.NotNull(nameof(bytes), bytes);
			Ensure.That(nameof(bytes), () => bytes.Length == uint160.Size);

			Bytes = bytes;
		}

		public abstract Script ScriptPubKey { get; }

		public byte[] ToByteArray()
		{
			return Bytes.CloneByteArray();
		}

		public override int GetHashCode()
		{
			return Bytes.GetHashCode();
		}

		public override string ToString()
		{
			return Encoders.Hex.GetString(Bytes);
		}
	}
}