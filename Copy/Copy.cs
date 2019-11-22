using System.Collections.Generic;
using System.Threading.Tasks;
using Copy.Factories;
using Npgsql;

namespace Copy
{
    static class Copy
    {
        public static async Task InsertAsync<T>(IEnumerable<T> entities, NpgsqlConnection conn)
        {
            await conn.OpenAsync().ConfigureAwait(false);
            using var writer = conn.BeginBinaryImport(SqlFactory.GetSql<T>());

            var write = DelegateFactory.GetDelegate<T>();
            foreach (var entity in entities)
                write(writer, entity);

            await writer.CompleteAsync().ConfigureAwait(false);
        }
    }
}