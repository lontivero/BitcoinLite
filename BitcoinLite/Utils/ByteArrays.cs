using System.ComponentModel;

namespace BitcoinLite.Utils
{
	internal class ByteArray
	{
		public static byte[] Empty = new byte[0];
		public static byte[] Zero ={ 0x00 };
		public static byte[] One = { 0x01 };
		public static byte[] Two = { 0x02 };

		public static byte[] Xor(byte[] src, int srcOffset, byte[] dst, int dstOffset, int count)
		{
			for (var i = 0; i < count; i++)
			{
				dst[dstOffset + i] ^= src[srcOffset + i];
			}
			return dst;
		}
	}
}
