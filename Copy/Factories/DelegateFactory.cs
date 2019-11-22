using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Copy.TypeMapper;
using GrEmit;
using Npgsql;
using NpgsqlTypes;

namespace Copy.Factories
{
    static class DelegateFactory
    {
        private static readonly ConcurrentDictionary<Type, object> _delegates =
            new ConcurrentDictionary<Type, object>();

        public static Action<NpgsqlBinaryImporter, T> GetDelegate<T>()
        {
            var type = typeof(T);
            return (Action<NpgsqlBinaryImporter, T>) _delegates.GetOrAdd(type, BuildDelegate<T>);
        }

        private static object BuildDelegate<T>(Type type)
        {
            var dynamicMethod = new DynamicMethod($"WriteProperties{type}",
                typeof(void),
                new[] {typeof(NpgsqlBinaryImporter), typeof(T)});
            var info = ExtractTypeInfo<T>();
            using (var il = new GroboIL(dynamicMethod))
            {
                il.Ldarg(0);
                il.Callnonvirt(typeof(NpgsqlBinaryImporter)
                    .GetMethod(nameof(NpgsqlBinaryImporter.StartRow),BindingFlags.Instance | BindingFlags.Public));
                
                foreach (var prop in info)
                {
                    il.Ldarg(0);
                    il.Ldarg(1);
                    il.Callnonvirt(prop.Getter);
                    if (prop.PostgresType != null)
                        il.Ldc_I4((int) prop.PostgresType.Value);
                    il.Callnonvirt(prop.WriteMethod);
                }

                il.Ret();
            }
            
            return dynamicMethod.CreateDelegate(typeof(Action<NpgsqlBinaryImporter, T>));
        }


        private static PropInfo[] ExtractTypeInfo<T>()
        {
            var type = typeof(T);
            var props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            return props.Select(p =>
                {
                    var dbType = GetDbType(p);
                    return new PropInfo
                    {
                        Getter = p.GetMethod,
                        PostgresType = dbType,
                        WriteMethod = WriteMethodFactory.GetWriteMethod(p.PropertyType, dbType)
                    };
                })
                .ToArray();
        }

        private static NpgsqlDbType? GetDbType(PropertyInfo propertyInfo)
        {
            return CopyTypeMapper.GetDbType(propertyInfo.DeclaringType, propertyInfo.Name);
        }

        private class PropInfo
        {
            public MethodInfo Getter { get; set; }
            public NpgsqlDbType? PostgresType { get; set; }
            public MethodInfo WriteMethod { get; set; }
        }
    }
}