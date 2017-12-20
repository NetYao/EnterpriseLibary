using System;
using System.Collections.Generic;
using System.Diagnostics;
using Enterprises.Framework.Utility;
using Enterprises.Test.Bussiness;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enterprises.Test.Utility
{
    [TestClass]
    public class ReflectionUtilTest
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void GetDescriptionTest()
        {
            var desc=ReflectionUtil.GetDescription(typeof (LeaveType), 3);
            TestContext.WriteLine(desc);
        }

        [TestMethod]
        public void GetEnumeratorsTest()
        {
            var desc = ReflectionUtil.GetEnumerators(typeof(LeaveType));
            foreach (var item in desc)
            {
                TestContext.WriteLine(string.Format("{0},{1}",item.Key,item.Value));
            }

            var leaveType = LeaveType.Applove;
            TestContext.WriteLine("GetDescription=" + leaveType.GetDescription());
        }


        [TestMethod]
        public void HasAttributeTest()
        {
            var result = ReflectionUtil.HasAttribute<YaolifengTestAttribute>(typeof(ReflectionUtilTestEntity), "Name");
            TestContext.WriteLine(result.ToString());
        }

        [TestMethod]
        public void GetGetAttributeTest()
        {
            var desc = ReflectionUtil.GetAttribute<YaolifengTestAttribute>(typeof(ReflectionUtilTestEntity), "Name");
            TestContext.WriteLine(string.Format("{0}", desc.Name));
        }

        [TestMethod]
        public void ExtractGenericInterface()
        {
            Type type = ReflectionUtil.ExtractGenericInterface(typeof(ReflectionUtilTestEntity), typeof(IEnumerable<>));
            if (type != null)
            {
                TestContext.WriteLine(string.Format("Name:{0}", type.Name));
            }

            Type type2 = ReflectionUtil.ExtractGenericInterface(typeof(ReflectionUtilTestEntity), typeof(IReflectionUtilTest));
            if (type2 != null)
            {
                TestContext.WriteLine(string.Format("Name:{0}", type2.Name));
            }

        }

        [TestMethod]
        public void IsUpdateTest()
        {
            var agent=new Agent()
                {
                    ID = Guid.NewGuid(),
                    Address = "杭州",
                    Sex = true,
                    CreateTime =new DateTime(2014,2,3),
                    Email = "yaolifeng@163.com",
                    Products = new List<Products>() { new Products() { Code = "1", Name = "1", ID = 100 }, new Products() { Code = "2", Name = "2", ID = 120 } }
                };

            var agent2 = new Agent()
            {
                ID = Guid.NewGuid(),
                Address = "上海",
                Sex = false,
                CreateTime = new DateTime(2014, 2, 4),
                Email = "yaolifeng@163.com",
                Products = new List<Products>() { new Products() { Code = "1", Name = "1", ID = 100 }, new Products() { Code = "5", Name = "5", ID = 120 } }
            };

            var agent3 = new Agent()
            {
                ID = Guid.NewGuid(),
                Address = "杭州",
                Sex = true,
                CreateTime = new DateTime(2014, 2, 3),
                Email = "yaolifeng@163.com",
                Products = new List<Products>() { new Products() { Code = "1", Name = "1", ID = 100 }, new Products() { Code = "2", Name = "2", ID = 120 } }
            };

            var r1 = ReflectionUtil.IsUpdate<Agent>(agent,agent2);
            var r2 = ReflectionUtil.IsUpdate<Agent>(agent, agent3);

            foreach (var b in r1)
            {
                TestContext.WriteLine(string.Format("{0}:{1}", b.Key,b.Value));
            }

            TestContext.WriteLine("\r\n");
            TestContext.WriteLine("\r\n");
            foreach (var b in r2)
            {
                TestContext.WriteLine(string.Format("{0}:{1}", b.Key, b.Value));
            }
        }


       

    }


    
}
