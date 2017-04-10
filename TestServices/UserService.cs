using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using ServiceFramework.ServiceDescription;
using System.Collections;

namespace Test
{
    [Service(name:"UserService")]
    public interface IUserService
    {
        [Operation]
        string SayHello(string a1, int a2, string[] a3, List<int> a4);
        [Operation]
        string Jump(string a1, int a2);
        [Operation]
        List<string> HugeData(int num);
        [Operation(isIterator:true)]
        IEnumerable<string> HugeData2(int num);
        [Operation(isIterator:true)]
        IEnumerable<string> Test(int num);

        [Operation]
        TestEntity GetEntity(int number, TestEntity entity);

        [Operation(isIterator:true)]
        IEnumerable<TestEntity> GetEntityCollection(int number);

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

        public List<string> HugeData(int num)
        {
            int rowNum = num;
            var l = new List<string>(rowNum);
            for(int j = 0; j < rowNum; j++)
            {
                char[] chs = new char[1024];
                for(int i = 0; i < chs.Length; i++)
                {
                    chs[i] = 'a';
                }

                l.Add(new string(chs));
                Console.WriteLine(j);
                
            }
            var t = Task.Run(() => Task.Delay(3000));
            t.Wait();
            return l;
        }

        public IEnumerable<string> HugeData2(int num)
        {
            int rowNum = num;
            for(int j = 0; j < rowNum; j++)
            {
                char[] chs = new char[1024];
                for(int i = 0; i < chs.Length; i++)
                {
                    chs[i] = 'a';
                }

                Console.WriteLine(j);
                yield return new string(chs);
                
            }
        }

        public IEnumerable<string> Test(int num)
        {
            yield return "aaa";
            yield return "bbb";
            yield return "ccc";
            yield return "ddd";
            yield return "xxxx";
        }

        public TestEntity GetEntity(int number, TestEntity entity)
        {
            List<TestTransactionEntity> transactions = new List<TestTransactionEntity>(number);
            for(int i = 0; i < number; i++)
            {
                transactions.Add(new TestTransactionEntity {
                    P1 = "xxx",
                    p2 = i,
                    p3 = DateTime.Now,
                    p4 = 12.99m,
                });
            }

            entity.P5 = transactions;
            return entity;
        }

        public IEnumerable<TestEntity> GetEntityCollection(int number)
        {
            for(int i = 0; i < number; i++)
            {
                List<TestTransactionEntity> l = new List<TestTransactionEntity>();
                for(int j = 0; j <=i; j++)
                {
                    l.Add(new TestTransactionEntity {
                        P1 = $"c1_{j}",
                        p2 = j,
                        p3 = DateTime.Now,
                        p4 = 56.99m,
                    });
                }
                yield return new TestEntity {
                    P1 = $"p1_{i}",
                    p2 = i,
                    p3 = DateTime.Now,
                    p4 = 99.99m,
                    P5 = l
                };

            }
        }
    }

    [Serializable]
    public class TestEntity
    {
        public string P1 { get; set; }
        public int p2 { get; set; }
        public DateTime p3 { get; set; }
        public decimal p4 { get; set; }
        public List<TestTransactionEntity> P5 { get; set; }
    }

    [Serializable]
    public class TestTransactionEntity
    {
        public string P1 { get; set; }
        public int p2 { get; set; }
        public DateTime p3 { get; set; }
        public decimal p4 { get; set; }
    }
}
