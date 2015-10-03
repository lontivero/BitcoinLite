using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace BitcoinLite.Tests
{
	class JsonFile
	{
		public static IEnumerable<object[]> GetData(string fileName)
		{
			using (var file = File.OpenText(Path.Combine("_Resources", fileName)))
			{
				var serializer = new JsonSerializer();
				var array = (object[][])serializer.Deserialize(file, typeof (object[][]));
				return array;
			}
		}
	}
}
