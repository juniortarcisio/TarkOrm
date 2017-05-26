using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TarkOrm.Extensions;

namespace TarkOrm
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
        /// <summary>
        /// Fill an object property according it's mapped column name
        /// </summary>
        /// <param name="obj">object to receive the value</param>
        /// <param name="columnName">name of the column mapped to the property</param>
        /// <param name="value">value to be filled in the property</param>
        /// <exception cref="ArgumentNullException"></exception>
        private void SetPropertyValue(object obj, string columnName, object value, TarkTypeMapping typeMapping)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            if (columnName == null)
                throw new ArgumentNullException("mappedColumnName");

            //First search the column before treating nulls,
            //Despite the search proccess, it's possible to get implementation mistake even when it's null
            PropertyInfo objectProperty;

            if (!typeMapping.PropertiesMappings.TryGetValue(columnName, out objectProperty))
            {            
                //2017-05-25: Changed to don't throw exceptions if there is no property for the field
                //throw new MissingMemberException(String.Format("Cannot find mapped property for column \"{0}\"",columnName));
                return;
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
                objectProperty.SetValue(obj, propertyValue); 
            }
            catch (Exception ex)
            {
                throw new InvalidCastException(
                    String.Format("Cannot set value to object property \"{1}\".", objectProperty.Name), ex);
            }
        }

        /// <summary>
        /// Create and object and fill it's mapped properties through a DataReader record 
        /// </summary>
        /// <param name="dr">DataReader in the current record with fields mapped to the object type</param>
        /// <returns>A new instance of the object type with the filled properties</returns>
        public T CreateObject<T>(DataRow dr)
        {
            var mapping = TarkConfigurationMapping.AutoMapType<T>();
            return CreateObject<T>(dr, mapping);
        }

        /// <summary>
        /// Create and object and fill it's mapped properties through a DataRow 
        /// </summary>
        /// <param name="dr">DataRow with columns mapped to the object type</param>
        /// <returns>A new instance of the object type with the filled properties</returns>
        public T CreateObject<T>(DataRow dr, TarkTypeMapping typeMapping)
        {
            Type typeT = typeof(T);

            var finalObject = (T)Activator.CreateInstance(typeT, new object[] { });
 
            for (int i = 0; i < dr.Table.Columns.Count; i++)
            {
                SetPropertyValue(finalObject, dr.Table.Columns[i].ColumnName, dr[i], typeMapping);
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
            var mapping = TarkConfigurationMapping.AutoMapType<T>();
            return CreateObject<T>(dr, mapping);
        }

        /// <summary>
        /// Create and object and fill it's mapped properties through a DataReader record 
        /// </summary>
        /// <param name="dr">DataReader in the current record with fields mapped to the object type</param>
        /// <returns>A new instance of the object type with the filled properties</returns>
        public T CreateObject<T>(IDataReader dr, TarkTypeMapping typeMapping)
        {
            Type typeT = typeof(T);

            var finalObject = (T)Activator.CreateInstance(typeT);

            for (int i = 0; i < dr.FieldCount; i++)
            {
                var drColumnName = dr.GetName(i);

                SetPropertyValue(finalObject, drColumnName, dr[i], typeMapping);
            }

            return finalObject;
        }

        public IEnumerable<T> ToList<T>(IDataReader dr)
        {
            List<T> lista = new List<T>();

            //Search or map the type into the singleton manager
            var mapping = TarkConfigurationMapping.AutoMapType<T>();

            while (dr.Read())
            {
                lista.Add(CreateObject<T>(dr, mapping));
            }

            return lista;
        }
    }
}
