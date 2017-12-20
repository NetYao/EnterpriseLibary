using Microsoft.VisualStudio.TestTools.UnitTesting;
using Enterprises.Framework.Plugin.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Enterprises.Framework.Plugin.Mapper.Tests
{
    [TestClass()]
    public class ShengMapperTests
    {
        [TestMethod()]
        public void SetValuesTest()
        {
            A a = new A()
            {
                Name = "张三",
                Age = 10,
                Class = "一班",
                CObject = new SubC()
                {
                    Message = "Hello"
                },
                P1 = "1",
                P2 = "2",
                P3 = "3",
                P4 = "4",
                P5 = "5",
                P6 = "6"
            };

            B b = new B();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            for (int i = 0; i < 100000; i++)
            {
                //全部属性拷贝
                ShengMapper.SetValues(a, b);

                //拷贝指定的属性
                // ShengMapper.SetValuesWithProperties(a, b, new string[] { "Name", "Age", "P1" });

                //排除指定的属性
                //ShengMapper.SetValuesWithoutProperties(a, b, new string[] { "Name", "Age", "P1" });


            }

            stopwatch.Stop();

            Console.WriteLine("对包含 10 个属性的对象的属性值拷贝 10 万次，耗时：" + stopwatch.Elapsed.ToString());

            Console.ReadLine();
        }

        [TestMethod()]
        public void SetValuesWithPropertiesTest()
        {
            A a = new A()
            {
                Name = "张三",
                Age = 10,
                Class = "一班",
                CObject = new SubC()
                {
                    Message = "Hello"
                },
                P1 = "1",
                P2 = "2",
                P3 = "3",
                P4 = "4",
                P5 = "5",
                P6 = "6"
            };

            B b = new B();
            ShengMapper.SetValuesWithProperties(a, b, new string[] { "Name", "Age", "P1" });
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(a));
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(b));
        }

        [TestMethod()]
        public void SetValuesWithoutPropertiesTest()
        {
            A a = new A()
            {
                Name = "张三",
                Age = 10,
                Class = "一班",
                CObject = new SubC()
                {
                    Message = "Hello"
                },
                P1 = "1",
                P2 = "2",
                P3 = "3",
                P4 = "4",
                P5 = "5",
                P6 = "6"
            };

            B b = new B();
            //排除指定的属性
            ShengMapper.SetValuesWithoutProperties(a, b, new string[] { "Name", "Age", "P1" });
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(a));
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(b));
        }
    }

    class A
    {
        public string Name
        {
            get; set;
        }

        public int Age
        {
            get; set;
        }

        public string Class
        {
            get; set;
        }

        public SubC CObject
        {
            get; set;
        }

        public string P1
        {
            get; set;
        }

        public string P2
        {
            get; set;
        }

        public string P3
        {
            get; set;
        }

        public string P4
        {
            get; set;
        }

        public string P5
        {
            get; set;
        }

        public string P6
        {
            get; set;
        }

    }

    class B
    {
        public string Name
        {
            get; set;
        }

        public int Age
        {
            get; set;
        }

        public C CObject
        {
            get; set;
        }

        public string P1
        {
            get; set;
        }

        public string P2
        {
            get; set;
        }

        public string P3
        {
            get; set;
        }

        public string P4
        {
            get; set;
        }

        public string P5
        {
            get; set;
        }

        public string P6
        {
            get; set;
        }
    }

    class C
    {
        public string Message
        {
            get; set;
        }
    }

    class SubC : C
    {

    }
}