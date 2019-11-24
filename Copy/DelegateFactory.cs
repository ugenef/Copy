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
    class DelegateFactory
    {
        public Action<NpgsqlBinaryImporter, T> GetDelegate<T>(MetadataForModel metadata)
        {
            return (Action<NpgsqlBinaryImporter, T>) BuildDelegate<T>(metadata);
        }

        private object BuildDelegate<T>(MetadataForModel metadata)
        {
            var dynamicMethod = GetDynamicMethod<T>();

            using (var il = new GroboIL(dynamicMethod))
            {
                il.Ldarg(0);
                il.Callnonvirt(GetStartRowMethod());

                foreach (var prop in metadata.EmitInfos)
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

        private MethodInfo GetStartRowMethod()
        {
            return typeof(NpgsqlBinaryImporter)
                .GetMethod(nameof(NpgsqlBinaryImporter.StartRow), BindingFlags.Instance | BindingFlags.Public);
        }

        private DynamicMethod GetDynamicMethod<T>()
        {
            return new DynamicMethod($"WriteProperties{Guid.NewGuid().ToString()}",
                typeof(void),
                new[] {typeof(NpgsqlBinaryImporter), typeof(T)});
        }
    }
}