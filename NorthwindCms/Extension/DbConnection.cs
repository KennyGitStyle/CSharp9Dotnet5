using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NorthwindCms.Models;
using System.IO;

namespace NorthwindCms.Extension
{
    public static class DbConnection
    {
        public static IServiceCollection AddSqliteDbConnection(this IServiceCollection services)
        {
            var connectionString = Path.Combine("..", "Northwind.db");
            
            services.AddDbContext<Northwind>(opt => 
                opt.UseSqlite($"Data Source={connectionString}"));

            return services;
        }
    }
}
