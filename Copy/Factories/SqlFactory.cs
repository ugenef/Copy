using System;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Copy.Factories
{
    static class SqlFactory
    {
        private static readonly Regex _wordInCamelCase = 
            new Regex(@"([A-Z][a-z]*)", RegexOptions.Compiled);

        private static readonly ConcurrentDictionary<Type, string> _sql =
            new ConcurrentDictionary<Type, string>();

        public static string GetSql<T>()
        {
            var type = typeof(T);
            return _sql.GetOrAdd(type, BuildCommand);
        }

        private static string BuildCommand(Type type)
        {
            var tableName = type.GetCustomAttribute<TableAttribute>().Name;
            var colNames = GetColNames(type);
            return $"COPY {tableName} ({string.Join(',', colNames)}) FROM STDIN (FORMAT BINARY)";
        }

        private static string[] GetColNames(Type type)
        {
            return type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Select(p => p.Name)
                .Select(ConvertToSnakeCase)
                .ToArray();
        }

        private static string ConvertToSnakeCase(string camelCase)
        {
            var words = _wordInCamelCase.Matches(camelCase);
            return string.Join('_', words).ToLower();
        }
    }
}