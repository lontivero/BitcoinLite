using BitcoinLite.Structures;
using NUnit.Framework;

namespace BitcoinLite.Tests
{
	[TestFixture(Category = "Bitcoin,Target")]
	public class TargetTest
	{
		[Test, 
		TestCase(0x1b0404cb, 16307.420938523983, TestName = "Target - Calculate Easy Block Difficulty"),
		TestCase(0x181443c4, 54256630327.88996, TestName = "Target - Calculate Hard Block Difficulty")]
		public void CalculateDifficulty(int bits, double difficulty)
		{
			var target = new Target(bits);
			Assert.That(target.Difficulty, Is.EqualTo(difficulty).Within(.00005));
		}
	}
}
