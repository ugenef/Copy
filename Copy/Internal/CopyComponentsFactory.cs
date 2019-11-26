using Copy.Internal.Models;
using Microsoft.EntityFrameworkCore;

namespace Copy.Internal
{
    class CopyComponentsFactory
    {
        private readonly SqlFactory _sqlFactory = new SqlFactory();
        private readonly DelegateFactory _delegateFactory = new DelegateFactory();
        private readonly MetadataFactory _metadataFactory = new MetadataFactory();

        public CopyComponents<T> GetComponents<T>(DbContext context)
        {
            var metadata = _metadataFactory.GetMetadata<T>(context);
            
            return new CopyComponents<T>
            {
                Sql = _sqlFactory.GetSql(metadata),
                Write = _delegateFactory.GetDelegate<T>(metadata)
            };
        }
    }
}