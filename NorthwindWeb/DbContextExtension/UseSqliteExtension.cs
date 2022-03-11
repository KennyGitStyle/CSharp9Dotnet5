using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NorthwindEntitiesLib;
using System.IO;

namespace NorthwindWeb.DbContextExtension
{
    public static class UseSqliteExtension
    {
        public static IServiceCollection AddSqliteDbContext(this IServiceCollection services)
        {
            string databasePath = Path.Combine("..", "Northwind.db");
            services.AddDbContext<Northwind>(opt =>
            {
                opt.UseSqlite($"Data Source={databasePath}");
            });

            return services;
        }
    }
}
