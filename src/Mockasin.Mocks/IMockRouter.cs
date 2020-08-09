namespace Mockasin.Mocks
{
	public interface IMockRouter
	{
		MockResponse Route(string verb, string route);
	}
}
