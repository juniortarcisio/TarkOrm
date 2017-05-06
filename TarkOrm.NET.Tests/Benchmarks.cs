using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Entity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace TarkOrm.NET.Tests
{
    [TestClass]
    public class Benchmarks
    {

        [TestMethod]
        public void Benchmark1()
        {
            Stopwatch watch = new Stopwatch();

            watch.Start();

            var listEntity = new DbLabsContext().Countries;
            foreach (var item in listEntity)
            {
                item.Name.ToString();
            }

            watch.Stop();

            Debugger.Log(0, "", $"Entity Elapsed MS: {watch.ElapsedMilliseconds.ToString()}{Environment.NewLine}" );


            var data = new TarkDataAccess("data source=PH03N1XR4V4N-PC\\DBLABS;initial catalog=MyPortal;persist security info=True;user id=app_login;password=ph03n1xr4v3n;MultipleActiveResultSets=True;App=TarkOrm.NET");

            var lista = data.GetAll<Country>();

            foreach (var item in lista)
            {
                item.Name.ToString();
            }

            watch.Stop();

            Debugger.Log(0, "", $"TarkORM Elapsed MS: {watch.ElapsedMilliseconds.ToString()}{Environment.NewLine}");

            watch.Reset();
            watch.Start();

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

            var data = new TarkDataAccess("data source=PH03N1XR4V4N-PC\\DBLABS;initial catalog=MyPortal;persist security info=True;user id=app_login;password=ph03n1xr4v3n;MultipleActiveResultSets=True;App=TarkOrm.NET");

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

            var con = new System.Data.SqlClient.SqlConnection("data source=PH03N1XR4V4N-PC\\DBLABS;initial catalog=MyPortal;persist security info=True;user id=app_login;password=ph03n1xr4v3n;MultipleActiveResultSets=True;App=TarkOrm.NET");

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

            var data = new TarkDataAccess("data source=PH03N1XR4V4N-PC\\DBLABS;initial catalog=MyPortal;persist security info=True;user id=app_login;password=ph03n1xr4v3n;MultipleActiveResultSets=True;App=TarkOrm.NET");

            var lista = data.GetAll<City>();

            foreach (var item in lista)
            {
                var x = item.CityID;
            }

            watch.Stop();

            Debugger.Log(0, "", $"TarkORM Elapsed MS: {watch.ElapsedMilliseconds.ToString()}{Environment.NewLine}");
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

            var data = new TarkDataAccess("data source=PH03N1XR4V4N-PC\\DBLABS;initial catalog=MyPortal;persist security info=True;user id=app_login;password=ph03n1xr4v3n;MultipleActiveResultSets=True;App=TarkOrm.NET");

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
            //DbContext context = new DbContext("data source=PH03N1XR4V4N-PC\\DBLABS;initial catalog=MyPortal;persist security info=True;user id=app_login;password=ph03n1xr4v3n;MultipleActiveResultSets=True;App=TarkOrm.NET");

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
    }

    public class DbLabsContext : DbContext
    {
        public DbLabsContext() : base("data source=PH03N1XR4V4N-PC\\DBLABS;initial catalog=MyPortal;persist security info=True;user id=app_login;password=ph03n1xr4v3n;MultipleActiveResultSets=True;App=TarkOrm.NET")
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
        public DbLabsContext() : base("data source=PH03N1XR4V4N-PC\\DBLABS;initial catalog=MyPortal;persist security info=True;user id=app_login;password=ph03n1xr4v3n;MultipleActiveResultSets=True;App=TarkOrm.NET")
        {

        }

        //public DbSet<T> Countries { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.RegisterEntityType(typeof(T));
        }
    }

}
