using System;

namespace Enterprises.Framework.Plugin.Logging
{
    using log4net.Appender;
    using log4net.Util;
    using System.Collections.Generic;

    /// <summary>
    /// 按log文件最大长度限度生成新文件
    /// </summary>
    public class OrchardFileAppender : RollingFileAppender
    {
        private static readonly Dictionary<string, int> _suffixes = new Dictionary<string, int>();
        private const int MaxSuffixes = 100;
        private const int Retries = 50;

        protected virtual void BaseOpenFile(string fileName, bool append)
        {
            base.OpenFile(fileName, append);
        }

        protected override void OpenFile(string fileName, bool append)
        {
            lock (this)
            {
                bool flag = false;
                string nextOutputFileName = base.GetNextOutputFileName(fileName);
                string str2 = fileName;
                if (_suffixes.Count > 100)
                {
                    _suffixes.Clear();
                }
                if (!_suffixes.ContainsKey(nextOutputFileName))
                {
                    _suffixes[nextOutputFileName] = 0;
                }
                int num = _suffixes[nextOutputFileName];
                for (int i = 1; !flag && (i <= 50); i++)
                {
                    try
                    {
                        if (num > 0)
                        {
                            str2 = string.Format("{0}-{1}", fileName, num);
                        }

                        BaseOpenFile(str2, append);
                        flag = true;
                    }
                    catch
                    {
                        num = _suffixes[nextOutputFileName] + i;
                        LogLog.Error(typeof(OrchardFileAppender).ToString(), new Exception(string.Format("OrchardFileAppender: Failed to open [{0}]. Attempting [{1}-{2}] instead.", fileName, fileName, num)));
                    }
                }
                _suffixes[nextOutputFileName] = num;
            }
        }
    }
}

