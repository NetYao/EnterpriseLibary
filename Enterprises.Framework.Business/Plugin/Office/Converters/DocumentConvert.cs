using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Threading;
using System.IO;

namespace Enterprises.Framework.Plugin.Office.Converters
{
    public partial class DocumentConvert : ServiceBase
    {
        private System.Threading.Thread _thread;
        private ManualResetEvent _event;


        public DocumentConvert()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _event = new ManualResetEvent(false);
            _thread = new System.Threading.Thread(Process);
            _thread.IsBackground = true;
            _thread.Start();
        }

        protected override void OnStop()
        {
            try
            {
                _event.Reset();
                _thread.Abort();
            }
            catch (ThreadAbortException)
            {
                Thread.ResetAbort();
            }
        }

        protected override void OnPause()
        {
            _event.Reset();

        }

        protected override void OnContinue()
        {
            _event.Set();
        }

        public void Process()
        {
            try
            {
                _event.Set();
                while (true)
                {
                    List<DocumentEntity> documents = DocumentService.Instance.GetDocument(DocumentStatus.New | DocumentStatus.Modified);
                    WriteLog(string.Format("获取到 {0} 条记录\r\n", documents.Count));
                    var converter = new SimpleConverter();
                    foreach (var item in documents)
                    {
                        if (_event.WaitOne(Timeout.Infinite))
                        {
                            WriteLog(string.Format("处理编号为 {0} 的记录\r\n", item.ID));
                            converter.Convert(item);
                            WriteLog(string.Format("编号为 {0} 的记录处理完毕\r\n", item.ID));
                        }
                    }
                    WriteLog("目前所有文件处理完毕，服务将在休眠1分钟后继续！");
                    Thread.Sleep(60 * 1000);
                }
            }
            catch (ThreadAbortException)
            {
                Thread.ResetAbort();
                WriteLog("用户取消任务！\r\n");
            }
            catch(Exception ex)
            {
                WriteLog(ex.ToString());
            }
        }

        protected void WriteLog(string message)
        {
            if (DocumentConvertConfig.LogEnabled)
            {
                File.AppendAllText(DocumentConvertConfig.LogFile, message + "\r\n");
            }
        }
    }
}
