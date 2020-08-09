namespace Mockasin.Mocks.Router
{
	public interface IMockRouter
	{
		MockResponse Route(string verb, string route);
	}
}
