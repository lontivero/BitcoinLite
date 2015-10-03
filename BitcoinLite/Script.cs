using System;
using System.Collections.Generic;
using System.ComponentModel;
using BitcoinLite.Crypto;
using BitcoinLite.Encoding;

namespace BitcoinLite
{
	public enum Opcode : byte
	{
		#region Push Value
		// ReSharper disable InconsistentNaming
		OP_0 = 0x00,
		OP_FALSE = OP_0,
		OP_PUSHDATA1 = 0x4c,
		OP_PUSHDATA2 = 0x4d,
		OP_PUSHDATA4 = 0x4e,
		OP_1NEGATE = 0x4f,
		OP_RESERVED = 0x50,
		OP_TRUE = OP_1,
		OP_1 = 0x51,
		OP_2 = 0x52,
		OP_3 = 0x53,
		OP_4 = 0x54,
		OP_5 = 0x55,
		OP_6 = 0x56,
		OP_7 = 0x57,
		OP_8 = 0x58,
		OP_9 = 0x59,
		OP_10 = 0x5a,
		OP_11 = 0x5b,
		OP_12 = 0x5c,
		OP_13 = 0x5d,
		OP_14 = 0x5e,
		OP_15 = 0x5f,
		OP_16 = 0x60,
		#endregion

		#region Flow Control
		OP_NOP = 0x61,
		OP_VER = 0x62,
		OP_IF = 0x63,
		OP_NOTIF = 0x64,
		OP_VERIF = 0x65,
		OP_VERNOTIF = 0x66,
		OP_ELSE = 0x67,
		OP_ENDIF = 0x68,
		OP_VERIFY = 0x69,
		OP_RETURN = 0x6a,
		#endregion

		#region Stack OP_ERATIONS
		OP_TOALTSTACK = 0x6b,
		OP_FROMALTSTACK = 0x6c,
		OP_2DROP = 0x6d,
		OP_2DUP = 0x6e,
		OP_3DUP = 0x6f,
		OP_2OVER = 0x70,
		OP_2ROT = 0x71,
		OP_2SWAP = 0x72,
		OP_IFDUP = 0x73,
		OP_DEPTH = 0x74,
		OP_DROP = 0x75,
		OP_DUP = 0x76,
		OP_NIP = 0x77,
		OP_OVER = 0x78,
		OP_PICK = 0x79,
		OP_ROLL = 0x7a,
		OP_ROT = 0x7b,
		OP_SWAP = 0x7c,
		OP_TUCK = 0x7d,
		#endregion

		#region Splice ops
		OP_CAT = 0x7e,
		OP_SUBSTR = 0x7f,
		OP_LEFT = 0x80,
		OP_RIGHT = 0x81,
		OP_SIZE = 0x82,
		#endregion

		#region Bit logic
		OP_INVERT = 0x83,
		OP_AND = 0x84,
		OP_OR = 0x85,
		OP_XOR = 0x86,
		OP_EQUAL = 0x87,
		OP_EQUALVERIFY = 0x88,
		OP_RESERVED1 = 0x89,
		OP_RESERVED2 = 0x8a,
		#endregion

		#region Numeric
		OP_1ADD = 0x8b,
		OP_1SUB = 0x8c,
		OP_2MUL = 0x8d,
		OP_2DIV = 0x8e,
		OP_NEGATE = 0x8f,
		OP_ABS = 0x90,
		OP_NOT = 0x91,
		OP_0NOTEQUAL = 0x92,

		OP_ADD = 0x93,
		OP_SUB = 0x94,
		OP_MUL = 0x95,
		OP_DIV = 0x96,
		OP_MOD = 0x97,
		OP_LSHIFT = 0x98,
		OP_RSHIFT = 0x99,

		OP_BOOLAND = 0x9a,
		OP_BOOLOR = 0x9b,
		OP_NUMEQUAL = 0x9c,
		OP_NUMEQUALVERIFY = 0x9d,
		OP_NUMNOTEQUAL = 0x9e,
		OP_LESSTHAN = 0x9f,
		OP_GREATERTHAN = 0xa0,
		OP_LESSTHANOREQUAL = 0xa1,
		OP_GREATERTHANOREQUAL = 0xa2,
		OP_MIN = 0xa3,
		OP_MAX = 0xa4,

		OP_WITHIN = 0xa5,
		#endregion

		#region Crypto
		OP_RIPEMD160 = 0xa6,
		OP_SHA1 = 0xa7,
		OP_SHA256 = 0xa8,
		OP_HASH160 = 0xa9,
		OP_HASH256 = 0xaa,
		OP_CODESEPARATOR = 0xab,
		OP_CHECKSIG = 0xac,
		OP_CHECKSIGVERIFY = 0xad,
		OP_CHECKMULTISIG = 0xae,
		OP_CHECKMULTISIGVERIFY = 0xaf,
		#endregion

		#region Expansion
		OP_NOP1 = 0xb0,
		OP_NOP2 = 0xb1,
		OP_NOP3 = 0xb2,
		OP_NOP4 = 0xb3,
		OP_NOP5 = 0xb4,
		OP_NOP6 = 0xb5,
		OP_NOP7 = 0xb6,
		OP_NOP8 = 0xb7,
		OP_NOP9 = 0xb8,
		OP_NOP10 = 0xb9,

		OP_INVALIDOPERATION = 0xff
		// ReSharper restore InconsistentNaming
		#endregion
	};

	public class Op
	{
		public byte Code { get; set; }
		public byte[] Data { get; set; }
		public Opcode Opcode { get { return (Opcode)Code; } }

		public Op(byte code, byte[] data = null)
		{
			Code = code;
			Data = data;
		}

		public override string ToString()
		{
			var opcodeName = Enum.GetName(typeof(Opcode), Opcode);
			if (Data == null)
			{
				return opcodeName ?? Code.ToString("x2");
			}

			var str = "";
			if (Opcode == Opcode.OP_PUSHDATA1 || Opcode == Opcode.OP_PUSHDATA2 || Opcode == Opcode.OP_PUSHDATA4)
			{
				str += opcodeName + " ";
			}
			if (Data.Length > 0)
			{
				str += Encoders.Hex.Encode(Data);
			}
			return str;
		}

		internal static Op Push(byte[] data)
		{
			var len = data.Length;
			var op = Opcode.OP_INVALIDOPERATION;

			if(len == 0) 
				op = Opcode.OP_0;

			var data0 = data[0];

			if( len == 1 && (data0 >= 1 && data0 <= 16) )
				op = Opcode.OP_1 + (byte)len - 1;

			else if (len == 1 && data0 == 0x81)
				op = Opcode.OP_1NEGATE;

			else if (len >= 1 && len <= 0x4b)
				op = (Opcode)len;

			else if (len <= 0xFF)
				op = Opcode.OP_PUSHDATA1;

			else if (len <= 0xFFFF)
				op = Opcode.OP_PUSHDATA2;

			else if (len <= 0xFFFFFFFF)
				op = Opcode.OP_PUSHDATA4;
			else
				throw new ArgumentException("data");

			return new Op((byte)op, data);
		}
	}

	public class Script
	{
		public static Script Empty
		{
			get { return new Script(); }
		}

		private readonly List<Op> _items = new List<Op>();

		public static Script FromAddress(Address address)
		{
			var addressHex = Encoders.Hex.Encode(address.ToByteArray());
			return Parse("OP_DUP OP_HASH160 " + addressHex + " OP_EQUALVERIFY OP_CHECKSIG");
		}

		public static Script FromPubKey(PublicKey publicKey)
		{
			var s = new Script();
			s.Add(Op.Push(publicKey.ToByteArray()));
			s.Add(Opcode.OP_CHECKSIG);
			return s;
		}

		public static Script Parse(string str)
		{
			var script = new Script();
			var tokens = str.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
			for (var i = 0; i < tokens.Length; i++)
			{
				var token = tokens[i];
				Opcode opcode;
				if(Enum.TryParse(token, out opcode))
				{
					script.Add(opcode);
					if (opcode == Opcode.OP_PUSHDATA1 || opcode == Opcode.OP_PUSHDATA2 || opcode == Opcode.OP_PUSHDATA4)
					{
						script.Add(opcode, Encoders.Hex.Decode(tokens[i + 2]));
						i += 2;
					}
					continue;
				}

				var opcodevi = (Opcode)token[0];
				if (opcodevi > 0 && opcodevi < Opcode.OP_PUSHDATA1)
				{
					var val = Encoders.Hex.Decode(token);
					script.Add(val);
				}
			}
			return script;
		}

		public override string ToString()
		{
			return string.Join(" ", _items);
		}

		private void Add(Op op)
		{
			_items.Add(op);
		}

		private void Add(Opcode op, byte[] data = null, byte lenght = 0)
		{
			_items.Add(new Op((byte)op, data));
		}

		private void Add(byte[] data)
		{
			_items.Add(new Op((byte)data.Length, data));
		}
	}
}
