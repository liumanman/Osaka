using System;
using System.Collections.Generic;
using System.Text;

using ServiceFramework.ServiceDescription;

namespace TestServices
{
    [Service]
    public interface IOrderService
    {
        [Operation]
        int Test1(string a1, int a2);
        [Operation]
        string Test2(int a1, List<string> a2);
        [Operation(name:"Test_2")]
        List<int> Test2(int a1, string a2);
    }

    public class OrderService : IOrderService
    {
        public int Test1(string a1, int a2)
        {
            throw new NotImplementedException();
        }

        public string Test2(int a1, List<string> a2)
        {
            throw new NotImplementedException();
        }

        public List<int> Test2(int a1, string a2)
        {
            throw new NotImplementedException();
        }
    }
}
