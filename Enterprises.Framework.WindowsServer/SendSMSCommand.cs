using System;
using System.Globalization;
using Enterprises.Framework.Services;
using System.Threading;
using System.IO;

namespace Enterprises.Framework.WindowsServer
{
    public class SendSmsCommand:ICommand
    {
        public void Execute()
        {
            // 服务的业务逻辑核心代码
            DoSomething();
        }

        private void DoSomething()
        {
            for (int i = 0; i < 10; i++)
            {
                Console.Write("threadOne" + i.ToString(CultureInfo.InvariantCulture));
                WriterLog("threadOne" + i.ToString(CultureInfo.InvariantCulture), fileName: "log");
                Thread.Sleep(2000);
            }

            
        }


        private void ThreadTwo()
        {
            for (int i = 0; i < 10; i++)
            {
                Console.Write("threadTwo" + i.ToString(CultureInfo.InvariantCulture));
                WriterLog("threadTwo" + i.ToString(CultureInfo.InvariantCulture), "log");
                Thread.Sleep(5000);
            }

            
        }


        /// <summary>
        /// 写入日志文件。
        /// </summary>
        /// <param name="text"></param>
        /// <param name="fileName"></param>
        public static void WriterLog(string text, string fileName)
        {
            string appPath = AppDomain.CurrentDomain.BaseDirectory + @"\Log";
            if (!Directory.Exists(appPath))
            {
                Directory.CreateDirectory(appPath);
            }

            try
            {
                var strW = new StreamWriter(appPath + @"\" + fileName + ".log", true);
                strW.WriteLine("{0},{1} \r\n",  text,DateTime.Now);
                strW.Flush();
                strW.Close();
            }
            catch { }
        }
    }
}
