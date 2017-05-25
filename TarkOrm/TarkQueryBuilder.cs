using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TarkOrm.Attributes;

namespace TarkOrm
{

    // Stage 1 - Querying
    //TarkQueryBuilderClass?
    // - Query prepare (no params) query prepare can be similar 
    // - Query executing -> similar
    // - Receiving a DataReader
    public partial class TarkQueryBuilder
    {
        public TarkOrm _tarkDataAccess;

        public TarkQueryBuilder(TarkOrm tarkDataAccess)
        {
            _tarkDataAccess = tarkDataAccess;
        }

        public TarkQueryBuilderMocker GetMockCommand()
        {
            return new TarkQueryBuilderMocker(_tarkDataAccess);
        }

        public string TableHint;
        
        public string GetMapperTablePath<T>()
        {
            Type type = typeof(T);
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
                return String.Format("{0}.{1}.{2} {3}", databaseName, schema, tableName, TableHint);
            }
            else
            {
                return String.Format("{0}.{1} {2}", schema, tableName, TableHint);
            }
        }

        public string GetProcedureName<T>()
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
                //TODO: Move the procedure name pattern to a external config file, maybe it could check if a procedure exists before using the default method
                return String.Format("{0}.{1}.sp_{2}_GetAll", databaseName, schema, tableName);
            }
            else
            {
                return String.Format("{1}.p_{2}_GetAll", schema, tableName);
            }
        }
    }

    public class TarkQueryBuilderMocker
    {
        public TarkOrm _tarkDataAccess;

        public TarkQueryBuilderMocker(TarkOrm tarkDataAccess)
        {
            _tarkDataAccess = tarkDataAccess;
        }

        public IDbCommand GetById<T>(params object[] keyValues)
        {
            _tarkDataAccess.MockEnabled = true;
            _tarkDataAccess.GetById<T>(keyValues);
            return _tarkDataAccess.MockCommand;
        }

        public IDbCommand GetAll<T>()
        {
            _tarkDataAccess.MockEnabled = true;
            _tarkDataAccess.GetAll<T>();
            return _tarkDataAccess.MockCommand;
        }

        public IDbCommand Add<T>(T entity)
        {
            _tarkDataAccess.MockEnabled = true;
            _tarkDataAccess.Add(entity);
            return _tarkDataAccess.MockCommand;
        }

        public IDbCommand Update<T>(T entity)
        {
            _tarkDataAccess.MockEnabled = true;
            _tarkDataAccess.Update(entity);
            return _tarkDataAccess.MockCommand;
        }

        public IDbCommand Remove<T>(T entity)
        {
            _tarkDataAccess.MockEnabled = true;
            _tarkDataAccess.Remove(entity);
            return _tarkDataAccess.MockCommand;
        }

        public IDbCommand RemoveById<T>(params object[] keyValues)
        {
            _tarkDataAccess.MockEnabled = true;
            _tarkDataAccess.RemoveById<T>(keyValues);
            return _tarkDataAccess.MockCommand;
        }
    }
    

    public static class TableHints
    {        
        public static class SQLServer
        {
            public static string NOLOCK = "WITH(NOLOCK)";
        }
    }
}
