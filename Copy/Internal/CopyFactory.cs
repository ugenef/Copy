using Copy.Internal.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Copy.Internal
{
    static class CopyFactory
    {
        private static readonly CopyComponentsFactory _componentsFactory = new CopyComponentsFactory();
        private static readonly CopyComponentsStorage _componentsStorage = new CopyComponentsStorage();

        public static Copy<T> GetCopy<T>(DbContext context)
        {
            var conn = context.Database.GetDbConnection();
            if(!(conn is NpgsqlConnection))
                throw new CopyException("Copy works with Npgsql only");
            var copyComponents = GetComponents<T>(context);
            return new Copy<T>(copyComponents, (NpgsqlConnection) conn);
        }

        private static CopyComponents<T> GetComponents<T>(DbContext context)
        {
            var contextType = context.GetType();
            var copyComponents = _componentsStorage
                .GetOrAdd(contextType, typeof(T), _componentsFactory.GetComponents<T>(context));
            return copyComponents;
        }
    }
}