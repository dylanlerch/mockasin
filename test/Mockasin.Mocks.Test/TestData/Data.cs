using System.IO;

namespace Mockasin.Mocks.Test.TestData
{
	internal static class Data
	{
		internal static class Files
		{
			public const string Blank = "blank.json";
			public const string Invalid = "invalid.json";
			public const string OnlyRootJson = "onlyRootJson.json";
			public const string SingleEndpoint = "singleEndpoint.json";
		}

		private const string BasePath = "TestData";

		public static string ReadFile(string fileName)
		{
			var path = GetPath(fileName);
			return File.ReadAllText(path);
		}

		public static string GetPath(string fileName)
		{
			return Path.Join(BasePath, fileName);
		}
	}
}