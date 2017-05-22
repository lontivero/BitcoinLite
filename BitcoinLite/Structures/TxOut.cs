namespace BitcoinLite.Structures
{
	public class TxOut : IVisitable
	{
		// Transaction Value
		public long Value { get; internal set; }

		// Usually contains the public key as a Bitcoin script setting up conditions to claim this output.
		public Script ScriptPubKey { get; internal set; }

		public TxOut()
		{
			Value = 0;
		}

		public TxOut(long value, ITxDestination destination)
		{
			Ensure.NotNull(nameof(destination), destination);
			Value = value;
			ScriptPubKey = destination.ScriptPubKey;
		}

		public TxOut(long value, Script scriptPubKey)
		{
			Ensure.NotNull(nameof(scriptPubKey), scriptPubKey);

			Value = value;
			ScriptPubKey = scriptPubKey;
		}

		public bool IsNull => Value == -1;

		public bool IsDust => ((1000 * Value) / (3 * (this.ToByteArray().Length + 148)) < ProtocolConstants.MinRelayTxFee);

		//		public static TxOut FromByteArray(byte[] bytes)
		//		{
		//			var mem = new MemoryStream(bytes);
		//			var reader = new BlockchainReader(new BitcoinBinaryReader(mem));
		//			return reader.ReadTxOut();
		//		}

		//		public static TxOut Parse(string hex)
		//		{
		//			return FromByteArray(Encoders.Hex.GetBytes(hex));
		//		}
	}
}