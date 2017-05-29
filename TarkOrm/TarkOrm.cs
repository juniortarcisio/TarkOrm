using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Data;
using TarkOrm.Extensions;
using TarkOrm.Attributes;
using System.Linq.Expressions;
using System.Configuration;

//Should the open and close connection be here? It's not flexible
//What about a fluent API -> Open, return itself, or a param open/close?

namespace TarkOrm
{
    public partial class TarkOrm : IDisposable
    {
        public readonly IDbConnection _connection;
        public TarkQueryBuilder QueryBuilder { get; set; }
        public TarkTransformer Transformer { get; set; }
        public IDbCommand MockCommand { get; set; }
        public bool MockEnabled { get; set; }

        /// <summary>
        /// </summary>
        /// <param name="connection"></param>
        public TarkOrm(IDbConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");

            _connection = connection;
            QueryBuilder = new TarkQueryBuilder(this);
            Transformer = new TarkTransformer();
        }

        /// <summary>
        /// </summary>
        /// <param name="nameOrConnectionString">Either the database name or a connection string.</param>
        public TarkOrm(string nameOrConnectionString)
        {
            var connectionSetting = ConfigurationManager.ConnectionStrings[nameOrConnectionString];
            if (connectionSetting != null)
                nameOrConnectionString = connectionSetting.ConnectionString;

            _connection = new System.Data.SqlClient.SqlConnection(nameOrConnectionString);
            QueryBuilder = new TarkQueryBuilder(this);
            Transformer = new TarkTransformer();
        }

        private void OpenConnection()
        {
            if (_connection.State != ConnectionState.Open)
            {
                if (_connection.State == ConnectionState.Closed)
                    _connection.Open();
                else
                    throw new DataException("Invalid connection state");
            }
        }

        protected bool IsMockCommand(IDbCommand cmd)
        {
            if (MockEnabled)
            {
                MockCommand = cmd;
                return true;
            }
            else {
                if (MockCommand != null)
                {
                    MockCommand = null;
                    //mockCommand.Dispose();
                }
                return false;
            }
        }

        public virtual IEnumerable<T> GetAll<T>()
        {
            OpenConnection();

            var tablePath = QueryBuilder.GetMapperTablePath<T>();

            using (IDbCommand cmd = _connection.CreateCommand())
            {
                cmd.CommandText = $"SELECT * FROM {tablePath}";
                cmd.CommandType = CommandType.Text;

                if (IsMockCommand(cmd))
                    return null;

                using (IDataReader dr = cmd.ExecuteReader())
                {
                    return Transformer.ToList<T>(dr);
                }
            }
        }

        public virtual T GetById<T>(params object[] keyValues)
        {
            OpenConnection();

            var type = typeof(T);
            var tablePath = QueryBuilder.GetMapperTablePath<T>();

            var typeMapping = (TarkTypeMapping<T>)TarkConfigurationMapping.ManageMapping<T>();
            var mappedKeys = typeMapping.GetMappedOrderedKeys();

            if (keyValues.Count() == 0 || mappedKeys.Count() != keyValues.Length)
                throw new MissingPrimaryKeyException();
            
            using (IDbCommand cmd = _connection.CreateCommand())
            {
                StringBuilder cmdFilter = new StringBuilder();
                
                for (int i = 0; i < keyValues.Count(); i++)
                {
                    cmdFilter.Append($"{ mappedKeys[i] } = @{ mappedKeys[i] } ");

                    if (i != keyValues.Count() - 1)
                        cmdFilter.Append("AND ");

                    //Uses ADO Sql Parameters in order to avoid SQL Injection attacks
                    var dbParam = cmd.CreateParameter();
                    dbParam.ParameterName = $"@{ mappedKeys[i] }";
                    dbParam.Value = keyValues[i];

                    cmd.Parameters.Add(dbParam);
                }

                cmd.CommandText = $"SELECT * FROM {tablePath} WHERE {cmdFilter.ToString()}";
                cmd.CommandType = CommandType.Text;

                if (IsMockCommand(cmd))
                    return default(T);

                using (IDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        return Transformer.CreateObject<T>(dr);
                    }
                    else
                        return default(T);
                }
            }
        }
        
        public virtual bool Exists<T>(params object[] keyValues)
        {
            OpenConnection();

            var type = typeof(T);
            var tablePath = QueryBuilder.GetMapperTablePath<T>();

            var typeMapping = (TarkTypeMapping<T>)TarkConfigurationMapping.ManageMapping<T>();
            var mappedKeys = typeMapping.GetMappedOrderedKeys();

            if (keyValues.Count() == 0 || mappedKeys.Count() != keyValues.Length)
                throw new MissingPrimaryKeyException();

            using (IDbCommand cmd = _connection.CreateCommand())
            {
                StringBuilder cmdFilter = new StringBuilder();

                for (int i = 0; i < keyValues.Count(); i++)
                {
                    cmdFilter.Append($"{ mappedKeys[i] } = @{ mappedKeys[i] } ");

                    if (i != keyValues.Count() - 1)
                        cmdFilter.Append("AND ");

                    //Uses ADO Sql Parameters in order to avoid SQL Injection attacks
                    var dbParam = cmd.CreateParameter();
                    dbParam.ParameterName = $"@{ mappedKeys[i] }";
                    dbParam.Value = keyValues[i];

                    cmd.Parameters.Add(dbParam);
                }

                cmd.CommandText = $"SELECT TOP 1 NULL FROM {tablePath} WHERE {cmdFilter.ToString()}";
                cmd.CommandType = CommandType.Text;

                if (IsMockCommand(cmd))
                    return false;

                using (IDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        return true;
                    }
                    else
                        return false;
                }
            }
        }

        public virtual T GetWhere<T, TProperty>(Expression<Func<T, TProperty>> propertyLambda, object value)
        {
            var property = propertyLambda.GetPropertyInfo();
            var columnName = property.GetMappedColumnName();

            if (columnName == null)
                throw new InvalidFilterCriteriaException("Property mapping not found");

            OpenConnection();

            var type = typeof(T);
            var tablePath = QueryBuilder.GetMapperTablePath<T>();

            var typeMapping = (TarkTypeMapping<T>)TarkConfigurationMapping.ManageMapping<T>();
            var mappedKeys = typeMapping.GetMappedOrderedKeys();

            using (IDbCommand cmd = _connection.CreateCommand())
            {
                //Uses ADO Sql Parameters in order to avoid SQL Injection attacks
                var dbParam = cmd.CreateParameter();
                dbParam.ParameterName = $"@{ columnName }";
                dbParam.Value = value;

                cmd.Parameters.Add(dbParam);

                cmd.CommandText = $"SELECT * FROM {tablePath} WHERE { columnName } = @{ columnName }";
                cmd.CommandType = CommandType.Text;

                if (IsMockCommand(cmd))
                    return default(T);

                using (IDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        return Transformer.CreateObject<T>(dr);
                    }
                    else
                        return default(T);
                }
            }
        }

        public virtual void Add<T>(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            OpenConnection();

            var tablePath = QueryBuilder.GetMapperTablePath<T>();

            using (IDbCommand cmd = _connection.CreateCommand())
            {
                StringBuilder cmdColumns = new StringBuilder();
                StringBuilder cmdValues = new StringBuilder();

                var properties = entity.GetType().GetProperties();

                var typeMapping = (TarkTypeMapping<T>)TarkConfigurationMapping.ManageMapping<T>();

                var lastItem = typeMapping.PropertiesMappings.Last();                
                foreach (var item in typeMapping.PropertiesMappings)
                {

                    var columnName = item.Key;

                    if (item.Value.IsReadOnlyColumn())
                        continue;

                    //Column name appending
                    cmdColumns.Append(columnName);
                    cmdValues.Append($"@{ columnName }");

                    if (item.Key != lastItem.Key)
                    {
                        cmdColumns.Append(", ");
                        cmdValues.Append(", ");
                    }

                    //Uses ADO Sql Parameters in order to avoid SQL Injection attacks
                    var dbParam = cmd.CreateParameter();
                    dbParam.ParameterName = $"@{ columnName }";

                    var paramValue = item.Value.GetValue(entity) ?? DBNull.Value;
                    dbParam.Value = paramValue;

                    cmd.Parameters.Add(dbParam);
                }

                cmd.CommandText = $"INSERT INTO {tablePath} ({cmdColumns.ToString()}) VALUES ({cmdValues.ToString()})";
                cmd.CommandType = CommandType.Text;

                if (IsMockCommand(cmd))
                    return;

                cmd.ExecuteNonQuery();
            }
        }
        
        public virtual void Remove<T>(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            OpenConnection();

            var type = typeof(T);
            var tablePath = QueryBuilder.GetMapperTablePath<T>();

            var typeMapping = (TarkTypeMapping<T>)TarkConfigurationMapping.ManageMapping<T>();
            var propertiesKey = typeMapping.PropertiesMappings.Values.Where(x => x.IsKeyColumn()).ToArray();

            StringBuilder cmdKeys = new StringBuilder();

            using (IDbCommand cmd = _connection.CreateCommand())
            {
                for (int i = 0; i < propertiesKey.Count(); i++)
                {
                    var columnName = propertiesKey[i].GetMappedColumnName();

                    //Keys appending
                    cmdKeys.Append($"{columnName} = @{ columnName }");

                    if (i != propertiesKey.Count() - 1)
                        cmdKeys.Append(", ");

                    //Uses ADO Sql Parameters in order to avoid SQL Injection attacks
                    var dbParam = cmd.CreateParameter();
                    dbParam.ParameterName = $"@{ columnName }";
                    dbParam.Value = propertiesKey[i].GetValue(entity);

                    cmd.Parameters.Add(dbParam);
                }

                cmd.CommandText = $"DELETE {tablePath} WHERE {cmdKeys.ToString()}";
                cmd.CommandType = CommandType.Text;

                if (IsMockCommand(cmd))
                    return;

                cmd.ExecuteNonQuery();
            }
        }

        public virtual void RemoveById<T>(params object[] keyValues)
        {
            OpenConnection();

            var type = typeof(T);
            var tablePath = QueryBuilder.GetMapperTablePath<T>();

            var typeMapping = (TarkTypeMapping<T>)TarkConfigurationMapping.ManageMapping<T>();
            var mappedKeys = typeMapping.GetMappedOrderedKeys();

            if (keyValues.Count() == 0 || mappedKeys.Count() != keyValues.Length)
                throw new MissingPrimaryKeyException();

            using (IDbCommand cmd = _connection.CreateCommand())
            {
                StringBuilder cmdFilter = new StringBuilder();

                for (int i = 0; i < keyValues.Count(); i++)
                {
                    cmdFilter.Append($"{ mappedKeys[i] } = @{ mappedKeys[i] } ");

                    if (i != keyValues.Count() - 1)
                        cmdFilter.Append("AND ");

                    //Uses ADO Sql Parameters in order to avoid SQL Injection attacks
                    var dbParam = cmd.CreateParameter();
                    dbParam.ParameterName = $"@{ mappedKeys[i] }";
                    dbParam.Value = keyValues[i];

                    cmd.Parameters.Add(dbParam);
                }

                cmd.CommandText = $"DELETE {tablePath} WHERE {cmdFilter.ToString()}";
                cmd.CommandType = CommandType.Text;

                if (IsMockCommand(cmd))
                    return;

                cmd.ExecuteNonQuery();
            }
        }

        public virtual void Update<T>(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            OpenConnection();

            var tablePath = QueryBuilder.GetMapperTablePath<T>();

            using (IDbCommand cmd = _connection.CreateCommand())
            {
                StringBuilder cmdUpdate = new StringBuilder();
                StringBuilder cmdKeys = new StringBuilder();

                var typeMapping = (TarkTypeMapping<T>)TarkConfigurationMapping.ManageMapping<T>();

                var properties = entity.GetType().GetProperties();
                var propertiesKey = typeMapping.PropertiesMappings.Values.Where(x => x.IsKeyColumn()).ToArray();
                var propertiesNonKey = typeMapping.PropertiesMappings.Values.Where(x => !x.IsKeyColumn()).ToArray();

                for (int i = 0; i < propertiesNonKey.Count(); i++)
                {
                    if (propertiesNonKey[i].IsReadOnlyColumn() || propertiesNonKey[i].IsIgnoreMappingColumn())
                        continue;

                    var columnName = propertiesNonKey[i].GetMappedColumnName();

                    //Column name appending
                    cmdUpdate.Append($"{columnName} = @{ columnName }");

                    if (i != propertiesNonKey.Count() - 1)
                        cmdUpdate.Append(", ");

                    //Uses ADO Sql Parameters in order to avoid SQL Injection attacks
                    var dbParam = cmd.CreateParameter();
                    dbParam.ParameterName = $"@{ columnName }";

                    var paramValue = propertiesNonKey[i].GetValue(entity) ?? DBNull.Value;
                    dbParam.Value = paramValue;

                    cmd.Parameters.Add(dbParam);
                }

                for (int i = 0; i < propertiesKey.Count(); i++)
                {
                    var columnName = propertiesKey[i].GetMappedColumnName();

                    //Keys appending
                    cmdKeys.Append($"{columnName} = @{ columnName }");

                    if (i != propertiesKey.Count() - 1)
                        cmdKeys.Append(", ");

                    //Uses ADO Sql Parameters in order to avoid SQL Injection attacks
                    var dbParam = cmd.CreateParameter();
                    dbParam.ParameterName = $"@{ columnName }";

                    var paramValue = propertiesKey[i].GetValue(entity) ?? DBNull.Value;
                    dbParam.Value = paramValue;

                    cmd.Parameters.Add(dbParam);
                }

                cmd.CommandText = $"UPDATE {tablePath} SET {cmdUpdate.ToString()} WHERE {cmdKeys.ToString()}";
                cmd.CommandType = CommandType.Text;

                if (IsMockCommand(cmd))
                    return;

                cmd.ExecuteNonQuery();
            }
        }
        
        public void Dispose()
        {
            if (_connection.State != ConnectionState.Open)
                _connection.Close();
        }
    }    
}


