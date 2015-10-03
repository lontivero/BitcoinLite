using System;
using System.Collections.Generic;
using System.Linq;
using BitcoinLite.Crypto;
using BitcoinLite.Utils;

namespace BitcoinLite
{
	public class Network
	{
		public readonly static Network Main = CreateMainNet();
		public readonly static Network Test = CreateTestNet();
		public readonly static Network Reg = new Network();

		private Dictionary<DataTypePrefix, byte[]> _prefixType;
		private int _rewardHalvingBlocks;
		private long _initialReward;

		private Network(){}

		private static Network CreateMainNet()
		{
			var network = new Network();
			network._prefixType = new Dictionary<DataTypePrefix, byte[]> {
				{ DataTypePrefix.PublicKeyHash, new byte[] { 0x00 } },
				{ DataTypePrefix.ScriptHash, new byte[] { 0x05 } },
				{ DataTypePrefix.PrivateKey, new byte[] { 0x80 } },
				{ DataTypePrefix.ExtPublicKey, new byte[] { 0x04, 0x88, 0xb2, 0x1e } } };

			network._rewardHalvingBlocks = 210 * 1000; // ~4 years
			network._initialReward = 50*100*1000*1000L;
			return network;
		}

		private static Network CreateTestNet()
		{
			var network = new Network();

			network._prefixType = new Dictionary<DataTypePrefix, byte[]> {
				{ DataTypePrefix.PublicKeyHash, new byte[] { 0x6f } },
				{ DataTypePrefix.ScriptHash, new byte[] { 0xc4 } },
				{ DataTypePrefix.PrivateKey, new byte[] { 0xef } },
				{ DataTypePrefix.ExtPublicKey, new byte[] { 0x04, 0x35, 0x87, 0xcf } } };

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