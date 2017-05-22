using System;
using System.Collections.Generic;
using System.Linq;
using BitcoinLite.Utils;

namespace BitcoinLite
{
	public class Network
	{
		private readonly string _name;
		public static readonly Network Main = CreateMainNet();
		public static readonly Network Test = CreateTestNet();
		public static readonly Network Reg = CreateRegTestNet();

		private Dictionary<DataTypePrefix, byte[]> _prefixType;
		private int _rewardHalvingBlocks;
		private long _initialReward;

		private Network(string name)
		{
			_name = name;
		}

		private static Network CreateMainNet()
		{
			var network = new Network("MainNet");
			network._prefixType = new Dictionary<DataTypePrefix, byte[]> {
				{ DataTypePrefix.PublicKeyHash, new byte[] { 0x00 } },
				{ DataTypePrefix.ScriptHash, new byte[] { 0x05 } },
				{ DataTypePrefix.PrivateKey, new byte[] { 0x80 } },
				{ DataTypePrefix.ExtPublicKey, new byte[] { 0x04, 0x88, 0xb2, 0x1e } },
				{ DataTypePrefix.ExtPrivateKey, new byte[] { 0x04, 0x88, 0xad, 0xe4 } },
				{ DataTypePrefix.EncryptedKeyEC, new byte[] { 0x01, 0x43 } },
				{ DataTypePrefix.EncryptedKeyNoEC, new byte[] { 0x01, 0x42 } },
				{ DataTypePrefix.PassphraseCode, new byte[] { 0x2c, 0xe9, 0xb3, 0xe1, 0xff, 0x39, 0xe2 } },
				{ DataTypePrefix.ConfirmationCode, new byte[] { 0x64, 0x3b, 0xf6, 0xa8, 0x9a } },
				{ DataTypePrefix.SegWitPublicKeyHash, new byte[] { 0x06 } },
				{ DataTypePrefix.SegWitScriptHash, new byte[] { 0x0a } },
			};

			network._rewardHalvingBlocks = 210 * 1000; // ~4 years
			network._initialReward = 50*100*1000*1000L;
			return network;
		}

		private static Network CreateTestNet()
		{
			var network = new Network("TestNet");

			network._prefixType = new Dictionary<DataTypePrefix, byte[]> {
				{ DataTypePrefix.PublicKeyHash, new byte[] { 0x6f } },
				{ DataTypePrefix.ScriptHash, new byte[] { 0xc4 } },
				{ DataTypePrefix.PrivateKey, new byte[] { 0xef } },
				{ DataTypePrefix.ExtPublicKey, new byte[] { 0x04, 0x35, 0x87, 0xcf } },
				{ DataTypePrefix.ExtPrivateKey, new byte[] { 0x04, 0x35, 0x83, 0x94 } },
				{ DataTypePrefix.EncryptedKeyEC, new byte[] { 0x01, 0x43 } },
				{ DataTypePrefix.EncryptedKeyNoEC, new byte[] { 0x01, 0x42 } },
				{ DataTypePrefix.PassphraseCode, new byte[] { 0x2c, 0xe9, 0xb3, 0xe1, 0xff, 0x39, 0xe2 } },
				{ DataTypePrefix.ConfirmationCode, new byte[] { 0x64, 0x3b, 0xf6, 0xa8, 0x9a } },
				{ DataTypePrefix.SegWitPublicKeyHash, new byte[] { 0x03 } },
				{ DataTypePrefix.SegWitScriptHash, new byte[] { 0x28 } },
			};

			network._rewardHalvingBlocks = 150; // ~25 hours
			network._initialReward = 50 * 100 * 1000 * 1000L;
			return network;
		}

		private static Network CreateRegTestNet()
		{
			var network = new Network("RegTest");

			network._prefixType = new Dictionary<DataTypePrefix, byte[]> {
				{ DataTypePrefix.PublicKeyHash, new byte[] { 0x6f } },
				{ DataTypePrefix.ScriptHash, new byte[] { 0xc4 } },
				{ DataTypePrefix.PrivateKey, new byte[] { 0xef } },
				{ DataTypePrefix.ExtPublicKey, new byte[] { 0x04, 0x35, 0x87, 0xcf } },
				{ DataTypePrefix.ExtPrivateKey, new byte[] { 0x04, 0x35, 0x83, 0x94 } },
				{ DataTypePrefix.EncryptedKeyEC, new byte[] { 0x01, 0x43 } },
				{ DataTypePrefix.EncryptedKeyNoEC, new byte[] { 0x01, 0x42 } },
				{ DataTypePrefix.SegWitPublicKeyHash, new byte[] { 0x03 } },
				{ DataTypePrefix.SegWitScriptHash, new byte[] { 0x28 } },
			};

			network._rewardHalvingBlocks = 210 * 1000; // ~4 years
			network._initialReward = 50 * 100 * 1000 * 1000L;
			return network;
		}

		public byte[] GetPrefixBytes(DataTypePrefix addressType)
		{
			return _prefixType[addressType];
		}

		public static Network GetFromPrefix(byte[] bytes, out DataTypePrefix type)
		{
			var p = from network in new[] {Main, Test, Reg}
				from prefix in network._prefixType
				let prefixLen = prefix.Value.Length
				where prefix.Value.IsEqualTo(bytes.Take(prefixLen).ToArray())
				select new {Network = network, Type = prefix.Key};

			var first = p.FirstOrDefault();
			if(first == null)
				throw new FormatException("Data with unknown prefix");

			type = first.Type;
			return first.Network;
		}

		public long CalculateReward(int height)
		{
			var halvings = height/_rewardHalvingBlocks;
			return _initialReward >>= (halvings < 64) ? halvings : 0;
		}
	}
}