using System;
using System.Collections.Generic;
using System.Linq;
using BitcoinLite.Crypto;
using BitcoinLite.Utils;

namespace BitcoinLite.Bip32
{
	public class HdPubKey : IBinarySerializable
	{
		private const int FingerprintLength = 4;
		private const int ChainCodeLength = 32;
		private const int KeyLength = 32;
		private const int PubKeyLength = 33;
		private const uint Hardened = 0x80000000;

		public static HdPubKey Parse(string wif)
		{
			DataTypePrefix prefix;
			Network network;
			var bytes = Base58Data.FromString(wif, out network, out prefix);
			if (prefix != DataTypePrefix.ExtPublicKey)
				throw new ArgumentException("not a valid extended public key", nameof(wif));

			var depth = bytes[0];
			var fingerprint = bytes.Slice(1, FingerprintLength);
			var child = Packer.BigEndian.ToUInt32(bytes, 5);
			var chain = bytes.Slice(9, ChainCodeLength);
			var key = bytes.Slice(41, PubKeyLength);

			return new HdPubKey(new PubKey(key), chain, depth, child, fingerprint);
		}

		public PubKey PubKey { get; }
		public byte Depth { get; }
		public uint Child { get; }
		public byte[] ChainCode { get; }
		public byte[] Fingerprint { get; }
		public bool IsHardened => (Child & Hardened) == Hardened;
		public bool IsRoot => Depth == 0;

		internal HdPubKey(PubKey pubkey, byte[] chain, byte depth, uint child, byte[] fingerprint)
		{
			PubKey = pubkey;
			ChainCode = chain;
			Depth = depth;
			Child = child;
			Fingerprint = fingerprint;
		}

		public HdPubKey Derive(IEnumerable<uint> indexes)
		{
			return indexes.Aggregate(this, (current, index) => current.Derive(index));
		}

		public HdPubKey Derive(uint index)
		{
			if ((index & Hardened) == Hardened)
				throw new InvalidOperationException("Cannot create a hardened child key from public child derivation");

			var pubKey = PubKey.ToByteArray();
			var salt = Packer.Pack("bIA", pubKey[0], index, pubKey.Slice(1));
			var i = Hashes.HMACSHA256(ChainCode, salt);

			var il = i.Slice(0, KeyLength);
			var ir = i.Slice(KeyLength, ChainCodeLength);
			var childCodeChain = ir;
			Key.CheckValidKey(il);

			var parse256il = il.ToBigIntegerUnsigned(false);

			var q = (Secp256k1.G * parse256il) + PubKey.Point;
			if (q.IsInfinity)
				throw new InvalidOperationException("Point is infinity, very rare. This event has a probability lower than 1 in 2^127");

			var childFingerprint = PubKey.Hash.ToByteArray().Slice(0, FingerprintLength);
			return new HdPubKey(new PubKey(q.Encode(true)), childCodeChain, (byte)(Depth + 1), index, childFingerprint);
		}

		public byte[] ToByteArray()
		{
			return Packer.Pack("bA^I_AA", Depth, Fingerprint, Child, ChainCode, PubKey.ToByteArray());
		}
		public string ToString(Network network)
		{
			return Base58Data.ToString(ToByteArray(), DataTypePrefix.ExtPublicKey, network);
		}
	}
}