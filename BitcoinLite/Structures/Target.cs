using System.Globalization;
using System.Numerics;
using BitcoinLite.Crypto;
using BitcoinLite.Encoding;

namespace BitcoinLite.Structures
{
	public class Target
	{
		public static readonly BigInteger MaxValue = Encoders.Hex.Decode("00000000FFFF0000000000000000000000000000000000000000000000000000").ToBigIntegerUnsigned(true);
		private static readonly BigInteger ScalingValue = 1000000000000;
		private readonly int _bits;

		public Target(int bits)
		{
			_bits = bits;
		}

		public double Difficulty
		{
			get 
			{
				var target = GetTargetHash();

				var diffStr = (MaxValue * ScalingValue / target).ToString(); 
				var pointPos = diffStr.Length - 12;
				var str = diffStr.Substring(0, pointPos) + "." + diffStr.Substring(pointPos);
				return double.Parse(str, CultureInfo.InvariantCulture);
			}
		}

		private BigInteger GetTargetHash()
		{
			var target = new BigInteger(_bits & 0xFFFFFF);
			target <<= 8*((_bits >> 24) - 3);
			return target;
		}

		internal uint256 AsTargetHash()
		{
			return new uint256(GetTargetHash().ToByteArray());
		}
	}
}
