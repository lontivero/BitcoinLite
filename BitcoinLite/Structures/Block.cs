using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BitcoinLite.Structures
{
	public class Block : IVisitable
	{
		// The block header
		public BlockHeader Header { get; internal set; }

		// Block transactions, in format of "tx" command
		public List<Transaction> Transactions { get; internal set; }


		public Block(BlockHeader header, List<Transaction> transactions)
		{
			if (header == null) throw new ArgumentNullException(nameof(header));
			if (transactions == null) throw new ArgumentNullException(nameof(transactions));

			Header = header;
			Transactions = transactions;
		}

		//		public static Block FromByteArray(byte[] bytes)
		//		{
		//			var mem = new MemoryStream(bytes);
		//			var reader = new BlockchainReader(new BitcoinBinaryReader(mem));
		//			return reader.ReadBlock();
		//		}

		//		public static Block Parse(string hex)
		//		{
		//			return FromByteArray(Encoders.Hex.GetBytes(hex));
		//		}
	}

	public interface IVisitable
	{
	}

	//public partial class Block
	//{
	//	private MerkleNode _merkleRoot;

	//	public MerkleNode MerkleRoot
	//	{
	//		get { return _merkleRoot ?? (_merkleRoot = MerkleNode.GetRoot(Transactions.Select(t => t.Hash))); }
	//	}

	//	public bool IsValid
	//	{
	//		get { return CheckMerkleRoot() && Header.CheckProofOfWork(); }
	//	}

	//	private bool CheckMerkleRoot()
	//	{
	//		return Header.MerkleRootHash == MerkleRoot.Hash;
	//	}

	//	public uint256 Hash
	//	{
	//		get { return Header.Hash; }
	//	}

	//	public Block CreateNextBlockWithCoinbase(Address address, int height)
	//	{
	//		return CreateNextBlockWithCoinbase(address, height, DateTimeOffset.UtcNow);
	//	}

	//	public Block CreateNextBlockWithCoinbase(Address address, int height, DateTimeOffset now)
	//	{
	//		if (address == null) throw new ArgumentNullException("address");
	//		if (height < 0) throw new ArgumentOutOfRangeException("height");

	//		var tx = new Transaction(
	//			1, // version
	//			new[] { new TxIn(new Script(Op.Push(RandomUtils.GetBytes(30)))) },
	//			new[] { new TxOut(address.Network.CalculateReward(height), address) },
	//			0 // lock time
	//		);

	//		var timestamp = now.ToEpochTime();
	//		var merkleRoot = MerkleNode.GetRoot(new[] { tx.Hash });
	//		var header = new BlockHeader(1, Hash, merkleRoot.Hash, timestamp, 0, RandomUtils.GetUInt32());

	//		return new Block(header, new[] { tx });
	//	}
	//}



	public static class BinarySerializableExtensions
	{
		public static byte[] ToByteArray(this IVisitable me)
		{
			var stream = new MemoryStream();
			//var writer = new BlockchainWriter(new BitcoinBinaryWriter(stream));
			//writer.Write(me);
			return stream.ToArray();
		}
	}

	public static class TransactionExtensions
	{
		//public static IEnumerable<IndexedTxIn> AsIndexedInputs(this IEnumerable<TxIn> me)
		//{
		//	// We want i as the index of txIn in Intputs[], not index in enumerable after where filter
		//	return me.Select((r, i) => new IndexedTxIn{
		//		TxIn = r,
		//		Index = (uint)i,
		//		Transaction = null //Transaction
		//	});
		//}

		//public static IEnumerable<IndexedTxOut> AsIndexedOutputs(this IEnumerable<TxOut> me)
		//{
		//	// We want i as the index of txIn in Intputs[], not index in enumerable after where filter
		//	return me.Select((r, i) => new IndexedTxOut
		//	{
		//		TxOut = r,
		//		Index = (uint)i,
		//		Transaction = null //Transaction
		//	});
		//}

		//public static IEnumerable<Coin> AsCoins(this IEnumerable<TxOut> me)
		//{
		//	return me.AsIndexedOutputs().Select(i => i.ToCoin());
		//}

		//public static IEnumerable<IndexedTxOut> AsSpendableIndexedOutputs(this IEnumerable<TxOut> me)
		//{
		//	return me.AsIndexedOutputs()
		//			.Where(r => !r.TxOut.ScriptPubKey.IsUnspendable);
		//}

		//public static IEnumerable<TxOut> To(this IEnumerable<TxOut> me, IDestination destination)
		//{
		//	return me.Where(r => r.IsTo(destination));
		//}

		public static IEnumerable<TxOut> To(this IEnumerable<TxOut> me, Script scriptPubKey)
		{
			return me.Where(r => r.ScriptPubKey == scriptPubKey);
		}
	}

	public static class ReadOnlyListExtensions
	{
		public static int FindIndex<T>(this IReadOnlyList<T> me, Func<T, bool> predicate)
		{
			var i = 0;
			foreach (var item in me)
			{
				if (predicate(item))
				{
					return i;
				}
				i++;
			}
			return -1;
		}
	}

	public static class ProtocolConstants
	{
		public static long MinTxFee = 10000;  // Override with -mintxfee
		public static long MinRelayTxFee = 1000;

		public static uint CurrentTxVersion = 2;
		public static uint MaxStandardTxSize = 100000;
	}
}
