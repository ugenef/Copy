using System;
using Npgsql;

namespace Copy.Internal.Models
{
    class CopyComponents<T>
    {
        public string Sql { get; set; }
        public Action<NpgsqlBinaryImporter, T> Write { get; set; }
    }
}