using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TarkOrm.NET.Extensions
{
    public static class MappingExtensions
    {
        /// <summary>
        /// Returns the Name of the column which the property is mapped to.
        /// If it uses ColumnAttribute it will returns the columnName or else it will returns the self property name.
        /// </summary>
        /// <param name="property"></param>
        /// <returns>Name of the column which the property is mapped to.</returns>
        public static string GetMappedColumnName(this PropertyInfo property)
        {
            var columnAttribute = property.GetCustomAttribute<ColumnAttribute>();
            if (columnAttribute != null && !String.IsNullOrWhiteSpace(columnAttribute.Name))
                return columnAttribute.Name;
            else
                return property.Name;
        }

        /// <summary>
        /// Returns the mapped property of a type from the columnName
        /// </summary>
        /// <param name="type"></param>
        /// <param name="columnName">name of the column which the property is mapped to.</param>
        /// <returns>Return the property mapper to the columnName</returns>
        /// <exception cref="MissingFieldException">If the mapper property cannot be found</exception>
        public static PropertyInfo GetMappedProperty(this Type type, string columnName)
        {
            PropertyInfo objectProperty = null;
            PropertyInfo[] properties = type.GetProperties();

            for (int i = 0; i < properties.Length; i++)
            {
                if (columnName == properties[i].GetMappedColumnName())
                {
                    objectProperty = properties[i];
                    break;
                }
            }

            if (objectProperty == null)
                throw new MissingFieldException(String.Format("Cannot find \"{0}\" object mapped property"));
            else
                return objectProperty;
        }
        
        public static string[] GetMappedOrderedKeys(this Type type)
        {
            PropertyInfo[] properties = type.GetProperties();
            HashSet<string> keys = new HashSet<string>();

            for (int i = 0; i < properties.Length; i++)
            {
                if (properties[i].GetCustomAttribute<KeyAttribute>() != null)
                {
                    keys.Add(properties[i].GetMappedColumnName());
                }
            }                      

            return keys.ToArray();
        }

    }
}
