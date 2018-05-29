using System;
using System.Security.Cryptography;

namespace BitcoinLite.Crypto
{
	public static class Hashes
	{
		#region SHA256
		public static byte[] SHA256(byte[] data)
		{
			return SHA256(data, 0, data.Length);
		}

		public static byte[] SHA256(byte[] data, int offset, int count)
		{
			using (var sha = new SHA256Managed())
			{
				return sha.ComputeHash(data, offset, count);
			}
		}
		#endregion

		#region RIPEMD160
		public static byte[] RIPEMD160(byte[] data)
		{
			return RIPEMD160(data, 0, data.Length);
		}

		public static byte[] RIPEMD160(byte[] data, int offset, int count)
		{
			using (var ripm = new RIPEMD160Managed())
			{
				return ripm.ComputeHash(data, offset, count);
			}
		}

		#endregion

		#region Hash256
		public static byte[] Hash256(byte[] data)
		{
			return Hash256(data, 0, data.Length);
		}

		public static byte[] Hash256(byte[] data, int offset, int count)
		{
			var h = SHA256(data, offset, count);
			return SHA256(h, 0, h.Length);
		}
		#endregion

		#region Hash160
		public static byte[] Hash160(byte[] data)
		{
			return Hash160(data, 0, data.Length);
		}

		public static byte[] Hash160(byte[] data, int offset, int count)
		{
			var h = SHA256(data, offset, count);
			return RIPEMD160(h, 0, h.Length);
		}
		#endregion

		public static byte[] HMACSHA256(byte[] key, byte[] data)
		{
			using (var hmac = new HMACSHA256(key))
			{
				return hmac.ComputeHash(data);
			}
		}

		public static byte[] HMACSHA512(byte[] key, byte[] data)
		{
			using (var hmac = new HMACSHA512(key))
			{
				return hmac.ComputeHash(data);
			}
		}
	}
}
