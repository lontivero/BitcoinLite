using System;

namespace BitcoinLite
{
	internal static class Guard
	{
		internal static void NotNull(string paramName, object obj)
		{
			if (obj == null) throw new ArgumentNullException(paramName);
		}
	}
}
