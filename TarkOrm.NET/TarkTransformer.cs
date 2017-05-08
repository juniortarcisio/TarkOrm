using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TarkOrm.NET.Extensions;

namespace TarkOrm.NET
{
    // Stage 2 - Transforming
    //TarkTransformer
    // - Receive any ADO Object as DataReader, DataSet, DataTable
    // - Mapping
    //   - Throught an existing mapping (it could be done through MappingConfiguration)
    //   - Or when the mapping doesn't exists, it could be done through just looking the members or MappingAttributes
    // - Creating object
    //Adding to the list if it's a list
    //Returning the list/object

    public class TarkTransformer
    {
        private Dictionary<string, PropertyInfo> _mappedProperties;

        /// <summary>
        /// Fill an object property according it's mapped column name
        /// </summary>
        /// <param name="obj">object to receive the value</param>
        /// <param name="mappedColumnName">name of the column mapped to the property</param>
        /// <param name="value">value to be filled in the property</param>
        /// <exception cref="ArgumentNullException"></exception>
        private void SetPropertyValue(object obj, string mappedColumnName, object value)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            if (mappedColumnName == null)
                throw new ArgumentNullException("mappedColumnName");

            //First search the column before treating nulls,
            //Despite the search proccess, it's possible to get implementation mistake even when it's null
            PropertyInfo objectProperty;// = obj.GetType().GetMappedProperty(mappedColumnName);

            if (!_mappedProperties.TryGetValue(mappedColumnName, out objectProperty))
            {
                objectProperty = obj.GetType().GetMappedProperty(mappedColumnName);
                _mappedProperties.Add(mappedColumnName, objectProperty);
            }

            if (value == DBNull.Value || value == null)
                return;

            object propertyValue;

            try
            {
                propertyValue = value.To(objectProperty.PropertyType);
            }
            catch (Exception ex)
            {
                throw new InvalidCastException(
                    String.Format("Cannot cast data value to object property type \"{1}\".", objectProperty.Name), ex);
            }

            try
            {
                objectProperty.SetValue(obj, value);
            }
            catch (Exception ex)
            {
                throw new InvalidCastException(
                    String.Format("Cannot set value to object property \"{1}\".", objectProperty.Name), ex);
            }
        }

        /// <summary>
        /// Create and object and fill it's mapped properties through a DataRow 
        /// </summary>
        /// <param name="dr">DataRow with columns mapped to the object type</param>
        /// <returns>A new instance of the object type with the filled properties</returns>
        public T CreateObject<T>(DataRow dr)
        {
            Type typeT = typeof(T);

            var finalObject = (T)Activator.CreateInstance(typeT, new object[] { });

            for (int i = 0; i < dr.Table.Columns.Count; i++)
            {
                SetPropertyValue(finalObject, dr.Table.Columns[i].ColumnName, dr[i]);
            }

            return finalObject;
        }

        /// <summary>
        /// Create and object and fill it's mapped properties through a DataReader record 
        /// </summary>
        /// <param name="dr">DataReader in the current record with fields mapped to the object type</param>
        /// <returns>A new instance of the object type with the filled properties</returns>
        public T CreateObject<T>(IDataReader dr)
        {
            Type typeT = typeof(T);

            var finalObject = (T)Activator.CreateInstance(typeT);

            for (int i = 0; i < dr.FieldCount; i++)
            {
                var drColumnName = dr.GetName(i);

                SetPropertyValue(finalObject, drColumnName, dr[i]);
            }

            return finalObject;
        }
        
        public IEnumerable<T> ToList<T>(IDataReader dr)
        {
            List<T> lista = new List<T>();
            
            //TODO: Actually it clears the list, in the future it will search for an existing mapping configuration
            _mappedProperties = new Dictionary<string, PropertyInfo>();

            while (dr.Read())
            {
                lista.Add(CreateObject<T>(dr));
            }

            return lista;
        }
    }
}
