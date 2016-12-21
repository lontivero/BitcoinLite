using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace BitcoinLite.Tests
{
	static class StringExtensions
	{
		public static string Clean(this string s)
		{
			return s.Replace("\t", "").Replace("\n", "").Replace("\r", "").Replace(" ", "");
		}
	}

	class JsonFile
	{
		public static IEnumerable<object[]> GetData(string fileName)
		{
			var basePath = AppDomain.CurrentDomain.BaseDirectory;
			using (var file = File.OpenText(Path.Combine(basePath, "_Resources", fileName)))
			{
				var serializer = new JsonSerializer();
				var array = (object[][])serializer.Deserialize(file, typeof (object[][]));
				return array;
			}
		}
	}
}
