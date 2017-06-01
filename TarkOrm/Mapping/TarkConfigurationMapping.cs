using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using TarkOrm.Extensions;
using System.Linq.Expressions;
using TarkOrm.Attributes;

namespace TarkOrm.Mapping
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

        public static TarkTypeMapping<T> GetMapping<T>()
        {
            return (TarkTypeMapping<T>)Mappings.
                Where(x => x.GetMappingType() == typeof(T)).
                FirstOrDefault();
        }

        public static ITarkTypeMapping ManageMapping<T>()
        {
            var existingMapping = GetMapping<T>();
            if (existingMapping != null)
                return existingMapping;
            else
                return AutoMapType<T>();
        }

        public static TarkTypeMapping<T> CreateEmptyMapType<T>()
        {
            var existingMapping = GetMapping<T>();
            if (existingMapping != null)
                Mappings.Remove(existingMapping);

            Type type = typeof(T);
            Dictionary<string, TarkColumnMapping> propertiesMappings =
                new Dictionary<string, TarkColumnMapping>(StringComparer.OrdinalIgnoreCase);

            var newMapping = new TarkTypeMapping<T>(type, propertiesMappings);
            Mappings.Add(newMapping);

            return newMapping;
        }

        public static TarkTypeMapping<T> AutoMapType<T>()
        {
            var existingMapping = GetMapping<T>();
            if (existingMapping != null)
                return existingMapping;

            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties();
            Dictionary<string, TarkColumnMapping> propertiesMappings = 
                new Dictionary<string, TarkColumnMapping>(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < properties.Length; i++)
            {
                if (properties[i].GetCustomAttribute<IgnoreMappingAttribute>() != null)
                    continue;

                string columnName;

                var columnAttribute = properties[i].GetCustomAttribute<ColumnAttribute>();
                if (columnAttribute != null && !string.IsNullOrWhiteSpace(columnAttribute.Name))
                    columnName = columnAttribute.Name;
                else
                    columnName = properties[i].Name;
                                
                propertiesMappings.Add(columnName, new TarkColumnMapping(properties[i]));
            }

            var newMapping = new TarkTypeMapping<T>(type, propertiesMappings);

            //Database
            var databaseAttribute = type.GetCustomAttribute<DatabaseAttribute>();
            if (databaseAttribute != null && !string.IsNullOrWhiteSpace(databaseAttribute.Name))
            {
                newMapping.Database = databaseAttribute.Name;
            }

            //Table and schema
            var tableAttribute = type.GetCustomAttribute<TableAttribute>();
            if (tableAttribute != null)
            {
                if (!string.IsNullOrWhiteSpace(tableAttribute.Name))
                    newMapping.Table = tableAttribute.Name;

                if (!string.IsNullOrWhiteSpace(tableAttribute.Schema))
                    newMapping.Schema = tableAttribute.Schema;
            }
            else
            {
                newMapping.Table = type.Name;
            }

            Mappings.Add(newMapping);
            
            return newMapping;
        }        
    }
    
    

}
