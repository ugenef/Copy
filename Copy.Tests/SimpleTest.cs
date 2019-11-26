using System;
using System.Threading.Tasks;
using Billing.Database.EntityFramework;
using Billing.Models;
using Npgsql;
using Xunit;

namespace Copy.Tests
{
    public class SimpleTest
    {
        [Fact]
        public async Task ShouldWorkAtLeast()
        {
            NpgsqlConnection.GlobalTypeMapper.MapEnum<CallType>("call_type_enum");
            using (var db = new BillingContext())
            {
                await db.BulkCopyAsync(new[] {new Call {CalledNumber = "bc"}});
            }
        }
    }
}