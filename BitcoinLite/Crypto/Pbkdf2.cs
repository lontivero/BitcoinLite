using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using BitcoinLite.Utils;

namespace BitcoinLite.Crypto
{
	class Pbkdf2
	{
		private readonly KeyedHashAlgorithm _hmacAlgorithm;
		private readonly int _iterations;
		private readonly byte[] _salt;
		private readonly byte[] _buffer;
		private readonly int _hashSizeInBytes;
		private uint _blockNumber;
		private int _startIndex;
		private int _endIndex;

		public Pbkdf2(KeyedHashAlgorithm hmacAlgorithm, byte[] salt, int iterations)
		{
			Ensure.NotNull(nameof(hmacAlgorithm), hmacAlgorithm);
			Ensure.NotNull(nameof(salt), salt);
			Ensure.InRange("salt.Lenght", salt.Length, 0, int.MaxValue - 4);
			Ensure.InRange(nameof(iterations), iterations, 1, int.MaxValue);

			_salt = salt;
			_iterations = iterations;
			_hmacAlgorithm = hmacAlgorithm;
			_hashSizeInBytes = _hmacAlgorithm.HashSize/8;
			_buffer = new byte[_hashSizeInBytes];
			_blockNumber = 1;
			_startIndex = 0;
			_endIndex = 0;
		}

		private byte[] ComputeBlock()
		{
			var tmp = Packer.Pack("A^I", _salt, _blockNumber++);
			tmp = _hmacAlgorithm.ComputeHash(tmp);
			var result = tmp;

			for (var i = 1; i < _iterations; i++)
			{
				tmp = _hmacAlgorithm.ComputeHash(tmp);
				result = ByteArray.Xor(tmp, 0, result, 0, tmp.Length);
			}

			return result;
		}

		public byte[] GetBytes(int count)
		{
			Ensure.InRange(nameof(count), count, 1, int.MaxValue);
			var localBuffer = new byte[count];

			var offset = 0;
			var inBuffer = _endIndex - _startIndex;
			if (inBuffer > 0)
			{
				if (count < inBuffer)
				{
					Buffer.BlockCopy(_buffer, _startIndex, localBuffer, 0, count);
					_startIndex += count;
					return localBuffer;
				}

				Buffer.BlockCopy(_buffer, _startIndex, localBuffer, 0, inBuffer);
				_startIndex = _endIndex = 0;
				offset += inBuffer;
			}

			while (offset < count)
			{
				var block = ComputeBlock();
				var remainder = count - offset;
				if (remainder <= _hashSizeInBytes)
				{
					Buffer.BlockCopy(block, 0, localBuffer, offset, remainder);
					Buffer.BlockCopy(block, remainder, _buffer, _startIndex, _hashSizeInBytes - remainder);
					_endIndex += (_hashSizeInBytes - remainder);
					return localBuffer;
				}

				Buffer.BlockCopy(block, 0, localBuffer, offset, _hashSizeInBytes);
				offset += _hashSizeInBytes;
			}
			return localBuffer;
		}
	}
}
