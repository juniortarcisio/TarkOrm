using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Data;
using TarkOrm.NET.Extensions;
using TarkOrm.NET.Attributes;
using System.Linq.Expressions;
using System.Configuration;

namespace TarkOrm.NET
{
    public class TarkDataAccess<T> : IDisposable
    {

        public bool EnableNoLock { get; set; }

        private IDbConnection _connection;
        
        public TarkDataAccess(IDbConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");

            _connection = connection;
        }

        /// <summary>
        /// </summary>
        /// <param name="nameOrConnectionString">Either the database name or a connection string.</param>
        public TarkDataAccess(string nameOrConnectionString)
        {
            var connectionSetting = ConfigurationManager.ConnectionStrings["nameOrConnectionString"];
            if (connectionSetting != null)
                nameOrConnectionString = connectionSetting.ConnectionString;

            _connection = new System.Data.SqlClient.SqlConnection(nameOrConnectionString);
        }

        public void Dispose()
        {
            if (_connection.State != ConnectionState.Open)
                _connection.Close();

            _connection.Dispose();
        }

        private Dictionary<string, PropertyInfo> _mappedProperties;

        /// <summary>
        /// Fill an object property according it's mapped column name
        /// </summary>
        /// <param name="obj">object to receive the value</param>
        /// <param name="mappedColumnName">name of the column mapped to the property</param>
        /// <param name="value">value to be filled in the property</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void FillProperty(object obj, string mappedColumnName, object value)
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

            //if (value == DBNull.Value || value == null)
            //    return;

            //object propertyValue;

            //try
            //{
            //    //propertyValue = value.To(objectProperty.PropertyType);
            //}
            //catch (Exception ex)
            //{
            //    throw new InvalidCastException(
            //        String.Format("Cannot cast value from Data Field \"{0}\" to object property \"{1}\".", mappedColumnName, objectProperty.Name), ex);
            //}

            //try
            //{
            //    //objectProperty.SetValue(obj, value);
            //}
            //catch (Exception ex)
            //{
            //    throw new InvalidCastException(
            //        String.Format("Cannot set value from Data Field \"{0}\" to object property \"{1}\".", mappedColumnName, objectProperty.Name), ex);
            //}
        }

        /// <summary>
        /// Create and object and fill it's mapped properties through a DataRow 
        /// </summary>
        /// <param name="dr">DataRow with columns mapped to the object type</param>
        /// <returns>A new instance of the object type with the filled properties</returns>
        public T CreateObject(DataRow dr)
        {
            Type typeT = typeof(T);

            var finalObject = (T)Activator.CreateInstance(typeT, new object[] { });

            for (int i = 0; i < dr.Table.Columns.Count; i++)
            {
                //FillProperty(finalObject, dr.Table.Columns[i].ColumnName, dr[i]);
            }

            return finalObject;
        }

        /// <summary>
        /// Create and object and fill it's mapped properties through a DataReader record 
        /// </summary>
        /// <param name="dr">DataReader in the current record with fields mapped to the object type</param>
        /// <returns>A new instance of the object type with the filled properties</returns>
        public T CreateObject(IDataReader dr)
        {
            Type typeT = typeof(T);

            var finalObject = (T)Activator.CreateInstance(typeT);

            for (int i = 0; i < dr.FieldCount; i++)
            {
                var drColumnName = dr.GetName(i);

                FillProperty(finalObject, drColumnName, dr[i]);
            }

            return finalObject;
        }

        public string GetProcedureName()
        {
            Type typeT = typeof(T);
            PropertyInfo[] columnProperty = typeT.GetProperties();

            string databaseName = null;

            var databaseAttribute = typeT.GetCustomAttribute<DatabaseAttribute>();
            if (databaseAttribute == null)
            {
                throw new CustomAttributeFormatException("DatabaseAttribute not defined");
            }

            if (!String.IsNullOrWhiteSpace(databaseAttribute.Name))
                databaseName = databaseAttribute.Name;

            string tableName = null;
            string schema = "dbo";

            var tableAttribute = typeT.GetCustomAttribute<TableAttribute>();
            if (tableAttribute != null)
            {
                if (!String.IsNullOrWhiteSpace(tableAttribute.Name))
                    tableName = tableAttribute.Name;

                if (!String.IsNullOrWhiteSpace(tableAttribute.Schema))
                    schema = tableAttribute.Schema;
            }
            else
            {
                tableName = typeT.Name;
            }

            if (!String.IsNullOrWhiteSpace(databaseName))
            {
                return String.Format("{0}.{1}.sp_{2}_GetAll", databaseName, schema, tableName);
            }
            else
            {
                return String.Format("{1}.p_{2}_GetAll", schema, tableName);
            }
        }

        /*
            --> from an open connection (dependency injection OR extension to IDbConnection?)
                --> requires the set works with the same server 
                --> parameter connection

            --> from a closed connection
                --> instance the connection inside
        */
        public string GetMapperTablePath(Type type)
        {
            string databaseName = null;
            string tableName = null;
            string schema = "dbo";

            var databaseAttribute = type.GetCustomAttribute<DatabaseAttribute>();
            if (databaseAttribute != null && !String.IsNullOrWhiteSpace(databaseAttribute.Name))
            {
                databaseName = databaseAttribute.Name;
            }

            var tableAttribute = type.GetCustomAttribute<TableAttribute>();
            if (tableAttribute != null)
            {
                if (!String.IsNullOrWhiteSpace(tableAttribute.Name))
                    tableName = tableAttribute.Name;

                if (!String.IsNullOrWhiteSpace(tableAttribute.Schema))
                    schema = tableAttribute.Schema;
            }
            else
            {
                tableName = type.Name;
            }

            if (!String.IsNullOrWhiteSpace(databaseName))
            {
                return String.Format("{0}.{1}.{2}", databaseName, schema, tableName);
            }
            else
            {
                return String.Format("{0}.{1}", schema, tableName);
            }
        }

        public virtual IEnumerable<T> GetAll()
        {
            if (_connection.State != ConnectionState.Open)
            {
                if (_connection.State == ConnectionState.Closed)
                    _connection.Open();
                else
                    throw new DataException("Invalid connection state");
            }

            var tablePath = GetMapperTablePath(typeof(T));

            using (IDbCommand cmd = _connection.CreateCommand())
            {
                cmd.CommandText = $"SELECT * FROM {tablePath}";
                cmd.CommandType = CommandType.Text;

                using (IDataReader dr = cmd.ExecuteReader())
                {
                    List<T> lista = new List<T>();

                    _mappedProperties = new Dictionary<string, PropertyInfo>();

                    while (dr.Read())
                    {
                        lista.Add(CreateObject(dr));
                    }

                    return lista;
                }
            }
        }

        public virtual T GetById(params object[] keyValues)
        {
            if (_connection.State != ConnectionState.Open)
            {
                if (_connection.State == ConnectionState.Closed)
                    _connection.Open();
                else
                    throw new DataException("Invalid connection state");
            }

            var type = typeof(T);
            var tablePath = GetMapperTablePath(type);
            var mappedKeys = type.GetMappedOrderedKeys();

            if (keyValues.Count() == 0 || mappedKeys.Count() != keyValues.Length)
                throw new MissingPrimaryKeyException();
            
            using (IDbCommand cmd = _connection.CreateCommand())
            {
                StringBuilder cmdFilter = new StringBuilder();
                cmdFilter.Append("WHERE ");
                
                for (int i = 0; i < keyValues.Count(); i++)
                {
                    cmdFilter.Append($"{ mappedKeys[i] } = @{ mappedKeys[i] } ");

                    if (i != keyValues.Count() - 1)
                        cmdFilter.Append("AND ");

                    var dbParam = cmd.CreateParameter();
                    dbParam.ParameterName = $"@{ mappedKeys[i] }";
                    dbParam.Value = keyValues[i];

                    cmd.Parameters.Add(dbParam);
                }

                cmd.CommandText = $"SELECT * FROM {tablePath} {cmdFilter.ToString()}";
                cmd.CommandType = CommandType.Text;

                using (IDataReader dr = cmd.ExecuteReader())
                {

                    List<T> lista = new List<T>();

                    if (dr.Read())
                    {
                        return CreateObject(dr);
                    }
                    else
                        return default(T);
                }
            }
        }

        public virtual IEnumerable<T> Get(string procedureName = null)
        {
            Type typeT = typeof(T);

            string databaseName = null;

            var databaseAttribute = typeT.GetCustomAttribute<DatabaseAttribute>();
            if (databaseAttribute == null)
            {
                throw new CustomAttributeFormatException("DatabaseAttribute not defined");
            }

            if (!String.IsNullOrWhiteSpace(databaseAttribute.Name))
                databaseName = databaseAttribute.Name;

            return new List<T>();
            //enumServer = databaseAttribute.Server;

            //using (cnn db = new cnn(enumServer))
            //{
            //    db.Connection.Open();
            //    IDataReader dr = db.GetDataReader(procedureName);

            //    List<T> lista = new List<T>();

            //    while (dr.Read())
            //    {
            //        lista.Add(CreateObject(dr));
            //    }

            //    return lista;
            //}
        }
        

        public virtual CommandBuilder<T> Where(/*Expression<Func<TSource, TProperty>> propertyLambda,*/ object value)
        {
            return new CommandBuilder<T>();
        }
        
        //public IEnumerable<T> Listar()
        //{
        //    return Listar(null);
        //}
    }
    
    public class CommandBuilder<T>
    {
        public IEnumerable<T> Execute()
        {
            return null;
        }
    }
}


