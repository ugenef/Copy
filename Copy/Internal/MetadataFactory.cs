using System;
using System.Linq;
using System.Reflection;
using Copy.Internal.Models;
using Copy.Internal.TypeMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using NpgsqlTypes;

namespace Copy.Internal
{
    class MetadataFactory
    {
        private readonly WriteMethodFactory _writeMethodFactory = new WriteMethodFactory();

        public MetadataForModel GetMetadata<T>(DbContext context)
        {
            var entityType = context.Model.FindEntityType(typeof(T));

            return new MetadataForModel
            {
                TableName = entityType.GetTableName(),
                EmitInfos = ExtractEmitInfo<T>(entityType, context.GetType())
            };
        }

        private EmitInfo[] ExtractEmitInfo<T>(IEntityType entityType, Type contextType)
        {
            var props = entityType.GetProperties();

            return props.Select(p =>
                {
                    var dbType = GetDbType(p.PropertyInfo, contextType);
                    return new EmitInfo
                    {
                        PostgresName = p.GetColumnName(),
                        Getter = p.PropertyInfo.GetMethod,
                        PostgresType = dbType,
                        WriteMethod = _writeMethodFactory.GetWriteMethod(p.PropertyInfo.PropertyType, dbType)
                    };
                })
                .ToArray();
        }

        private NpgsqlDbType? GetDbType(PropertyInfo propertyInfo, Type contextType)
        {
            return TypeMappingStorage.TryGetDbType(contextType, propertyInfo.DeclaringType, propertyInfo.Name);
        }
    }
}