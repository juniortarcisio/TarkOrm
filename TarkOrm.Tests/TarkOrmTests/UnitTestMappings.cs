using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;

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

            TarkConfigurationMapping.AutoMapType<TestOrmTestMapping>()
                .MapProperty(x => x.classx, "Classification")
                .MapProperty(x => x.description, "Name")
                .IgnoreProperty(x => x.Id)
                .ToTable("TestOrm"); 
                //TODO: Readonly, key    

            var list = tarkOrm.GetAll<TestOrmTestMapping>();

            watch.Stop();

            Debugger.Log(0, "", $"TarkORM Elapsed MS: {watch.ElapsedMilliseconds.ToString()}{Environment.NewLine}");
        }
    }
}
