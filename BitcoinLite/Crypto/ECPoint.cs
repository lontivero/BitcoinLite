using System;
using System.Numerics;
using BitcoinLite.Utils;

namespace BitcoinLite.Crypto
{
	public class ECPoint : IEquatable<ECPoint>
	{
		public static readonly ECPoint Infinity = new ECPoint();
		private readonly bool _isInfinity;
		private readonly BigInteger _x;
		private readonly BigInteger _y;

		private const byte UncompressedPrefix = 0x04;     // x coord + y coord
		private const byte CompressedEvenPrefix = 0x02;   // y_bit + x coord
		private const byte CompressedOddPrefix = 0x03;    // y_bit + x coord
		private const byte InfinityPrefix = 0x00;

		private const byte UncompressedBytesLen = 65;
		private const byte CompressedBytesLen = 33;
		private const byte InfinityBytesLen = 1;

		private ECPoint()
		{
			_isInfinity = true;
		}

		public ECPoint(BigInteger x, BigInteger y)
		{
			_x = x % Secp256k1.P;
			_y = y % Secp256k1.P;
			_isInfinity = y.IsZero && x.IsZero;
		}

		public BigInteger X => _x;

		public BigInteger Y => _y;

		public bool IsInfinity => _isInfinity;

		public byte[] Encode(bool compressed)
		{
			if (IsInfinity)
				return new[]{ InfinityPrefix };

			var x = X.ToUByteArray().ToBigEndian();
			byte[] encoded;
			if (!compressed)
			{
				var y = Y.ToUByteArray().ToBigEndian();
				encoded = new byte[UncompressedBytesLen];
				encoded[0] = UncompressedPrefix;
				Buffer.BlockCopy(y, 0, encoded, 33 + (32 - y.Length), y.Length);
			}
			else
			{
				encoded = new byte[CompressedBytesLen];
				encoded[0] = Y.IsEven ? CompressedEvenPrefix : CompressedOddPrefix;
			}

			Buffer.BlockCopy(x, 0, encoded, 1 + (32 - x.Length), x.Length);
			return encoded;
		}

		internal static bool IsCanonical(byte[] encoded)
		{
			var prefix = encoded[0];
			switch (prefix)
			{
				case CompressedEvenPrefix:
				case CompressedOddPrefix:
					return encoded.Length == CompressedBytesLen;
				case UncompressedPrefix:
					return encoded.Length == UncompressedBytesLen;
			}

			return false;
		}

		public static ECPoint Decode(byte[] encoded)
		{
			if (encoded == null || encoded.Length < 1)
				throw new ArgumentNullException(nameof(encoded));

			var prefix = encoded[0];
			var isInfinity = encoded.Length == InfinityBytesLen && prefix == InfinityPrefix;
			var isCompressed = encoded.Length == CompressedBytesLen && (prefix == CompressedEvenPrefix || prefix == CompressedOddPrefix);
			var isUncompressed = encoded.Length == UncompressedBytesLen && prefix == UncompressedPrefix;

			var isValid = isInfinity || isCompressed || isUncompressed;
			if(!isValid)
				throw new FormatException("Invalid encoded point");

			if (isInfinity)
				return Infinity;

			var unsigned = encoded.SafeSubarray(1, 32);
			var x = unsigned.ToBigIntegerUnsigned(true);
			BigInteger y;

			if (isUncompressed) 
			{
				Buffer.BlockCopy(encoded, 33, unsigned, 0, 32);
				y = unsigned.ToBigIntegerUnsigned(true);
			}
			else 
			{
				y = ((x * x * x + 7) % Secp256k1.P).ShanksSqrt(Secp256k1.P);

				if (y.IsEven ^ prefix == CompressedEvenPrefix)
					y = -y + Secp256k1.P; 
			}
			return new ECPoint(x, y);
		}

		public static ECPoint operator -(ECPoint p)
		{
			return new ECPoint(p._x, p._y + Secp256k1.P);
		}

		public static ECPoint operator -(ECPoint a, ECPoint b)
		{
			return a + (-b);
		}

		public static ECPoint operator +(ECPoint p, ECPoint q)
		{
			if(p.IsInfinity) return q;
			if(q.IsInfinity) return p;

			var dx = q.X - p.X;
			var dy = q.Y - p.Y;

			BigInteger C;

			if (dx == 0)
			{
				if(dy != 0) return Infinity;
				C = 3 * p.X * p.X * (2 * p.Y).ModInverse(Secp256k1.P);
			}
			else
			{
				if (dx.Sign < 0) dx += Secp256k1.P;
				C = dy * dx.ModInverse(Secp256k1.P);
			}

			C %= Secp256k1.P;
			var x3 = (C * C - p.X - q.X) % Secp256k1.P;
			var y3 = (C * (p.X - x3) - p.Y) % Secp256k1.P;

			if (x3.Sign < 0) x3 += Secp256k1.P;
			if (y3.Sign < 0) y3 += Secp256k1.P;

			return new ECPoint(x3, y3);
		}

		public static ECPoint operator * (ECPoint p, BigInteger n)
		{
			return n * p;
		}

		public static ECPoint operator *(BigInteger n, ECPoint p)
		{
			n = BigInteger.Abs(n);
			var result = Infinity;
			ECPoint temp = null;

			do
			{
				temp = temp == null ? p :  temp + temp;

				if (!n.IsEven)
					result += temp;
			} while ((n >>= 1) != 0);

			return result;
		}

		public bool IsInCurve()
		{
			// y2 = x3 + ax + b
			return (_y*_y) % Secp256k1.P == (_x*_x*_x + Secp256k1.a * _x + Secp256k1.b) % Secp256k1.P;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
				return false;

			return Equals((ECPoint)obj);
		}

		public bool Equals(ECPoint other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return _isInfinity.Equals(other._isInfinity) && _x.Equals(other._x) && _y.Equals(other._y);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = _isInfinity.GetHashCode();
				hashCode = (hashCode * 397) ^ _x.GetHashCode();
				hashCode = (hashCode * 397) ^ _y.GetHashCode();
				return hashCode;
			}
		}

		public static bool operator ==(ECPoint left, ECPoint right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(ECPoint left, ECPoint right)
		{
			return !Equals(left, right);
		}

		public override string ToString()
		{
			return $"X={_x.ToString("x2")}, Y={_y.ToString("x2")}";
		}
	}
}