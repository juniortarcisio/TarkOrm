using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Entity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using Dapper;

namespace TarkOrm.Tests
{
    [TestClass]
    public class Benchmarks
    {
        [TestMethod]
        public void Benchmark1()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            //Entity
            var listEntity = new DbLabsContext().Countries;
            foreach (var item in listEntity)
            {
                item.Name.ToString();
            }

            watch.Stop();

            Debugger.Log(0, "", $"Entity Elapsed MS: {watch.ElapsedMilliseconds.ToString()}{Environment.NewLine}" );

            watch.Reset();
            watch.Start();

            //Tark
            var data = new TarkOrm("data source=PH03N1XR4V4N-PC\\DBLABS;initial catalog=MyPortal;persist security info=True;user id=app_login;password=ph03n1xr4v3n;MultipleActiveResultSets=True;App=TarkOrm");

            var lista = data.GetAll<Country>();

            foreach (var item in lista)
            {
                item.Name.ToString();
            }

            watch.Stop();

            Debugger.Log(0, "", $"TarkORM Elapsed MS: {watch.ElapsedMilliseconds.ToString()}{Environment.NewLine}");

            watch.Reset();
            watch.Start();

            //Dapper
            IEnumerable<Country> listaDapper = data._connection.Query<Country>("SELECT * FROM Country");

            foreach (var item in listaDapper)
            {
                item.Name.ToString();
            }

            watch.Stop();

            Debugger.Log(0, "", $"Dapper Elapsed MS: {watch.ElapsedMilliseconds.ToString()}{Environment.NewLine}");

        }


        [TestMethod]
        public void BenchmarkCountryTark()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            //Tark
            var data = new TarkOrm("data source=PH03N1XR4V4N-PC\\DBLABS;initial catalog=MyPortal;persist security info=True;user id=app_login;password=ph03n1xr4v3n;MultipleActiveResultSets=True;App=TarkOrm");

            var lista = data.GetAll<Country>();

            foreach (var item in lista)
            {
                item.Name.ToString();
            }

            watch.Stop();

            Debugger.Log(0, "", $"TarkORM Elapsed MS: {watch.ElapsedMilliseconds.ToString()}{Environment.NewLine}");
        }


        [TestMethod]
        public void BenchmarkCountryDapper()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            var data = new TarkOrm("data source=PH03N1XR4V4N-PC\\DBLABS;initial catalog=MyPortal;persist security info=True;user id=app_login;password=ph03n1xr4v3n;MultipleActiveResultSets=True;App=TarkOrm");

            //Dapper
            IEnumerable<Country> listaDapper = data._connection.Query<Country>("SELECT * FROM Country");

            foreach (var item in listaDapper)
            {
                item.Name.ToString();
            }

            watch.Stop();

            Debugger.Log(0, "", $"Dapper Elapsed MS: {watch.ElapsedMilliseconds.ToString()}{Environment.NewLine}");
        }

        [TestMethod]
        public void BenchmarkCountryADO()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            var con = new System.Data.SqlClient.SqlConnection("data source=PH03N1XR4V4N-PC\\DBLABS;initial catalog=MyPortal;persist security info=True;user id=app_login;password=ph03n1xr4v3n;MultipleActiveResultSets=True;App=TarkOrm");

            var cmd = con.CreateCommand();

            cmd.CommandText = "SELECT * FROM Country";
            cmd.CommandType = CommandType.Text;
            con.Open();

            var dr = cmd.ExecuteReader();

            List<Country> listaADO = new List<Country>();

            while (dr.Read())
            {
                var country = new Country();
                country.CountryID = (int)dr["CountryID"];
                country.Name = (string)dr["Name"];
                country.ContinentID = (int)dr["ContinentID"];
                country.CountryCode = (string)dr["CountryCode"];

                if (dr["CurrencyID"] != DBNull.Value)
                    country.CurrencyID = (int)dr["CurrencyID"];

                if (dr["FlagB64"] != DBNull.Value)
                    country.FlagB64 = (string)dr["FlagB64"];
            }

            foreach (var item in listaADO)
            {
                item.Name.ToString();
            }

            watch.Stop();

            Debugger.Log(0, "", $"Dapper Elapsed MS: {watch.ElapsedMilliseconds.ToString()}{Environment.NewLine}");
        }


        [TestMethod]
        public void BenchmarkCountryEntity()
        {
            Stopwatch watch = new Stopwatch();

            watch.Start();

            var listEntity = new DbLabsContext().Countries;
            foreach (var item in listEntity)
            {
                var x = item.Name.ToString();
            }

            watch.Stop();

            Debugger.Log(0, "", $"Entity Elapsed MS: {watch.ElapsedMilliseconds.ToString()}{Environment.NewLine}");
        }

        [TestMethod]
        public void Benchmark2()
        {

            Stopwatch watch = new Stopwatch();

            var listEntity = new DbLabsContext().Countries;

            watch.Start();

            foreach (var item in listEntity)
            {
                item.Name.ToString();
            }

            watch.Stop();

            Debugger.Log(0, "", $"Entity Elapsed MS: {watch.ElapsedMilliseconds.ToString()}{Environment.NewLine}");

            watch.Reset();

            var data = new TarkOrm("data source=PH03N1XR4V4N-PC\\DBLABS;initial catalog=MyPortal;persist security info=True;user id=app_login;password=ph03n1xr4v3n;MultipleActiveResultSets=True;App=TarkOrm");

            watch.Start();
            var lista = data.GetAll<Country>();

            foreach (var item in lista)
            {
                item.Name.ToString();
            }

            watch.Stop();

            Debugger.Log(0, "", $"TarkORM Elapsed MS: {watch.ElapsedMilliseconds.ToString()}{Environment.NewLine}");

            var x = 2;
        }

        [TestMethod]
        public void BenchmarkCityEntity()
        {
            Stopwatch watch = new Stopwatch();

            watch.Start();

            var listEntity = new DbLabsContext().Cities;
            foreach (var item in listEntity)
            {
                var x = item.Name.ToString();
            }

            watch.Stop();

            Debugger.Log(0, "", $"Entity Elapsed MS: {watch.ElapsedMilliseconds.ToString()}{Environment.NewLine}");            
        }

        [TestMethod]
        public void BenchmarkCityADONET()
        {
            Stopwatch watch = new Stopwatch();
            //ADO.NET raw
            watch.Start();

            var con = new System.Data.SqlClient.SqlConnection("data source=PH03N1XR4V4N-PC\\DBLABS;initial catalog=MyPortal;persist security info=True;user id=app_login;password=ph03n1xr4v3n;MultipleActiveResultSets=True;App=TarkOrm");

            var cmd = con.CreateCommand();

            cmd.CommandText = "SELECT * FROM City";
            cmd.CommandType = CommandType.Text;
            con.Open();

            var dr = cmd.ExecuteReader();

            List<City> listaado = new List<City>();

            while (dr.Read())
            {
                listaado.Add(new City()
                {
                    CityID = (int)dr["CityID"],
                    Name = (string)dr["Name"],
                    ProvinceID = (int)dr["ProvinceID"]
                });
            }

            foreach (var item in listaado)
            {
                var x = item.CityID;
            }
            
            watch.Stop();

            Debugger.Log(0, "", $"ADO.NET Elapsed MS: {watch.ElapsedMilliseconds.ToString()}{Environment.NewLine}");
        }

        [TestMethod]
        public void BenchmarkCityTarkORM()
        {
            Stopwatch watch = new Stopwatch();
            //TarkORM
            watch.Reset();
            watch.Start();

            var data = new TarkOrm("data source=PH03N1XR4V4N-PC\\DBLABS;initial catalog=MyPortal;persist security info=True;user id=app_login;password=ph03n1xr4v3n;MultipleActiveResultSets=True;App=TarkOrm");

            var lista = data.GetAll<City>();

            foreach (var item in lista)
            {
                var x = item.CityID;
            }

            watch.Stop();

            Debugger.Log(0, "", $"TarkORM Elapsed MS: {watch.ElapsedMilliseconds.ToString()}{Environment.NewLine}");
        }

        [TestMethod]
        public void BenchmarkCityDapper()
        {
            Stopwatch watch = new Stopwatch();
            //ADO.NET raw
            watch.Start();

            var con = new System.Data.SqlClient.SqlConnection("data source=PH03N1XR4V4N-PC\\DBLABS;initial catalog=MyPortal;persist security info=True;user id=app_login;password=ph03n1xr4v3n;MultipleActiveResultSets=True;App=TarkOrm");

            var cmd = con.CreateCommand();

            cmd.CommandText = "SELECT * FROM City";
            cmd.CommandType = CommandType.Text;
            con.Open();
            
            IEnumerable<City> listaDapper = con.Query<City>("SELECT * FROM City");

            foreach (var item in listaDapper)
            {
                var x = item.CityID;
            }

            watch.Stop();

            Debugger.Log(0, "", $"Dapper Elapsed MS: {watch.ElapsedMilliseconds.ToString()}{Environment.NewLine}");
        }

        [TestMethod]
        public void Benchmark4()
        {
            Stopwatch watch = new Stopwatch();

            watch.Start();

            var listEntity = new DbLabsContext().Countries;
            foreach (var item in listEntity)
            {
                item.Name.ToString();
            }

            watch.Stop();

            Debugger.Log(0, "", $"Entity Elapsed MS: {watch.ElapsedMilliseconds.ToString()}{Environment.NewLine}");


            watch.Start();

            var listEntity2 = new DbLabsContext().Countries;
            foreach (var item in listEntity2)
            {
                item.CountryCode.ToString();
            }

            watch.Stop();

            Debugger.Log(0, "", $"Entity Elapsed MS: {watch.ElapsedMilliseconds.ToString()}{Environment.NewLine}");

            watch.Reset();
            watch.Start();

            var data = new TarkOrm("data source=PH03N1XR4V4N-PC\\DBLABS;initial catalog=MyPortal;persist security info=True;user id=app_login;password=ph03n1xr4v3n;MultipleActiveResultSets=True;App=TarkOrm");

            var lista = data.GetAll<Country>();

            foreach (var item in lista)
            {
                item.Name.ToString();
            }

            watch.Stop();

            Debugger.Log(0, "", $"TarkORM Elapsed MS: {watch.ElapsedMilliseconds.ToString()}{Environment.NewLine}");


            var lista2 = data.GetAll<Country>();

            foreach (var item in lista2)
            {
                item.CountryCode.ToString();
            }

            watch.Stop();

            Debugger.Log(0, "", $"TarkORM Elapsed MS: {watch.ElapsedMilliseconds.ToString()}{Environment.NewLine}");

        }

        [TestMethod]
        public void TestEntityDynamicInstance()
        {
            //DbContext context = new DbContext("data source=PH03N1XR4V4N-PC\\DBLABS;initial catalog=MyPortal;persist security info=True;user id=app_login;password=ph03n1xr4v3n;MultipleActiveResultSets=True;App=TarkOrm");

            //var set = context.Set<Country>();

            //var countryArg = set.Where(x => x.Name == "Argentina").FirstOrDefault();

            //countryArg.ToString();
            //context.Entry<Country>

            DbLabsContext<Country> ctx = new DbLabsContext<Country>();

            var set = ctx.Set<Country>();

            var countries = set.Where(x => x.CountryID > 0);
            countries.Count();
            
            //var countries = ctx.
            //countries.Count();
            //var countryArg = ctx.Entry<Country>().Where(x => x.CountryID == 1).FirstOrDefault();            
        }

        [TestMethod]
        public void BenchmarkV2CountryTark()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            //Tark
            var data = new TarkOrm("data source=PH03N1XR4V4N-PC\\DBLABS;initial catalog=MyPortal;persist security info=True;user id=app_login;password=ph03n1xr4v3n;MultipleActiveResultSets=True;App=TarkOrm");

            for (int i = 0; i < 50; i++)
            {           
                var lista = data.GetAll<Country>();

                foreach (var item in lista)
                {
                    item.Name.ToString();
                }

            }

            watch.Stop();
            Debugger.Log(0, "", $"TarkORM Elapsed MS: {watch.ElapsedMilliseconds.ToString()}{Environment.NewLine}");
        }


        [TestMethod]
        public void BenchmarkV2CountryDapper()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            //Dapper
            var data = new TarkOrm("data source=PH03N1XR4V4N-PC\\DBLABS;initial catalog=MyPortal;persist security info=True;user id=app_login;password=ph03n1xr4v3n;MultipleActiveResultSets=True;App=TarkOrm");

            for (int i = 0; i < 50; i++)
            {
                IEnumerable<Country> listaDapper = data._connection.Query<Country>("SELECT * FROM Country");

                foreach (var item in listaDapper)
                {
                    item.Name.ToString();
                }
            }

            watch.Stop();

            Debugger.Log(0, "", $"Dapper Elapsed MS: {watch.ElapsedMilliseconds.ToString()}{Environment.NewLine}");
        }

        [TestMethod]
        public void BenchmarkV2CountryADO()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            var con = new System.Data.SqlClient.SqlConnection("data source=PH03N1XR4V4N-PC\\DBLABS;initial catalog=MyPortal;persist security info=True;user id=app_login;password=ph03n1xr4v3n;MultipleActiveResultSets=True;App=TarkOrm");
            con.Open();

            for (int i = 0; i < 50; i++)
            {
                var cmd = con.CreateCommand();

                cmd.CommandText = "SELECT * FROM Country";
                cmd.CommandType = CommandType.Text;

                var dr = cmd.ExecuteReader();

                List<Country> listaADO = new List<Country>();

                while (dr.Read())
                {
                    var country = new Country();
                    country.CountryID = (int)dr["CountryID"];
                    country.Name = (string)dr["Name"];
                    country.ContinentID = (int)dr["ContinentID"];
                    country.CountryCode = (string)dr["CountryCode"];

                    if (dr["CurrencyID"] != DBNull.Value)
                        country.CurrencyID = (int)dr["CurrencyID"];

                    if (dr["FlagB64"] != DBNull.Value)
                        country.FlagB64 = (string)dr["FlagB64"];
                }

                foreach (var item in listaADO)
                {
                    item.Name.ToString();
                }
            }

            watch.Stop();

            Debugger.Log(0, "", $"Dapper Elapsed MS: {watch.ElapsedMilliseconds.ToString()}{Environment.NewLine}");
        }

        [TestMethod]
        public void BenchmarkV2CountryEntity()
        {
            Stopwatch watch = new Stopwatch();

            watch.Start();

            var context = new DbLabsContext();

            for (int i = 0; i < 50; i++)
            {
                var listEntity = context.Countries;
                foreach (var item in listEntity)
                {
                    var x = item.Name.ToString();
                }
            }

            watch.Stop();

            Debugger.Log(0, "", $"Entity Elapsed MS: {watch.ElapsedMilliseconds.ToString()}{Environment.NewLine}");
        }
    }

    public class DbLabsContext : DbContext
    {
        public DbLabsContext() : base("data source=PH03N1XR4V4N-PC\\DBLABS;initial catalog=MyPortal;persist security info=True;user id=app_login;password=ph03n1xr4v3n;MultipleActiveResultSets=True;App=TarkOrm")
        {

        }
        
        public DbSet<Country> Countries { get; set; }

        public DbSet<City> Cities { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new NullDatabaseInitializer<DbLabsContext>());
        }
    }

    public class DbLabsContext<T> : DbContext
    {
        public DbLabsContext() : base("data source=PH03N1XR4V4N-PC\\DBLABS;initial catalog=MyPortal;persist security info=True;user id=app_login;password=ph03n1xr4v3n;MultipleActiveResultSets=True;App=TarkOrm")
        {

        }

        //public DbSet<T> Countries { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.RegisterEntityType(typeof(T));
        }
    }




}
