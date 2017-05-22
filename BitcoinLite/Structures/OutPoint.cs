using System;

namespace BitcoinLite.Structures
{
	public class OutPoint : IVisitable
	{
		public static readonly OutPoint None = new OutPoint(uint256.Zero, 0);

		// The hash of the referenced transaction.
		public uint256 Hash { get; internal set; }

		// The index of the specific output in the transaction. The first output is 0, etc.
		public uint Index { get; internal set; }

		public OutPoint(byte[] hash, uint index)
			: this(new uint256(hash), index)
		{
		}

		public OutPoint(uint256 hash, uint index)
		{
			Hash = hash;
			Index = index;
		}

		public bool IsNull => Hash == 0 && Index == 0;
		
		public override string ToString()
		{
			return Index + "-" + Hash;
		}


		//		public static OutPoint FromByteArray(byte[] bytes)
		//		{
		//			var mem = new MemoryStream(bytes);
		//			var reader = new BlockchainReader(new BitcoinBinaryReader(mem));
		//			return reader.ReadOutPoint();
		//		}

		//		public static OutPoint Parse(string hex)
		//		{
		//			return FromByteArray(Encoders.Hex.GetBytes(hex));
		//		}
	}
}