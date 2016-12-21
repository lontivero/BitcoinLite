using BitcoinLite.Encoding;
using BitcoinLite.Utils;

namespace BitcoinLite.Crypto
{
	public class PubKey : IBinarySerializable
	{
		private readonly byte[] _key;

		public PubKey(byte[] key)
		{
			_key = key;
		}

		public PubKey(ECPoint point, bool isCompressed)
			: this(point.Encode(isCompressed))
		{
		}

		public KeyId Hash => new KeyId(Hashes.RIPEMD160(Hashes.SHA256(_key)));

		public Address ToAddress(Network network)
		{
			return Hash.GetAddress(network); // new PubKeyHashAddress(network, Hash);
		}

		public byte[] ToByteArray()
		{
			return _key.CloneByteArray();
		}

		public ECPoint Point => ECPoint.Decode(_key);

		public Script ScriptPubKey => Script.FromPubKey(this);

		public bool Verify(byte[] data, ECDSASignature signature)
		{
			return ECDsaSigner.VerifySignature(data, signature, this);
		}

		public bool IsCanonical => ECPoint.IsCanonical(_key);

		public override string ToString()
		{
			return Encoders.Hex.GetString(ToByteArray());
		}

		public string ToString(Network network)
		{
			return ToAddress(network).ToString();
		}
	}
}