using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mockasin.Mocks;
using Mockasin.Mocks.Configuration;
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
