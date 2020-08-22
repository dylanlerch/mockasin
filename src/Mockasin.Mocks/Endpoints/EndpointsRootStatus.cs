namespace Mockasin.Mocks.Endpoints
{
	public class EndpointsRootStatus
	{
		public bool IsInvalid { get => ErrorMessage is object; }

		public string ErrorMessage { get; set; }
	}
}
