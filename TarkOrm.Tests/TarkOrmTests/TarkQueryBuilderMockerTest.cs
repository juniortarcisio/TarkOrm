using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;
using TarkOrm;

namespace TarkOrm.Tests
{
    [TestClass]
    public class TarkQueryBuilderMockerTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["localhost"].ConnectionString);

            var cmd = connection.GetMockCommand().GetById<Country>(256);

            var squery = cmd.CommandText;
        }
    }
}
