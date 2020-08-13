using System.Collections;
using System.Collections.Generic;

namespace Mockasin.Mocks.Test.Validation
{
	public static class ValidationData
	{
		public static IEnumerable<object[]> NullAndWhitespace =>
			new List<object[]>
			{
				new object[] { null },
				new object[] { "" },
				new object[] { " 	" },
				new object[] { "\t\n" }
			};

		public static IEnumerable<object[]> Whitespace =>
			new List<object[]>
			{
				new object[] { "" },
				new object[] { " 	" },
				new object[] { "\t\n" }
			};
	}
}