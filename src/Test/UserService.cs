using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ServiceFramwork.Server.ServiceDescription;

namespace Test
{
    [Service(name:"UserService")]
    public interface IUserService
    {
        [Operation]
        string SayHello(string a1, int a2, string[] a3, List<int> a4);
        [Operation]
        string Jump(string a1, int a2);
    }

    public class UserService : IUserService
    {
        public string Jump(string a1, int a2)
        {
            return $"a1:{a1}, a2:{a2}";
        }

        public string SayHello(string a1, int a2, string[] a3, List<int> a4)
        {
            return $"a1:{a1}, a2:{a2}, a3:{a3}, a4:{a4.ToArray()}";
        }
    }

    public class TestEntity
    {
        public string P1 { get; set; }
        public int p2 { get; set; }
        public DateTime p3 { get; set; }
        public decimal p4 { get; set; }
    }
}
