using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BitcoinLite.Crypto;
using BitcoinLite.Utils;

namespace BitcoinLite.Structures
{
	public interface IVisitable
	{
	}

	public partial class BlockHeader
	{
		private uint256 _hash;

		public uint256 Hash
		{
			get { return _hash ?? (_hash = new uint256(Hashes.SHA256d(this.ToByteArray()))); }
		}

		public DateTimeOffset BlockTime
		{
			get
			{
				return Timestamp.ToDateTimeOffset();
			}
		}

		public bool CheckProofOfWork()
		{
			return Hash <= Bits.AsTargetHash();
		}
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

	public partial class Transaction
	{
		private uint256 _hash;

		public uint256 Hash
		{
			get { return _hash ?? (_hash = new uint256(Hashes.SHA256d(this.ToByteArray()))); }
		}

		public long TotalOut
		{
			get
			{
				return Outputs.Sum(v => v.Value);
			}
		}

		/*
		public uint256 GetSignatureHash(Script scriptPubKey, int nIn, SigHash sigHash = SigHash.All)
		{
			return Inputs.AsIndexedInputs().ToArray()[nIn].GetSignatureHash(scriptPubKey, sigHash);
		}

		public TransactionSignature SignInput(ISecret secret, Script scriptPubKey, int nIn, SigHash sigHash = SigHash.All)
		{
			return SignInput(secret.PrivateKey, scriptPubKey, nIn, sigHash);
		}

		public TransactionSignature SignInput(PrivateKey key, Script scriptPubKey, int nIn, SigHash sigHash = SigHash.All)
		{
			return Inputs.AsIndexedInputs().ToArray()[nIn].Sign(key, scriptPubKey, sigHash);
		}
		*/

		public bool IsCoinBase
		{
			get
			{
				return (Inputs.Count == 1 && Inputs[0].PreviousOutput.IsNull);
			}
		}
	}

	public partial class TxIn
	{
		public TxIn(Script sigScript)
			: this(sigScript, uint.MaxValue)
		{
		}

		public TxIn(Script sigScript, uint sequence)
		{
			if (sigScript == null) throw new ArgumentNullException("sigScript");
			SigScript = sigScript;
			Sequence = sequence;
			PreviousOutput = OutPoint.None;
		}

		public TxIn(OutPoint prevout)
		{
			if (prevout == null) throw new ArgumentNullException("prevout");
			SigScript = Script.Empty;
			Sequence = uint.MaxValue;
			PreviousOutput = prevout;
		}

		//public bool IsFrom(PublicKey pubKey)
		//{
		//	var result = PayToPubkeyHashTemplate.Instance.ExtractScriptSigParameters(SigScript);
		//	return result != null && result.PublicKey == pubKey;
		//}
	}

	public partial class TxOut
	{
		//public TxOut(long value, IDestination destination)
		//{
		//	Value = value;
		//	ScriptPubKey = destination.ScriptPubKey;
		//}

		//public TxOut(long value, Script scriptPubKey)
		//{
		//	Value = value;
		//	ScriptPubKey = scriptPubKey;
		//}

		public TxOut()
		{
			Value = 0;
		}

		public bool IsNull { get { return Value == -1; } }

		public bool IsDust
		{
			get
			{
				return ((1000 * Value) / (3 * (this.ToByteArray().Length + 148)) < ProtocolConstants.MinRelayTxFee);
			}
		}

		//public bool IsTo(IDestination destination)
		//{
		//	return ScriptPubKey == destination.ScriptPubKey;
		//}
	}

	public partial class OutPoint : IComparable<OutPoint>
	{
		public readonly static OutPoint None = new OutPoint(uint256.Zero, 0);

		public OutPoint(Transaction tx, uint i)
			: this(tx.Hash, i)
		{
		}

		public bool IsNull { get { return Hash == 0 && Index == 0; }}

		public static bool operator <(OutPoint a, OutPoint b)
		{
			return a.CompareTo(b) < 0;
		}
		public static bool operator >(OutPoint a, OutPoint b)
		{
			return a.CompareTo(b) > 0;
		}

		public static bool operator ==(OutPoint a, OutPoint b)
		{
			if (ReferenceEquals(a, null))
			{
				return ReferenceEquals(b, null);
			}
			if (ReferenceEquals(b, null))
			{
				return false;
			}
			return a.CompareTo(b) == 0;
		}

		public static bool operator !=(OutPoint a, OutPoint b)
		{
			return !(a == b);
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as OutPoint);
		}

		public bool Equals(OutPoint outpoint)
		{
			if (ReferenceEquals(outpoint, null))
				return false;
			return outpoint.CompareTo(this) == 0;
		}

		public override int GetHashCode()
		{
			return Tuple.Create(Hash, Index).GetHashCode();
		}

		public int CompareTo(OutPoint other)
		{
			var hashDiff = Hash - other.Hash;
			if(hashDiff == 0)
			{
				return (int)Index - (int)other.Index;
			}
			return hashDiff > 0 ? 1 : -1;
		}

		public override string ToString()
		{
			return Index + "-" + Hash;
		}

	}

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
