using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace TarkOrm
{
    public static class TarkDataExtensions
    {
        public static TarkOrm WithTableHint(this IDbConnection connection, string tableHint)
        {
            var dataAccess = new TarkOrm(connection);
            dataAccess.QueryBuilder.TableHint = tableHint;
            return dataAccess;
        }

        public static IEnumerable<T> GetWhere<T, TProperty>(this IDbConnection connection, Expression<Func<T, TProperty>> propertyLambda, object value)
        {
            using (var dataAccess = new TarkOrm(connection))
            {
                return dataAccess.GetWhere(propertyLambda, value);
            }
        }

        public static IEnumerable<T> GetAll<T>(this IDbConnection connection)
        {
            using (var dataAccess = new TarkOrm(connection))
            {
                return dataAccess.GetAll<T>();
            }
        }

        public static T GetById<T>(this IDbConnection connection, params object[] keyValues)
        {
            using (var dataAccess = new TarkOrm(connection))
            {
                return dataAccess.GetById<T>(keyValues);
            }
        }

        public static bool Exists<T>(this IDbConnection connection, params object[] keyValues)
        {
            using (var dataAccess = new TarkOrm(connection))
            {
                return dataAccess.Exists<T>(keyValues);
            }
        }

        public static void Add<T>(this IDbConnection connection, T entity)
        {
            using (var dataAccess = new TarkOrm(connection))
            {
                dataAccess.Add<T>(entity);
            }
        }

        public static void Update<T>(this IDbConnection connection, T entity)
        {
            using (var dataAccess = new TarkOrm(connection))
            {
                dataAccess.Update<T>(entity);
            }
        }

        public static void Remove<T>(this IDbConnection connection, T entity)
        {
            using (var dataAccess = new TarkOrm(connection))
            {
                dataAccess.Remove<T>(entity);
            }
        }

        public static void RemoveById<T>(this IDbConnection connection, params object[] keyValues)
        {
            using (var dataAccess = new TarkOrm(connection))
            {
                dataAccess.RemoveById<T>(keyValues);
            }
        }

        public static TarkQueryBuilderMocker GetMockCommand(this IDbConnection connection)
        {
            using (var dataAccess = new TarkOrm(connection))
            {
                return dataAccess.QueryBuilder.GetMockCommand();
            }
        }

        public static T ToObject<T>(this DataRow row)
        {
            return new TarkOrm(string.Empty).Transformer.CreateObject<T>(row);
        }

        public static T ToObject<T>(this IDataReader dr)
        {
            if (dr.Read())
                return new TarkOrm(string.Empty).Transformer.CreateObject<T>(dr);
            else
                return default(T);
        }
        
        public static IEnumerable<T> ToObjectList<T>(this IDataReader dr)
        {
                return new TarkOrm(string.Empty).Transformer.ToList<T>(dr);
        }


        public static IEnumerable<T> ToObjectList<T>(this DataTable dt)
        {
            return new TarkOrm(string.Empty).Transformer.ToList<T>(dt);
        }
    }
}
