using System;
using System.Linq;
using System.Reflection;
using Npgsql;
using NpgsqlTypes;

namespace Copy.Internal
{
    class WriteMethodFactory
    {
        public MethodInfo GetWriteMethod(Type paramType)
        {
            var paramTypes = new[] {paramType, typeof(string)};

            return MakeGenericMethod(paramTypes);
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
                .Where(x => x.Params.Length == 2 &&
                            x.Params[1].ParameterType == paramTypes[1]
                            && x.GenericArgs.Length == 1
                            && x.Params[0].ParameterType == x.GenericArgs[0])
                .Select(x => x.Method.MakeGenericMethod(paramTypes[0]))
                .First();
        }
    }
}