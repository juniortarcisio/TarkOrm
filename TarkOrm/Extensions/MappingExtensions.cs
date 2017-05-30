using System;
using System.Linq;
using System.Reflection;
using TarkOrm.Attributes;
using System.Linq.Expressions;

namespace TarkOrm.Extensions
{
    public static class MappingExtensions
    {
        /// <summary>
        /// Returns the Name of the column which the property is mapped to.
        /// If it uses ColumnAttribute it will returns the columnName or else it will returns the self property name.
        /// </summary>
        /// <param name="property"></param>
        /// <returns>Name of the column which the property is mapped to.</returns>
        public static string GetMappedColumnName<T>(this PropertyInfo property)
        {
            var typeMapping = (TarkTypeMapping<T>)TarkConfigurationMapping.ManageMapping<T>();

            var propertyMapping = typeMapping.PropertiesMappings.Where(x => x.Value == property);

            if (propertyMapping.Count() == 0)
                return null;

            return propertyMapping.FirstOrDefault().Key;
        }

        public static bool IsKeyColumn(this PropertyInfo property)
        {
            //TODO: use the mapping from Configuration Mapping
            var identityAttribute = property.GetCustomAttribute<KeyAttribute>();

            if (identityAttribute == null)
                return false;
            else
                return true;
        }

        public static bool IsReadOnlyColumn (this PropertyInfo property)
        {
            //TODO: use the mapping from Configuration Mapping
            //Can be migrated only when the ReadOnly exists on the ConfigurationMapping
            var readonlyAttribute = property.GetCustomAttribute<ReadonlyAttribute>();

            if (readonlyAttribute == null)
                return false;
            else
                return true;
        }

        public static bool IsIgnoreMappingColumn(this PropertyInfo property)
        {
            //TODO: use the mapping from Configuration Mapping
            var ignoreMappingAttribute = property.GetCustomAttribute<IgnoreMappingAttribute>();

            if (ignoreMappingAttribute == null)
                return false;
            else
                return true;
        }

        public static PropertyInfo GetPropertyInfo<TSource, TProperty>(
            this Expression<Func<TSource, TProperty>> propertyLambda)
        {
            Type type = typeof(TSource);

            MemberExpression member = propertyLambda.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    propertyLambda.ToString()));

            PropertyInfo propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a field, not a property.",
                    propertyLambda.ToString()));

            if (type != propInfo.ReflectedType &&
                !type.IsSubclassOf(propInfo.ReflectedType))
                throw new ArgumentException(string.Format(
                    "Expresion '{0}' refers to a property that is not from type {1}.",
                    propertyLambda.ToString(),
                    type));

            return propInfo;
        }

    }
}
