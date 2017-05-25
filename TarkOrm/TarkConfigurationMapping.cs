using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using TarkOrm.Extensions;

namespace TarkOrm
{
    /// <summary>
    /// Component for mapping configuration management
    /// </summary>
    public static class TarkConfigurationMapping
    {
        private static HashSet<TarkTypeMapping> _mappings;        

        public static HashSet<TarkTypeMapping> Mappings
        {
            get
            {
                if (_mappings == null)
                    _mappings = new HashSet<TarkTypeMapping>();

                return _mappings;
            }
        }

        public static TarkTypeMapping GetMapping<T>()
        {
            return Mappings.Where(x => x.Type == typeof(T)).FirstOrDefault();
        }
        
        public static TarkTypeMapping AutoMapType<T>()
        {
            var existingMapping = GetMapping<T>();
            if (existingMapping != null)
                return existingMapping;

            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties();
            Dictionary<string, PropertyInfo> propertiesMappings = 
                new Dictionary<string, PropertyInfo>();

            for (int i = 0; i < properties.Length; i++)
            {
                var columnName = properties[i].GetMappedColumnName();
                propertiesMappings.Add(columnName, properties[i]);
            }

            var newMapping = new TarkTypeMapping(type, propertiesMappings);
            Mappings.Add(newMapping);
            
            return newMapping;
        }
    }

    public class TarkTypeMapping
    {
        public TarkTypeMapping(Type type)
        {
            Type = type;
        }

        public TarkTypeMapping(Type type, Dictionary<string, PropertyInfo> propertiesMappings)
        {
            Type = type;
            PropertiesMappings = propertiesMappings;
        }

        public Type Type { get; set; }
        
        public Dictionary<string, PropertyInfo> PropertiesMappings;
    }
}
