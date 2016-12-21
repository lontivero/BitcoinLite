using System;
using System.Numerics;
using BitcoinLite.Bip38;
using BitcoinLite.Encoding;
using BitcoinLite.Utils;

namespace BitcoinLite.Crypto
{
	public class Key : IBinarySerializable
	{
		private readonly byte[] _key;
		private readonly bool _isCompressed;
		private PubKey _pubKey;

		public static Key Parse(string wif)
		{
			var key = Base58Data.FromString(wif);
			var isCompressed = key.Length > 32 && key[32] == 0x01;
			return new Key(key.Slice(0, 32), isCompressed);
		}

		public static Key Create()
		{
			var prng = new WinCryptoPrng();
			var rnd = prng.GetBytes(32);
			var key = Hashes.SHA256(rnd);
			return new Key(key);
		}

		public static Key Create(string entropy)
		{
			var prng = new WinCryptoPrng();
			var rnd = prng.GetBytes(32);
			var data = Encoders.ASCII.GetBytes(entropy);
			var key = Hashes.HMACSHA256(rnd, data);
			return new Key(key);
		}

		public Key(byte[] key)
			: this(key, true)
		{}

		public Key(byte[] key, bool compressed)
		{
			Ensure.NotNull(nameof(key), key);
			Ensure.That(nameof(key), ()=>key.Length == 0x20, "private key must be a 32 bytes length array");
			_isCompressed = compressed;
			CheckValidKey(key);

			_key = key;
		}

		internal static void CheckValidKey(byte[] key)
		{
			var candidateKey = key.ToBigIntegerUnsigned(false);
			if (candidateKey <= 0 || candidateKey >= Secp256k1.N)
				throw new ArgumentException("Invalid key (out of range). Save it: 0x" + Encoders.Hex.GetString(key), nameof(key));
		}

		public bool IsCompressed => _isCompressed;

		public PubKey PubKey => _pubKey ?? (_pubKey = new PubKey(PublicPoint, IsCompressed));

		internal ECPoint PublicPoint => K * Secp256k1.G;

		internal BigInteger K => _key.ToBigIntegerUnsigned(true);

		public ECDSASignature Sign(byte[] input)
		{
			var signer = new ECDsaSigner(this);
			return signer.GenerateSignature(input);
		}

		public byte[] ToByteArray()
		{
			return _key.CloneByteArray();
		}

		public override string ToString()
		{
			return Encoders.Hex.GetString(_key);
		}

		public string ToString(Network network)
		{
			var key = _isCompressed ? _key.Concat(ByteArray.One) : _key;
			return Base58Data.ToString(key, DataTypePrefix.PrivateKey, network);
		}

		public EncryptedKey GetEncryptedKey(string passphrase, Network network)
		{
			return new EncryptedKey(this, passphrase, network);
		}
	}
}
