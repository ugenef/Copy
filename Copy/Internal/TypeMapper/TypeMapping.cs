using System.Collections.Generic;
using NpgsqlTypes;

namespace Copy.Internal.TypeMapper
{
    class TypeMapping
    {
        private readonly Dictionary<string, NpgsqlDbType> _propertyMappings =
            new Dictionary<string, NpgsqlDbType>();

        public void MapProperty(string name, NpgsqlDbType dbType)
        {
            if (_propertyMappings.ContainsKey(name))
                throw new CopyException($"Property {name} is mapped already");
            _propertyMappings[name] = dbType;
        }

        public NpgsqlDbType? GetDbType(string propertyName)
        {
            _propertyMappings.TryGetValue(propertyName, out var dbType);
            if (dbType == default)
                return null;
            return dbType;
        }
    }
}