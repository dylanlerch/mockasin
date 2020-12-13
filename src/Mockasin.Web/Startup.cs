using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mockasin.Mocks.Configuration;
using Mockasin.Mocks.Endpoints;
using Mockasin.Mocks.Router;
using Mockasin.Mocks.Validation;
using Mockasin.Mocks.Validation.Abstractions;
using Mockasin.Web.Configuration;

namespace Mockasin.Web
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers()
				.AddJsonOptions(JsonConfiguration.Configure);
			
			services.AddSingleton<Settings>();
			services.AddSingleton<ISettings>(c => c.GetService<Settings>());
			services.AddSingleton<IMockSettings>(c => c.GetService<Settings>());
			
			services.AddSingleton<IMockRouter, MockRouter>();

			services.AddSingleton<IMockSectionValidator<Response>, ResponseValidator>();
			services.AddSingleton<IMockSectionValidator<EndpointAction>, EndpointActionValidator>();
			services.AddSingleton<IMockSectionValidator<IEndpoint>, EndpointValidator>();
			services.AddSingleton<IMockSectionValidator<EndpointsRoot>, EndpointsRootValidator>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			app.UseRouting();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
				endpoints.MapFallbackToController("Index", "Mock");
			});
		}
	}
}
