using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitcoinLite.Crypto;
using BitcoinLite.Utils;

namespace BitcoinLite
{
	public class XPrivateKey
	{
		private Key _privateKey;
		private byte[] _childCodeChain;
		public byte Depth { get; private set; }
		public uint Child { get; private set; }

		public XPrivateKey(Key childKey, byte[] childCodeChain, byte depth, uint child)
		{
			_privateKey = childKey;
			_childCodeChain = childCodeChain;
			Depth = depth;
			Child = child;
		}

		public XPrivateKey Derive(uint i)
		{
			byte[] childCodeChain;
			var childKey = Derive(i, out childCodeChain);
			return new XPrivateKey(childKey, childCodeChain, (byte) (Depth + 1), i);
		}

		private Key Derive(uint i, out byte[] childCodeChain)
		{
			var k = (i >> 31) == 0
				? ByteArray.Zero.Concat(_privateKey.ToByteArray())
				: _privateKey.PublicKey.ToByteArray();

			var l = Hashes.HMACSHA256(_childCodeChain, k.Concat(Packer.LittleEndian.GetBytes(i)));

			var ll = l.SafeSubarray(0, 32);
			var lr = l.SafeSubarray(32, 32);
			Key.CheckValidKey(ll);

			var parse256ll = ll.ToBigIntegerUnsigned(false);
			var kPar = _privateKey.ToByteArray().ToBigIntegerUnsigned(false);

			childCodeChain = lr;
			var childKey = (parse256ll + kPar) % Secp256k1.N;
			Key.CheckValidKey(childKey.ToByteArray());

			var keyBytes = childKey.ToUByteArray();
			if (keyBytes.Length < 32)
				keyBytes = new byte[32 - keyBytes.Length].Concat(keyBytes);
			return new Key(keyBytes);
		}
	}

	public class XPublicKey
	{
		private PublicKey _publicKey;
		private byte _depth;
		private uint _child;
		internal byte[] vchFingerprint = new byte[4];

	}

}
