using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Copy
{
    public static class DbContextExtensions
    {
        public static Task BulkCopyAsync<T>(this DbContext context, IEnumerable<T> entities)
        {
            var conn = context.Database.GetDbConnection();
            return Copy.InsertAsync(entities, (NpgsqlConnection) conn);
        }
    }
}