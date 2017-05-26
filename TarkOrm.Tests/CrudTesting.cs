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

            var countryPartial = tarkOrm.GetById<CountryPartial>(30);

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
            country.Name = "Testing Country Update3";
            country.ContinentID = 2;
            country.CountryCode = "YX";
            country.CurrencyID = 2;
            country.FlagB64 = "ND";
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
        
        [TestMethod]
        public void CreatePartial()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            var tarkOrm = new TarkOrm("localhost");

            tarkOrm.Add(new TestOrm
            {
                Name = "Polskyman"            
            });

            watch.Stop();

            Debugger.Log(0, "", $"TarkORM Elapsed MS: {watch.ElapsedMilliseconds.ToString()}{Environment.NewLine}");
        }
    }
}
