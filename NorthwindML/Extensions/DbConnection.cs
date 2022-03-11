using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NorthwindEntitiesLib;
using System.IO;

namespace NorthwindML.Extensions
{
    public static class DbConnection
    {
        public static IServiceCollection AddSqliteDbConnection(this IServiceCollection services)
        {
            string connectionString = Path.Combine("..", "Northwind.db");
            services.AddDbContext<Northwind>(opt => opt.UseSqlite($"Data Source={connectionString}"));

            return services;
        }
    }
}
