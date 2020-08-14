using Mockasin.Mocks.Endpoints;

namespace Mockasin.Mocks.Router
{
	public interface IMockRouter
	{
		Response Route(string method, string route);
	}
}
