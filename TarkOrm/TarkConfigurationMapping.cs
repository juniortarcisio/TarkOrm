using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using TarkOrm.Extensions;
using System.Linq.Expressions;

namespace TarkOrm
{
    /// <summary>
    /// Component for mapping configuration management
    /// </summary>
    public static class TarkConfigurationMapping
    {
        private static HashSet<ITarkTypeMapping> _mappings;        

        public static HashSet<ITarkTypeMapping> Mappings
        {
            get
            {
                if (_mappings == null)
                    _mappings = new HashSet<ITarkTypeMapping>();

                return _mappings;
            }
        }

        public static ITarkTypeMapping GetMapping<T>()
        {
            return Mappings.
                Where(x => x.GetMappingType() == typeof(T)).
                FirstOrDefault();
        }

        public static ITarkTypeMapping ManageMapping<T>()
        {
            var existingMapping = GetMapping<T>();
            if (existingMapping != null)
                return (TarkTypeMapping<T>)existingMapping;
            else
                return AutoMapType<T>();
        }

        public static TarkTypeMapping<T> AutoMapType<T>()
        {
            var existingMapping = GetMapping<T>();
            if (existingMapping != null)
                return (TarkTypeMapping<T>)existingMapping;

            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties();
            Dictionary<string, PropertyInfo> propertiesMappings = 
                new Dictionary<string, PropertyInfo>(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < properties.Length; i++)
            {
                if (properties[i].IsIgnoreMappingColumn())
                    continue;

                var columnName = properties[i].GetMappedColumnName();
                propertiesMappings.Add(columnName, properties[i]);
            }

            var newMapping = new TarkTypeMapping<T>(type, propertiesMappings);
            Mappings.Add(newMapping);
            
            return newMapping;
        }        
    }
    
    public interface ITarkTypeMapping
    {
        Type GetMappingType();

        Dictionary<string, PropertyInfo> GetPropertiesMapping();
    }

    public class TarkTypeMapping<T> : ITarkTypeMapping
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

        public Type GetMappingType()
        {
            return Type;
        }

        public Dictionary<string, PropertyInfo> GetPropertiesMapping()
        {
            return PropertiesMappings;
        }

        public TarkTypeMapping<T> MapProperty<TProperty>(Expression<Func<T, TProperty>> propertyLambda, string columnName)
        {
            var property = propertyLambda.GetPropertyInfo();
            
            if (PropertiesMappings.ContainsKey(columnName))
                PropertiesMappings[columnName] = property;
            else
                PropertiesMappings.Add(columnName, property);

            return this;
        }

        public TarkTypeMapping<T> IgnoreProperty<TProperty>(Expression<Func<T, TProperty>> propertyLambda)
        {
            var property = propertyLambda.GetPropertyInfo();

            if (PropertiesMappings.ContainsValue(property))
            {
                var propertyKeys = PropertiesMappings.Where(x => x.Value == property).Select(x=> x.Key).ToArray();

                foreach (var key in propertyKeys)
                {
                    PropertiesMappings.Remove(key);
                }                
            }

            return this;
        }
    }
}
