using System;
using System.Collections.Generic;
using System.IO;
using BitcoinLite.Encoding;


namespace BitcoinLite.Structures
{
	public partial class BlockHeader : IVisitable
	{
		// Block version information, based upon the software version creating this block
		public uint Version	{ get; internal set; }

		// The hash value of the previous block this particular block references
		public uint256 PrevBlockHash	{ get; internal set; }

		// The reference to a Merkle tree collection which is a hash of all transactions related to this block
		public uint256 MerkleRootHash	{ get; internal set; }

		// A Unix timestamp recording when this block was created (Currently limited to dates before the year 2106!)
		public uint Timestamp	{ get; internal set; }

		// The calculated difficulty target being used for this block
		public Target Bits	{ get; internal set; }

		// The nonce used to generate this block… to allow variations of the header and compute different hashes
		public uint Nonce	{ get; internal set; }

				 
		public BlockHeader(uint version, uint256 prev_block_hash, uint256 merkle_root_hash, uint timestamp, Target bits, uint nonce)
		{
			if(prev_block_hash == null) throw new ArgumentNullException("prev_block_hash"); 
			if(merkle_root_hash == null) throw new ArgumentNullException("merkle_root_hash"); 
			if(bits == null) throw new ArgumentNullException("bits"); 

			Version = version; 
			PrevBlockHash = prev_block_hash; 
			MerkleRootHash = merkle_root_hash; 
			Timestamp = timestamp; 
			Bits = bits; 
			Nonce = nonce; 
		}

//		public static BlockHeader FromByteArray(byte[] bytes)
//		{
//			var mem = new MemoryStream(bytes);
//			var reader = new BlockchainReader(new BitcoinBinaryReader(mem));
//			return reader.ReadBlockHeader();
//		}

//		public static BlockHeader Parse(string hex)
//		{
//			return FromByteArray(Encoders.Hex.Decode(hex));
//		}
	}

	public partial class Block : IVisitable
	{
		// The block header
		public BlockHeader Header	{ get; internal set; }

		// Block transactions, in format of "tx" command
		public IReadOnlyList<Transaction> Transactions	{ get; internal set; }

				 
		public Block(BlockHeader header, IReadOnlyList<Transaction> transactions)
		{
			if(header == null) throw new ArgumentNullException("header"); 
			if(transactions == null) throw new ArgumentNullException("transactions"); 

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
//			return FromByteArray(Encoders.Hex.Decode(hex));
//		}
	}

	public partial class Transaction : IVisitable
	{
		// Transaction data format version
		public uint Version	{ get; internal set; }

		// A list of 1 or more transaction inputs or sources for coins
		public IReadOnlyList<TxIn> Inputs	{ get; internal set; }

		// A list of 1 or more transaction outputs or destinations for coins
		public IReadOnlyList<TxOut> Outputs	{ get; internal set; }

		// 
		public uint LockTime	{ get; internal set; }

				 
		public Transaction(uint version, IReadOnlyList<TxIn> inputs, IReadOnlyList<TxOut> outputs, uint lock_time)
		{
			if(inputs == null) throw new ArgumentNullException("inputs"); 
			if(outputs == null) throw new ArgumentNullException("outputs"); 

			Version = version; 
			Inputs = inputs; 
			Outputs = outputs; 
			LockTime = lock_time; 
		}

//		public static Transaction FromByteArray(byte[] bytes)
//		{
//			var mem = new MemoryStream(bytes);
//			var reader = new BlockchainReader(new BitcoinBinaryReader(mem));
//			return reader.ReadTransaction();
//		}

//		public static Transaction Parse(string hex)
//		{
//			return FromByteArray(Encoders.Hex.Decode(hex));
//		}
	}

	public partial class TxIn : IVisitable
	{
		// The previous output transaction reference, as an OutPoint structure
		public OutPoint PreviousOutput	{ get; internal set; }

		// Computational Script for confirming transaction authorization
		public Script SigScript	{ get; internal set; }

		// Transaction version as defined by the sender. Intended for "replacement" of transactions when information is updated before inclusion into a block.
		public uint Sequence	{ get; internal set; }

				 
		public TxIn(OutPoint previous_output, Script sig_script, uint sequence)
		{
			if(previous_output == null) throw new ArgumentNullException("previous_output"); 
			if(sig_script == null) throw new ArgumentNullException("sig_script"); 

			PreviousOutput = previous_output; 
			SigScript = sig_script; 
			Sequence = sequence; 
		}

//		public static TxIn FromByteArray(byte[] bytes)
//		{
//			var mem = new MemoryStream(bytes);
//			var reader = new BlockchainReader(new BitcoinBinaryReader(mem));
//			return reader.ReadTxIn();
//		}

//		public static TxIn Parse(string hex)
//		{
//			return FromByteArray(Encoders.Hex.Decode(hex));
//		}
	}

	public partial class OutPoint : IVisitable
	{
		// The hash of the referenced transaction.
		public uint256 Hash	{ get; internal set; }

		// The index of the specific output in the transaction. The first output is 0, etc.
		public uint Index	{ get; internal set; }

				 
		public OutPoint(uint256 hash, uint index)
		{
			if(hash == null) throw new ArgumentNullException("hash"); 

			Hash = hash; 
			Index = index; 
		}

//		public static OutPoint FromByteArray(byte[] bytes)
//		{
//			var mem = new MemoryStream(bytes);
//			var reader = new BlockchainReader(new BitcoinBinaryReader(mem));
//			return reader.ReadOutPoint();
//		}

//		public static OutPoint Parse(string hex)
//		{
//			return FromByteArray(Encoders.Hex.Decode(hex));
//		}
	}

	public partial class TxOut : IVisitable
	{
		// Transaction Value
		public long Value	{ get; internal set; }

		// Usually contains the public key as a Bitcoin script setting up conditions to claim this output.
		public Script ScriptPubKey	{ get; internal set; }

				 
		public TxOut(long value, Script script_pub_key)
		{
			if(script_pub_key == null) throw new ArgumentNullException("script_pub_key"); 

			Value = value; 
			ScriptPubKey = script_pub_key; 
		}

//		public static TxOut FromByteArray(byte[] bytes)
//		{
//			var mem = new MemoryStream(bytes);
//			var reader = new BlockchainReader(new BitcoinBinaryReader(mem));
//			return reader.ReadTxOut();
//		}

//		public static TxOut Parse(string hex)
//		{
//			return FromByteArray(Encoders.Hex.Decode(hex));
//		}
	}

}

