using System;

namespace BitcoinLite.Utils
{
	static class DateTimeExtensions
	{
		private static readonly DateTimeOffset Epoch = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

		public static DateTimeOffset ToDateTimeOffset(this uint unixTime)
		{
			return ToDateTimeOffset((long)unixTime);
		}

		public static DateTimeOffset ToDateTimeOffset(this long unixTime)
		{
			var span = TimeSpan.FromSeconds(unixTime);
			return Epoch + span;
		}

		public static long ToEpochTime(this DateTimeOffset date)
		{
			var utc = date.ToUniversalTime();
			if (utc < Epoch)
				throw new ArgumentOutOfRangeException("date", "The supplied datetime can't be expressed in unix timestamp");
			var result = (utc - Epoch).TotalSeconds;
			if (result > UInt32.MaxValue)
				throw new ArgumentOutOfRangeException("date", "The supplied datetime can't be expressed in unix timestamp");
			return Convert.ToInt64(result);
		}
	}
}
