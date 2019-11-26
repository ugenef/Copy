using Billing.Models;
using Copy;
using Microsoft.EntityFrameworkCore;
using NpgsqlTypes;

namespace Billing.Database.EntityFramework
{
    public class BillingContext : DbContext
    {
        public DbSet<Call> Calls { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseNpgsql("Server=localhost;Port=5432;Database=billing;User ID=postgres;Password=11111111")
                .UseSnakeCaseNamingConvention()
                .UseCopyTypeMapping<BillingContext>(x =>
                    x.MapType<Call>(m => m
                        .MapProperty(nameof(Call.Duration), NpgsqlDbType.Integer)
                        .MapProperty(nameof(Call.StartTime), NpgsqlDbType.Timestamp)));
        }
    }
}