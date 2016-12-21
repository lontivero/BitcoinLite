using System;
using System.Diagnostics.SymbolStore;
using BitcoinLite.Utils;

namespace BitcoinLite.Math
{
	public class BigInteger
	{
		private readonly uint[] _data; 
		private uint _length;
		private int _sign;
		private int _bits = -1;

		public static readonly BigInteger MinusOne = new BigInteger(-1, ByteArray.One);
		public static readonly BigInteger Zero = new BigInteger(0);
		public static readonly BigInteger One = new BigInteger(1);
		public static readonly BigInteger Two = new BigInteger(2);
		public static readonly BigInteger Three = new BigInteger(3);
		public static readonly BigInteger Ten = new BigInteger(10);

		private bool IsNegative => _sign < 0;
		private bool IsPositive => _sign > 0;

		public bool IsZero => _sign == 0;

		#region Constructors
		private BigInteger(long n)
		{
			_data = new uint[2];
			_data[0] = (uint)(n & 0xffffffff);
			_data[1] = (uint)((n >> 32 ) & 0xffffffff);
			_length = 2;
		}

		public BigInteger(BigInteger bi)
		{
			_data = (uint[])bi._data.Clone();
			_length = bi._length;
			_sign = bi._sign;
		}

		public BigInteger(byte[] bytes)
			: this(1, bytes)
		{ }

		public BigInteger(int sign, byte[] bytes)
		{
			_sign = sign;
			if (bytes.Length == 0)
			{
				_sign = 0;
				_data = new uint[1];
				_length = 1;
				return;
			}

			if (sign<0 )//(bytes[bytes.Length - 1] & 0x80) == 0x80)
			{
				_sign = -1;
				for (var i = 0; i < bytes.Length; i++)
				{
					bytes[i] = (byte)(~bytes[i]);
				}
			}

			var remainingBytes = bytes.Length & 3;

			_length = 0;
			_data = new uint[(bytes.Length + 3) / 4];

			for (var i = bytes.Length - 1; i >= 3; i -= 4)
			{
				_data[_length++] = (uint)(bytes[i - 3] << 24) 
					| (uint)(bytes[i - 2] << 16) 
					| (uint)(bytes[i - 1] << 8) 
					| (uint)bytes[i];
			}

			switch (remainingBytes)
			{
				case 1:
					_data[_length++] = bytes[0];
					break;
				case 2:
					_data[_length++] = (uint) ((bytes[0] << 8) + bytes[1]);
					break;
				case 3:
					_data[_length++] = (uint) ((bytes[0] << 16) + (bytes[1] << 8) + bytes[2]);
					break;
			}

			FixLength();
		}

		private BigInteger(int sign, uint[] uints)
		{
			_sign = sign;
			_data = new uint[uints.Length];

			foreach (var t in uints)
				_data[_length++] = t;
		}
		#endregion

		#region Implicit cast
		public static implicit operator BigInteger(long value)
		{
			return new BigInteger(value);
		}

		public static implicit operator BigInteger(ulong value)
		{
			return new BigInteger(value);
		}

		public static implicit operator BigInteger(int value)
		{
			return new BigInteger((long) value);
		}

		public static implicit operator BigInteger(uint value)
		{
			return new BigInteger((ulong) value);
		}
		#endregion

		#region Math operators
		public static BigInteger operator +(BigInteger bi1, BigInteger bi2)
		{
			if (bi1.IsZero) return new BigInteger(bi2);
			if (bi2.IsZero) return new BigInteger(bi1);

			BigInteger longest, shortest;
			var cmp = Compare(bi1, bi2);

			if (cmp < 0)
			{
				longest = bi2;
				shortest = bi1;
			}
			else
			{
				longest = bi1;
				shortest = bi2;
			}
			var buffer = new uint[longest._length+1];

			ulong carry = 0;
			var i = 0;
			while (i < shortest._length)
			{ 
				var sum = ((ulong)longest._data[i]) + ((ulong)shortest._data[i]) + carry;
				buffer[i++] = (uint)sum;
				carry = sum >> 32;
			}

			if (carry != 0)
			{
				while (i < longest._length && carry !=0 )
				{
					var sum = (ulong)longest._data[i] + 1;
					buffer[i++] = (uint) sum;
					carry = sum >> 32;
				}

				if (carry != 0)
				{
					buffer[i] = 1;
					return new BigInteger(1, buffer);
				}
			}

			while (i < longest._length)
			{
				buffer[i] = longest._data[i++];
			}

			return new BigInteger(1, buffer);
		}

		public static BigInteger operator -(BigInteger bi1, BigInteger bi2)
		{
			if (bi1 == Zero) return -bi2;
			if (bi2 == Zero) return bi1;

			if (bi1._sign != bi2._sign)
				return bi1 + (-bi2);

			var cmp = Compare(bi1, bi2);
			if (cmp == 0) return Zero;

			BigInteger longest, shortest;
			if (cmp < 0)
			{
				longest = bi2;
				shortest = bi1;
			}
			else
			{
				longest = bi1;
				shortest = bi2;
			}

			var buffer = (uint[])longest._data.Clone();

			int carry = 0;
			var i = 0;
			while (i < shortest._length)
			{
				var dif = ((long)longest._data[i]) - ((long)shortest._data[i]) + carry;
				buffer[i++] = (uint)dif;
				carry = (int)(dif >> 63);
			}

			if (carry != 0)
			{
				buffer[i++] -= 1;
				while (buffer[i++] == 0)
				{
					buffer[i] -= 1;
				}
			}

			return new BigInteger(bi1._sign * cmp, buffer);
		}

		public BigInteger Inc()
		{
			if (this == MinusOne) return Zero;
			if (this == Zero) return One;
			if (this == One) return Two;

			var r = this + One;
			return new BigInteger(IsNegative? -1: 1, r._data);
		}

		/*
		public static BigInteger operator *(BigInteger bi1, BigInteger bi2)
		{
			var len = bi1._length + bi2._length;
			if(len > MaxLength) 
				throw new ArithmeticException("Overflow");

			bi1 = Abs(bi1);
			bi2 = Abs(bi2);

			var buffer = new uint[MaxLength];
			for (var i = 0; i < bi1._length; i++)
			{
				if (bi1._data[i] == 0) continue;

				ulong mcarry = 0;
				for (int j = 0, k = i; j < bi2._length; j++, k++)
				{
					// k = i + j
					ulong val = ((ulong) bi1._data[i]*(ulong) bi2._data[j]) +
						        (ulong) buffer[k] + mcarry;

					buffer[k] = (uint) (val & 0xffffffff);
					mcarry = val >> 32;
				}

				if (mcarry != 0)
					buffer[i + bi2._length] = (uint) mcarry;
			}

			var result = new BigInteger(buffer);
			result.FixLength();

			if (result.IsPositive)
				return bi1.Sign != bi2.Sign ? -result : result;

			if (bi1.Sign != bi2.Sign)
			{
				if (result._length == 1)
					return result;

				bool isMaxNeg = true;
				for (var i = 0; i < result._length - 1 && isMaxNeg; i++)
				{
					if (result._data[i] != 0)
						isMaxNeg = false;
				}

				if (isMaxNeg)
					return result;
			}
			throw new ArithmeticException("Multiplication overflow.");
		}

		public static BigInteger operator << (BigInteger bi1, int shift)
		{
			var result = new BigInteger(bi1);
			result._length = ShiftLeft(result._data, shift);

			return result;
		}

		private static int ShiftLeft(uint[] buffer, int shift)
		{
			var shiftAmount = 32;
			var bufLen = buffer.Length;

			while (bufLen > 1 && buffer[bufLen - 1] == 0)
				bufLen--;

			for (int count = shift; count > 0;)
			{
				if (count < shiftAmount)
					shiftAmount = count;

				ulong carry = 0;
				for (var i = 0; i < bufLen; i++)
				{
					var val = ((ulong) buffer[i]) << shiftAmount;
					val |= carry;

					buffer[i] = (uint) (val & 0xffffffff);
					carry = val >> 32;
				}

				if (carry != 0)
				{
					if (bufLen + 1 <= buffer.Length)
					{
						buffer[bufLen] = (uint) carry;
						bufLen++;
					}
				}
				count -= shiftAmount;
			}
			return bufLen;
		}

		public static BigInteger operator >>(BigInteger bi1, int shift)
		{
			var result = new BigInteger(bi1);
			result._length = ShiftRight(result._data, shift);

			if (!bi1.IsNegative) return result;

			for (int i = MaxLength - 1; i >= result._length; i--)
				result._data[i] = 0xffffffff;

			uint mask = SignMask;
			for (int i = 0; i < 32; i++)
			{
				if ((result._data[result._length - 1] & mask) != 0)
					break;

				result._data[result._length - 1] |= mask;
				mask >>= 1;
			}
			result._length = MaxLength;

			return result;
		}

		private static int ShiftRight(uint[] buffer, int shift)
		{
			var shiftAmount = 32;
			var invShift = 0;
			var bufLen = buffer.Length;

			while (bufLen > 1 && buffer[bufLen - 1] == 0)
				bufLen--;

			for (var count = shift; count > 0;)
			{
				if (count < shiftAmount)
				{
					shiftAmount = count;
					invShift = 32 - shiftAmount;
				}

				ulong carry = 0;
				for (var i = bufLen - 1; i >= 0; i--)
				{
					ulong val = ((ulong) buffer[i]) >> shiftAmount;
					val |= carry;

					carry = ((ulong) buffer[i]) << invShift & 0xffffffff;
					buffer[i] = (uint) (val);
				}

				count -= shiftAmount;
			}

			while (bufLen > 1 && buffer[bufLen - 1] == 0)
				bufLen--;

			return bufLen;
		}

		public static BigInteger operator ~(BigInteger bi)
		{
			var result = new BigInteger(bi);

			for (var i = 0; i < MaxLength; i++)
				result._data[i] = ~bi._data[i];

			result._length = MaxLength;
			result.FixLength();

			return result;
		}
		*/

		public static BigInteger operator -(BigInteger bi)
		{
			if (bi.IsZero)
				return Zero;

			return new BigInteger(-bi._sign, bi._data);
		}

		#endregion

		#region Comparison
		public static bool operator ==(BigInteger bi1, BigInteger bi2)
		{
			return bi1.Equals(bi2);
		}

		public static bool operator !=(BigInteger bi1, BigInteger bi2)
		{
			return !(bi1.Equals(bi2));
		}

		public override bool Equals(object o)
		{
			if (o == null) return false;

			var bi = (BigInteger) o;

			return Compare(this, bi) == 0;
		}

		public static int Compare(BigInteger bi1, BigInteger bi2)
		{
			if (bi1._sign < bi2._sign) return -1;
			if (bi1._sign > bi2._sign) return 1;

			uint l1 = bi1._length, l2 = bi2._length;

			while (l1 > 0 && bi1._data[l1 - 1] == 0) l1--;
			while (l2 > 0 && bi2._data[l2 - 1] == 0) l2--;

			if (l1 == 0 && l2 == 0) return 0;

			if (l1 < l2) return -1;
			if (l1 > l2) return 1;

			uint pos = l1 - 1;

			while (pos != 0 && bi1._data[pos] == bi2._data[pos]) pos--;

			if (bi1._data[pos] < bi2._data[pos])
				return -1;
			if (bi1._data[pos] > bi2._data[pos])
				return 1;
			
			return 0;
		}

		public override int GetHashCode()
		{
			return ToString().GetHashCode();
		}

		public static bool operator >(BigInteger bi1, BigInteger bi2)
		{
			if (bi1.IsNegative && bi2.IsPositive)
				return false;

			if (bi1.IsPositive && bi2.IsNegative)
				return true;

			// same sign
			int pos;
			uint len = System.Math.Max(bi1._length, bi2._length);
			for (pos = (int)len - 1; pos >= 0 && bi1._data[pos] == bi2._data[pos]; pos--) ;

			if (pos >= 0)
			{
				return bi1._data[pos] > bi2._data[pos];
			}
			return false;
		}


		public static bool operator <(BigInteger bi1, BigInteger bi2)
		{
			if (bi1.IsNegative && bi2.IsPositive)
				return true;

			if (bi1.IsPositive && bi2.IsNegative)
				return false;

			// same sign
			uint len = System.Math.Max(bi1._length, bi2._length);
			int pos;
			for (pos = (int)len - 1; pos >= 0 && bi1._data[pos] == bi2._data[pos]; pos--) ;

			if (pos >= 0)
			{
				return bi1._data[pos] < bi2._data[pos];
			}
			return false;
		}

		public static bool operator >=(BigInteger bi1, BigInteger bi2)
		{
			return bi1 == bi2 || bi1 > bi2;
		}

		public static bool operator <=(BigInteger bi1, BigInteger bi2)
		{
			return bi1 == bi2 || bi1 < bi2;
		}
		#endregion
		/*
		private static void MultiByteDivide(BigInteger bi1, BigInteger bi2,
			BigInteger outQuotient, BigInteger outRemainder)
		{
			uint[] result = new uint[MaxLength];

			int remainderLen = bi1._length + 1;
			uint[] remainder = new uint[remainderLen];

			uint mask = SignMask;
			uint val = bi2._data[bi2._length - 1];
			int shift = 0, resultPos = 0;

			while (mask != 0 && (val & mask) == 0)
			{
				shift++;
				mask >>= 1;
			}

			for (var i = 0; i < bi1._length; i++)
				remainder[i] = bi1._data[i];
			ShiftLeft(remainder, shift);
			bi2 = bi2 << shift;

			int j = remainderLen - bi2._length;
			int pos = remainderLen - 1;

			ulong firstDivisorByte = bi2._data[bi2._length - 1];
			ulong secondDivisorByte = bi2._data[bi2._length - 2];

			int divisorLen = bi2._length + 1;
			uint[] dividendPart = new uint[divisorLen];

			while (j > 0)
			{
				ulong dividend = ((ulong) remainder[pos] << 32) + remainder[pos - 1];

				ulong q_hat = dividend / firstDivisorByte;
				ulong r_hat = dividend % firstDivisorByte;

				bool done = false;
				while (!done)
				{
					done = true;

					if (q_hat == 0x100000000 ||
					    (q_hat*secondDivisorByte) > ((r_hat << 32) + remainder[pos - 2]))
					{
						q_hat--;
						r_hat += firstDivisorByte;

						if (r_hat < 0x100000000)
							done = false;
					}
				}

				for (var h = 0; h < divisorLen; h++)
					dividendPart[h] = remainder[pos - h];

				var kk = new BigInteger(dividendPart);
				var ss = bi2*(long) q_hat;

				while (ss > kk)
				{
					q_hat--;
					ss -= bi2;
				}
				var yy = kk - ss;

				for (var h = 0; h < divisorLen; h++)
					remainder[pos - h] = yy._data[bi2._length - h];

				result[resultPos++] = (uint) q_hat;

				pos--;
				j--;
			}

			outQuotient._length = resultPos;
			int y = 0;
			for (int x = outQuotient._length - 1; x >= 0; x--, y++)
				outQuotient._data[y] = result[x];
			for (; y < MaxLength; y++)
				outQuotient._data[y] = 0;

			outQuotient.FixLength();

			if (outQuotient._length == 0)
				outQuotient._length = 1;

			outRemainder._length = ShiftRight(remainder, shift);

			for (y = 0; y < outRemainder._length; y++)
				outRemainder._data[y] = remainder[y];
			for (; y < MaxLength; y++)
				outRemainder._data[y] = 0;
		}

		private static void SingleByteDivide(BigInteger bi1, BigInteger bi2,
			BigInteger outQuotient, BigInteger outRemainder)
		{
			var result = new uint[MaxLength];
			var resultPos = 0;

			// copy dividend to reminder
			for (var i = 0; i < MaxLength; i++)
				outRemainder._data[i] = bi1._data[i];
			outRemainder._length = bi1._length;

			outRemainder.FixLength();

			var divisor = (ulong) bi2._data[0];
			int pos = outRemainder._length - 1;
			var dividend = (ulong) outRemainder._data[pos];

			if (dividend >= divisor)
			{
				ulong quotient = dividend/divisor;
				result[resultPos++] = (uint) quotient;

				outRemainder._data[pos] = (uint) (dividend%divisor);
			}
			pos--;

			while (pos >= 0)
			{

				dividend = ((ulong) outRemainder._data[pos + 1] << 32) + (ulong) outRemainder._data[pos];
				ulong quotient = dividend/divisor;
				result[resultPos++] = (uint) quotient;

				outRemainder._data[pos + 1] = 0;
				outRemainder._data[pos--] = (uint) (dividend%divisor);
			}

			outQuotient._length = resultPos;
			int j = 0;
			for (int i = outQuotient._length - 1; i >= 0; i--, j++)
				outQuotient._data[j] = result[i];
			for (; j < MaxLength; j++)
				outQuotient._data[j] = 0;

			outQuotient.FixLength();

			if (outQuotient._length == 0)
				outQuotient._length = 1;

			outRemainder.FixLength();
		}

		public static BigInteger operator /(BigInteger bi1, BigInteger bi2)
		{
			var quotient = new BigInteger();
			var remainder = new BigInteger();

			int lastPos = MaxLength - 1;
			bool divisorNeg = false, dividendNeg = false;

			if (bi1.IsNegative) // bi1 negative
			{
				bi1 = -bi1;
				dividendNeg = true;
			}
			if (bi2.IsNegative) // bi2 negative
			{
				bi2 = -bi2;
				divisorNeg = true;
			}

			if (bi1 < bi2)
			{
				return quotient;
			}

			if (bi2._length == 1)
				SingleByteDivide(bi1, bi2, quotient, remainder);
			else
				MultiByteDivide(bi1, bi2, quotient, remainder);

			if (dividendNeg != divisorNeg)
				return -quotient;

			return quotient;
		}

		public static BigInteger operator %(BigInteger bi1, BigInteger bi2)
		{
			var quotient = new BigInteger();
			var remainder = new BigInteger(bi1);

			int lastPos = MaxLength - 1;
			bool dividendNeg = false;

			if (bi1.IsNegative) // bi1 negative
			{
				bi1 = -bi1;
				dividendNeg = true;
			}
			if (bi2.IsNegative) // bi2 negative
				bi2 = -bi2;

			if (bi1 < bi2)
			{
				return remainder;
			}

			if (bi2._length == 1)
				SingleByteDivide(bi1, bi2, quotient, remainder);
			else
				MultiByteDivide(bi1, bi2, quotient, remainder);

			if (dividendNeg)
				return -remainder;

			return remainder;
		}

		public static BigInteger operator &(BigInteger bi1, BigInteger bi2)
		{
			var result = new BigInteger();

			int len = System.Math.Max(bi1._length, bi2._length);

			for (int i = 0; i < len; i++)
			{
				uint sum = (uint) (bi1._data[i] & bi2._data[i]);
				result._data[i] = sum;
			}

			result._length = MaxLength;
			result.FixLength();

			return result;
		}

		public static BigInteger operator |(BigInteger bi1, BigInteger bi2)
		{
			var result = new BigInteger();

			int len = System.Math.Max(bi1._length, bi2._length);

			for (var i = 0; i < len; i++)
			{
				var sum = bi1._data[i] | bi2._data[i];
				result._data[i] = sum;
			}

			result._length = MaxLength;

			result.FixLength();

			return result;
		}

		public static BigInteger operator ^(BigInteger bi1, BigInteger bi2)
		{
			var result = new BigInteger();

			int len = System.Math.Max(bi1._length, bi2._length);

			for (int i = 0; i < len; i++)
			{
				uint sum = (uint) (bi1._data[i] ^ bi2._data[i]);
				result._data[i] = sum;
			}

			result._length = MaxLength;
			result.FixLength();

			return result;
		}
		*/
		public BigInteger Not()
		{
			return -(Inc());
		}

		public static BigInteger Max(BigInteger bi1, BigInteger bi2)
		{
			return bi1 > bi2 ? bi1: bi2;
		}

		public BigInteger Min(BigInteger bi1, BigInteger bi2)
		{
			return bi1 < bi2 ? bi1 : bi2;
		}

		public static BigInteger Abs(BigInteger bi)
		{
			return bi.IsNegative ? -bi : bi;
		}
		/*
		public override string ToString()
		{
			return ToString(10);
		}

		public string ToString(int radix)
		{
			if (radix < 2 || radix > 36)
				throw new ArgumentException("Radix must be >= 2 and <= 36");

			var charSet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
			var result = "";

			BigInteger a = new BigInteger(this);

			bool negative = false;
			if (a.IsNegative)
			{
				negative = true;
				try
				{
					a = -a;
				}
				catch (Exception)
				{
				}
			}

			var quotient = new BigInteger(0);
			var remainder = new BigInteger(0);
			var biRadix = new BigInteger(radix);

			if (a._length == 1 && a._data[0] == 0)
			{
				result = "0";
			}
			else
			{
				while (a._length > 1 || (a._length == 1 && a._data[0] != 0))
				{
					SingleByteDivide(a, biRadix, quotient, remainder);

					if (remainder._data[0] < 10)
						result = remainder._data[0] + result;
					else
						result = charSet[(int) remainder._data[0] - 10] + result;

					a = quotient;
				}
				if (negative)
					result = "-" + result;
			}

			return result;
		}

		public string ToHexString()
		{
			var result = _data[_length - 1].ToString("X");

			for (var i = _length - 2; i >= 0; i--)
			{
				result += _data[i].ToString("X8");
			}

			return result;
		}

		public BigInteger ModPow(BigInteger exp, BigInteger n)
		{
			if (exp.Sign != 0)
				throw new ArithmeticException("Positive exponents only.");

			BigInteger resultNum = 1;
			BigInteger tempNum;
			bool thisNegative = false;

			if (IsNegative) // negative this
			{
				tempNum = -this % n;
				thisNegative = true;
			}
			else
				tempNum = this % n; // ensures (tempNum * tempNum) < b^(2k)

			n = Abs(n);

			// calculate constant = b^(2k) / m
			var constant = new BigInteger();

			int i = n._length << 1;
			constant._data[i] = 0x00000001;
			constant._length = i + 1;

			constant = constant/n;
			int totalBits = exp.BitCount();
			int count = 0;

			// perform squaring and multiply exponentiation
			for (int pos = 0; pos < exp._length; pos++)
			{
				uint mask = 0x01;

				for (int index = 0; index < 32; index++)
				{
					if ((exp._data[pos] & mask) != 0)
						resultNum = BarrettReduction(resultNum*tempNum, n, constant);

					mask <<= 1;

					tempNum = BarrettReduction(tempNum*tempNum, n, constant);


					if (tempNum._length == 1 && tempNum._data[0] == 1)
					{
						return thisNegative && (exp._data[0] & 0x1) != 0 
							? -resultNum 
							: resultNum;
					}
					count++;
					if (count == totalBits)
						break;
				}
			}

			if (thisNegative && (exp._data[0] & 0x1) != 0) //odd exp
				return -resultNum;

			return resultNum;
		}

		private BigInteger BarrettReduction(BigInteger x, BigInteger n, BigInteger constant)
		{
			int k = n._length,
				kPlusOne = k + 1,
				kMinusOne = k - 1;

			var q1 = new BigInteger();

			// q1 = x / b^(k-1)
			for (int i = kMinusOne, j = 0; i < x._length; i++, j++)
				q1._data[j] = x._data[i];
			q1._length = x._length - kMinusOne;
			if (q1._length <= 0)
				q1._length = 1;


			var q2 = q1*constant;
			var q3 = new BigInteger();

			// q3 = q2 / b^(k+1)
			for (int i = kPlusOne, j = 0; i < q2._length; i++, j++)
				q3._data[j] = q2._data[i];
			q3._length = q2._length - kPlusOne;
			if (q3._length <= 0)
				q3._length = 1;

			// r1 = x mod b^(k+1)
			// i.e. keep the lowest (k+1) words
			var r1 = new BigInteger();
			int lengthToCopy = (x._length > kPlusOne) ? kPlusOne : x._length;
			for (int i = 0; i < lengthToCopy; i++)
				r1._data[i] = x._data[i];
			r1._length = lengthToCopy;

			// r2 = (q3 * n) mod b^(k+1)
			// partial multiplication of q3 and n

			var r2 = new BigInteger();
			for (int i = 0; i < q3._length; i++)
			{
				if (q3._data[i] == 0) continue;

				ulong mcarry = 0;
				int t = i;
				for (int j = 0; j < n._length && t < kPlusOne; j++, t++)
				{
					// t = i + j
					ulong val = ((ulong) q3._data[i]*(ulong) n._data[j]) +
					            (ulong) r2._data[t] + mcarry;

					r2._data[t] = (uint) (val & 0xffffffff);
					mcarry = (val >> 32);
				}

				if (t < kPlusOne)
					r2._data[t] = (uint) mcarry;
			}
			r2._length = kPlusOne;
			r2.FixLength();

			r1 -= r2;
			if ((r1._data[MaxLength - 1] & SignMask) != 0) // negative
			{
				var val = new BigInteger();
				val._data[kPlusOne] = 0x00000001;
				val._length = kPlusOne + 1;
				r1 += val;
			}

			while (r1 >= n)
				r1 -= n;

			return r1;
		}

		public BigInteger gcd(BigInteger bi)
		{
			var x = Abs(this);
			var y = Abs(bi);

			var g = y;

			while (x._length > 1 || (x._length == 1 && x._data[0] != 0))
			{
				g = x;
				x = y%x;
				y = g;
			}

			return g;
		}
		*/
		private int BitCount()
		{
			if (_bits != -1) return _bits;
			if (IsZero) return 1;

			FixLength();

			_bits = _length > 1 ? ((int)_length -1) << 5 : 0;

			uint value = _data[_length - 1];
			uint mask = 0x80000000;
			int bits = 32;

			while (bits > 0 && (value & mask) == 0)
			{
				bits--;
				mask >>= 1;
			}
			_bits += bits;
			return _bits;
		}


		/*
		public static int Jacobi(BigInteger a, BigInteger b)
		{
			// Jacobi defined only for odd integers
			if ((b._data[0] & 0x1) == 0)
				throw (new ArgumentException("Jacobi defined only for odd integers."));

			if (a >= b) a %= b;
			if (a._length == 1 && a._data[0] == 0) return 0; // a == 0
			if (a._length == 1 && a._data[0] == 1) return 1; // a == 1

			if (a < 0)
			{
				return (((b - 1)._data[0]) & 0x2) == 0 ? Jacobi(-a, b) : -Jacobi(-a, b);
			}

			int e = 0;
			for (int index = 0; index < a._length; index++)
			{
				uint mask = 0x01;

				for (int i = 0; i < 32; i++)
				{
					if ((a._data[index] & mask) != 0)
					{
						index = a._length; // to break the outer loop
						break;
					}
					mask <<= 1;
					e++;
				}
			}

			BigInteger a1 = a >> e;

			int s = 1;
			if ((e & 0x1) != 0 && ((b._data[0] & 0x7) == 3 || (b._data[0] & 0x7) == 5))
				s = -1;

			if ((b._data[0] & 0x3) == 3 && (a1._data[0] & 0x3) == 3)
				s = -s;

			return a1._length == 1 && a1._data[0] == 1 ? s : s*Jacobi(b%a1, a1);
		}

		public BigInteger ModInverse(BigInteger modulus)
		{
			BigInteger[] p = {Zero, One};
			BigInteger[] q = new BigInteger[2]; // quotients
			BigInteger[] r = { Zero, Zero }; // remainders

			int step = 0;

			BigInteger a = modulus;
			BigInteger b = this;

			while (b._length > 1 || (b._length == 1 && b._data[0] != 0))
			{
				var quotient = Zero;
				var remainder = Zero;

				if (step > 1)
				{
					BigInteger pval = (p[0] - (p[1]*q[0]))%modulus;
					p[0] = p[1];
					p[1] = pval;
				}

				if (b._length == 1)
					SingleByteDivide(a, b, quotient, remainder);
				else
					MultiByteDivide(a, b, quotient, remainder);

				q[0] = q[1];
				r[0] = r[1];
				q[1] = quotient;
				r[1] = remainder;

				a = b;
				b = remainder;

				step++;
			}

			if (r[0]._length > 1 || (r[0]._length == 1 && r[0]._data[0] != 1))
				throw (new ArithmeticException("No inverse!"));

			BigInteger result = ((p[0] - (p[1]*q[0]))%modulus);

			if (result.IsNegative)
				result += modulus; // get the least positive modulus

			return result;
		}
		*/

		private int ByteCount()
		{
			var numBits = BitCount();

			var numBytes = numBits >> 3;
			if ((numBits & 0x7) != 0)
				numBytes++;

			return numBytes;
		}

		public byte[] ToByteArray()
		{
			if (IsZero) return ByteArray.Zero;
			var numBytes = ByteCount();

			uint v0 =  _data[_length - 1];
			bool pad0 = IsPositive &&
					((v0 & 0x80000000) == 0x80000000)
				 || ((v0 & 0xff800000) == 0x00800000)
				 || ((v0 & 0xffff8000) == 0x00008000)
				 || ((v0 & 0xffffff80) == 0x00000080)
			 ;
			var npad = pad0 ? 1 : 0;

			var pos = npad;
			var len = numBytes + npad;
			var result = new byte[len];
			var remains = numBytes & 0x03;
			var bytesIndword = remains > 0 ? remains : 4;

			if (IsPositive)
			{
				for (var i = (int) _length - 1; i >= 0; i--)
				{
					var val = _data[i];
					for (var j = bytesIndword - 1; j >= 0; j--)
					{
						result[pos + j] = (byte) val;
						val >>= 8;
					}
					pos += bytesIndword;
					bytesIndword = 4;
				}
			}
			else // IsNegative
			{
				var carry = true;
				for (var i = (int)_length - 1; i >= 0; i--)
				{
					var val = ~ _data[i];
					carry = carry && (++val == uint.MinValue);

					for (var j = bytesIndword - 1; j >= 0; j--)
					{
						result[pos + j] = (byte)val;
						val >>= 8;
					}
					pos += bytesIndword;
					bytesIndword = 4;
				}
				uint lastMag = _data[0];

				if (carry)
					--lastMag;

				pos -= bytesIndword+1;
				var jj = bytesIndword;
				while (lastMag > byte.MaxValue)
				{
					result[pos+jj] = (byte)~lastMag;
					lastMag >>= 8;
					jj--;
				}

				if (pos + jj < result.Length-1)
				{
					result[0] = 0xff;
				}
			}
			return result;
		}

		/*
		public void SetBit(uint bitNum)
		{
			uint bytePos = bitNum >> 5; // divide by 32
			byte bitPos = (byte) (bitNum & 0x1F); // get the lowest 5 bits

			uint mask = (uint) 1 << bitPos;
			_data[bytePos] |= mask;

			if (bytePos >= _length)
				_length = (int) bytePos + 1;
		}


		public void UnsetBit(uint bitNum)
		{
			uint bytePos = bitNum >> 5;

			if (bytePos < _length)
			{
				byte bitPos = (byte) (bitNum & 0x1F);

				uint mask = (uint) 1 << bitPos;
				uint mask2 = 0xffffffff ^ mask;

				_data[bytePos] &= mask2;

				if (_length > 1 && _data[_length - 1] == 0)
					_length--;
			}
		}

		public BigInteger Sqrt()
		{
			uint numBits = (uint) this.BitCount();

			if ((numBits & 0x1) != 0) // odd number of bits
				numBits = (numBits >> 1) + 1;
			else
				numBits = (numBits >> 1);

			uint bytePos = numBits >> 5;
			byte bitPos = (byte) (numBits & 0x1F);

			uint mask;

			var result = Zero;
			if (bitPos == 0)
				mask = SignMask;
			else
			{
				mask = (uint) 1 << bitPos;
				bytePos++;
			}
			result._length = (int) bytePos;

			for (var i = (int) bytePos - 1; i >= 0; i--)
			{
				while (mask != 0)
				{
					// guess
					result._data[i] ^= mask;

					// undo the guess if its square is larger than this
					if ((result*result) > this)
						result._data[i] ^= mask;

					mask >>= 1;
				}
				mask = SignMask;
			}
			return result;
		}
		*/

		private void FixLength()
		{
			while (_length > 1 && _data[_length - 1] == 0)
				_length--;

			_length = System.Math.Max(1, _length);
			if (_length == 1 && _data[0] == 0)
				this._sign = 0;
		}
	}
}
