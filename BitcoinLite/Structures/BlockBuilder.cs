using System;
using System.Collections.Generic;
using System.Linq;
using BitcoinLite;
using BitcoinLite.Structures;

namespace NBitcoin.Structures
{
	class BlockBuilder
	{
		private readonly List<Transaction> _transactions = new List<Transaction>();

		public BlockBuilder()
		{
			Version = 1;
			Nonce = 0;
			Timestamp = DateTimeOffset.UtcNow;
			Target = 0;
			PreviousBlockHash = uint256.Zero;
		}

		protected uint256 PreviousBlockHash { get; set; }
		public uint Version { get; set; }
		public uint Nonce { get; set; }
		public uint Target { get; set; }
		public DateTimeOffset Timestamp { get; set; }

		public void AddTransaction(Transaction tx)
		{
			_transactions.Add(tx);
		}

		public Block Build()
		{
			throw new NotImplementedException();
			// var txHashes = from tx in _transactions select tx.Hash;
			// var merkleRoot = MerkleNode.GetRoot(txHashes);
			// var header = new BlockHeader(Version, PreviousBlockHash, merkleRoot.Hash, Utils.DateTimeToUnixTime(Timestamp), Target, Nonce);
			// return new Block(header, _transactions);
		}
	}
}
