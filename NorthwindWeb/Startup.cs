using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NorthwindWeb.DbContextExtension;
using System.IO;
using System.Threading.Tasks;
using System;

namespace NorthwindWeb
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddSqliteDbContext();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            } else
            {
                app.UseHsts();
            }

            app.UseRouting();
            app.Use(async (HttpContext context, Func<Task> next) =>
            {
                var rep = context.GetEndpoint() as RouteEndpoint;
                if(rep != null)
                {
                    Console.WriteLine($"Endpoint name: {rep.DisplayName}");
                    Console.WriteLine($"Endpoint route pattern: {rep.RoutePattern.RawText}");
                }
                if(context.Request.Path == "/bonjour")
                {
                    await context.Response.WriteAsync("Bonjour Monde");
                    return;
                }
                await next();
            });
            app.UseHttpsRedirection();
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapGet("/", async context =>
                {
                    
                    await context.Response.WriteAsync("Hello World!");
                });
            });
        }
    }
}
