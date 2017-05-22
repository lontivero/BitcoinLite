using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitcoinLite.Crypto;
using BitcoinLite.Structures;
using BitcoinLite.Utils;
using BitcoinLite.Visitors;
using NUnit.Framework;

namespace BitcoinLite.Tests
{
	[TestFixture]
	public class TransactionTests
	{
		[Test]
		public void x()
		{
			var tx = new Transaction();
			tx.Inputs.Add(new TxIn(new OutPoint(uint256.Zero, 0xffffffff)));
			tx.Outputs.Add(new TxOut(10000, Key.Create().PubKey.Hash.ScriptPubKey));
			var xy = tx.ToJson();
		}
	}
}
