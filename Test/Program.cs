using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;
using System.Reflection;
using System.Data.SqlClient;
using System.Data;
using Enterprises.Framework;
using Enterprises.Framework.Plugin.Office;
using Enterprises.Framework.Utility;
using Test.ThreadTest;


namespace Test
{
    public class Program
    {
        static int theResource = 0;

        static ReaderWriterLock rwl = new ReaderWriterLock();
        public delegate void OperatorDelegate();

        public static void Main()
        {
            #region 折叠
            // 1、测试Linq
            //LinqTest linqt = new LinqTest();
            //linqt.GetMyList();

            // 2、测单件模式
            //SingleTonTest test = new SingleTonTest();
            //test.StartMain();

            // 3、测试工厂模式
            //AbstractFactory fac = AbstractFactory.GetInstance();
            //Console.WriteLine(fac.CreateBonus().Calculate(233));

            // 4、测试多线程执行效率
            //ThreadDemo demo = new ThreadDemo(100000);
            //demo.Action();
            //DateTime beginTime1 = DateTime.Now;
            //Console.WriteLine("单线程执行100000条数据：" + beginTime1.ToString());
            //ThreadDemo.OneThreadTest(100000);
            //Console.WriteLine("单线程执行100000条数据耗时：" + DateTime.Now.ToString() + "共用时：" + DateTime.Now.Subtract(beginTime1).TotalSeconds.ToString());

            //Thread threadOne = new Thread(new ThreadStart(ThreadDemo.Threed1Run)); //两个线程共同做一件事情
            //Thread threadTwo = new Thread(new ThreadStart(ThreadDemo.Threed2Run)); //两个线程共同做一件事情
            //threadOne.Start();
            //threadTwo.Start();
            // 5、测试原件模式
            //ColorManager.Main();


            #region 6、测试AOP面向切面编程Demo
            //var examples = new List<IExample>();
            //examples.Add(new Example1.ConstructorInterceptionExample());
            //examples.Add(new Example2.MethodAndPropertiesInterception());
            //examples.Add(new Example3.EventsInterception());
            //examples.Add(new Example4.TypeInitializerInterception());
            //examples.Add(new Example5.StaticClassInterception());

            //examples.ForEach(e => e.RegisterJoinPoints());

            //examples.ForEach(e =>
            //{
            //    var title = e.About;
            //    Console.WriteLine(title);
            //    Console.WriteLine(new string('*', title.IndexOf(Environment.NewLine) != -1 ? title.IndexOf(Environment.NewLine) : title.Length));
            //    Console.WriteLine();
            //    e.RunExample();
            //    Console.WriteLine();
            //    Console.WriteLine("Press a key to continue...");
            //    Console.ReadKey();
            //    Console.Clear();
            //});
            #endregion
            // 运用符合法性验证
            //Console.WriteLine("aaaaaaaaaaaaa");
            //var a = Kuohaobihe.Check("((a>0) or (a<y)) and (a>9 or (a<8 and y>8))");
            //if (a.Count>0)
            //{
            //    foreach (var conditionError in a)
            //    {
            //        Console.WriteLine(conditionError.Msg); 
            //    }
            //}
            //else
            //{
            //    Console.WriteLine("正确");  
            //}

            //Console.WriteLine("bbbbb...........");
            //var b = Kuohaobihe.Check("(a>0)) or ((a<y and ((a>9 or (a<8 and y>8))))");
            //if (b.Count > 0)
            //{
            //    foreach (var conditionError in b)
            //    {
            //        Console.WriteLine(conditionError.Msg);
            //    }
            //}
            //else
            //{
            //    Console.WriteLine("正确");
            //}
            #endregion

            #region 委托金典实例
            //string instruct;
            //var yuTu3 = new Detector(0, 0, (int)Diretion.East);
            //var dictory = new Dictionary<string, OperatorDelegate>();
            //dictory["F"] = yuTu3.DealFont;
            //dictory["L"] = yuTu3.DealLeft;
            //dictory["R"] = yuTu3.DealRight;

            //while ("exit" != (instruct = Console.ReadLine()))
            //{
            //    if (instruct != null && dictory.ContainsKey(instruct))
            //    {
            //        dictory[instruct]();
            //    }
            //};

            //yuTu3.Print();
            #endregion

            #region Word操作
            //var missing = Type.Missing;
            //var wordHelper = new WordHelper();
            //try
            //{
            //    var isOpen = wordHelper.OpenAndActive(@"D:\yibiyi\FileRoot\销售合同模板-安全.docx", false, false);
            //    if (isOpen)
            //    {
            //        wordHelper.ReplaceBookMark("ContractCode", "合同编号");
            //    }
            //    wordHelper.SavePdf("text1",@"D:\yibiyi\FileRoot");
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            //finally
            //{
            //    Console.WriteLine("word 保存成功.");
            //    wordHelper.Close();
            //}

            #endregion

            #region Word书签转换
            //var wordConvert = new WordConvert();
            //wordConvert.WordConvertTest();
            //Console.WriteLine("word 书签转换成功.");
            #endregion

            #region 随机数

            //for (int i = 0; i < 10; i++)
            //{
            //    Console.WriteLine(RandomHelper.CreateRandomValue(4, true));
            //    Console.WriteLine(RandomHelper.CreateRandomValue(4, false));
            //}

            //Console.WriteLine(RandomHelper.GenerateUniqueId());
            //Console.WriteLine(RandomHelper.GenerateUniqueId());
            //Thread.Sleep(1000);
            //Console.WriteLine(RandomHelper.GenerateUniqueId());
            //Console.WriteLine(RandomHelper.GenerateUniqueId());

            #endregion

            #region 委托相关特效
            var funtest = new FunTest();
            Func<int, int, int> add = (a, b) => a + b;

            var result=funtest.Jisuan(add,1,3);
            Console.WriteLine(result);
            Console.WriteLine(funtest.Jisuan((a, b) => a - b, 5, 4));
          
            var ss=funtest.CreateExcute(p=>p>3,(a,b)=>a+b);
            Console.WriteLine(ss);
            #endregion

            Console.ReadLine();
        }
    }

}


