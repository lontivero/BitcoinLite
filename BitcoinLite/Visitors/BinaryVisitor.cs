using System;
using System.Collections.Generic;
using BitcoinLite.Structures;
using BitcoinLite.Utils;

namespace BitcoinLite.Visitors
{
	public static class BinaryVisitor
	{
		private static readonly Dictionary<Type, Func<IVisitable, object>> Methods
			= new Dictionary<Type, Func<IVisitable, object>> {
				{ typeof(Block), SerializeBlock },
				{ typeof(BlockHeader), SerializeBlockHeader },
				{ typeof(Transaction), SerializeTransaction },
				{ typeof(Target), SerializeTarget },
				{ typeof(TxIn), SerializeTxIn },
				{ typeof(TxOut), SerializeTxOut },
				{ typeof(OutPoint), SerializeOutPoint },
			};

		private static object SerializeOutPoint(IVisitable arg)
		{
			throw new NotImplementedException();
		}

		private static object SerializeTxOut(IVisitable arg)
		{
			throw new NotImplementedException();
		}

		private static object SerializeTxIn(IVisitable arg)
		{
			throw new NotImplementedException();
		}

		private static object SerializeTarget(IVisitable arg)
		{
			throw new NotImplementedException();
		}

		private static object SerializeTransaction(IVisitable arg)
		{
			var tx = arg as Transaction;
			foreach (var input in tx.Inputs)
			{
				
			}
			return Packer.Pack("II", tx.Version, tx.Inputs.Count);
		}

		private static object SerializeBlockHeader(IVisitable arg)
		{
			throw new NotImplementedException();
		}

		private static object SerializeBlock(IVisitable arg)
		{
			throw new NotImplementedException();
		}

		public static byte[] ToByteArray(this IVisitable arg)
		{
			return (byte[])Methods[arg.GetType()](arg);
		}
	}
}
