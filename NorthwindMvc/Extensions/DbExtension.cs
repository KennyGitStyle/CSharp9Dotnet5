using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NorthwindEntitiesLib;
using NorthwindMvc.Data;
using System.IO;

namespace NorthwindMvc.Extensions
{
    public static class DbExtension
    {
        public static IServiceCollection AddSqliteDbConnection(this IServiceCollection services)
        {
            string databasePath = Path.Combine("..", "Northwind.db");
            services.AddDbContext<Northwind>(opt => opt.UseSqlite($"Data Source={databasePath}"));
            return services;
        }

        public static IServiceCollection AddSqlServerDbConnection(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    config.GetConnectionString("DefaultConnection")));
            return services;
        }
    }
}
