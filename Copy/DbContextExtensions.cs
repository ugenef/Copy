using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Copy
{
    public static class DbContextExtensions
    {
        public static async Task BulkCopyAsync<T>(this DbContext context, IEnumerable<T> entities)
        {
            try
            {
                var copy = CopyFactory.GetCopy<T>(context);
                await copy.InsertAsync(entities).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                throw new CopyException(e);
            }
        }
    }
}