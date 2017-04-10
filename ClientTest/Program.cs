using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ServiceFramework.Client;
using Test;

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
            Test2();
        }

        static void Test1()
        {
            TestEntity t = new TestEntity {
                P1 = "p_xxx",
                p2 = 123,
                p3 = DateTime.Now,
                p4 = 49.99m,
            };

            var r = ServiceFactory.CreateInstance<IUserService>().GetEntity(10, t);
        }

        static void Test2()
        {
            var r = ServiceFactory.CreateInstance<IUserService>().GetEntityCollection(10);
        }

    }
}
