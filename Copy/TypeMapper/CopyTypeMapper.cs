using System;
using System.Collections.Generic;
using NpgsqlTypes;

namespace Copy.TypeMapper
{
    public static class CopyTypeMapper
    {
        private static readonly Dictionary<Type, CopyTypeMapping> _mappings
            = new Dictionary<Type, CopyTypeMapping>();

        public static CopyTypeMapping MapType(Type type)
        {
            if (_mappings.ContainsKey(type))
                throw new Exception($"Postgres type mapping for {type} is configured already");
            _mappings[type] = new CopyTypeMapping();
            return _mappings[type];
        }

        internal static NpgsqlDbType? GetDbType(Type type, string propertyName)
        {
            _mappings.TryGetValue(type, out var typeMapping);
            return typeMapping?.GetDbType(propertyName);
        }
    }
}