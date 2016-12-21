namespace BitcoinLite.Crypto
{
	public interface ITxDestination
	{
		Script ScriptPubKey { get; }
	}
}