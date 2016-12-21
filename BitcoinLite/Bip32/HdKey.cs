using System;
using System.Collections.Generic;
using System.Linq;
using BitcoinLite.Crypto;
using BitcoinLite.Encoding;
using BitcoinLite.Utils;

namespace BitcoinLite.Bip32
{
	public class HdKey : IBinarySerializable
	{
		private const int FingerprintLength = 4;
		private const int ChainCodeLength = 32;
		private const int KeyLength = 32;
		private const uint Hardened = 0x80000000;

		public static HdKey Parse(string wif)
		{
			DataTypePrefix prefix;
			Network network;
			var bytes = Base58Data.FromString(wif, out network, out prefix);
			if (prefix != DataTypePrefix.ExtPrivateKey)
				throw new NotSupportedException("not a valid extended private key");

			var depth = bytes[0];
			var fingerprint = bytes.Slice(1, FingerprintLength);
			var child = Packer.BigEndian.ToUInt32(bytes, 5);
			var chain = bytes.Slice(9, ChainCodeLength);
			var key = bytes.Slice(42, KeyLength);

			return new HdKey(new Key(key), chain, depth, child, fingerprint);
		}

		public static HdKey FromEntrophy(byte[] entrophy)
		{
			return new HdKey(entrophy);
		}

		public Key Key { get; }
		public PubKey PubKey => Key.PubKey;
		public byte Depth { get; }
		public uint Child { get; }
		public byte[] ChainCode { get; }
		public byte[] Fingerprint { get; }

		public bool IsHardened => (Child & Hardened) == Hardened;
		public bool IsRoot => Depth == 0;

		public HdKey()
			:this(new WinCryptoPrng().GetBytes(KeyLength + ChainCodeLength))
		{
		}

		private HdKey(byte[] seed)
		{
			var hashkey = Encoders.ASCII.GetBytes("Bitcoin seed");

			var mac = Hashes.HMACSHA512(hashkey, seed);
			Key = new Key(mac.Slice(0, KeyLength));
			ChainCode = mac.Slice(KeyLength, ChainCodeLength);
			Depth = 0;
			Child = 0;
			Fingerprint = new byte[FingerprintLength];
		}

		public HdKey(Key key, byte[] chain, byte depth, uint child, byte[] fingerprint)
		{
			Key = key;
			ChainCode = chain;
			Depth = depth;
			Child = child;
			Fingerprint = fingerprint;
		}

		public HdKey Derive(IEnumerable<uint> indexes)
		{
			return indexes.Aggregate(this, (current, index) => current.Derive(index));
		}

		public HdKey Derive(uint index)
		{
			var keyArray = Key.ToByteArray();
			var pubKeyArray = PubKey.ToByteArray();
			var data = (index & Hardened) != Hardened 
				? pubKeyArray 
				: ByteArray.Zero.Concat(keyArray);

			var salt = Packer.Pack("A^I", data, index);
			var i = Hashes.HMACSHA512(ChainCode, salt);

			var il = i.Slice(0, KeyLength);
			var ir = i.Slice(KeyLength, ChainCodeLength);
			Key.CheckValidKey(il);

			var parse256il = il.ToBigIntegerUnsigned(true);
			var kpar = keyArray.ToBigIntegerUnsigned(true);

			var childCodeChain = ir;
			var childKey = (parse256il + kpar) % Secp256k1.N;
			var keyBytes = childKey.ToUByteArray().ToBigEndian();

			keyBytes = keyBytes.PadLeft(KeyLength);
			var childFingerprint = PubKey.Hash.ToByteArray().Slice(0, FingerprintLength);
			return new HdKey(new Key(keyBytes), childCodeChain, (byte)(Depth + 1), index, childFingerprint);
		}

		public HdKey GetParentExtKey(HdPubKey parent)
		{
			Ensure.NotNull(nameof(parent), parent);
			if (IsRoot)
				throw new InvalidOperationException("This ExtKey is the root key of the HD tree");

			if (IsHardened)
				throw new InvalidOperationException("This private key is hardened, so you can't get its parent");

			var expectedFingerPrint = parent.PubKey.Hash.ToByteArray().Slice(0, FingerprintLength);
			if (parent.Depth != Depth - 1 || !expectedFingerPrint.IsEqualTo(Fingerprint))
				throw new ArgumentException("The parent ExtPubKey is not the immediate parent of this ExtKey", nameof(parent));

			var pubKey = parent.PubKey.ToByteArray();
			var salt = Packer.Pack("bA^I", pubKey[0], pubKey.Slice(1), Child);
			var i = Hashes.HMACSHA512(parent.ChainCode, salt);
			var il = i.Slice(0, KeyLength);
			var ir = i.Slice(KeyLength, ChainCodeLength);
			var ccChild = ir;

			if (!ccChild.IsEqualTo(ChainCode))
				throw new InvalidOperationException("The derived chain code of the parent is not equal to this child chain code");

			var parse256il = il.ToBigIntegerUnsigned(true);
			var key = Key.ToByteArray().ToBigIntegerUnsigned(true);

			var kpar =  (key - parse256il) % Secp256k1.N;
			kpar = kpar.Sign >= 0 ? kpar : kpar + Secp256k1.N;

			var keyParentBytes = kpar.ToUByteArray().ToBigEndian();
			if (keyParentBytes.Length < KeyLength)
				keyParentBytes = new byte[KeyLength - keyParentBytes.Length].Concat(keyParentBytes);

			return new HdKey(new Key(keyParentBytes), parent.ChainCode, parent.Depth, parent.Child, parent.Fingerprint);
		}

		public HdPubKey Neuter =>
			new HdPubKey(PubKey, ChainCode, Depth, Child, Fingerprint);

		public byte[] ToByteArray()
		{
			return Packer.Pack("bA^I_AbA", Depth, Fingerprint, Child, ChainCode, 0, Key.ToByteArray());
		}
		public string ToString(Network network)
		{
			return Base58Data.ToString(ToByteArray(), DataTypePrefix.ExtPrivateKey, network);
		}
	}

	public static class KeyPath
	{
		public static IEnumerable<uint> Parse(string path)
		{
			var parts = path.Split('/');
			foreach (var part in parts.Skip(1))
			{
				var isHardened = part[part.Length-1] == '\'';
				var len = part.Length - (isHardened ? 1 : 0);
				var val = uint.Parse(part.Substring(0, len));
				var index = isHardened ? val | 0x80000000 : val;
				yield return index;
			}
		}
	}
}