using System;
using BitcoinLite.Utils;

namespace BitcoinLite.Crypto
{
	public class Salsa20
	{
		public static void Compute(byte[] B)
		{
			var len = 16;
			var Bi = new uint[len];
			for (var i = 0; i < len; i++)
			{
				Bi[i] = Packer.LittleEndian.ToUInt32(B, i * sizeof(uint));
			}
			var x0 = Bi[0];
			var x1 = Bi[1];
			var x2 = Bi[2];
			var x3 = Bi[3];
			var x4 = Bi[4];
			var x5 = Bi[5];
			var x6 = Bi[6];
			var x7 = Bi[7];
			var x8 = Bi[8];
			var x9 = Bi[9];
			var x10 = Bi[10];
			var x11 = Bi[11];
			var x12 = Bi[12];
			var x13 = Bi[13];
			var x14 = Bi[14];
			var x15 = Bi[15];

			for (var i = 0; i < 8; i += 2)
			{
				x4 ^= RotateLeft(x0 + x12, 7);
				x8 ^= RotateLeft(x4 + x0, 9);
				x12 ^= RotateLeft(x8 + x4, 13);
				x0 ^= RotateLeft(x12 + x8, 18);

				x9 ^= RotateLeft(x5 + x1, 7);
				x13 ^= RotateLeft(x9 + x5, 9);
				x1 ^= RotateLeft(x13 + x9, 13);
				x5 ^= RotateLeft(x1 + x13, 18);

				x14 ^= RotateLeft(x10 + x6, 7);
				x2 ^= RotateLeft(x14 + x10, 9);
				x6 ^= RotateLeft(x2 + x14, 13);
				x10 ^= RotateLeft(x6 + x2, 18);

				x3 ^= RotateLeft(x15 + x11, 7);
				x7 ^= RotateLeft(x3 + x15, 9);
				x11 ^= RotateLeft(x7 + x3, 13);
				x15 ^= RotateLeft(x11 + x7, 18);
				//---------
				x1 ^= RotateLeft(x0 + x3, 7);
				x2 ^= RotateLeft(x1 + x0, 9);
				x3 ^= RotateLeft(x2 + x1, 13);
				x0 ^= RotateLeft(x3 + x2, 18);

				x6 ^= RotateLeft(x5 + x4, 7);
				x7 ^= RotateLeft(x6 + x5, 9);
				x4 ^= RotateLeft(x7 + x6, 13);
				x5 ^= RotateLeft(x4 + x7, 18);

				x11 ^= RotateLeft(x10 + x9, 7);
				x8 ^= RotateLeft(x11 + x10, 9);
				x9 ^= RotateLeft(x8 + x11, 13);
				x10 ^= RotateLeft(x9 + x8, 18);

				x12 ^= RotateLeft(x15 + x14, 7);
				x13 ^= RotateLeft(x12 + x15, 9);
				x14 ^= RotateLeft(x13 + x12, 13);
				x15 ^= RotateLeft(x14 + x13, 18);
			}

			Bi[0] += x0;
			Bi[1] += x1;
			Bi[2] += x2;
			Bi[3] += x3;
			Bi[4] += x4;
			Bi[5] += x5;
			Bi[6] += x6;
			Bi[7] += x7;
			Bi[8] += x8;
			Bi[9] += x9;
			Bi[10] += x10;
			Bi[11] += x11;
			Bi[12] += x12;
			Bi[13] += x13;
			Bi[14] += x14;
			Bi[15] += x15;
			for (var i = 0; i < len; i++)
			{
				var v = Packer.LittleEndian.GetBytes(Bi[i]);
				Buffer.BlockCopy(v, 0, B, i * sizeof(uint), sizeof(uint));
			}
		}

		private static uint RotateLeft(uint a, int b)
		{
			return (a << b) | (a >> (32 - b));
		}
	}
}
