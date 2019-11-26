using System.Linq;
using Copy.Internal.Models;

namespace Copy.Internal
{
    class SqlFactory
    {
        public string GetSql(MetadataForModel metadata)
        {
            var colNames = metadata.EmitInfos.Select(x => x.PostgresName).ToArray();
            return $"COPY {metadata.TableName} ({string.Join(',', colNames)})" +
                   $" FROM STDIN (FORMAT BINARY)";
        }
    }
}