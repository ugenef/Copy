using System;
using Copy.Internal.TypeMapper;
using Copy.MappingBuilder;
using Microsoft.EntityFrameworkCore;

namespace Copy
{
    public static class DbContextOptionsBuilderExtensions
    {
        public static DbContextOptionsBuilder UseCopyTypeMapping<TContext>(
            this DbContextOptionsBuilder optionsBuilder,
            Action<TypeMappingBuilder> mapType)
        where TContext:DbContext
        {
            var contextType = typeof(TContext);
            if (!TypeMappingStorage.ContextMappingConfigured(contextType))
            {
                var typeMappingBuilder = new TypeMappingBuilder {ContextType = contextType};
                mapType(typeMappingBuilder);
            }

            return optionsBuilder;
        }
    }
}