using System.Collections.Generic;
using Mockasin.Mocks.Endpoints;
using Mockasin.Mocks.Router;
using Xunit;

namespace Mockasin.Mocks.Test.Routing
{
	public class MockRouterTests
	{
		[Fact]
		public void GetEndpointsForPath_NullEndpoints_ReturnsEmptyArray()
		{
			// Arrange
			var root = new EndpointsRoot { Endpoints = null };

			// Act
			var result = MockRouter.GetEndpointsForPath("any/path", root);

			// Assert
			Assert.Empty(result);
		}

		[Fact]
		public void GetEndpointsForPath_EmptyEndpoints_ReturnsEmptyArray()
		{
			// Arrange
			var root = new EndpointsRoot { Endpoints = new List<Endpoint>() };

			// Act
			var result = MockRouter.GetEndpointsForPath("any/path", root);

			// Assert
			Assert.Empty(result);
		}

		[Theory]
		[InlineData(null)]
		[InlineData("")]
		[InlineData("     ")]
		[InlineData("/")]
		[InlineData("  /  ")]
		public void GetEndpointsForPath_EmptyPathVariants_ReturnsAllTopLevelEndpointsWithNoPath(string path)
		{
			// Arrange
			var expectedEndpoint1 = new Endpoint { Path = "" };
			var expectedEndpoint2 = new Endpoint { Path = null };
			var expectedEndpoint3 = new Endpoint { Path = "     " };
			var expectedEndpoint4 = new Endpoint { Path = "   /  " };
			var expectedEndpoint5 = new Endpoint { Path = " //    " };
			var root = new EndpointsRoot
			{
				Endpoints = new List<Endpoint>
				{
					new Endpoint { Path = "path" },
					expectedEndpoint1,
					new Endpoint { Path = "/something/something" },
					expectedEndpoint2,
					expectedEndpoint3,
					new Endpoint { Path = "/test/path" },
					expectedEndpoint4,
					expectedEndpoint5
				}
			};

			// Act
			var result = MockRouter.GetEndpointsForPath(path, root);

			// Assert
			Assert.Equal(5, result.Length);
			Assert.Same(expectedEndpoint1, result[0]);
			Assert.Same(expectedEndpoint2, result[1]);
			Assert.Same(expectedEndpoint3, result[2]);
			Assert.Same(expectedEndpoint4, result[3]);
			Assert.Same(expectedEndpoint5, result[4]);
		}

		[Theory]
		[InlineData("/path/to/the/endpoint")]
		[InlineData("/path/to/the/endpoint/")]
		[InlineData("path/to/the/endpoint")]
		public void GetEndpointsForPath_NestedLeafEndpoint_ReturnsMatch(string path)
		{
			// Arrange
			var root = new EndpointsRoot
			{
				Endpoints = new List<Endpoint>
				{
					CreateEndpointChain(out var endpoints, "path", "to", "the", "endpoint")
				}
			};

			// Act
			var result = MockRouter.GetEndpointsForPath(path, root);

			// Assert
			var endpoint = Assert.Single(result);
			Assert.Equal(endpoints[3], endpoint);
		}

		[Fact]
		public void GetEndpointsForPath_NestedInternalEndpoint_ReturnsMatch()
		{
			// Arrange
			var root = new EndpointsRoot
			{
				Endpoints = new List<Endpoint>
				{
					CreateEndpointChain(out var endpoints, "path", "to", "the", "endpoint")
				}
			};

			// Act
			var result = MockRouter.GetEndpointsForPath("path/to", root);

			// Assert
			var endpoint = Assert.Single(result);
			Assert.Equal(endpoints[1], endpoint);
		}

		[Fact]
		public void GetEndpointsForPath_NestedEndpointMultipleMatches_ReturnsMatch()
		{
			// Arrange
			var root = new EndpointsRoot
			{
				Endpoints = new List<Endpoint>
				{
					CreateEndpointChain(out var endpoints1, "path", "to", "the", "endpoint", "with/a/whole", "lot", "of", "others"),
					CreateEndpointChain(out var endpoints2, "path", "to", "the"),
					CreateEndpointChain(out var endpoints3, "path", "to", "the", "endpoint"),
					CreateEndpointChain(out var endpoints4, "path/to", "the", "endpoint"),
				}
			};

			// Act
			var result = MockRouter.GetEndpointsForPath("path/to/the/", root);

			// Assert
			Assert.Equal(4, result.Length);
			Assert.Equal(endpoints1[2], result[0]);
			Assert.Equal(endpoints2[2], result[1]);
			Assert.Equal(endpoints3[2], result[2]);
			Assert.Equal(endpoints4[1], result[3]);
		}

		[Fact]
		public void GetEndpointsForPath_NoMatches_ReturnsEmptyList()
		{
			// Arrange
			var root = new EndpointsRoot
			{
				Endpoints = new List<Endpoint>
				{
					CreateEndpointChain(out _, "path", "to", "the", "endpoint", "with/a/whole", "lot", "of", "others"),
					CreateEndpointChain(out _, "just/one/big/long/path/all/in/the/one"),
					CreateEndpointChain(out _, "this", "is", "a", "lot", "of", "effort"),
				}
			};

			// Act
			var result = MockRouter.GetEndpointsForPath("some/other/rando/path", root);

			// Assert
			Assert.Empty(result);
		}

		[Fact]
		public void GetEndpointsForPath_ChainOfEmptyPaths_ReturnsForTheRightEmptySegments()
		{
			// Arrange
			var root = new EndpointsRoot
			{
				Endpoints = new List<Endpoint>
				{
					CreateEndpointChain(out var endpoints1, "", "", "", "", "", "", "", ""),
					CreateEndpointChain(out _, "path", "to", "the", "endpoint", "with/a/whole", "lot", "of", "others"),
					CreateEndpointChain(out var endpoints2, "", "", "", "", "", ""),
					CreateEndpointChain(out var endpoints3, "", "", "", "", ""),
					CreateEndpointChain(out _, "this", "is", "a", "lot", "of", "effort"),
					CreateEndpointChain(out _, "just/one/big/long/path/all/in/the/one"),
					CreateEndpointChain(out var endpoints4, "", "", "", "", "", "", "", ""),
					CreateEndpointChain(out var endpoints5, "", "", "", "", ""),
				}
			};

			// Act
			var result = MockRouter.GetEndpointsForPath("////", root);

			// Assert
			Assert.Equal(5, result.Length);
			Assert.Same(endpoints1[2], result[0]);
			Assert.Same(endpoints2[2], result[1]);
			Assert.Same(endpoints3[2], result[2]);
			Assert.Same(endpoints4[2], result[3]);
			Assert.Same(endpoints5[2], result[4]);
		}



		/// <summary>
		/// Test helper method. Creates a chain of endpoints with the given paths.
		/// Each endpoint is a child of the one listed before it
		/// </summary>
		/// <returns></returns>
		private Endpoint CreateEndpointChain(out List<Endpoint> endpoints, string path, params string[] childPaths)
		{
			var root = new Endpoint { Path = path };
			endpoints = new List<Endpoint> { root };
			var current = root;

			foreach (var childPath in childPaths)
			{
				var child = new Endpoint { Path = childPath };
				endpoints.Add(child);

				current.Endpoints.Add(child);
				current = child;
			}

			return root;
		}
	}
}