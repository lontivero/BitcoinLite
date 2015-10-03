using System;
using System.Numerics;

namespace BitcoinLite.Crypto
{
	public static class BigIntExtensions
	{
		public static BigInteger ModInverse(this BigInteger n, BigInteger p)
		{
			BigInteger x = 1;
			BigInteger y = 0;
			BigInteger a = p;
			BigInteger b = n;

			while (b != 0)
			{
				BigInteger t = b;
				BigInteger q = BigInteger.Divide(a, t);
				b = a - q * t;
				a = t;
				t = x;
				x = y - q * t;
				y = t;
			}

			if (y < 0)
				return y + p;
			//else
			return y;
		}

		public static byte[] ToUByteArray(this BigInteger i)
		{
			var bytes = i.ToByteArray();
			if (bytes[bytes.Length - 1] == 0x00)
				Array.Resize(ref bytes, bytes.Length - 1);

			return bytes;
		}

		public static byte[] ToBigEndian(this byte[] bytes)
		{
			Array.Reverse(bytes, 0, bytes.Length);
			return bytes;
		}

		public static int Order(this BigInteger b, BigInteger p)
		{
			var m = BigInteger.One;
			var e = 0;

			while (BigInteger.ModPow(b, m, p) != 1)
			{
				m <<= 1;
				e++;
			}

			return e;
		}

		public static BigInteger ToBigInteger(this byte[] bytes)
		{
			var clone = new byte[bytes.Length];
			Buffer.BlockCopy(bytes, 0, clone, 0, bytes.Length);

			return new BigInteger(clone.ToBigEndian());
		}

		public static BigInteger ToBigIntegerUnsigned(this byte[] bytes, bool bigEndian)
		{
			byte[] clone;
			if (bigEndian)
			{
				if (bytes[0] == 0x00)
					return bytes.ToBigInteger();

				clone = new byte[bytes.Length + 1];
				Buffer.BlockCopy(bytes, 0, clone, 1, bytes.Length);
				return new BigInteger(clone.ToBigEndian());
			}

			if (bytes[bytes.Length - 1] == 0x00)
				return new BigInteger(bytes);

			clone = new byte[bytes.Length + 1];
			Buffer.BlockCopy(bytes, 0, clone, 0, bytes.Length);
			return new BigInteger(clone);
		}

		public static BigInteger ShanksSqrt(this BigInteger a, BigInteger p)
		{
			var p1 = (p - 1);
			if (BigInteger.ModPow(a, p1 / 2, p) == p1)
				return -1;

			if (p % 4 == 3)
				return BigInteger.ModPow(a, (p + 1) / 4, p);

			var s = FindS(p);
			var e = FindE(p);
			var n = new BigInteger(2);

			while (BigInteger.ModPow(n, p1 / 2, p).IsOne)
				n++;

			var x = BigInteger.ModPow(a, (s + 1) / 2, p);
			var b = BigInteger.ModPow(a, s, p);
			var g = BigInteger.ModPow(n, s, p);
			var r = e;
			var m = b.Order(p);

			while (m > 0)
			{
				var rm = r - m;
				x = (x * BigInteger.ModPow(g, TwoExp(rm - 1), p)) % p;
				b = (b * BigInteger.ModPow(g, TwoExp(rm), p)) % p;
				g = BigInteger.ModPow(g, TwoExp(rm), p);
				r = m;
				m = b.Order(p);
			}

			return x;
		}

		private static BigInteger FindS(BigInteger p)
		{
			var s = p - 1;
			while (s.IsEven) s /= 2;
			return s;
		}

		private static int FindE(BigInteger p)
		{
			var s = p - 1;
			var e = 0;

			while (s.IsEven)
			{
				s >>= 2;
				e++;
			}

			return e;
		}

		private static BigInteger TwoExp(int e)
		{
			return BigInteger.One << e;
		}
	}
}