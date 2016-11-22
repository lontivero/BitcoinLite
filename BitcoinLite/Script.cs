using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using BitcoinLite.Crypto;
using BitcoinLite.Encoding;
using BitcoinLite.Utils;

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
		#region Lookup tables
		private static readonly Dictionary<byte, Opcode> Code2Opcode = new Dictionary<byte, Opcode>
		{
			[0x00] = Opcode.OP_0,
			[0x4c] = Opcode.OP_PUSHDATA1,
			[0x4d] = Opcode.OP_PUSHDATA2,
			[0x4e] = Opcode.OP_PUSHDATA4,
			[0x4f] = Opcode.OP_1NEGATE,
			[0x50] = Opcode.OP_RESERVED,
			[0x51] = Opcode.OP_1,
			[0x52] = Opcode.OP_2,
			[0x53] = Opcode.OP_3,
			[0x54] = Opcode.OP_4,
			[0x55] = Opcode.OP_5,
			[0x56] = Opcode.OP_6,
			[0x57] = Opcode.OP_7,
			[0x58] = Opcode.OP_8,
			[0x59] = Opcode.OP_9,
			[0x5a] = Opcode.OP_10,
			[0x5b] = Opcode.OP_11,
			[0x5c] = Opcode.OP_12,
			[0x5d] = Opcode.OP_13,
			[0x5e] = Opcode.OP_14,
			[0x5f] = Opcode.OP_15,
			[0x60] = Opcode.OP_16,
			[0x61] = Opcode.OP_NOP,
			[0x62] = Opcode.OP_VER,
			[0x63] = Opcode.OP_IF,
			[0x64] = Opcode.OP_NOTIF,
			[0x65] = Opcode.OP_VERIF,
			[0x66] = Opcode.OP_VERNOTIF,
			[0x67] = Opcode.OP_ELSE,
			[0x68] = Opcode.OP_ENDIF,
			[0x69] = Opcode.OP_VERIFY,
			[0x6a] = Opcode.OP_RETURN,
			[0x6b] = Opcode.OP_TOALTSTACK,
			[0x6c] = Opcode.OP_FROMALTSTACK,
			[0x6d] = Opcode.OP_2DROP,
			[0x6e] = Opcode.OP_2DUP,
			[0x6f] = Opcode.OP_3DUP,
			[0x70] = Opcode.OP_2OVER,
			[0x71] = Opcode.OP_2ROT,
			[0x72] = Opcode.OP_2SWAP,
			[0x73] = Opcode.OP_IFDUP,
			[0x74] = Opcode.OP_DEPTH,
			[0x75] = Opcode.OP_DROP,
			[0x76] = Opcode.OP_DUP,
			[0x77] = Opcode.OP_NIP,
			[0x78] = Opcode.OP_OVER,
			[0x79] = Opcode.OP_PICK,
			[0x7a] = Opcode.OP_ROLL,
			[0x7b] = Opcode.OP_ROT,
			[0x7c] = Opcode.OP_SWAP,
			[0x7d] = Opcode.OP_TUCK,
			[0x7e] = Opcode.OP_CAT,
			[0x7f] = Opcode.OP_SUBSTR,
			[0x80] = Opcode.OP_LEFT,
			[0x81] = Opcode.OP_RIGHT,
			[0x82] = Opcode.OP_SIZE,
			[0x83] = Opcode.OP_INVERT,
			[0x84] = Opcode.OP_AND,
			[0x85] = Opcode.OP_OR,
			[0x86] = Opcode.OP_XOR,
			[0x87] = Opcode.OP_EQUAL,
			[0x88] = Opcode.OP_EQUALVERIFY,
			[0x89] = Opcode.OP_RESERVED1,
			[0x8a] = Opcode.OP_RESERVED2,
			[0x8b] = Opcode.OP_1ADD,
			[0x8c] = Opcode.OP_1SUB,
			[0x8d] = Opcode.OP_2MUL,
			[0x8e] = Opcode.OP_2DIV,
			[0x8f] = Opcode.OP_NEGATE,
			[0x90] = Opcode.OP_ABS,
			[0x91] = Opcode.OP_NOT,
			[0x92] = Opcode.OP_0NOTEQUAL,
			[0x93] = Opcode.OP_ADD,
			[0x94] = Opcode.OP_SUB,
			[0x95] = Opcode.OP_MUL,
			[0x96] = Opcode.OP_DIV,
			[0x97] = Opcode.OP_MOD,
			[0x98] = Opcode.OP_LSHIFT,
			[0x99] = Opcode.OP_RSHIFT,
			[0x9a] = Opcode.OP_BOOLAND,
			[0x9b] = Opcode.OP_BOOLOR,
			[0x9c] = Opcode.OP_NUMEQUAL,
			[0x9d] = Opcode.OP_NUMEQUALVERIFY,
			[0x9e] = Opcode.OP_NUMNOTEQUAL,
			[0x9f] = Opcode.OP_LESSTHAN,
			[0xa0] = Opcode.OP_GREATERTHAN,
			[0xa1] = Opcode.OP_LESSTHANOREQUAL,
			[0xa2] = Opcode.OP_GREATERTHANOREQUAL,
			[0xa3] = Opcode.OP_MIN,
			[0xa4] = Opcode.OP_MAX,
			[0xa5] = Opcode.OP_WITHIN,
			[0xa6] = Opcode.OP_RIPEMD160,
			[0xa7] = Opcode.OP_SHA1,
			[0xa8] = Opcode.OP_SHA256,
			[0xa9] = Opcode.OP_HASH160,
			[0xaa] = Opcode.OP_HASH256,
			[0xab] = Opcode.OP_CODESEPARATOR,
			[0xac] = Opcode.OP_CHECKSIG,
			[0xad] = Opcode.OP_CHECKSIGVERIFY,
			[0xae] = Opcode.OP_CHECKMULTISIG,
			[0xaf] = Opcode.OP_CHECKMULTISIGVERIFY,
			[0xb0] = Opcode.OP_NOP1,
			[0xb1] = Opcode.OP_NOP2,
			[0xb2] = Opcode.OP_NOP3,
			[0xb3] = Opcode.OP_NOP4,
			[0xb4] = Opcode.OP_NOP5,
			[0xb5] = Opcode.OP_NOP6,
			[0xb6] = Opcode.OP_NOP7,
			[0xb7] = Opcode.OP_NOP8,
			[0xb8] = Opcode.OP_NOP9,
			[0xb9] = Opcode.OP_NOP10,
			[0xff] = Opcode.OP_INVALIDOPERATION
		};

		private static readonly Dictionary<Opcode, byte> Opcode2Code = new Dictionary<Opcode, byte>
		{
			[Opcode.OP_0] = 0x00,
			[Opcode.OP_FALSE] = 0x00,
			[Opcode.OP_PUSHDATA1] = 0x4c,
			[Opcode.OP_PUSHDATA2] = 0x4d,
			[Opcode.OP_PUSHDATA4] = 0x4e,
			[Opcode.OP_1NEGATE] = 0x4f,
			[Opcode.OP_RESERVED] = 0x50,
			[Opcode.OP_TRUE] = 0x00,
			[Opcode.OP_1] = 0x51,
			[Opcode.OP_2] = 0x52,
			[Opcode.OP_3] = 0x53,
			[Opcode.OP_4] = 0x54,
			[Opcode.OP_5] = 0x55,
			[Opcode.OP_6] = 0x56,
			[Opcode.OP_7] = 0x57,
			[Opcode.OP_8] = 0x58,
			[Opcode.OP_9] = 0x59,
			[Opcode.OP_10] = 0x5a,
			[Opcode.OP_11] = 0x5b,
			[Opcode.OP_12] = 0x5c,
			[Opcode.OP_13] = 0x5d,
			[Opcode.OP_14] = 0x5e,
			[Opcode.OP_15] = 0x5f,
			[Opcode.OP_16] = 0x60,
			[Opcode.OP_NOP] = 0x61,
			[Opcode.OP_VER] = 0x62,
			[Opcode.OP_IF] = 0x63,
			[Opcode.OP_NOTIF] = 0x64,
			[Opcode.OP_VERIF] = 0x65,
			[Opcode.OP_VERNOTIF] = 0x66,
			[Opcode.OP_ELSE] = 0x67,
			[Opcode.OP_ENDIF] = 0x68,
			[Opcode.OP_VERIFY] = 0x69,
			[Opcode.OP_RETURN] = 0x6a,
			[Opcode.OP_TOALTSTACK] = 0x6b,
			[Opcode.OP_FROMALTSTACK] = 0x6c,
			[Opcode.OP_2DROP] = 0x6d,
			[Opcode.OP_2DUP] = 0x6e,
			[Opcode.OP_3DUP] = 0x6f,
			[Opcode.OP_2OVER] = 0x70,
			[Opcode.OP_2ROT] = 0x71,
			[Opcode.OP_2SWAP] = 0x72,
			[Opcode.OP_IFDUP] = 0x73,
			[Opcode.OP_DEPTH] = 0x74,
			[Opcode.OP_DROP] = 0x75,
			[Opcode.OP_DUP] = 0x76,
			[Opcode.OP_NIP] = 0x77,
			[Opcode.OP_OVER] = 0x78,
			[Opcode.OP_PICK] = 0x79,
			[Opcode.OP_ROLL] = 0x7a,
			[Opcode.OP_ROT] = 0x7b,
			[Opcode.OP_SWAP] = 0x7c,
			[Opcode.OP_TUCK] = 0x7d,
			[Opcode.OP_CAT] = 0x7e,
			[Opcode.OP_SUBSTR] = 0x7f,
			[Opcode.OP_LEFT] = 0x80,
			[Opcode.OP_RIGHT] = 0x81,
			[Opcode.OP_SIZE] = 0x82,
			[Opcode.OP_INVERT] = 0x83,
			[Opcode.OP_AND] = 0x84,
			[Opcode.OP_OR] = 0x85,
			[Opcode.OP_XOR] = 0x86,
			[Opcode.OP_EQUAL] = 0x87,
			[Opcode.OP_EQUALVERIFY] = 0x88,
			[Opcode.OP_RESERVED1] = 0x89,
			[Opcode.OP_RESERVED2] = 0x8a,
			[Opcode.OP_1ADD] = 0x8b,
			[Opcode.OP_1SUB] = 0x8c,
			[Opcode.OP_2MUL] = 0x8d,
			[Opcode.OP_2DIV] = 0x8e,
			[Opcode.OP_NEGATE] = 0x8f,
			[Opcode.OP_ABS] = 0x90,
			[Opcode.OP_NOT] = 0x91,
			[Opcode.OP_0NOTEQUAL] = 0x92,
			[Opcode.OP_ADD] = 0x93,
			[Opcode.OP_SUB] = 0x94,
			[Opcode.OP_MUL] = 0x95,
			[Opcode.OP_DIV] = 0x96,
			[Opcode.OP_MOD] = 0x97,
			[Opcode.OP_LSHIFT] = 0x98,
			[Opcode.OP_RSHIFT] = 0x99,
			[Opcode.OP_BOOLAND] = 0x9a,
			[Opcode.OP_BOOLOR] = 0x9b,
			[Opcode.OP_NUMEQUAL] = 0x9c,
			[Opcode.OP_NUMEQUALVERIFY] = 0x9d,
			[Opcode.OP_NUMNOTEQUAL] = 0x9e,
			[Opcode.OP_LESSTHAN] = 0x9f,
			[Opcode.OP_GREATERTHAN] = 0xa0,
			[Opcode.OP_LESSTHANOREQUAL] = 0xa1,
			[Opcode.OP_GREATERTHANOREQUAL] = 0xa2,
			[Opcode.OP_MIN] = 0xa3,
			[Opcode.OP_MAX] = 0xa4,
			[Opcode.OP_WITHIN] = 0xa5,
			[Opcode.OP_RIPEMD160] = 0xa6,
			[Opcode.OP_SHA1] = 0xa7,
			[Opcode.OP_SHA256] = 0xa8,
			[Opcode.OP_HASH160] = 0xa9,
			[Opcode.OP_HASH256] = 0xaa,
			[Opcode.OP_CODESEPARATOR] = 0xab,
			[Opcode.OP_CHECKSIG] = 0xac,
			[Opcode.OP_CHECKSIGVERIFY] = 0xad,
			[Opcode.OP_CHECKMULTISIG] = 0xae,
			[Opcode.OP_CHECKMULTISIGVERIFY] = 0xaf,
			[Opcode.OP_NOP1] = 0xb0,
			[Opcode.OP_NOP2] = 0xb1,
			[Opcode.OP_NOP3] = 0xb2,
			[Opcode.OP_NOP4] = 0xb3,
			[Opcode.OP_NOP5] = 0xb4,
			[Opcode.OP_NOP6] = 0xb5,
			[Opcode.OP_NOP7] = 0xb6,
			[Opcode.OP_NOP8] = 0xb7,
			[Opcode.OP_NOP9] = 0xb8,
			[Opcode.OP_NOP10] = 0xb9,
			[Opcode.OP_INVALIDOPERATION] = 0xff
		};

		private static readonly Dictionary<Opcode, string> Opcode2Mnemo = new Dictionary<Opcode, string>
		{
			[Opcode.OP_0] = "OP_0",
			[Opcode.OP_FALSE] = "OP_FALSE",
			[Opcode.OP_PUSHDATA1] = "OP_PUSHDATA1",
			[Opcode.OP_PUSHDATA2] = "OP_PUSHDATA2",
			[Opcode.OP_PUSHDATA4] = "OP_PUSHDATA4",
			[Opcode.OP_1NEGATE] = "OP_1NEGATE",
			[Opcode.OP_RESERVED] = "OP_RESERVED",
			[Opcode.OP_TRUE] = "OP_TRUE",
			[Opcode.OP_1] = "OP_1",
			[Opcode.OP_2] = "OP_2",
			[Opcode.OP_3] = "OP_3",
			[Opcode.OP_4] = "OP_4",
			[Opcode.OP_5] = "OP_5",
			[Opcode.OP_6] = "OP_6",
			[Opcode.OP_7] = "OP_7",
			[Opcode.OP_8] = "OP_8",
			[Opcode.OP_9] = "OP_9",
			[Opcode.OP_10] = "OP_10",
			[Opcode.OP_11] = "OP_11",
			[Opcode.OP_12] = "OP_12",
			[Opcode.OP_13] = "OP_13",
			[Opcode.OP_14] = "OP_14",
			[Opcode.OP_15] = "OP_15",
			[Opcode.OP_16] = "OP_16",
			[Opcode.OP_NOP] = "OP_NOP",
			[Opcode.OP_VER] = "OP_VER",
			[Opcode.OP_IF] = "OP_IF",
			[Opcode.OP_NOTIF] = "OP_NOTIF",
			[Opcode.OP_VERIF] = "OP_VERIF",
			[Opcode.OP_VERNOTIF] = "OP_VERNOTIF",
			[Opcode.OP_ELSE] = "OP_ELSE",
			[Opcode.OP_ENDIF] = "OP_ENDIF",
			[Opcode.OP_VERIFY] = "OP_VERIFY",
			[Opcode.OP_RETURN] = "OP_RETURN",
			[Opcode.OP_TOALTSTACK] = "OP_TOALTSTACK",
			[Opcode.OP_FROMALTSTACK] = "OP_FROMALTSTACK",
			[Opcode.OP_2DROP] = "OP_2DROP",
			[Opcode.OP_2DUP] = "OP_2DUP",
			[Opcode.OP_3DUP] = "OP_3DUP",
			[Opcode.OP_2OVER] = "OP_2OVER",
			[Opcode.OP_2ROT] = "OP_2ROT",
			[Opcode.OP_2SWAP] = "OP_2SWAP",
			[Opcode.OP_IFDUP] = "OP_IFDUP",
			[Opcode.OP_DEPTH] = "OP_DEPTH",
			[Opcode.OP_DROP] = "OP_DROP",
			[Opcode.OP_DUP] = "OP_DUP",
			[Opcode.OP_NIP] = "OP_NIP",
			[Opcode.OP_OVER] = "OP_OVER",
			[Opcode.OP_PICK] = "OP_PICK",
			[Opcode.OP_ROLL] = "OP_ROLL",
			[Opcode.OP_ROT] = "OP_ROT",
			[Opcode.OP_SWAP] = "OP_SWAP",
			[Opcode.OP_TUCK] = "OP_TUCK",
			[Opcode.OP_CAT] = "OP_CAT",
			[Opcode.OP_SUBSTR] = "OP_SUBSTR",
			[Opcode.OP_LEFT] = "OP_LEFT",
			[Opcode.OP_RIGHT] = "OP_RIGHT",
			[Opcode.OP_SIZE] = "OP_SIZE",
			[Opcode.OP_INVERT] = "OP_INVERT",
			[Opcode.OP_AND] = "OP_AND",
			[Opcode.OP_OR] = "OP_OR",
			[Opcode.OP_XOR] = "OP_XOR",
			[Opcode.OP_EQUAL] = "OP_EQUAL",
			[Opcode.OP_EQUALVERIFY] = "OP_EQUALVERIFY",
			[Opcode.OP_RESERVED1] = "OP_RESERVED1",
			[Opcode.OP_RESERVED2] = "OP_RESERVED2",
			[Opcode.OP_1ADD] = "OP_1ADD",
			[Opcode.OP_1SUB] = "OP_1SUB",
			[Opcode.OP_2MUL] = "OP_2MUL",
			[Opcode.OP_2DIV] = "OP_2DIV",
			[Opcode.OP_NEGATE] = "OP_NEGATE",
			[Opcode.OP_ABS] = "OP_ABS",
			[Opcode.OP_NOT] = "OP_NOT",
			[Opcode.OP_0NOTEQUAL] = "OP_0NOTEQUAL",
			[Opcode.OP_ADD] = "OP_ADD",
			[Opcode.OP_SUB] = "OP_SUB",
			[Opcode.OP_MUL] = "OP_MUL",
			[Opcode.OP_DIV] = "OP_DIV",
			[Opcode.OP_MOD] = "OP_MOD",
			[Opcode.OP_LSHIFT] = "OP_LSHIFT",
			[Opcode.OP_RSHIFT] = "OP_RSHIFT",
			[Opcode.OP_BOOLAND] = "OP_BOOLAND",
			[Opcode.OP_BOOLOR] = "OP_BOOLOR",
			[Opcode.OP_NUMEQUAL] = "OP_NUMEQUAL",
			[Opcode.OP_NUMEQUALVERIFY] = "OP_NUMEQUALVERIFY",
			[Opcode.OP_NUMNOTEQUAL] = "OP_NUMNOTEQUAL",
			[Opcode.OP_LESSTHAN] = "OP_LESSTHAN",
			[Opcode.OP_GREATERTHAN] = "OP_GREATERTHAN",
			[Opcode.OP_LESSTHANOREQUAL] = "OP_LESSTHANOREQUAL",
			[Opcode.OP_GREATERTHANOREQUAL] = "OP_GREATERTHANOREQUAL",
			[Opcode.OP_MIN] = "OP_MIN",
			[Opcode.OP_MAX] = "OP_MAX",
			[Opcode.OP_WITHIN] = "OP_WITHIN",
			[Opcode.OP_RIPEMD160] = "OP_RIPEMD160",
			[Opcode.OP_SHA1] = "OP_SHA1",
			[Opcode.OP_SHA256] = "OP_SHA256",
			[Opcode.OP_HASH160] = "OP_HASH160",
			[Opcode.OP_HASH256] = "OP_HASH256",
			[Opcode.OP_CODESEPARATOR] = "OP_CODESEPARATOR",
			[Opcode.OP_CHECKSIG] = "OP_CHECKSIG",
			[Opcode.OP_CHECKSIGVERIFY] = "OP_CHECKSIGVERIFY",
			[Opcode.OP_CHECKMULTISIG] = "OP_CHECKMULTISIG",
			[Opcode.OP_CHECKMULTISIGVERIFY] = "OP_CHECKMULTISIGVERIFY",
			[Opcode.OP_NOP1] = "OP_NOP1",
			[Opcode.OP_NOP2] = "OP_NOP2",
			[Opcode.OP_NOP3] = "OP_NOP3",
			[Opcode.OP_NOP4] = "OP_NOP4",
			[Opcode.OP_NOP5] = "OP_NOP5",
			[Opcode.OP_NOP6] = "OP_NOP6",
			[Opcode.OP_NOP7] = "OP_NOP7",
			[Opcode.OP_NOP8] = "OP_NOP8",
			[Opcode.OP_NOP9] = "OP_NOP9",
			[Opcode.OP_NOP10] = "OP_NOP10",
			[Opcode.OP_INVALIDOPERATION] = "INVALID"
		};

		private static readonly Dictionary<string, Opcode> Mnemo2Opcode = new Dictionary<string, Opcode>
		{
			["OP_0"] = Opcode.OP_0,
			["OP_FALSE"] = Opcode.OP_FALSE,
			["OP_PUSHDATA1"] = Opcode.OP_PUSHDATA1,
			["OP_PUSHDATA2"] = Opcode.OP_PUSHDATA2,
			["OP_PUSHDATA4"] = Opcode.OP_PUSHDATA4,
			["OP_1NEGATE"] = Opcode.OP_1NEGATE,
			["OP_RESERVED"] = Opcode.OP_RESERVED,
			["OP_TRUE"] = Opcode.OP_TRUE,
			["OP_1"] = Opcode.OP_1,
			["OP_2"] = Opcode.OP_2,
			["OP_3"] = Opcode.OP_3,
			["OP_4"] = Opcode.OP_4,
			["OP_5"] = Opcode.OP_5,
			["OP_6"] = Opcode.OP_6,
			["OP_7"] = Opcode.OP_7,
			["OP_8"] = Opcode.OP_8,
			["OP_9"] = Opcode.OP_9,
			["OP_10"] = Opcode.OP_10,
			["OP_11"] = Opcode.OP_11,
			["OP_12"] = Opcode.OP_12,
			["OP_13"] = Opcode.OP_13,
			["OP_14"] = Opcode.OP_14,
			["OP_15"] = Opcode.OP_15,
			["OP_16"] = Opcode.OP_16,
			["OP_NOP"] = Opcode.OP_NOP,
			["OP_VER"] = Opcode.OP_VER,
			["OP_IF"] = Opcode.OP_IF,
			["OP_NOTIF"] = Opcode.OP_NOTIF,
			["OP_VERIF"] = Opcode.OP_VERIF,
			["OP_VERNOTIF"] = Opcode.OP_VERNOTIF,
			["OP_ELSE"] = Opcode.OP_ELSE,
			["OP_ENDIF"] = Opcode.OP_ENDIF,
			["OP_VERIFY"] = Opcode.OP_VERIFY,
			["OP_RETURN"] = Opcode.OP_RETURN,
			["OP_TOALTSTACK"] = Opcode.OP_TOALTSTACK,
			["OP_FROMALTSTACK"] = Opcode.OP_FROMALTSTACK,
			["OP_2DROP"] = Opcode.OP_2DROP,
			["OP_2DUP"] = Opcode.OP_2DUP,
			["OP_3DUP"] = Opcode.OP_3DUP,
			["OP_2OVER"] = Opcode.OP_2OVER,
			["OP_2ROT"] = Opcode.OP_2ROT,
			["OP_2SWAP"] = Opcode.OP_2SWAP,
			["OP_IFDUP"] = Opcode.OP_IFDUP,
			["OP_DEPTH"] = Opcode.OP_DEPTH,
			["OP_DROP"] = Opcode.OP_DROP,
			["OP_DUP"] = Opcode.OP_DUP,
			["OP_NIP"] = Opcode.OP_NIP,
			["OP_OVER"] = Opcode.OP_OVER,
			["OP_PICK"] = Opcode.OP_PICK,
			["OP_ROLL"] = Opcode.OP_ROLL,
			["OP_ROT"] = Opcode.OP_ROT,
			["OP_SWAP"] = Opcode.OP_SWAP,
			["OP_TUCK"] = Opcode.OP_TUCK,
			["OP_CAT"] = Opcode.OP_CAT,
			["OP_SUBSTR"] = Opcode.OP_SUBSTR,
			["OP_LEFT"] = Opcode.OP_LEFT,
			["OP_RIGHT"] = Opcode.OP_RIGHT,
			["OP_SIZE"] = Opcode.OP_SIZE,
			["OP_INVERT"] = Opcode.OP_INVERT,
			["OP_AND"] = Opcode.OP_AND,
			["OP_OR"] = Opcode.OP_OR,
			["OP_XOR"] = Opcode.OP_XOR,
			["OP_EQUAL"] = Opcode.OP_EQUAL,
			["OP_EQUALVERIFY"] = Opcode.OP_EQUALVERIFY,
			["OP_RESERVED1"] = Opcode.OP_RESERVED1,
			["OP_RESERVED2"] = Opcode.OP_RESERVED2,
			["OP_1ADD"] = Opcode.OP_1ADD,
			["OP_1SUB"] = Opcode.OP_1SUB,
			["OP_2MUL"] = Opcode.OP_2MUL,
			["OP_2DIV"] = Opcode.OP_2DIV,
			["OP_NEGATE"] = Opcode.OP_NEGATE,
			["OP_ABS"] = Opcode.OP_ABS,
			["OP_NOT"] = Opcode.OP_NOT,
			["OP_0NOTEQUAL"] = Opcode.OP_0NOTEQUAL,
			["OP_ADD"] = Opcode.OP_ADD,
			["OP_SUB"] = Opcode.OP_SUB,
			["OP_MUL"] = Opcode.OP_MUL,
			["OP_DIV"] = Opcode.OP_DIV,
			["OP_MOD"] = Opcode.OP_MOD,
			["OP_LSHIFT"] = Opcode.OP_LSHIFT,
			["OP_RSHIFT"] = Opcode.OP_RSHIFT,
			["OP_BOOLAND"] = Opcode.OP_BOOLAND,
			["OP_BOOLOR"] = Opcode.OP_BOOLOR,
			["OP_NUMEQUAL"] = Opcode.OP_NUMEQUAL,
			["OP_NUMEQUALVERIFY"] = Opcode.OP_NUMEQUALVERIFY,
			["OP_NUMNOTEQUAL"] = Opcode.OP_NUMNOTEQUAL,
			["OP_LESSTHAN"] = Opcode.OP_LESSTHAN,
			["OP_GREATERTHAN"] = Opcode.OP_GREATERTHAN,
			["OP_LESSTHANOREQUAL"] = Opcode.OP_LESSTHANOREQUAL,
			["OP_GREATERTHANOREQUAL"] = Opcode.OP_GREATERTHANOREQUAL,
			["OP_MIN"] = Opcode.OP_MIN,
			["OP_MAX"] = Opcode.OP_MAX,
			["OP_WITHIN"] = Opcode.OP_WITHIN,
			["OP_RIPEMD160"] = Opcode.OP_RIPEMD160,
			["OP_SHA1"] = Opcode.OP_SHA1,
			["OP_SHA256"] = Opcode.OP_SHA256,
			["OP_HASH160"] = Opcode.OP_HASH160,
			["OP_HASH256"] = Opcode.OP_HASH256,
			["OP_CODESEPARATOR"] = Opcode.OP_CODESEPARATOR,
			["OP_CHECKSIG"] = Opcode.OP_CHECKSIG,
			["OP_CHECKSIGVERIFY"] = Opcode.OP_CHECKSIGVERIFY,
			["OP_CHECKMULTISIG"] = Opcode.OP_CHECKMULTISIG,
			["OP_CHECKMULTISIGVERIFY"] = Opcode.OP_CHECKMULTISIGVERIFY,
			["OP_NOP1"] = Opcode.OP_NOP1,
			["OP_NOP2"] = Opcode.OP_NOP2,
			["OP_NOP3"] = Opcode.OP_NOP3,
			["OP_NOP4"] = Opcode.OP_NOP4,
			["OP_NOP5"] = Opcode.OP_NOP5,
			["OP_NOP6"] = Opcode.OP_NOP6,
			["OP_NOP7"] = Opcode.OP_NOP7,
			["OP_NOP8"] = Opcode.OP_NOP8,
			["OP_NOP9"] = Opcode.OP_NOP9,
			["OP_NOP10"] = Opcode.OP_NOP10,
			["OP_INVALIDOPERATION"] = Opcode.OP_INVALIDOPERATION
		};
		#endregion

		public Opcode GetOpcode(byte code)
		{
			return Code2Opcode[code];
		}

		public byte Code { get; set; }
		public byte[] Data { get; set; }
		public Opcode Opcode => (Opcode)Code;

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

			var str = string.Empty;
			if (Opcode == Opcode.OP_PUSHDATA1 || Opcode == Opcode.OP_PUSHDATA2 || Opcode == Opcode.OP_PUSHDATA4)
			{
				str = opcodeName;
			}
			if (Data.Length > 0)
			{
				str += " " + Encoders.Hex.Encode(Data);
			}
			return str;
		}

		internal static Op Push(byte[] data)
		{
			var len = (ulong)data.Length;
			var op = Opcode.OP_INVALIDOPERATION;

			if(len == 0) 
				op = Opcode.OP_0;

			var data0 = data[0];

			if( len == 1 && (data0 >= 1 && data0 <= 16) )
				op = Opcode.OP_1 + (byte)len - 1;

			else if (len == 1 && data0 == 0x81)
				op = Opcode.OP_1NEGATE;

			else if (len >  (long)Opcode.OP_0 && len < (ulong)Opcode.OP_PUSHDATA1)
				op = (Opcode)len;

			else if (len <= byte.MaxValue)
				op = Opcode.OP_PUSHDATA1;

			else if (len <= ushort.MaxValue)
				op = Opcode.OP_PUSHDATA2;

			else if (len <= uint.MaxValue)
				op = Opcode.OP_PUSHDATA4;

			return new Op((byte)op, data);
		}

		public string ToHex()
		{
			var code = Code.ToString("x2");
			if (Data == null)
			{
				return code;
			}

			byte[] buf = null;
			var len = Data.Length;
			if(len > (int)Opcode.OP_0 && len < (int)Opcode.OP_PUSHDATA1)
			{
				buf = Packer.Pack("bA", len, Data);
			}
			else if (len >= (int)Opcode.OP_PUSHDATA1)
			{
				buf = Packer.Pack("bbA", Opcode.OP_PUSHDATA1, len, Data);
			}
			else if (len > byte.MaxValue)
			{
				buf = Packer.Pack("bSA", Opcode.OP_PUSHDATA2, len, Data);
			}
			else if (len >= ushort.MaxValue)
			{
				buf = Packer.Pack("bIA", Opcode.OP_PUSHDATA4, len, Data);
			}
			return Encoders.Hex.Encode(buf);
		}
	}

	public class Script
	{
		public static readonly Script Empty = new Script();

		private readonly List<Op> _items = new List<Op>();

		public static Script FromAddress(Address address)
		{
			var addressHex = Encoders.Hex.Encode(address.ToByteArray());
			return FromAsm($"OP_DUP OP_HASH160 {addressHex} OP_EQUALVERIFY OP_CHECKSIG");
		}

		public static Script FromPubKey(PublicKey publicKey)
		{
			var s = new Script();
			s.Add(Op.Push(publicKey.ToByteArray()));
			s.Add(Opcode.OP_CHECKSIG);
			return s;
		}

		public static Script FromPubKeyHash(PublicKey publicKey)
		{
			var s = new Script();
			s.Add(Op.Push(publicKey.Hash));
			s.Add(Opcode.OP_CHECKSIG);
			return s;
		}

		public static Script FromAsm(string str)
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

		public Script()
		{}

		public Script(byte[] data)
		{
			using (var reader = new BinaryReader(new MemoryStream(data)))
			{
				while (true)
				{
					Opcode opcode;
					try
					{
						opcode = (Opcode) reader.ReadByte();
					}
					catch (EndOfStreamException e)
					{
						break;
					}
					if (opcode > Opcode.OP_0 && opcode < Opcode.OP_PUSHDATA1)
					{
						Add(reader.ReadBytes((int)opcode));
					}
					else switch (opcode)
					{
						case Opcode.OP_PUSHDATA1:
						{
							var len = reader.ReadByte();
							Add(reader.ReadBytes(len));
						}
							break;
						case Opcode.OP_PUSHDATA2:
						{
							var len = reader.ReadUInt16();
							Add(reader.ReadBytes(len));
						}
							break;
						case Opcode.OP_PUSHDATA4:
						{
							var len = reader.ReadUInt32();
							Add(reader.ReadBytes((int)len));
						}
							break;
						default:
							Add(opcode);
							break;
					}
				}
			}
		}

		public string ToAsm()
		{
			return string.Join(" ", _items);
		}

		public static Script FromHex(string str)
		{
			return Empty;
		}

		public string ToHex()
		{
			return string.Join("", _items.Select(x=>x.ToHex()));
		}

		public override string ToString()
		{
			return ToAsm();
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
