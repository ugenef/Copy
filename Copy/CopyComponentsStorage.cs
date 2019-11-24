using System;
using System.Collections.Concurrent;

namespace Copy
{
    class CopyComponentsStorage
    {
        private readonly
            ConcurrentDictionary<Type, ConcurrentDictionary<Type, object>> _contextModelsPairs =
                new ConcurrentDictionary<Type, ConcurrentDictionary<Type, object>>();

        public CopyComponents<T> GetOrAdd<T>(
            Type contextType,
            Type modelType,
            CopyComponents<T> copyComponents)
        {
            var modelsOfContext = _contextModelsPairs
                .GetOrAdd(contextType, new ConcurrentDictionary<Type, object>());

            return (CopyComponents<T>) modelsOfContext
                .GetOrAdd(modelType, _ => copyComponents);
        }
    }
}