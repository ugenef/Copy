using System;
using Copy.Internal.TypeMapper;
using NpgsqlTypes;

namespace Copy.MappingBuilder
{
    public class PropertyMappingBuilder<TModel>
    {
        internal Type ContextType { get; set; }

        public PropertyMappingBuilder<TModel> MapProperty(string propertyName, NpgsqlDbType dbType)
        {
            var modelType = typeof(TModel);
            TypeMappingStorage.AddMapping(ContextType, modelType, propertyName, dbType);
            return this;
        }
    }
}