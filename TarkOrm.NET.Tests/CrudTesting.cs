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

            var tarkDataAcess = new TarkDataAccess("localhost");

            tarkDataAcess.Add<Country>(new Country
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


        [TestMethod]
        public void Delete()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            var tarkDataAcess = new TarkDataAccess("localhost");

            tarkDataAcess.RemoveById<Country>(247);

            watch.Stop();

            Debugger.Log(0, "", $"TarkORM Elapsed MS: {watch.ElapsedMilliseconds.ToString()}{Environment.NewLine}");
        }

    }
}
