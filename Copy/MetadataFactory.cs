using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Copy.Factories;
using Copy.TypeMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using NpgsqlTypes;

namespace Copy
{
    class MetadataFactory
    {
        private readonly Regex _wordInCamelCase =
            new Regex(@"([A-Z][a-z]*)", RegexOptions.Compiled);

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
            return props.Select(p =>
                {
                    
                    var dbType = GetDbType(p.PropertyInfo);
                    return new EmitInfo
                    {
                        PostgresName =p.GetColumnName(),
                        Getter = p.PropertyInfo.GetMethod,
                        PostgresType = dbType,
                        WriteMethod = _writeMethodFactory.GetWriteMethod(p.PropertyInfo.PropertyType, dbType)
                    };
                })
                .ToArray();
        }

        private NpgsqlDbType? GetDbType(PropertyInfo propertyInfo)
        {
            return CopyTypeMapper.GetDbType(propertyInfo.DeclaringType, propertyInfo.Name);
        }

        private string ConvertToSnakeCase(string camelCase)
        {
            var words = _wordInCamelCase.Matches(camelCase);
            return string.Join('_', words).ToLower();
        }
    }
}