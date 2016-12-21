using System;
using BitcoinLite.Encoding;
using BitcoinLite.Utils;

namespace BitcoinLite.Crypto
{
	public class Base58Data
	{
		private Base58Data()
		{
		}

		public static byte[] FromString(string wif)
		{
			Network network;
			DataTypePrefix type;
			return FromString(wif, out network, out type);
		}

		public static byte[] FromString(string wif, out Network network, out DataTypePrefix type)
		{
			var bytes = Encoders.Base58Check.GetBytes(wif);
			network = Network.GetFromPrefix(bytes, out type);
			var prefix = network.GetPrefixBytes(type);
			return bytes.Slice(prefix.Length);
		}

		public static string ToString(byte[] wif, DataTypePrefix type, Network network)
		{
			var prefix = network.GetPrefixBytes(type);
			return Encoders.Base58Check.GetString(prefix.Concat(wif));
		}

		public static string ToString(byte[] wif, DataTypePrefix type)
		{
			var prefix = Network.Main.GetPrefixBytes(type); // bytes that doesnt depend on any network
			return Encoders.Base58Check.GetString(prefix.Concat(wif));
		}

	}
}