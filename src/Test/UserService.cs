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
        string SayHello();
        [Operation]
        string Jump();
    }

    public class UserService : IUserService
    {
        public string Jump()
        {
            throw new NotImplementedException();
        }

        public string SayHello()
        {
            throw new NotImplementedException();
        }
    }
}
