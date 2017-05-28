using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.Linq.Expressions;

namespace TarkOrm.Tests
{
    [TestClass]
    public class PrototypingCmdBuilder
    {
        delegate int del(DateTime abacate);
        
        [TestMethod]
        public void TestGetWhere()
        {
            //var con = new DataConn
            //SqlConnection con = new SqlConnection();

            var data = new TarkOrm("data source=PH03N1XR4V4N-PC\\DBLABS;initial catalog=MyPortal;persist security info=True;user id=app_login;password=ph03n1xr4v3n;MultipleActiveResultSets=True;App=TarkOrm");

            //var item = data.GetById<Country>(10, "AR");

            //Creates a Where function that receives a lambda
            // the returns of lambda is a field / field property
            // Where will adds those fields to a parameter list
            // the final return of the function is a command

            //Command has a property "Execute", that triggers the query

            //var filtered = data.Where(x => x.field, value).Execute();
            //var filtered = data.Where(x => x.field.property???, value).Execute();

            //var filtered = data.Where("colname", value) (?)
            //var a = new Country();

            ////var filtered = data.Where<Country>(1).Execute();

            ////var rowsAffected data.Insert(new country...);
            //del x = ((y) => 1 * 2);

            ////var x = () => 1.ToString();
            //Func<int, bool> myfunc = (q => q == 6);

            //Func<int, string> myfunc2 = (q => q.ToString());

            ////Commands stack before executing? .Execute?

            var item = data.GetById<Country>(10);
            var xpto = GetPropertyInfo(item, w => w.Name);

            var x = data.GetWhere<Country, string>(y=>y.Name, "Brazil");

        }

        public PropertyInfo GetPropertyInfo<TSource, TProperty>(
            TSource source,
            Expression<Func<TSource, TProperty>> propertyLambda)
        {
            Type type = typeof(TSource);

            MemberExpression member = propertyLambda.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    propertyLambda.ToString()));

            PropertyInfo propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a field, not a property.",
                    propertyLambda.ToString()));

            if (type != propInfo.ReflectedType &&
                !type.IsSubclassOf(propInfo.ReflectedType))
                throw new ArgumentException(string.Format(
                    "Expresion '{0}' refers to a property that is not from type {1}.",
                    propertyLambda.ToString(),
                    type));

            return propInfo;
        }

    }
}
