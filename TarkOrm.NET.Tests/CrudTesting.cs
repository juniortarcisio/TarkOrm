using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace TarkOrm.NET.Tests
{
    [TestClass]
    public class CrudTesting
    {
        [TestMethod]
        public void Insert()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            var data = new TarkDataAccess("data source=PH03N1XR4V4N-PC\\DBLABS;initial catalog=MyPortal;persist security info=True;user id=app_login;password=ph03n1xr4v3n;MultipleActiveResultSets=True;App=TarkOrm.NET");

            //data.GetAll<Country>();
            data.Add<Country>(new Country
            {
                CountryID = 247,
                ContinentID = 1,
                CountryCode = "ND",
                CurrencyID = 1,
                FlagB64 = "",
                Name = "Testing Country"
            });

            watch.Stop();

            Debugger.Log(0, "", $"TarkORM Elapsed MS: {watch.ElapsedMilliseconds.ToString()}{Environment.NewLine}");
        }

    }
}
