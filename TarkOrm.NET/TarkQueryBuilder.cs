using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TarkOrm.NET.Attributes;

namespace TarkOrm.NET
{

    // Stage 1 - Querying
    //TarkQueryBuilderClass?
    // - Query prepare (no params) query prepare can be similar 
    // - Query executing -> similar
    // - Receiving a DataReader
    public partial class TarkQueryBuilder
    {
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
                return String.Format("{0}.{1}.{2}", databaseName, schema, tableName);
            }
            else
            {
                return String.Format("{0}.{1}", schema, tableName);
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
}
