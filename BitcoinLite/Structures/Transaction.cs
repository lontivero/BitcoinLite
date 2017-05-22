using System.Collections.Generic;
using System.Linq;
using BitcoinLite.Crypto;

namespace BitcoinLite.Structures
{
	public class Transaction : IVisitable
	{
		// Transaction data format version
		public uint Version { get; internal set; }

		// A list of 1 or more transaction inputs or sources for coins
		public List<TxIn> Inputs { get; internal set; }

		// A list of 1 or more transaction outputs or destinations for coins
		public List<TxOut> Outputs { get; internal set; }

		// 
		public uint LockTime { get; internal set; }


		public Transaction()
			: this(ProtocolConstants.CurrentTxVersion, new List<TxIn>(), new List<TxOut>(), 0)
		{ }

		public Transaction(uint version, List<TxIn> inputs, List<TxOut> outputs, uint lock_time)
		{
			Ensure.NotNull(nameof(inputs), inputs);
			Ensure.NotNull(nameof(outputs), outputs);

			Version = version;
			Inputs = inputs;
			Outputs = outputs;
			LockTime = lock_time;
		}

		public uint256 Hash => new uint256(Hashes.Hash256(this.ToByteArray()));

		public long TotalOut
		{
			get
			{
				return Outputs.Sum(v => v.Value);
			}
		}

		public bool IsCoinBase => (Inputs.Count == 1 && Inputs[0].PreviousOutput.IsNull);

		//		public static Transaction FromByteArray(byte[] bytes)
		//		{
		//			var mem = new MemoryStream(bytes);
		//			var reader = new BlockchainReader(new BitcoinBinaryReader(mem));
		//			return reader.ReadTransaction();
		//		}

		//		public static Transaction Parse(string hex)
		//		{
		//			return FromByteArray(Encoders.Hex.GetBytes(hex));
		//		}
		public Transaction Clone()
		{
			return new Transaction();
		}
	}
}