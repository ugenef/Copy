using System;
using System.Collections.Concurrent;
using NpgsqlTypes;

namespace Copy.Internal.TypeMapper
{
    static class TypeMappingStorage
    {
        private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<Type, TypeMapping>>
            _contextMappings = new ConcurrentDictionary<Type, ConcurrentDictionary<Type, TypeMapping>>();

        public static bool ContextMappingConfigured(Type contextType)
        {
            return _contextMappings.ContainsKey(contextType);
        }

        public static NpgsqlDbType? TryGetDbType(Type contextType, Type modelType, string propertyName)
        {
            _contextMappings.TryGetValue(contextType, out var modelMappings);
            TypeMapping modelMapping = null;
            modelMappings?.TryGetValue(modelType, out modelMapping);
            return modelMapping?.GetDbType(propertyName);
        }

        public static void AddMapping(Type contextType, Type modelType, string propertyName, NpgsqlDbType dbType)
        {
            var modelMappings =
                _contextMappings.GetOrAdd(contextType, new ConcurrentDictionary<Type, TypeMapping>());
            var modelMapping = modelMappings.GetOrAdd(modelType, new TypeMapping());
            modelMapping.MapProperty(propertyName, dbType);
        }
    }
}