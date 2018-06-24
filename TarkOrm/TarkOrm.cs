using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Data;
using TarkOrm.Extensions;
using System.Linq.Expressions;
using System.Configuration;
using TarkOrm.Mapping;

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
                    var result = Transformer.ToList<T>(dr);
                    dr.Close();
                    return result;
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

                    if (dbParam.Value is string)
                        dbParam.DbType = TarkConfigurationMapping.DefaultStringDbType; 

                    cmd.Parameters.Add(dbParam);
                }

                cmd.CommandText = $"SELECT * FROM {tablePath} WHERE {cmdFilter.ToString()}";
                cmd.CommandType = CommandType.Text;

                if (IsMockCommand(cmd))
                    return default(T);

                using (IDataReader dr = cmd.ExecuteReader())
                {
                    T result;

                    if (dr.Read())
                    {
                        result = Transformer.CreateObject<T>(dr);
                    }
                    else
                        result = default(T);

                    dr.Close();

                    return result;
                }
            }
        }
        
        public virtual bool ExistsById<T>(params object[] keyValues)
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

                    if (dbParam.Value is string)
                        dbParam.DbType = TarkConfigurationMapping.DefaultStringDbType;

                    cmd.Parameters.Add(dbParam);
                }

                cmd.CommandText = $"SELECT TOP 1 NULL FROM {tablePath} WHERE {cmdFilter.ToString()}";
                cmd.CommandType = CommandType.Text;

                if (IsMockCommand(cmd))
                    return false;

                using (IDataReader dr = cmd.ExecuteReader())
                {
                    var result = dr.Read();
                    dr.Close();
                    return result;
                }
            }
        }
        
        public virtual bool ExistsWhere<T, TProperty>(Expression<Func<T, TProperty>> propertyLambda, object value)
        {
            var typeMapping = (TarkTypeMapping<T>)TarkConfigurationMapping.ManageMapping<T>();
            var property = propertyLambda.GetPropertyInfo();

            KeyValuePair<string, TarkColumnMapping> columnMapping =
                typeMapping.PropertiesMappings.Where(x => x.Value.Property == property).FirstOrDefault();

            if (columnMapping.Equals(default(KeyValuePair<string, TarkColumnMapping>)))
                throw new InvalidFilterCriteriaException("Property mapping not found");

            var columnName = columnMapping.Key;

            OpenConnection();

            var tablePath = QueryBuilder.GetMapperTablePath<T>();
            var mappedKeys = typeMapping.GetMappedOrderedKeys();

            using (IDbCommand cmd = _connection.CreateCommand())
            {
                //Uses ADO Sql Parameters in order to avoid SQL Injection attacks
                var dbParam = cmd.CreateParameter();
                dbParam.ParameterName = $"@{ columnName }";
                dbParam.Value = value;

                if (dbParam.Value is string)
                    dbParam.DbType = TarkConfigurationMapping.DefaultStringDbType;

                cmd.Parameters.Add(dbParam);

                cmd.CommandText = $"SELECT * FROM {tablePath} WHERE {columnName} = @{columnName}";
                cmd.CommandType = CommandType.Text;

                if (IsMockCommand(cmd))
                    return false;

                using (IDataReader dr = cmd.ExecuteReader())
                {
                    var result = dr.Read();
                    dr.Close();
                    return result;
                }
            }
        }

        public virtual IEnumerable<T> GetWhere<T, TProperty>(Expression<Func<T, TProperty>> propertyLambda, object value)
        {
            var typeMapping = (TarkTypeMapping<T>)TarkConfigurationMapping.ManageMapping<T>();
            var property = propertyLambda.GetPropertyInfo();

            KeyValuePair<string,TarkColumnMapping> columnMapping = 
                typeMapping.PropertiesMappings.Where(x => x.Value.Property == property).FirstOrDefault();

            if (columnMapping.Equals(default(KeyValuePair<string, TarkColumnMapping>)))
                throw new InvalidFilterCriteriaException("Property mapping not found");

            var columnName = columnMapping.Key;

            OpenConnection();

            var tablePath = QueryBuilder.GetMapperTablePath<T>();
            var mappedKeys = typeMapping.GetMappedOrderedKeys();

            using (IDbCommand cmd = _connection.CreateCommand())
            {
                //Uses ADO Sql Parameters in order to avoid SQL Injection attacks
                var dbParam = cmd.CreateParameter();
                dbParam.ParameterName = $"@{ columnName }";
                dbParam.Value = value;

                if (dbParam.Value is string)
                    dbParam.DbType = TarkConfigurationMapping.DefaultStringDbType;

                cmd.Parameters.Add(dbParam);

                cmd.CommandText = $"SELECT * FROM {tablePath} WHERE {columnName} = @{columnName}";
                cmd.CommandType = CommandType.Text;

                if (IsMockCommand(cmd))
                    return null;

                using (IDataReader dr = cmd.ExecuteReader())
                {
                    var result = Transformer.ToList<T>(dr);
                    dr.Close();
                    return result;
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

                    var paramValue = item.Value.Property.GetValue(entity) ?? DBNull.Value;
                    dbParam.Value = paramValue;

                    if (dbParam.Value is string)
                        dbParam.DbType = TarkConfigurationMapping.DefaultStringDbType;

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
            var mappingKeys = typeMapping.PropertiesMappings.Where(x => x.Value.IsKeyColumn()).ToArray();
            
            StringBuilder cmdKeys = new StringBuilder();

            using (IDbCommand cmd = _connection.CreateCommand())
            {
                for (int i = 0; i < mappingKeys.Count(); i++)
                {
                    var columnName = mappingKeys[i].Key;

                    //Keys appending
                    cmdKeys.Append($"{columnName} = @{ columnName }");

                    if (i != mappingKeys.Count() - 1)
                        cmdKeys.Append(", ");

                    //Uses ADO Sql Parameters in order to avoid SQL Injection attacks
                    var dbParam = cmd.CreateParameter();
                    dbParam.ParameterName = $"@{ columnName }";
                    dbParam.Value = mappingKeys[i].Value.Property.GetValue(entity);

                    if (dbParam.Value is string)
                        dbParam.DbType = TarkConfigurationMapping.DefaultStringDbType;

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

                    if (dbParam.Value is string)
                        dbParam.DbType = TarkConfigurationMapping.DefaultStringDbType;

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
                var mappedKeys = typeMapping.PropertiesMappings.Where(x => x.Value.IsKeyColumn()).ToArray();
                var mappedNonKeys = typeMapping.PropertiesMappings.Where(x => !x.Value.IsKeyColumn()).ToArray();
                
                for (int i = 0; i < mappedNonKeys.Count(); i++)
                {
                    if (mappedNonKeys[i].Value.IsReadOnlyColumn())
                        continue;

                    var columnName = mappedNonKeys[i].Key;

                    //Column name appending
                    cmdUpdate.Append($"{columnName} = @{ columnName }");

                    if (i != mappedNonKeys.Count() - 1)
                        cmdUpdate.Append(", ");

                    //Uses ADO Sql Parameters in order to avoid SQL Injection attacks
                    var dbParam = cmd.CreateParameter();
                    dbParam.ParameterName = $"@{ columnName }";

                    var paramValue = mappedNonKeys[i].Value.Property.GetValue(entity) ?? DBNull.Value;
                    dbParam.Value = paramValue;

                    if (dbParam.Value is string)
                        dbParam.DbType = TarkConfigurationMapping.DefaultStringDbType;

                    cmd.Parameters.Add(dbParam);
                }

                for (int i = 0; i < mappedKeys.Count(); i++)
                {
                    var columnName = mappedKeys[i].Key;

                    //Keys appending
                    cmdKeys.Append($"{columnName} = @{ columnName }");

                    if (i != mappedKeys.Count() - 1)
                        cmdKeys.Append(", ");

                    //Uses ADO Sql Parameters in order to avoid SQL Injection attacks
                    var dbParam = cmd.CreateParameter();
                    dbParam.ParameterName = $"@{ columnName }";

                    var paramValue = mappedKeys[i].Value.Property.GetValue(entity) ?? DBNull.Value;
                    dbParam.Value = paramValue;

                    if (dbParam.Value is string)
                        dbParam.DbType = TarkConfigurationMapping.DefaultStringDbType;

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


