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
    public class TableHintTests
    {
        [TestMethod]
        public void UnitTest_TableHints()
        {
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["localhost"].ConnectionString);
            
            var countries = connection.GetAll<Country>();

            var country = connection.GetById<Country>(246);

            var country3 = connection.WithTableHint(TableHints.SQLServer.NOLOCK).GetById<Country>(246);
        }
    }
}
