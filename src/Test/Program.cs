using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using System.Text;

using ServiceFramwork.Server.Http;
using ServiceFramwork.Serialization;
using ServiceFramwork.Server.ServiceDescription;

using ConsoleApp2;

namespace Test
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .Configure(app => app.UseMiddleware<SimpleServiceMiddleware>(
                    new DefaultServiceManager(new Type[] { typeof(UserService) })
                    , "/test/abc/v2/{service}/{operation}"))
                .Build();

            host.Run();

            //Test2();

        }

        public static void Test()
        {
            var serializer = new JsonSerializer(Encoding.UTF8);
            var data = serializer.Serialize(9527);
            string v1 = Encoding.UTF8.GetString(data);

            data = serializer.Serialize("9527");
            string v2 = Encoding.UTF8.GetString(data);

            data = serializer.Serialize(new string[] { "abc", "123" });
            string v3 = Encoding.UTF8.GetString(data);

            data = serializer.Serialize(new List<int> { 1, 2 });
            string v4 = Encoding.UTF8.GetString(data);
        }

        public static void Test2()
        {
            var creator = new SerializationTypeCreator("test", new Dictionary<string, Type>{
                {"a1", typeof(int) },
                {"a2", typeof(string)},
            });
            var t = creator.Create();
        }

        public static void Test3()
        {
            var t = CreateType.DynamicCreateType();
        }


    }
}
