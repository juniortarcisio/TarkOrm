using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace TarkOrm.Tests
{
    [TestClass]
    public class CrudTesting
    {
        [TestMethod]
        public void Create()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            var tarkOrm = new TarkOrm("localhost");

            if (tarkOrm.GetById<Country>(247) != null)
                tarkOrm.RemoveById<Country>(247);

            tarkOrm.Add(new Country
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
        public void Read()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            var tarkOrm = new TarkOrm("localhost");
            var countryList = tarkOrm.GetAll<Country>();
            var country = tarkOrm.GetById<Country>(30);


            watch.Stop();

            Debugger.Log(0, "", $"TarkORM Elapsed MS: {watch.ElapsedMilliseconds.ToString()}{Environment.NewLine}");
        }

        [TestMethod]
        public void Update()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            var tarkOrm = new TarkOrm("localhost");

            var country = tarkOrm.GetById<Country>(247);
            country.Name = "Testing Country Update2";
            country.ContinentID = 3;
            country.CountryCode = "XX";
            country.CurrencyID = 3;
            country.FlagB64 = "nd";
            tarkOrm.Update(country);

            watch.Stop();

            Debugger.Log(0, "", $"TarkORM Elapsed MS: {watch.ElapsedMilliseconds.ToString()}{Environment.NewLine}");
        }

        [TestMethod]
        public void Delete()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            var tarkOrm = new TarkOrm("localhost");
            tarkOrm.RemoveById<Country>(247);

            watch.Stop();

            Debugger.Log(0, "", $"TarkORM Elapsed MS: {watch.ElapsedMilliseconds.ToString()}{Environment.NewLine}");
        }

    }
}
