using System;
using BitcoinLite.Encoding;
using BitcoinLite.Utils;

namespace BitcoinLite
{
	public class uint256 : IComparable, IComparable<uint256>, IEquatable<uint256>, IBinarySerializable
	{
		public static readonly uint256 Zero = new uint256(0);
		public static readonly uint256 One = new uint256(1);
		public static readonly uint256 Two = new uint256(2);
		public static readonly uint256 MaxValue = ~Zero;

		public static readonly int Size = 256 / 8;

		private static readonly HexEncoder Encoder = new HexEncoder();

		private const int Integers = 256 / 32;
		private readonly uint[] _data = new uint[Integers];

		public uint256(uint256 b)
		{
			if(b == null)
				throw new ArgumentNullException(nameof(b));

			Array.Copy(b._data, _data, Integers);
		}

		public uint256(ulong b)
		{
			_data[0] = (uint)b;
			_data[1] = (uint)(b >> 32);
		}

		public uint256(byte[] array, int offset)
		{
			if (array == null)
				throw new ArgumentNullException(nameof(array));
			if (array.Length - offset != Size)
				throw new FormatException("the byte array should be 256 bits long");

			for (var i = 0; i < Integers; i++)
			{
				_data[i] = Packer.LittleEndian.ToUInt32(array, 4*i + offset);
			}
		}

		public uint256(byte[] array)
		: this(array, 0)
		{
			
		}

		private uint256()
		{
		}

		private uint256(uint[] array)
		{
			Array.Copy(array, _data, Integers);
		}

		public static uint256 Parse(string hex)
		{
			var bytes = Packer.BigEndian.GetBytes(Encoder.GetBytes(hex.Trim()));
			Array.Resize(ref bytes, Size);
			return new uint256(bytes);
		}

		public static bool TryParse(string hex, out uint256 result)
		{
			if(string.IsNullOrEmpty(hex))
				throw new ArgumentException("invalid null or empty string", nameof(hex));

			result = null;
			if (hex.Length != Size * 2)
				return false;

			try
			{
				result = Parse(hex);
				return true;
			}
			catch(Exception)
			{
				return false;
			}
		}

		public byte this[int index]
		{
			get
			{
				Ensure.InRange(nameof(index), index, 0, Size);

				var uintIndex = index / sizeof(uint);
				var byteIndex = index%sizeof (uint);
				var value = _data[uintIndex];
				return (byte) (value >> (byteIndex*8));
			}
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
				return false; 
			
			return this == (uint256)obj;
		}

		public bool Equals(uint256 b)
		{
			if ((object)b == null) return false;

			return this == b;
		}

		public static bool operator ==(uint256 a, uint256 b)
		{
			if (ReferenceEquals(a, b)) return true;
			if (((object)a == null) || ((object)b == null)) return false;

			return a._data.IsEqualTo(b._data);
		}

		public static bool operator <(uint256 a, uint256 b)
		{
			return Comparison(a, b) < 0;
		}

		public static bool operator >(uint256 a, uint256 b)
		{
			return Comparison(a, b) > 0;
		}

		public static bool operator <=(uint256 a, uint256 b)
		{
			return Comparison(a, b) <= 0;
		}

		public static bool operator >=(uint256 a, uint256 b)
		{
			return Comparison(a, b) >= 0;
		}

		private static int Comparison(uint256 a, uint256 b)
		{
			for (var i = Integers-1; i >= 0; i--)
			{
				if (a._data[i] < b._data[i])
					return -1;
				if (a._data[i] > b._data[i])
					return 1;
			}
			return 0;
		}

		public static bool operator !=(uint256 a, uint256 b)
		{
			return !(a == b);
		}

		public static bool operator ==(uint256 a, ulong b)
		{
			return (a == new uint256(b));
		}

		public static bool operator !=(uint256 a, ulong b)
		{
			return !(a == new uint256(b));
		}

		public static uint256 operator ^(uint256 a, uint256 b)
		{
			var data = new uint[Integers];
			for (var i = 0; i < Integers; i++)
			{
				data[i] = a._data[i] ^ b._data[i];
			}
			return new uint256(data);
		}

		public static bool operator!(uint256 a)
		{
			for (var i = 0; i < Integers; i++)
				if (a._data[i] != 0)
					return false;
			return true;
		}

		public static uint256 operator-(uint256 a, uint256 b)
		{
			return a + (-b);
		}

		public static uint256 operator+(uint256 a, uint256 b)
		{
			var result = new uint256();
			ulong carry = 0;
			for (var i = 0; i < Integers; i++)
			{
				var n = carry + a._data[i] + b._data[i];
				result._data[i] = (uint)(n & 0xffffffff);
				carry = n >> 32;
			}
			return result;
		}

		public static uint256 operator+(uint256 a, ulong b)
		{
			return a + new uint256(b);
		}

		public static implicit operator uint256(ulong value)
		{
			return new uint256(value);
		}

		public static uint256 operator &(uint256 a, uint256 b)
		{
			var n = new uint256(a);
			for(var i = 0 ; i < Integers ; i++)
				n._data[i] &= b._data[i];
			return n;
		}

		public static uint256 operator |(uint256 a, uint256 b)
		{
			var n = new uint256(a);
			for(var i = 0 ; i < Integers ; i++)
				n._data[i] |= b._data[i];
			return n;
		}

		public static uint256 operator <<(uint256 a, int shift)
		{
			var result = new uint256();
			var k = shift / 32;
			shift = shift % 32;
			for(var i = 0 ; i < Integers ; i++)
			{
				if(i + k + 1 < Integers && shift != 0)
					result._data[i + k + 1] |= (a._data[i] >> (32 - shift));
				if(i + k < Integers)
					result._data[i + k] |= (a._data[i] << shift);
			}
			return result;
		}

		public static uint256 operator >>(uint256 a, int shift)
		{
			var result = new uint256();
			var k = shift / 32;
			shift = shift % 32;
			for(var i = 0 ; i < Integers ; i++)
			{
				if(i - k - 1 >= 0 && shift != 0)
					result._data[i - k - 1] |= (a._data[i] << (32 - shift));
				if(i - k >= 0)
					result._data[i - k] |= (a._data[i] >> shift);
			}
			return result;
		}

		
		public static uint256 operator ~(uint256 a)
		{
			var b = new uint256();
			for(var i = 0 ; i < b._data.Length ; i++)
			{
				b._data[i] = ~a._data[i];
			}
			return b;
		}

		public static uint256 operator -(uint256 a)
		{
			var b = new uint256();
			for(var i = 0 ; i < b._data.Length ; i++)
			{
				b._data[i] = ~a._data[i];
			}
			b++;
			return b;
		}

		public static uint256 operator ++(uint256 a)
		{
			return a + 1;
		}

		public static uint256 operator --(uint256 a)
		{
			return a - 1;
		}
		
		public byte[] ToByteArray()
		{
			var arr = new byte[Size];
			for (var i = 0; i < Integers; i++)
			{
				Buffer.BlockCopy(Packer.LittleEndian.GetBytes(_data[i]), 0, arr, 4 * i, 4);
			}
			return arr;
		}

		public ulong GetLow64()
		{
			return _data[0] | (ulong)_data[1] << 32;
		}

		public uint GetLow32()
		{
			return _data[0];
		}

		public override int GetHashCode()
		{
			var hash = 17;
			foreach(var element in _data)
			{
				hash = hash * 31 + element.GetHashCode();
			}
			return hash;
		}

		public int CompareTo(uint256 other)
		{
			return Comparison(this, other);
		}

		public int CompareTo(object obj)
		{
			if (obj == null)
				return 1;
			if (!(obj is uint256))
			{
				throw new ArgumentException("object must be a uint256 instance");
			}
			return CompareTo((uint256)obj);
		}

		public override string ToString()
		{
			return Encoder.GetString(Packer.BigEndian.GetBytes(ToByteArray()));
		}
	}




	public class uint160 : IComparable, IComparable<uint160>, IEquatable<uint160>, IBinarySerializable
	{
		public static readonly uint160 Zero = new uint160(0);
		public static readonly uint160 One = new uint160(1);
		public static readonly uint160 Two = new uint160(2);
		public static readonly uint160 MaxValue = ~Zero;

		public static readonly int Size = 160 / 8;

		private static readonly HexEncoder Encoder = new HexEncoder();

		private const int Integers = 160 / 32;
		private readonly uint[] _data = new uint[Integers];

		public uint160(uint160 b)
		{
			if (b == null)
				throw new ArgumentNullException(nameof(b));

			Array.Copy(b._data, _data, Integers);
		}

		public uint160(ulong b)
		{
			_data[0] = (uint)b;
			_data[1] = (uint)(b >> 32);
		}


		public uint160(byte[] array, int offset)
		{
			if (array == null)
				throw new ArgumentNullException(nameof(array));
			if (array.Length - offset != Size)
				throw new FormatException("the byte array should be 160 byte long");

			for (var i = 0; i < Integers; i++)
			{
				_data[i] = Packer.LittleEndian.ToUInt32(array, 4 * i + offset);
			}
		}

		public uint160(byte[] array)
			: this(array, 0)
		{
		}

		private uint160()
		{
		}

		private uint160(uint[] array)
		{
			Array.Copy(array, _data, Integers);
		}

		public static uint160 Parse(string hex)
		{
			var bytes = Packer.BigEndian.GetBytes(Encoder.GetBytes(hex.Trim()));
			Array.Resize(ref bytes, Size);
			return new uint160(bytes);
		}

		public static bool TryParse(string hex, out uint160 result)
		{
			if (string.IsNullOrEmpty(hex))
				throw new ArgumentException("invalid null or empty string", nameof(hex));

			result = null;
			if (hex.Length != Size * 2)
				return false;

			try
			{
				result = Parse(hex);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public byte this[int index]
		{
			get
			{
				if(index < 0 || index >= Size )
					throw new IndexOutOfRangeException("index");
				var uintIndex = index/sizeof (uint);
				var byteIndex = index%sizeof (uint);
				var value = _data[uintIndex];
				return (byte) (value >> (byteIndex*8));
			}
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
				return false;

			return this == (uint160)obj;
		}

		public bool Equals(uint160 b)
		{
			if ((object)b == null) return false;

			return this == b;
		}

		public static bool operator ==(uint160 a, uint160 b)
		{
			if (ReferenceEquals(a, b)) return true;
			if (((object)a == null) || ((object)b == null)) return false;

			return a._data.IsEqualTo(b._data);
		}

		public static bool operator <(uint160 a, uint160 b)
		{
			return Comparison(a, b) < 0;
		}

		public static bool operator >(uint160 a, uint160 b)
		{
			return Comparison(a, b) > 0;
		}

		public static bool operator <=(uint160 a, uint160 b)
		{
			return Comparison(a, b) <= 0;
		}

		public static bool operator >=(uint160 a, uint160 b)
		{
			return Comparison(a, b) >= 0;
		}

		private static int Comparison(uint160 a, uint160 b)
		{
			for (var i = Integers-1; i >= 0; i--)
			{
				if (a._data[i] < b._data[i])
					return -1;
				if (a._data[i] > b._data[i])
					return 1;
			}
			return 0;
		}

		public static bool operator !=(uint160 a, uint160 b)
		{
			return !(a == b);
		}

		public static bool operator ==(uint160 a, ulong b)
		{
			return (a == new uint160(b));
		}

		public static bool operator !=(uint160 a, ulong b)
		{
			return !(a == new uint160(b));
		}

		public static uint160 operator ^(uint160 a, uint160 b)
		{
			var data = new uint[Integers];
			for(var i = 0 ; i < Integers; i++)
			{
				data[i] = a._data[i] ^ b._data[i];
			}
			return new uint160(data);
		}

		public static bool operator!(uint160 a)
		{
			for (var i = 0; i < Integers; i++)
				if (a._data[i] != 0)
					return false;
			return true;
		}

		public static uint160 operator-(uint160 a, uint160 b)
		{
			return a + (-b);
		}

		public static uint160 operator+(uint160 a, uint160 b)
		{
			var result = new uint160();
			ulong carry = 0;
			for (var i = 0; i < Integers; i++)
			{
				var n = carry + a._data[i] + b._data[i];
				result._data[i] = (uint)(n & 0xffffffff);
				carry = n >> 32;
			}
			return result;
		}

		public static uint160 operator+(uint160 a, ulong b)
		{
			return a + new uint160(b);
		}

		public static implicit operator uint160(ulong value)
		{
			return new uint160(value);
		}

		public static uint160 operator &(uint160 a, uint160 b)
		{
			var n = new uint160(a);
			for(var i = 0 ; i < Integers ; i++)
				n._data[i] &= b._data[i];
			return n;
		}

		public static uint160 operator |(uint160 a, uint160 b)
		{
			var n = new uint160(a);
			for(var i = 0 ; i < Integers ; i++)
				n._data[i] |= b._data[i];
			return n;
		}

		public static uint160 operator <<(uint160 a, int shift)
		{
			var result = new uint160();
			var k = shift / 32;
			shift = shift % 32;
			for(var i = 0 ; i < Integers ; i++)
			{
				if(i + k + 1 < Integers && shift != 0)
					result._data[i + k + 1] |= (a._data[i] >> (32 - shift));
				if(i + k < Integers)
					result._data[i + k] |= (a._data[i] << shift);
			}
			return result;
		}

		public static uint160 operator >>(uint160 a, int shift)
		{
			var result = new uint160();
			var k = shift / 32;
			shift = shift % 32;
			for(var i = 0 ; i < Integers ; i++)
			{
				if(i - k - 1 >= 0 && shift != 0)
					result._data[i - k - 1] |= (a._data[i] << (32 - shift));
				if(i - k >= 0)
					result._data[i - k] |= (a._data[i] >> shift);
			}
			return result;
		}
		
		public static uint160 operator ~(uint160 a)
		{
			var b = new uint160();
			for(var i = 0 ; i < b._data.Length ; i++)
			{
				b._data[i] = ~a._data[i];
			}
			return b;
		}

		public static uint160 operator -(uint160 a)
		{
			var b = new uint160();
			for(var i = 0 ; i < b._data.Length ; i++)
			{
				b._data[i] = ~a._data[i];
			}
			b++;
			return b;
		}

		public static uint160 operator ++(uint160 a)
		{
			return a + 1;
		}

		public static uint160 operator --(uint160 a)
		{
			return a - 1;
		}

		public byte[] ToByteArray()
		{
			var arr = new byte[Size];
			for (var i = 0; i < Integers; i++)
			{
				Buffer.BlockCopy(Packer.LittleEndian.GetBytes(_data[i]), 0, arr, 4 * i, 4);
			}
			return arr;
		}

		public ulong GetLow64()
		{
			return _data[0] | (ulong)_data[1] << 32;
		}

		public uint GetLow32()
		{
			return _data[0];
		}

		public override int GetHashCode()
		{
			var hash = 17;
			foreach(var element in _data)
			{
				hash = hash * 31 + element.GetHashCode();
			}
			return hash;
		}

		public int CompareTo(uint160 other)
		{
			return Comparison(this, other);
		}

		public int CompareTo(object obj)
		{
			if (obj == null)
				return 1;
			if (!(obj is uint160))
			{
				throw new ArgumentException("object must be a uint160 instance");
			}
			return CompareTo((uint160)obj);
		}

		public override string ToString()
		{
			return Encoder.GetString(Packer.BigEndian.GetBytes(ToByteArray()));
		}
	}
}