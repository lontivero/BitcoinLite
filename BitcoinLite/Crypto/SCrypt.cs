using System;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using BitcoinLite.Utils;

namespace BitcoinLite.Crypto
{
	public class SCrypt
	{
		private const int BlockSize = 128;

		// passwd    Password.
		// salt      Salt.
		// N         CPU cost parameter.
		// r         Memory cost parameter.
		// p         Parallelization parameter.
		// dkLen     Intended length of the derived key.
		public static byte[] Hash(byte[] passwd, byte[] salt, int N, int r, int p, int dkLen)
		{
			Ensure.That(nameof(N), ()=> N >= 1 || (N & (N - 1)) == 0, "N must be a power of 2 greater than 1");
			Ensure.That(nameof(N), ()=> N <= int.MaxValue / BlockSize / r, "Parameter N is too large");
			Ensure.That(nameof(N), () => r <= int.MaxValue / BlockSize / p, "Parameter r is too large");

			var pbkdf2Single = new Pbkdf2(new HMACSHA256(passwd), salt, 1);
			var B = pbkdf2Single.GetBytes(p * BlockSize * r);

			var tasks = new Task[p];
			for (var j = 0; j < p; j++)
			{
				var j1 = j;
				tasks[j] = Task.Run(() =>
				{
					Smix(B, j1*BlockSize*r, r, N);
				});
			}
			Task.WaitAll(tasks);
			
			pbkdf2Single = new Pbkdf2(new HMACSHA256(passwd), B, 1);
			return pbkdf2Single.GetBytes(dkLen);
		}

		public static void Smix(byte[] B, int Bi, int r, int N)
		{
			var XY = new byte[2 * BlockSize * r];
			var V = new byte[BlockSize * r * N];

			var xi = 0;
			var yi = BlockSize * r;

			Buffer.BlockCopy(B, Bi, XY, xi, BlockSize * r);

			for (var i = 0; i < N; i++)
			{
				Buffer.BlockCopy(XY, xi, V, i * (BlockSize * r), BlockSize * r);
				BlockMixSalsa8(XY, xi, yi, r);
			}

			for (var i = 0; i < N; i++)
			{
				var bip = xi + (2 * r - 1) * 64;
				var j = Packer.LittleEndian.ToUInt32(XY, bip) & (N - 1);
				ByteArray.Xor(V, (int)(j * (BlockSize * r)), XY, xi, BlockSize * r);
				BlockMixSalsa8(XY, xi, yi, r);
			}

			Buffer.BlockCopy(XY, xi, B, Bi, BlockSize * r);
		}

		public static void BlockMixSalsa8(byte[] BY, int Bi, int Yi, int r)
		{
			var X = new byte[64];
			Buffer.BlockCopy(BY, Bi + (2 * r - 1) * 64, X, 0, 64);

			for (var i = 0; i < 2 * r; i++)
			{
				ByteArray.Xor(BY, i*64, X, 0, 64);
				Salsa20.Compute(X);
				Buffer.BlockCopy(X, 0, BY, Yi + (i * 64), 64);
			}

			for (var i = 0; i < r; i++)
			{
				Buffer.BlockCopy(BY, Yi + (i * 2) * 64, BY, Bi + (i * 64), 64);
			}

			for (var i = 0; i < r; i++)
			{
				Buffer.BlockCopy(BY, Yi + (i * 2 + 1) * 64, BY, Bi + (i + r) * 64, 64);
			}
		}
	}
}