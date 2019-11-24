using System;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Copy.Factories
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