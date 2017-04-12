using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ServiceFramework.Client;
using Test;
using Newtonsoft.Json;

namespace ClientTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //var r = ServiceFactory.CreateInstance<IUserService>().SayHello("123",
            //    1,
            //    new string[] { "abs", "xyz" }
            //    , new List<int> { 100, 101, 102 });
            Test4();
        }

        static void Test1()
        {
            TestEntity t = new TestEntity {
                P1 = "p_xxx",
                p2 = 123,
                p3 = DateTime.Now,
                p4 = 49.99m,
            };

            //var r = ServiceFactory.CreateInstance<IUserService>().GetEntity(10, t);
        }

        static void Test2()
        {
            //var r = ServiceFactory.CreateInstance<IUserService>().GetEntityCollection(10);
        }

        static void Test3()
        {
            //var r = ServiceFactory.CreateInstance<IUserService>().Jump(123, 3);
        }

        static void Test4()
        {
            var r = ServiceFactory.CreateInstance<IUserService>().Test2(DateTime.Now);
            var r2 = ServiceFactory.CreateInstance<IUserService>().Test2(1);
        }

        static void Test5()
        {
            string r = JsonConvert.SerializeObject(126);
            object i = JsonConvert.DeserializeObject(r, typeof(int));
        }

    }
}
