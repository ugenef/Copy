using System.Reflection;
using NpgsqlTypes;

namespace Copy
{
    class EmitInfo
    {
        public string PostgresName { get; set; }
        public NpgsqlDbType? PostgresType { get; set; }
        public MethodInfo Getter { get; set; }
        public MethodInfo WriteMethod { get; set; }
    }
}