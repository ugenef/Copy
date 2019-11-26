using System;
using System.Reflection;
using System.Reflection.Emit;
using Copy.Internal.Models;
using GrEmit;
using Npgsql;

namespace Copy.Internal
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

                foreach (var emitInfo in metadata.EmitInfos)
                {
                    il.Ldarg(0);
                    il.Ldarg(1);
                    il.Callnonvirt(emitInfo.Getter);
                    if (emitInfo.PostgresType != null)
                        il.Ldstr(emitInfo.PostgresType);
                    il.Callnonvirt(emitInfo.WriteMethod);
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