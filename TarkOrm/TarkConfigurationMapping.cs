using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using TarkOrm.Extensions;
using System.Linq.Expressions;
using TarkOrm.Attributes;

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

        public static TarkTypeMapping<T> AutoMapType<T>()
        {
            var existingMapping = GetMapping<T>();
            if (existingMapping != null)
                return existingMapping;

            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties();
            Dictionary<string, PropertyInfo> propertiesMappings = 
                new Dictionary<string, PropertyInfo>(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < properties.Length; i++)
            {
                if (properties[i].IsIgnoreMappingColumn())
                    continue;

                string columnName;

                var columnAttribute = properties[i].GetCustomAttribute<ColumnAttribute>();
                if (columnAttribute != null && !string.IsNullOrWhiteSpace(columnAttribute.Name))
                    columnName = columnAttribute.Name;
                else
                    columnName = properties[i].Name;

                propertiesMappings.Add(columnName, properties[i]);
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

        public string Database { get; set; }

        public string Schema { get; set; } = "dbo";

        public string Table { get; set; }
        
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

        public string[] GetMappedOrderedKeys()
        {
            HashSet<string> keys = new HashSet<string>();

            foreach (var item in PropertiesMappings)
            {
                if (item.Value.IsKeyColumn())
                    keys.Add(item.Key);
            }

            return keys.ToArray();
        }

        public TarkTypeMapping<T> ToDatabase(string database)
        {
            Database = database;
            return this;
        }

        public TarkTypeMapping<T> ToSchema(string schema)
        {
            Schema = schema;
            return this;
        }

        public TarkTypeMapping<T> ToTable(string tableName)
        {
            Table = tableName;
            return this;
        }

        public TarkTypeMapping<T> ToTable(string schema, string tableName)
        {
            Schema = schema;
            Table = tableName;
            return this;
        }

        public TarkTypeMapping<T> ToTable(string database, string schema, string tableName)
        {
            Database = database;
            Schema = schema;
            Table = tableName;
            return this;
        }
    }
}
