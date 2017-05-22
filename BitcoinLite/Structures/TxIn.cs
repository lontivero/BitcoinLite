using System;
using System.Collections.Generic;
using System.Linq;
using BitcoinLite.Crypto;
using BitcoinLite.Utils;

namespace BitcoinLite.Structures
{
	public class TxIn : IVisitable
	{
		// The previous output transaction reference, as an OutPoint structure
		public OutPoint PreviousOutput { get; internal set; }

		// Computational Script for confirming transaction authorization
		public Script ScriptSig { get; internal set; }

		// Transaction version as defined by the sender. Intended for "replacement" of transactions when information is updated before inclusion into a block.
		public uint Sequence { get; internal set; }


		public TxIn(OutPoint previousOutput, Script scriptSig, uint sequence)
		{
			Ensure.NotNull(nameof(previousOutput), previousOutput);
			Ensure.NotNull(nameof(scriptSig), scriptSig);

			PreviousOutput = previousOutput;
			ScriptSig = scriptSig;
			Sequence = sequence;
		}

		public TxIn(Script sigScript)
			: this(sigScript, uint.MaxValue)
		{
		}

		public TxIn(Script sigScript, uint sequence)
		{
			Ensure.NotNull(nameof(sigScript), sigScript);
			ScriptSig = sigScript;
			Sequence = sequence;
			PreviousOutput = OutPoint.None;
		}

		public TxIn(OutPoint prevout)
		{
			Ensure.NotNull(nameof(prevout), prevout);
			ScriptSig = Script.Empty;
			Sequence = uint.MaxValue;
			PreviousOutput = prevout;
		}

		public void Sign(Key key)
		{
			byte[] hash = null;
			var ecdsaSign = key.Sign(hash).ToDER();
			ScriptSig = new Script(ecdsaSign.Concat(key.PubKey.ToByteArray()));
		}

		public void Sign(Script script, Transaction tx, int inIdx, SigHash hashtype)
		{
			var hash = Script.SignatureHash(script, tx, inIdx, hashtype);
		}


		//		public static TxIn FromByteArray(byte[] bytes)
		//		{
		//			var mem = new MemoryStream(bytes);
		//			var reader = new BlockchainReader(new BitcoinBinaryReader(mem));
		//			return reader.ReadTxIn();
		//		}

		//		public static TxIn Parse(string hex)
		//		{
		//			return FromByteArray(Encoders.Hex.GetBytes(hex));
		//		}
	}
}