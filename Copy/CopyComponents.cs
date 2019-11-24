using System;
using Npgsql;

namespace Copy
{
    class CopyComponents<T>
    {
        public string Sql { get; set; }
        public Action<NpgsqlBinaryImporter, T> Write { get; set; }
    }
}