using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Test
{
    /// <summary>
    /// 单件模式
    /// </summary>
    public sealed class SingleTon
    {

        SingleTon()
        {
        }

        public static SingleTon Instance
        {
            get
            {
                return Nested.instance;
            }
        }

        int totNum = 0;

        public void Add()
        {
            totNum++;
        }

        ///获得当前计数值 
        public int GetCounter()
        {
            return totNum;
        }

        class Nested
        {
            static Nested()
            {
            }

            internal static readonly SingleTon instance = new SingleTon();
        }
    }

    public class SingleTonTest
    {
        public static void DoSomeWork()
        {
            ///构造显示字符串
            string results = "";

            ///创建一个Sigleton实例
            SingleTon MyCounter = SingleTon.Instance;

            ///循环调用四次
            for (int i = 1; i < 5; i++)
            {
                ///开始计数
                MyCounter.Add();

                results += "线程";
                results += Thread.CurrentThread.Name.ToString() + "——〉";
                results += "当前的计数：";
                results += MyCounter.GetCounter().ToString();
                results += "\n";

                Console.WriteLine(results);

                ///清空显示字符串
                results = "";
            }
        }

        public void StartMain()
        {

            Thread thread0 = Thread.CurrentThread;

            thread0.Name = "Thread 0";

            Thread thread1 = new Thread(new ThreadStart(DoSomeWork));

            thread1.Name = "Thread 1";

            Thread thread2 = new Thread(new ThreadStart(DoSomeWork));

            thread2.Name = "Thread 2";

            Thread thread3 = new Thread(new ThreadStart(DoSomeWork));

            thread3.Name = "Thread 3";

            thread1.Start();

            thread2.Start();

            thread3.Start();

            ///线程0也只执行和其他线程相同的工作
            DoSomeWork();
        }
    }
}
