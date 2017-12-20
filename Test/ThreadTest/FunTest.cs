using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Test.ThreadTest
{
    /// <summary>
    /// 委托Func 
    /// </summary>
    public class FunTest
    {


        public int Jisuan(Func<int, int, int> excute,int a,int b)
        {
            return excute(a, b);
        }

        /// <summary>
        /// 可以传人表达式
        /// </summary>
        /// <param name="express"></param>
        /// <param name="excute"></param>
        /// <returns></returns>
        public int CreateExcute(Expression<Func<int, bool>> express, Func<int, int, int> excute)
         {
             int condition = 3, propertys=5;
             Console.WriteLine(express.Body);
             return excute(propertys, condition);
         }
    }
}
