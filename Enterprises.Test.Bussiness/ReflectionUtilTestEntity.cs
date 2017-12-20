using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Enterprises.Test.Bussiness
{
    public class ReflectionUtilTestEntity : IReflectionUtilTest
    {
        /// <summary>
        ///  注释说明相关信息哦.
        /// </summary>
        [YaolifengTest("姚立峰")]
        public string Name { get; set; }
        public Guid Id { get; set; }
        public string GetName()
        {
            return "Hello,姚立峰";
        }
    }

    public interface IReflectionUtilTest
    {
        string GetName();
    }


    public class YaolifengTestAttribute : Attribute
    {
        public string Name { get; set; }

        public YaolifengTestAttribute(string name)
        {
            Name = name;
        }
    }

    public enum LeaveType
    {
        [Description("进行中")]
        Execute = 1,

        [Description("到期结束")]
        Finish = 2,

        [Description("活动取消")]
        Cancel = 3,

        [Description("审核中")]
        Applove = 4,

        [Description("审核通过")]
        ApplovePass = 5,

        [Description("审核失败")]
        ApploveCancal = 6
    }

    public class Agent
    {
        public Guid ID { get; set; }
        public string AgentName { get; set; }
        public bool Sex { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string Address { set; get; }
        public string MainDirection { set; get; }
        public Guid CreateID { get; set; }
        public Guid ModifyID { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime ModifyTime { get; set; }
        public List<Products> Products { get; set; }
    }

    public class Products
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }
}
