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

//Should the open and close connection be here? It's not flexible
//What about a fluent API -> Open, return itself, or a param open/close?

namespace TarkOrm.NET
{
    public partial class TarkDataAccess : IDisposable
    {
        public readonly IDbConnection _connection;
        private TarkQueryBuilder tarkQueryBuilder = new TarkQueryBuilder();
        private TarkTransformer tarkTransformer = new TarkTransformer();

        /// <summary>
        /// </summary>
        /// <param name="connection"></param>
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
            var connectionSetting = ConfigurationManager.ConnectionStrings[nameOrConnectionString];
            if (connectionSetting != null)
                nameOrConnectionString = connectionSetting.ConnectionString;

            _connection = new System.Data.SqlClient.SqlConnection(nameOrConnectionString);
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

        public virtual IEnumerable<T> GetAll<T>()
        {
            OpenConnection();

            var tablePath = tarkQueryBuilder.GetMapperTablePath<T>();

            using (IDbCommand cmd = _connection.CreateCommand())
            {
                cmd.CommandText = $"SELECT * FROM {tablePath}";
                cmd.CommandType = CommandType.Text;

                using (IDataReader dr = cmd.ExecuteReader())
                {
                    return tarkTransformer.ToList<T>(dr);
                }
            }
        }

        public virtual T GetById<T>(params object[] keyValues)
        {
            OpenConnection();

            var type = typeof(T);
            var tablePath = tarkQueryBuilder.GetMapperTablePath<T>();
            var mappedKeys = type.GetMappedOrderedKeys();

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

                using (IDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        return tarkTransformer.CreateObject<T>(dr);
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

            var tablePath = tarkQueryBuilder.GetMapperTablePath<T>();

            using (IDbCommand cmd = _connection.CreateCommand())
            {
                StringBuilder cmdColumns = new StringBuilder();
                StringBuilder cmdValues = new StringBuilder();

                var properties = entity.GetType().GetProperties();

                for (int i = 0; i < properties.Count(); i++)
                {
                    var columnName = properties[i].GetMappedColumnName();

                    if (properties[i].IsIdentityColumn())
                        continue;

                    //Column name appending
                    cmdColumns.Append(columnName);
                    cmdValues.Append($"@{ columnName }");

                    if (i != properties.Count() - 1)
                    {
                        cmdColumns.Append(", ");
                        cmdValues.Append(", ");
                    }
                    
                    //Uses ADO Sql Parameters in order to avoid SQL Injection attacks
                    var dbParam = cmd.CreateParameter();
                    dbParam.ParameterName = $"@{ columnName }";
                    dbParam.Value = properties[i].GetValue(entity);

                    cmd.Parameters.Add(dbParam);
                }

                cmd.CommandText = $"INSERT INTO {tablePath} ({cmdColumns.ToString()}) VALUES ({cmdValues.ToString()})";
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
            }
        }

        public void Dispose()
        {
            if (_connection.State != ConnectionState.Open)
                _connection.Close();

            _connection.Dispose();
        }
    }    
}


