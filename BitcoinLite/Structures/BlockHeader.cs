using BitcoinLite.Crypto;
using BitcoinLite.Utils;
using System;


namespace BitcoinLite.Structures
{
	public class BlockHeader : IVisitable
	{
		// Block version information, based upon the software version creating this block
		public uint Version { get; internal set; }

		// The hash value of the previous block this particular block references
		public uint256 PrevBlockHash { get; internal set; }

		// The reference to a Merkle tree collection which is a hash of all transactions related to this block
		public uint256 MerkleRootHash { get; internal set; }

		// A Unix timestamp recording when this block was created (Currently limited to dates before the year 2106!)
		public uint Timestamp { get; internal set; }

		// The calculated difficulty target being used for this block
		public Target Bits { get; internal set; }

		// The nonce used to generate this block… to allow variations of the header and compute different hashes
		public uint Nonce { get; internal set; }


		public BlockHeader(uint version, uint256 prev_block_hash, uint256 merkle_root_hash, uint timestamp, Target bits, uint nonce)
		{
			Ensure.NotNull(nameof(prev_block_hash), prev_block_hash);
			Ensure.NotNull(nameof(merkle_root_hash), merkle_root_hash);
			Ensure.NotNull(nameof(bits), bits);

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
		//			return FromByteArray(Encoders.Hex.GetBytes(hex));
		//		}

		public uint256 Hash => new uint256(Hashes.Hash256(this.ToByteArray()));

		public DateTimeOffset BlockTime => Timestamp.ToDateTimeOffset();

		public bool CheckProofOfWork()
		{
			return Hash <= Bits.AsTargetHash();
		}
	}
}

