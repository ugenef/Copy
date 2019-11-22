using System;
using System.Linq;
using System.Reflection;
using Npgsql;
using NpgsqlTypes;

namespace Copy.Factories
{
    static class WriteMethodFactory
    {
        public static MethodInfo GetWriteMethod(Type paramType, NpgsqlDbType? dbType)
        {
            var paramTypes = GetParamTypes(paramType, dbType);
            
            if (paramType.IsClass)
                return MakeGenericMethod(nameof(WriteRefType), paramTypes);

            return IsNullableStruct(paramType) ? 
                MakeGenericMethod(nameof(WriteNullableStruct), paramTypes) :
                MakeGenericMethod(paramTypes);
        }

        private static Type[] GetParamTypes(Type paramType, NpgsqlDbType? dbType)
        {
            return dbType == null ? 
                new[] {paramType} : 
                new[] {paramType, typeof(NpgsqlDbType)};
        }

        private static bool IsNullableStruct(Type paramType)
        {
            return Nullable.GetUnderlyingType(paramType) != null;
        }

        private static MethodInfo MakeGenericMethod(Type[] paramTypes)
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

        private static MethodInfo MakeGenericMethod(string methodName, Type[] paramTypes)
        {
            return typeof(WriteMethodFactory)
                .GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
                .First(m =>
                    m.Name == methodName &&
                    m.GetParameters().Length == paramTypes.Length + 1)
                .MakeGenericMethod(paramTypes);
        }

        private static void WriteRefType<T>(NpgsqlBinaryImporter writer, T value)
            where T : class
        {
            if (value == null)
                writer.WriteNull();
            else
                writer.Write(value);
        }

        private static void WriteRefType<T>(NpgsqlBinaryImporter writer, T value, NpgsqlDbType dbType)
            where T : class
        {
            if (value == null)
                writer.WriteNull();
            else
                writer.Write(value, dbType);
        }

        private static void WriteNullableStruct<T>(NpgsqlBinaryImporter writer, T? value)
            where T : struct
        {
            if (value == null)
                writer.WriteNull();
            else
                writer.Write(value.Value);
        }

        private static void WriteNullableStruct<T>(NpgsqlBinaryImporter writer, T? value, NpgsqlDbType dbType)
            where T : struct
        {
            if (value == null)
                writer.WriteNull();
            else
                writer.Write(value.Value, dbType);
        }
    }
}