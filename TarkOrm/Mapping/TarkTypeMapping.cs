using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TarkOrm.Attributes;
using TarkOrm.Extensions;

namespace TarkOrm.Mapping
{

    public class TarkTypeMapping<T> : ITarkTypeMapping
    {
        public TarkTypeMapping(Type type)
        {
            Type = type;
        }

        public TarkTypeMapping(Type type, Dictionary<string, TarkColumnMapping> propertiesMappings)
        {
            Type = type;
            PropertiesMappings = propertiesMappings;
        }

        public Type Type { get; set; }

        public string Database { get; set; }

        public string Schema { get; set; } = "dbo";

        public string Table { get; set; }

        public Dictionary<string, TarkColumnMapping> PropertiesMappings;

        public Type GetMappingType()
        {
            return Type;
        }

        public Dictionary<string, TarkColumnMapping> GetPropertiesMapping()
        {
            return PropertiesMappings;
        }

        public TarkTypeMapping<T> MapProperty<TProperty>(Expression<Func<T, TProperty>> propertyLambda, string columnName, params Attribute[] attributes)
        {
            var property = propertyLambda.GetPropertyInfo();

            if (PropertiesMappings.ContainsKey(columnName))
                throw new DuplicatedMappingException(columnName,typeof(T).Name);

            PropertiesMappings.Add(columnName, new TarkColumnMapping(property, attributes));

            return this;
        }

        public TarkTypeMapping<T> MapProperty<TProperty>(Expression<Func<T, TProperty>> propertyLambda, string columnName)
        {
            var property = propertyLambda.GetPropertyInfo();

            if (PropertiesMappings.ContainsKey(columnName))
                throw new DuplicatedMappingException(columnName, typeof(T).Name);

            PropertiesMappings.Add(columnName, new TarkColumnMapping(property));

            return this;
        }

        public TarkTypeMapping<T> MapProperty<TProperty>(Expression<Func<T, TProperty>> propertyLambda, params Attribute[] attributes)
        {
            var property = propertyLambda.GetPropertyInfo();

            if (PropertiesMappings.ContainsKey(property.Name))
                throw new DuplicatedMappingException(property.Name, typeof(T).Name);

            PropertiesMappings.Add(property.Name, new TarkColumnMapping(property, attributes));

            return this;
        }

        public TarkTypeMapping<T> MapProperty<TProperty>(Expression<Func<T, TProperty>> propertyLambda)
        {
            var property = propertyLambda.GetPropertyInfo();

            if (PropertiesMappings.ContainsKey(property.Name))
                throw new DuplicatedMappingException(property.Name, typeof(T).Name);

            PropertiesMappings.Add(property.Name, new TarkColumnMapping(property));

            return this;
        }

        public TarkTypeMapping<T> IgnoreProperty<TProperty>(Expression<Func<T, TProperty>> propertyLambda)
        {
            var property = propertyLambda.GetPropertyInfo();

            if (PropertiesMappings.Values.Any(x => x.Property == property))
            {
                var propertyKeys = PropertiesMappings.Where(x => x.Value.Property == property).Select(x => x.Key).ToArray();

                foreach (var key in propertyKeys)
                {
                    PropertiesMappings.Remove(key);
                }
            }

            return this;
        }

        public string[] GetMappedOrderedKeys()
        {
            var keys = from p in PropertiesMappings
                       where p.Value.Attributes.Any(x => x is KeyAttribute)
                       orderby ((KeyAttribute)p.Value.Attributes.Where(x => x is KeyAttribute).FirstOrDefault()).Order
                       select p.Key;

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
