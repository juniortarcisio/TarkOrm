using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
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

        //public Tuple<string, PropertyInfo, List<Attribute>> PropertiesMappings2;

        public Type GetMappingType()
        {
            return Type;
        }

        public Dictionary<string, TarkColumnMapping> GetPropertiesMapping()
        {
            return PropertiesMappings;
        }

        public TarkTypeMapping<T> MapProperty<TProperty>(Expression<Func<T, TProperty>> propertyLambda, string columnName)
        {
            var property = propertyLambda.GetPropertyInfo();

            if (PropertiesMappings.ContainsKey(columnName))
                PropertiesMappings[columnName] = new TarkColumnMapping(property);
            else
                PropertiesMappings.Add(columnName, new TarkColumnMapping(property));

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
