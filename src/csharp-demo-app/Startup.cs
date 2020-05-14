using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ScopeAgent;

namespace csharp_demo_app
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
            services.AddControllers();
            services.AddResponseCompression();
            services.AddCors(options =>
            {
                options.AddPolicy("all",
                    builder =>
                    {
                        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyMethod();
                    });
            });
            services.AddHealthChecks();
            Console.WriteLine("Applying migrations...");
            Thread.Sleep(TimeSpan.FromSeconds(5));
            using var ctx = new ImagesContext();
            try
            {
                ctx.Database.Migrate();
            }
            catch
            {
                Thread.Sleep(TimeSpan.FromSeconds(30));
                ctx.Database.Migrate();
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            Console.WriteLine("Configuring middlewares...");
            app.UseResponseCompression();
            app.UseCors("all"); 
            app.UseRouting();
            app.UseAuthorization();
            app.UseMiddleware<ErrorInjectionMiddleware>();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });
        }
    }
}