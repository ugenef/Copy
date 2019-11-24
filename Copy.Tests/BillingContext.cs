using Billing.Models;
using Microsoft.EntityFrameworkCore;

namespace Billing.Database.EntityFramework
{
    public class BillingContext : DbContext
    {
        public DbSet<Call> Calls { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
           
            optionsBuilder
                .UseNpgsql("Server=localhost;Port=5432;Database=billing;User ID=postgres;Password=11111111")
                .UseSnakeCaseNamingConvention();
                
        }

     

    }
}