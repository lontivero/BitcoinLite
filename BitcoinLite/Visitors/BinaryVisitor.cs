using System;
using System.Collections.Generic;
using System.IO;
using BitcoinLite.Structures;
using BitcoinLite.Utils;

namespace BitcoinLite.Visitors
{
	public static class BinaryVisitor
	{
		private static readonly Dictionary<Type, Action<IVisitable, BinaryWriter>> Methods
			= new Dictionary<Type, Action<IVisitable, BinaryWriter>> {
				{ typeof(Block), SerializeBlock },
				{ typeof(BlockHeader), SerializeBlockHeader },
				{ typeof(Transaction), SerializeTransaction },
				{ typeof(Target), SerializeTarget },
				{ typeof(TxIn), SerializeTxIn },
				{ typeof(TxOut), SerializeTxOut },
				{ typeof(OutPoint), SerializeOutPoint },
			};

		private static void SerializeOutPoint(IVisitable arg, BinaryWriter writer)
		{
			throw new NotImplementedException();
		}

		private static void SerializeTxOut(IVisitable arg, BinaryWriter writer)
		{
			throw new NotImplementedException();
		}

		private static void SerializeTxIn(IVisitable arg, BinaryWriter writer)
		{
			throw new NotImplementedException();
		}

		private static void SerializeTarget(IVisitable arg, BinaryWriter writer)
		{
			throw new NotImplementedException();
		}

		private static void SerializeTransaction(IVisitable arg, BinaryWriter writer)
		{
			var tx = arg as Transaction;
			writer.Write(tx.Version);
			writer.Write(tx.Inputs.Count);
			foreach (var input in tx.Inputs)
			{
				SerializeTxIn(input, writer);
			}
			writer.Write(tx.Outputs.Count);
			foreach (var output in tx.Outputs)
			{
				SerializeTxOut(output, writer);
			}

			//return Packer.Pack("II", tx.Version, tx.Inputs.Count);
		}

		private static void SerializeBlockHeader(IVisitable arg, BinaryWriter writer)
		{
			throw new NotImplementedException();
		}

		private static void SerializeBlock(IVisitable arg, BinaryWriter writer)
		{
			throw new NotImplementedException();
		}

		public static byte[] ToByteArray(this IVisitable arg)
		{
			var stream = new MemoryStream();
			var writer = new BinaryWriter(stream);
			Methods[arg.GetType()](arg, writer);
			return stream.ToArray();
		}
	}
}
