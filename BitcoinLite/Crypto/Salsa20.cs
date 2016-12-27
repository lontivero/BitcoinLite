namespace BitcoinLite.Crypto
{
	public class Salsa20
	{
		public static void Compute(byte[] B)
		{
			var x0  = ((B[ 0]) | (uint)(B[ 1] << 8) | (uint)(B[ 2] << 16) | (uint)(B[ 3] << 24));
			var x1  = ((B[ 4]) | (uint)(B[ 5] << 8) | (uint)(B[ 6] << 16) | (uint)(B[ 7] << 24));
			var x2  = ((B[ 8]) | (uint)(B[ 9] << 8) | (uint)(B[10] << 16) | (uint)(B[11] << 24));
			var x3  = ((B[12]) | (uint)(B[13] << 8) | (uint)(B[14] << 16) | (uint)(B[15] << 24));
			var x4  = ((B[16]) | (uint)(B[17] << 8) | (uint)(B[18] << 16) | (uint)(B[19] << 24));
			var x5  = ((B[20]) | (uint)(B[21] << 8) | (uint)(B[22] << 16) | (uint)(B[23] << 24));
			var x6  = ((B[24]) | (uint)(B[25] << 8) | (uint)(B[26] << 16) | (uint)(B[27] << 24));
			var x7  = ((B[28]) | (uint)(B[29] << 8) | (uint)(B[30] << 16) | (uint)(B[31] << 24));
			var x8  = ((B[32]) | (uint)(B[33] << 8) | (uint)(B[34] << 16) | (uint)(B[35] << 24));
			var x9  = ((B[36]) | (uint)(B[37] << 8) | (uint)(B[38] << 16) | (uint)(B[39] << 24));
			var x10 = ((B[40]) | (uint)(B[41] << 8) | (uint)(B[42] << 16) | (uint)(B[43] << 24));
			var x11 = ((B[44]) | (uint)(B[45] << 8) | (uint)(B[46] << 16) | (uint)(B[47] << 24));
			var x12 = ((B[48]) | (uint)(B[49] << 8) | (uint)(B[50] << 16) | (uint)(B[51] << 24));
			var x13 = ((B[52]) | (uint)(B[53] << 8) | (uint)(B[54] << 16) | (uint)(B[55] << 24));
			var x14 = ((B[56]) | (uint)(B[57] << 8) | (uint)(B[58] << 16) | (uint)(B[59] << 24));
			var x15 = ((B[60]) | (uint)(B[61] << 8) | (uint)(B[62] << 16) | (uint)(B[63] << 24));
			var b0 = x0; 
			var b1 =  x1 ; 
			var b2 =  x2 ; 
			var b3 =  x3 ; 
			var b4 =  x4 ; 
			var b5 =  x5 ; 
			var b6 =  x6 ; 
			var b7 =  x7 ; 
			var b8 =  x8 ; 
			var b9 =  x9 ; 
			var b10 = x10; 
			var b11 = x11; 
			var b12 = x12; 
			var b13 = x13; 
			var b14 = x14; 
			var b15 = x15;

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

			var Bi = new uint[16];
			Bi[0]  = b0 + x0;
			Bi[1]  = b1 + x1;
			Bi[2]  = b2 + x2;
			Bi[3]  = b3 + x3;
			Bi[4]  = b4 + x4;
			Bi[5]  = b5 + x5;
			Bi[6]  = b6 + x6;
			Bi[7]  = b7 + x7;
			Bi[8]  = b8 + x8;
			Bi[9]  = b9 + x9;
			Bi[10] = b10 + x10;
			Bi[11] = b11 + x11;
			Bi[12] = b12 + x12;
			Bi[13] = b13 + x13;
			Bi[14] = b14 + x14;
			Bi[15] = b15 + x15;

			for (var i = 0; i < 16; i++)
			{
				B[i * 4 + 0] = (byte)(Bi[i] & 0xff);
				B[i * 4 + 1] = (byte)((Bi[i] >> 8) & 0xff);
				B[i * 4 + 2] = (byte)((Bi[i] >> 16) & 0xff);
				B[i * 4 + 3] = (byte)((Bi[i] >> 24) & 0xff);
			}
		}

		private static uint RotateLeft(uint a, int b)
		{
			return (a << b) | (a >> (32 - b));
		}
	}
}
