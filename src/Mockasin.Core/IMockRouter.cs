namespace Mockasin.Core
{
	public interface IMockRouter
	{
		MockResponse Route(string verb, string route);
	}
}
