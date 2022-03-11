using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NorthwindEntitiesLib;
using System.IO;

namespace NorthwindService.Extensions
{
    public static class DbContextExtension
    {
        public static IServiceCollection AddSqliteDbConnection(this IServiceCollection services)
        {
            string connectionString = Path.Combine("..", "Northwind.db");
            return services.AddDbContext<Northwind>(opt => opt.UseSqlite($"Data Source={connectionString}"));
        }
    }
}
