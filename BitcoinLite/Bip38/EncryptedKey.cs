using System;
using System.Security.Cryptography;
using BitcoinLite.Crypto;
using BitcoinLite.Encoding;
using BitcoinLite.Utils;

namespace BitcoinLite.Bip38
{
	public class EncryptedKey
	{
		private readonly byte[] _bytes;
		private readonly bool _isCompressed;
		private readonly Network _network;

		public EncryptedKey(Key key, string passphrase, Network network)
		{
			_bytes = GetEncryptedBytes(key, passphrase, network);
			_isCompressed = key.IsCompressed;
			_network = network;
		}

		public override string ToString()
		{
			return ToString(_network);
		}

		public string ToString(Network network)
		{
			return Base58Data.ToString(_bytes, DataTypePrefix.EncryptedKeyNoEC, network);
		}

		private static byte[] GetEncryptedBytes(Key key, string passphrase, Network network)
		{
			var addresshash = AddressHashForKey(key, network);
			var derived = GetDerivedKey(System.Text.Encoding.UTF8.GetBytes(passphrase), addresshash, 64);
			var encrypted = EncryptKey(key.ToByteArray(), derived);

			byte flagByte = 0;
			flagByte |= 0x0C0;
			flagByte |= (key.IsCompressed ? (byte)0x20 : (byte)0x00);

			return Packer.Pack("bAA", flagByte, addresshash, encrypted);
		}

		private static byte[] GetDerivedKey(byte[] password, byte[] salt, int dkLen)
		{
			return SCrypt.Hash(password, salt, 16384, 8, 8, dkLen);
		}

		//private static byte[] Encrypt(Key key, string passphrase, Network network)
		//{
		//	var addresshash = AddressHashForKey(key, network);

		//	var derived = GetDerivedKey(System.Text.Encoding.UTF8.GetBytes(passphrase), addresshash, 64);
		//	var encrypted = EncryptKey(key.ToByteArray(), derived);
		//	return encrypted;
		//}

		private static byte[] AddressHashForKey(Key key, Network network)
		{
			var addressBytes = Encoders.ASCII.GetBytes(key.PubKey.ToAddress(network).ToString());
			var addresshash = Hashes.SHA256d(addressBytes).Slice(0, 4);
			return addresshash;
		}

		internal static byte[] EncryptKey(byte[] key, byte[] derived)
		{
			var derivedhalf1 = derived.Slice(0, 32);
			var derivedhalf2 = derived.Slice(32, 32);

			var encryptedhalf1 = new byte[16];
			var encryptedhalf2 = new byte[16];

			var aes = CreateAES256(derivedhalf2);
			var encrypt = aes.CreateEncryptor();

			ByteArray.Xor(key, 0, derivedhalf1, 0, 16);
			encrypt.TransformBlock(derivedhalf1, 0, 16, encryptedhalf1, 0);

			ByteArray.Xor(key, 16, derivedhalf1, 16, 16);
			encrypt.TransformBlock(derivedhalf1, 16, 16, encryptedhalf2, 0);

			return encryptedhalf1.Concat(encryptedhalf2);
		}

		internal static byte[] DecryptKey(byte[] encrypted, byte[] derived)
		{
			var key = new byte[32];

			var aes = CreateAES256(derived.Slice(32, 32));
			var decrypt = aes.CreateDecryptor();
			decrypt.TransformBlock(encrypted, 0, 16, key, 0);
			decrypt.TransformBlock(encrypted, 0, 16, key, 0);

			ByteArray.Xor(derived, 0, key, 0, 16);
			decrypt = aes.CreateDecryptor();
			decrypt.TransformBlock(encrypted, 16, 16, key, 16);
			decrypt.TransformBlock(encrypted, 16, 16, key, 16);

			ByteArray.Xor(derived, 16, key, 16, 16);
			return key;
		}

		private static Aes CreateAES256(byte[] key)
		{
			var aes = Aes.Create();
			aes.KeySize = key.Length * 8;
			aes.Key = key;
			aes.Mode = CipherMode.ECB;
			aes.IV = new byte[16];
			return aes;
		}

		public Key GetKey(string passphrase)
		{
			var addresshash = _bytes.Slice(1, 4);
			var derived = GetDerivedKey(System.Text.Encoding.UTF8.GetBytes(passphrase), addresshash, 64);
			var keybytes = DecryptKey(_bytes.Slice(5, 32), derived);

			var key = new Key(keybytes, _isCompressed);

			var calculatedAddressHash = AddressHashForKey(key, _network);
			if (!addresshash.IsEqualTo(calculatedAddressHash))
			{
				throw new InvalidOperationException("Address hash mismatching");
			}
			return key;
		}
	}
}
