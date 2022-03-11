using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using NorthwindEntitiesLib;
using NorthwindService.Extensions;
using NorthwindService.Repositories;
using NorthwindService.Repositories.Implementations;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;

namespace NorthwindService
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
            services.AddCors();
            services.AddSqliteDbConnection(); // Sqlite Development connection 
            services.AddControllers(opt =>
            {
                Console.WriteLine("Default output formatters:");
                foreach (IOutputFormatter formatter in opt.OutputFormatters)
                {
                    var mediaFormatter = formatter as OutputFormatter;
                    if (mediaFormatter == null)
                        Console.WriteLine($" {formatter.GetType().Name}");
                    else
                    {
                        Console.WriteLine(" {0}, Media types: {1}",
                            arg0: mediaFormatter.GetType().Name,
                            arg1: string.Join(",", mediaFormatter.SupportedMediaTypes));
                    }
                }
            })
                .AddXmlDataContractSerializerFormatters()
                .AddXmlSerializerFormatters()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "NorthwindService", Version = "v1" });
            });
            
            services.AddHealthChecks().AddDbContextCheck<Northwind>();
        }
        

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => 
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "NorthwindService v1");
                    c.SupportedSubmitMethods(new[]
                    {
                        SubmitMethod.Get, SubmitMethod.Post,
                        SubmitMethod.Put, SubmitMethod.Delete
                    });
                    
                });
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseCors(configurePolicy: opts =>
            {
                opts.WithMethods("GET", "POST", "PUT", "DELETE");
                opts.WithOrigins("https://localhost:5002"); // MVC client app...
            });

            app.UseHealthChecks(path: "/howdoyoufeel");
            app.Use(next => (context) =>
            {
                var endPoint = context.GetEndpoint();
                
                if(endPoint != null)
                    Console.WriteLine("*** Name: {0}; Route: {1}; Metadata: {2}", 
                        arg0: endPoint.DisplayName, 
                        arg1: (endPoint as RouteEndpoint)?.RoutePattern, 
                        arg2: string.Join(",", endPoint.Metadata));
                
                return next(context);
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers()
                .AllowAnonymous();
            });
        }
    }
}
