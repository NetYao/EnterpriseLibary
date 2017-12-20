using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Linq;

namespace Test
{
    /// <summary>
    /// 在开发中经常会遇到线程的例子，如果某个后台操作比较费时间，我们就可以启动一个线程去执行那个费时的操作，同时程序继续执行。在某些情况下可能会出现多个线程的同步协同的问题，下面的例子就展示了在两个线程之间如何协同工作。
    ///
    ///这个程序的思路是共同做一件事情（从一个ArrayList中删除元素）,如果执行完成了，两个线程都停止执行。
    /// </summary>
    public class ThreadDemo
    {
        private Thread threadOne;
        private Thread threadTwo;
        private static ArrayList stringList;
        private static DateTime beginTime;

        private event EventHandler OnNumberClear;//数据删除完成引发的事件
        //public static void Main()
        //{

        //    //ThreadDemo demo = new ThreadDemo(1000);
        //    //demo.Action();

        //    //DateTime beginTime1 = DateTime.Now;
        //    //Console.WriteLine("单线程执行100000条数据：" + beginTime1.ToString());
        //    //OneThreadTest(1000);
        //    //Console.WriteLine("单线程执行100000条数据耗时：" + DateTime.Now.ToString() + "共用时：" + DateTime.Now.Subtract(beginTime1).TotalSeconds.ToString());

        //    XDocument modeldoc = new XDocument();
        //    string modelDocPath = "D:\\OrganAbnoramlPO.hbm.xml";
        //    modeldoc = XDocument.Load(modelDocPath);
        //    //XElement element = XElement.Load(@"");
        //    var modelNodeList = from b in modeldoc.Descendants("column")
        //                        select new { itemKey = b.Attribute("name").Value };//, itemValue = b.Attribute("sql-type").Value };
        //    foreach (var item in modelNodeList)
        //    {
        //        Console.WriteLine(string.Format("{0}", item.itemKey));
        //    }

        //    Console.ReadLine();
        //}

        public ThreadDemo(int number)
        {
            beginTime = DateTime.Now;
            Console.WriteLine("程序开始执行100000条数据：" + beginTime.ToString());
            Random random = new Random(1000000);
            stringList = new ArrayList(number);
            for (int i = 0; i < number; i++)
            {
                stringList.Add(random.Next().ToString());
            }
            threadOne = new Thread(new ThreadStart(Run));//两个线程共同做一件事情
            threadTwo = new Thread(new ThreadStart(Run));//两个线程共同做一件事情
            threadOne.Name = "线程1";
            threadTwo.Name = "线程2";
            OnNumberClear += new EventHandler(ThreadDemo_OnNumberClear);

        }
        /// <summary>
        /// 开始工作
        /// </summary>
        public void Action()
        {
            threadOne.Start();
            threadTwo.Start();
        }
        /// <summary>
        /// 共同做的工作
        /// </summary>
        private void Run()
        {
            string stringValue = null;
            while (true)
            {
                Monitor.Enter(this);//锁定，保持同步
                stringValue = (string)stringList[0];
                Console.WriteLine(Thread.CurrentThread.Name + "删除了" + stringValue);
                stringList.RemoveAt(0);//删除ArrayList中的元素
                if (stringList.Count == 0)
                {
                    OnNumberClear(this, new EventArgs());//引发完成事件
                }
                Monitor.Exit(this);//取消锁定
                Thread.Sleep(5);
            }
        }

        //执行完成之后，停止所有线程
        void ThreadDemo_OnNumberClear(object sender, EventArgs e)
        {
            Console.WriteLine("执行完了，停止了所有线程的执行。" + DateTime.Now.ToString() + "共用时：" +  DateTime.Now.Subtract(beginTime).TotalSeconds.ToString());
            threadTwo.Abort();
            threadOne.Abort();

        }

       public  static void OneThreadTest(int number)
        {
            Random random = new Random(1000000);
            stringList = new ArrayList(number);
            for (int i = 0; i < number; i++)
            {
                stringList.Add(random.Next().ToString());
            }

            while (stringList.Count>0)
            {
                string stringValue = null;
                stringValue = (string)stringList[0];
                Console.WriteLine("主进程删除了" + stringValue);
                stringList.RemoveAt(0);//删除ArrayList中的元素
                Thread.Sleep(5);
            }


        }

       public static void Threed1Run()
       {
           for (int i = 0; i < 20; i++)
           {
               Console.WriteLine("1111111...."+i.ToString());
               Thread.Sleep(1000);
           }

         
       }

       public static void Threed2Run()
       {
           for (int i = 0; i < 20; i++)
           {
               Console.WriteLine("22222...." + i.ToString());
           }

          
       }
    }
}
