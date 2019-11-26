using System;
using System.Linq;
using System.Reflection;
using Npgsql;
using NpgsqlTypes;

namespace Copy.Internal
{
    class WriteMethodFactory
    {
        public MethodInfo GetWriteMethod(Type paramType, NpgsqlDbType? dbType)
        {
            var paramTypes = GetParamTypes(paramType, dbType);

            return MakeGenericMethod(paramTypes);
        }

        private Type[] GetParamTypes(Type paramType, NpgsqlDbType? dbType)
        {
            return dbType == null ? 
                new[] {paramType} : 
                new[] {paramType, typeof(NpgsqlDbType)};
        }

        private MethodInfo MakeGenericMethod(Type[] paramTypes)
        {
            return typeof(NpgsqlBinaryImporter)
                .GetMethods()
                .Where(m => m.Name == nameof(NpgsqlBinaryImporter.Write))
                .Select(m => new
                {
                    Method = m,
                    Params = m.GetParameters(),
                    GenericArgs = m.GetGenericArguments()
                })
                .Where(x => x.Params.Length == paramTypes.Length
                            && x.GenericArgs.Length == 1
                            && x.Params[0].ParameterType == x.GenericArgs[0])
                .Select(x => x.Method.MakeGenericMethod(paramTypes[0]))
                .First();
        }
    }
}