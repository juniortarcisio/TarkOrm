using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TarkOrm.Extensions;
using TarkOrm.Mapping;

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
        private void SetPropertyValue<T>(T obj, string columnName, object value, TarkTypeMapping<T> typeMapping)
        {
            TarkColumnMapping columnMapping;

            if (!typeMapping.GetPropertiesMapping().TryGetValue(columnName, out columnMapping))
                return;
            //2017-05-25: Changed to don't throw exceptions if there is no property for the field
            //throw new MissingMemberException(String.Format("Cannot find mapped property for column \"{0}\"",columnName));

            if (value == DBNull.Value)
                return;
            
            columnMapping.Property.SetValue(obj,
                Convert.ChangeType(value,
                columnMapping.GetCachePropertyConvertType(),
                CultureInfo.InvariantCulture
                )
            );
        }

        /// <summary>
        /// Create and object and fill it's mapped properties through a DataReader record 
        /// </summary>
        /// <param name="dr">DataReader in the current record with fields mapped to the object type</param>
        /// <returns>A new instance of the object type with the filled properties</returns>
        public T CreateObject<T>(DataRow dr)
        {
            var mapping = TarkConfigurationMapping.ManageMapping<T>();
            return CreateObject<T>(dr, mapping);
        }

        /// <summary>
        /// Create and object and fill it's mapped properties through a DataRow 
        /// </summary>
        /// <param name="dr">DataRow with columns mapped to the object type</param>
        /// <returns>A new instance of the object type with the filled properties</returns>
        public T CreateObject<T>(DataRow dr, TarkTypeMapping<T> typeMapping)
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
            var mapping = TarkConfigurationMapping.ManageMapping<T>();

            string[] columnNames = new string[dr.FieldCount];
            for (int i = 0; i < dr.FieldCount; i++)
                columnNames[i] = dr.GetName(i);

            return CreateObject(dr, mapping, columnNames);
        }

        /// <summary>
        /// Create and object and fill it's mapped properties through a DataReader record 
        /// </summary>
        /// <param name="dr">DataReader in the current record with fields mapped to the object type</param>
        /// <returns>A new instance of the object type with the filled properties</returns>
        private T CreateObject<T>(IDataReader dr, TarkTypeMapping<T> typeMapping, string[] columnNames)
        {
            var finalObject = (T)Activator.CreateInstance(typeof(T));
            
            object[] values = new object[dr.FieldCount];
            dr.GetValues(values);

            for (int i = 0; i < dr.FieldCount; i++)
                SetPropertyValue(finalObject, columnNames[i], values[i], typeMapping);

            return finalObject; 
        }

        public IEnumerable<T> ToList<T>(IDataReader dr)
        {
            List<T> lista = new List<T>();

            //Search or map the type into the singleton manager
            var mapping = TarkConfigurationMapping.ManageMapping<T>();

            string[] columnNames = new string[dr.FieldCount];
            for (int i = 0; i < dr.FieldCount; i++)
                columnNames[i] = dr.GetName(i);

            while (dr.Read())
            {
                lista.Add(CreateObject(dr, mapping, columnNames));
            }

            return lista;
        }


        private dynamic CreateDynamicObject(IDataReader dr, string[] columnNames)
        {
            dynamic finalObject = new ExpandoObject();

            object[] values = new object[dr.FieldCount];
            dr.GetValues(values);

            for (int i = 0; i < dr.FieldCount; i++)
            {
                if (values[i] == DBNull.Value) continue;

                ((IDictionary<string, object>) finalObject)[columnNames[i]] = values[i];
            }

            return finalObject; 
        }
        
        public IEnumerable<dynamic> ToDynamicList(IDataReader dr)
        {
            List<dynamic> lista = new List<dynamic>();

            string[] columnNames = new string[dr.FieldCount];
            for (int i = 0; i < dr.FieldCount; i++)
                columnNames[i] = dr.GetName(i);

            while (dr.Read())
            {
                lista.Add(CreateDynamicObject(dr, columnNames));
            }

            return lista;
        }

        public IEnumerable<T> ToList<T>(DataTable dt)
        {
            List<T> lista = new List<T>();

            //Search or map the type into the singleton manager
            var mapping = TarkConfigurationMapping.ManageMapping<T>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                lista.Add(CreateObject<T>(dt.Rows[i], mapping));
            }

            return lista;
        }
    }
}
