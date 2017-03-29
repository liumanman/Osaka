using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;

using ServiceFramwork.Server.Http;

namespace Test
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .Configure(app => app.UseMiddleware<DispatchMiddleware>(
                    new DefaultServiceManager(new Type[] { typeof(UserService) })
                    ,"/test/abc/v2/{service}/{operation}"))
                .Build();

            host.Run();
            
        }
    }
}
