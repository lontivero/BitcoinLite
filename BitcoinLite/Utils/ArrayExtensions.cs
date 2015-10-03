using System;
using System.Linq;

namespace BitcoinLite.Utils
{
	internal static class ArrayExtensions
	{
		internal static T[] SafeSubarray<T>(this T[] me, int offset, int count)
		{
			var data = new T[count];
			Buffer.BlockCopy(me, offset, data, 0, count);
			return data;
		}

		internal static T[] SafeSubarray<T>(this T[] me, int offset)
		{
			var count = me.Length - offset;
			var data = new T[count];
			Buffer.BlockCopy(me, offset, data, 0, count);
			return data;
		}

		internal static T[] Concat<T>(this T[] me, params T[][] arrays)
		{
			var len = me.Length + arrays.Sum(x => x.Length);
			var buffer = new T[len];
			Array.Copy(me, 0, buffer, 0, me.Length);

			var pos = me.Length;
			foreach (var arr in arrays)
			{
				Array.Copy(arr, 0, buffer, pos, arr.Length);
				pos += arr.Length;
			}
			return buffer;
		}

		internal static bool IsEqualTo<T>(this T[] me, T[] other)
		{
			if(ReferenceEquals(me, other)) return true;
			if(me == null ^ other == null) return false;
			return me.SequenceEqual(other);
		}
	}
}
