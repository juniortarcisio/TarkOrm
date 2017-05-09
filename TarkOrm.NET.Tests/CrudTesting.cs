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
        public void Create()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            var tarkDataAcess = new TarkDataAccess("localhost");

            tarkDataAcess.Add(new Country
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

            var tarkDataAcess = new TarkDataAccess("localhost");
            var countryList = tarkDataAcess.GetAll<Country>();
            var country = tarkDataAcess.GetById<Country>(30);


            watch.Stop();

            Debugger.Log(0, "", $"TarkORM Elapsed MS: {watch.ElapsedMilliseconds.ToString()}{Environment.NewLine}");
        }


        [TestMethod]
        public void Update()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            var tarkDataAcess = new TarkDataAccess("localhost");

            var country = tarkDataAcess.GetById<Country>(247);
            country.Name = "Testing Country Update2";
            country.ContinentID = 3;
            country.CountryCode = "XX";
            country.CurrencyID = 3;
            country.FlagB64 = "nd";
            tarkDataAcess.Update(country);

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
