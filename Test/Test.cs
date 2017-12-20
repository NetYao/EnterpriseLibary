using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test
{
    public abstract class TestBoot
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class Test2 : TestBoot
    {
        public string Code { get; set; }
        public string Remark { get; set; }
    }

    public static class Test
    {
        public static List<Test2> GetDate()
        {
            var test=new List<Test2>
                {
                    new Test2 {Key = "1", Value = "1111", Code = "111", Remark = "111"},
                    new Test2 {Key = "2", Value = "2222", Code = "222", Remark = "222"}
                };
            return test;
        }

    }
}
