using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Copy
{
    class DemoContext : DbContext
    {
    }

    class DemoInsert
    {
        public async Task EfInsertAsync<T>(IEnumerable<T> entities)
        {
            using (var context = new DemoContext())
            {
                context.AddRange(entities);
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        public async Task CopyInsertAsync<T>(IEnumerable<T> entities)
        {
            using (var context = new DemoContext())
            {
                await context.BulkCopyAsync(entities).ConfigureAwait(false);
            }
        }
    }
}