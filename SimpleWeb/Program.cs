using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace SimpleWeb
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
           {
               webBuilder.Configure(app =>
               {
                   app.Run(context => context.Response.WriteAsync("Hello World Wide Web!"));
               });
               

           }).Build();
            
            await host.RunAsync();
        }
    }
}
