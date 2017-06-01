using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using TarkOrm.Attributes;
using TarkOrm.Mapping;

namespace TarkOrm.Tests.TarkOrmTests
{
    [TestClass]
    public class UnitTestMappings
    {
        [TestMethod]
        public void UnitTest_ManualMapping()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            var tarkOrm = new TarkOrm("localhost");

            TarkConfigurationMapping.AutoMapType<TestOrmTestMapping>()
                .MapProperty(x => x.classx, "Classification")
                .MapProperty(x => x.description, "Name")
                .ToTable("TestOrm");

            var list = tarkOrm.GetAll<TestOrmTestMapping>();

            watch.Stop();

            Debugger.Log(0, "", $"TarkORM Elapsed MS: {watch.ElapsedMilliseconds.ToString()}{Environment.NewLine}");
        }

        [TestMethod]
        public void UnitTest_IgnoreMapping()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            var tarkOrm = new TarkOrm("localhost");

            TarkConfigurationMapping.CreateEmptyMapType<TestOrmTestMapping>()
                .MapProperty(x => x.classx, "Classification")
                .MapProperty(x => x.description, "Name")
                .MapProperty(x => x.CreationDate)
                .MapProperty(x => x.Id, new KeyAttribute())
                .ToTable("TestOrm");   

            var list = tarkOrm.GetAll<TestOrmTestMapping>();

            var item = tarkOrm.GetById<TestOrmTestMapping>(1);

            watch.Stop();

            Debugger.Log(0, "", $"TarkORM Elapsed MS: {watch.ElapsedMilliseconds.ToString()}{Environment.NewLine}");
        }
    }
}
