using System;
using System.Diagnostics;

namespace BitcoinLite
{
	[DebuggerStepThrough]
	public static class Ensure
	{
		internal static void NotNull(string paramName, object obj)
		{
			if (obj == null) throw new ArgumentNullException(paramName);
		}
		public static void NotNullOrEmpty(string paramName, string str)
		{
			if (string.IsNullOrEmpty(str)) throw new ArgumentNullException(paramName);
		}
		internal static void That(string paramName, Func<bool> that, string message )
		{
			if (!that()) throw new ArgumentException(message, paramName);
		}
		internal static void That(string paramName, Func<bool> that)
		{
			if (!that()) throw new ArgumentException(paramName);
		}

		public static void InRange(string paramName, int value, int min, int max)
		{
			if(value < min || value > max)
				throw new ArgumentOutOfRangeException(paramName, $"Allowed values are beetwen {min} and {max}");
		}
	}
}
