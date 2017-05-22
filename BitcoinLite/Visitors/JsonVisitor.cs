using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BitcoinLite.Structures;

namespace BitcoinLite.Visitors
{
	public static class JsonVisitor
	{
		private static readonly Dictionary<Type, Func<IVisitable, string>> Methods
			= new Dictionary<Type, Func<IVisitable, string>> {
				{ typeof(Block), SerializeBlock },
				{ typeof(BlockHeader), SerializeBlockHeader },
				{ typeof(Transaction), SerializeTransaction },
				{ typeof(Target), SerializeTarget },
				{ typeof(TxIn), SerializeTxIn },
				{ typeof(Script), SerializeScript },
				{ typeof(TxOut), SerializeTxOut },
				{ typeof(OutPoint), SerializeOutPoint },
			};

		private static string SerializeOutPoint(IVisitable arg)
		{
			var outPoint = arg as OutPoint;
			var s = new StringBuilder();
			var w = new JsonWriter(s);
			w.WriteString("hash", outPoint.Hash.ToString());
			w.WriteNumber("index", outPoint.Index);
			return w.ToJson();
		}

		private static string SerializeScript(IVisitable arg)
		{
			var script = arg as Script;
			var s = new StringBuilder();
			var w = new JsonWriter(s);
			w.WriteString("asm", script.ToAsm());
			w.WriteString("hex", script.ToString());
			w.WriteString("hash", script.Hash.ToString());
			w.WriteBoolean("is_p2pkh", script.IsPayToPubKeyHash());
			return w.ToJson();
		}

		private static string SerializeTxOut(IVisitable arg)
		{
			var txout = arg as TxOut;
			var s = new StringBuilder();
			var w = new JsonWriter(s);
			w.WriteNumber("value", txout.Value);
			w.WriteBoolean("is_dust", txout.IsDust);
			w.WriteBoolean("is_null", txout.IsNull);
			w.BeginObject("script");
			w.WriteString("asm", txout.ScriptPubKey.ToAsm());
			w.WriteString("hex", txout.ScriptPubKey.ToString());
			w.WriteString("hash", txout.ScriptPubKey.Hash.ToString());
			w.WriteString("address", txout.ScriptPubKey.Hash.ToString());
			w.WriteBoolean("is_p2pkh", txout.ScriptPubKey.IsPayToPubKeyHash());
			w.EndObject();
			return w.ToJson();
		}

		private static string SerializeTxIn(IVisitable arg)
		{
			var txin = arg as TxIn;
			var s = new StringBuilder();
			var w = new JsonWriter(s);
			w.WriteJson("prev_output", txin.PreviousOutput.ToJson());
			w.WriteJson("scriptSig", txin.ScriptSig.ToJson());
			w.WriteNumber("sequence", txin.Sequence);
			return w.ToJson();
		}

		private static string SerializeTarget(IVisitable arg)
		{
			throw new NotImplementedException();
		}

		private static string SerializeTransaction(IVisitable arg)
		{
			var tx = arg as Transaction;
			var s = new StringBuilder();
			var w = new JsonWriter(s);
			w.WriteString("id", tx.Hash.ToString());
			w.WriteString("tx", tx.ToString());
			w.WriteBoolean("is_coinbase", tx.IsCoinBase);
			w.BeginList("inputs");
			foreach (var input in tx.Inputs)
			{
				w.WriteJson(input.ToJson());
			}
			w.EndList();
			w.BeginList("outputs");
			foreach (var output in tx.Outputs)
			{
				w.WriteJson(output.ToJson());
			}
			w.EndList();

			return w.ToJson();
		}

		private static string SerializeBlockHeader(IVisitable arg)
		{
			throw new NotImplementedException();
		}

		private static string SerializeBlock(IVisitable arg)
		{
			throw new NotImplementedException();
		}

		public static string ToJson(this IVisitable arg)
		{
			return Methods[arg.GetType()](arg);
		}
	}

	class JsonWriter
	{
		private readonly StringWriter _writer;

		public JsonWriter(StringBuilder builder)
		{
			_writer = new StringWriter(builder);
		}

		public void WriteString(string field, string value)
		{
			_writer.Write("\"{0}\": \"{1}\",", field, value);
		}

		public void WriteNumber(string field, long value)
		{
			_writer.Write("\"{0}\": {1},", field, value);
		}

		public void WriteBoolean(string field, bool value)
		{
			_writer.Write("\"{0}\": {1},", field, value ? "true" : "false");
		}

		public void BeginList(string field)
		{
			_writer.Write("\"{0}\": [", field);
		}

		public void EndList()
		{
			_writer.Write("],");
		}
		public void BeginObject(string field)
		{
			_writer.Write("\"{0}\": {{", field);

		}

		public void EndObject()
		{
			_writer.Write("},");
		}

		public string ToJson()
		{
			return "{" + _writer + "}";
		}

		public void WriteJson(string json)
		{
			_writer.Write(json);
		}
		public void WriteJson(string field, string json)
		{
			_writer.Write("\"{0}\": ", field);
			_writer.Write(json + ",");
		}
	}
}
