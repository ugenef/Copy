using System.Linq;
using Copy.Internal.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

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
                EmitInfos = ExtractEmitInfo(entityType)
            };
        }

        private EmitInfo[] ExtractEmitInfo(IEntityType entityType)
        {
            var props = entityType.GetProperties();
           
            return props.Select(p => new EmitInfo
                {
                    PostgresName = p.GetColumnName(),
                    Getter = p.PropertyInfo.GetMethod,
                    PostgresType = p.GetColumnType(),
                    WriteMethod = _writeMethodFactory.GetWriteMethod(p.PropertyInfo.PropertyType)
                })
                .ToArray();
        }
    }
}