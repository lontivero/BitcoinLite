using BitcoinLite.Encoding;
using BitcoinLite.Utils;

namespace BitcoinLite
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
			Ensure.That("bytes", () => bytes.Length > 0);
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
		public override bool Equals(object obj)
		{
			return ReferenceEquals(this, obj) || Equals(obj as TxDestination);
		}

		private bool Equals(TxDestination other)
		{
			return other != null && Bytes.IsEqualTo(other.Bytes);
		}

		public abstract Address GetAddress(Network network);

		public Address GetAddress()
		{
			return GetAddress(Network.Current);
		}
	}
}