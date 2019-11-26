using System;

namespace Copy.MappingBuilder
{
    public class TypeMappingBuilder
    {
        internal Type ContextType { get; set; }
        
        public TypeMappingBuilder MapType<TModel>(Action<PropertyMappingBuilder<TModel>> mapProperties)
        {
            var propertyMappingBuilder=new PropertyMappingBuilder<TModel>{ContextType = ContextType};
            mapProperties(propertyMappingBuilder);
            return this;
        }
    }
}