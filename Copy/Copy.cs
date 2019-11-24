using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Copy.Factories;
using Npgsql;

namespace Copy
{
    class Copy<T>
    {
        private readonly string _sql;
        private readonly NpgsqlConnection _conn;
        private readonly Action<NpgsqlBinaryImporter, T> _write;

        public Copy(CopyComponents<T> components,NpgsqlConnection conn)
        {
            _conn = conn;
            _sql = components.Sql;
            _write = components.Write;
        }

        public async Task InsertAsync(IEnumerable<T> entities)
        {
            await _conn.OpenAsync().ConfigureAwait(false);
            using (var writer = _conn.BeginBinaryImport(_sql))
            {
                foreach (var entity in entities)
                    _write(writer, entity);

                await writer.CompleteAsync().ConfigureAwait(false);
            }
        }
    }
}