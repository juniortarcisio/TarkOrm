using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TarkOrm.NET.Tests
{
    [TestClass]
    public class PrototypingCmdBuilder
    {
        delegate int del(DateTime abacate);
        
        [TestMethod]
        public void TestMethod1()
        {
            //var con = new DataConn
            //SqlConnection con = new SqlConnection();

            var data = new TarkDataAccess("data source=PH03N1XR4V4N-PC\\DBLABS;initial catalog=MyPortal;persist security info=True;user id=app_login;password=ph03n1xr4v3n;MultipleActiveResultSets=True;App=TarkOrm.NET");

            var lista = data.GetAll<Country>();
            
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

            //var xpto = GetPropertyInfo(item, w => w.Name);
        }

    }
}
