using System;
using System.Numerics;
using BitcoinLite.Encoding;
using BitcoinLite.Utils;

namespace BitcoinLite.Crypto
{
	public class Base58Data
	{
		public static byte[] FromString(string wif)
		{
			Network network;
			DataTypePrefix type;
			return FromString(wif, out network);
		}

		public static byte[] FromString(string wif, out Network network)
		{
			var bytes = Encoders.Base58Check.Decode(wif);
			DataTypePrefix type;
			network = Network.GetFromPrefix(bytes, out type);
			if (network == null)
				throw new FormatException("Invalid Key Format");

			var prefix = network.GetPrefixBytes(type);
			return bytes.SafeSubarray(prefix.Length);
		}

		public static string ToString(byte[] wif, DataTypePrefix type, Network network)
		{
			var prefix = network.GetPrefixBytes(type);
			return Encoders.Base58Check.Encode(prefix.Concat(wif));
		}
	}

	public class Key
	{
		private readonly byte[] _key;
		private readonly bool _isCompressed;
		private PublicKey _pubKey;

		public static Key Parse(string wif)
		{
			var key = Base58Data.FromString(wif);
			var isCompressed = key.Length > 32 && key[32] == 0x01;
			return new Key(key.SafeSubarray(0, 32), isCompressed);
		}

		public static Key Create()
		{
			var prng = new WinCryptoPrng();
			var rnd = prng.Next();
			var key = Hashes.SHA256(rnd);
			return new Key(key);
		}

		public static Key Create(string entropy)
		{
			var prng = new WinCryptoPrng();
			var rnd = prng.Next();
			var data = Encoders.ASCII.Decode(entropy);
			var key = Hashes.HMACSHA256(rnd, data);
			return new Key(key);
		}

		public Key(byte[] key)
			: this(key, true)
		{}

		public Key(byte[] key, bool compressed)
		{
			if(key.Length != 32)
				throw new ArgumentException("private key must be a 32 bytes length array", "key");
			_isCompressed = compressed;
			CheckValidKey(key);

			_key = key;
		}

		internal static void CheckValidKey(byte[] key)
		{
			var candidateKey = key.ToBigIntegerUnsigned(false);
			if (candidateKey <= 0 || candidateKey >= Secp256k1.N)
				throw new ArgumentException("Invalid key");
		}

		public bool IsCompressed
		{
			get { return _isCompressed; }
		}

		public PublicKey PublicKey
		{
			get { return _pubKey ?? (_pubKey = new PublicKey(PublicPoint, IsCompressed)); }
		}

		internal ECPoint PublicPoint
		{
			get { return K * Secp256k1.G; }
		}

		internal BigInteger K
		{
			get { return _key.ToBigIntegerUnsigned(true); }
		}

		public ECDSASignature Sign(byte[] input)
		{
			var signer = new ECDsaSigner(this);
			return signer.GenerateSignature(input);
		}

		public byte[] ToByteArray()
		{
			return _key.SafeSubarray(0);
		}

		public override string ToString()
		{
			return Encoders.Hex.Encode(ToByteArray());
		}

		public string ToString(Network network)
		{
			var key = _isCompressed ? _key.Concat(ByteArray.One) : _key;
			return Base58Data.ToString(key, DataTypePrefix.PrivateKey, network);
		}
	}
}
