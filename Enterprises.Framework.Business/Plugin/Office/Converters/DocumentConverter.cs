using System;
using System.Runtime.InteropServices;
using System.IO;

namespace Enterprises.Framework.Plugin.Office.Converters
{
    /// <summary>
    /// 文档转换抽象类
    /// </summary>
    public abstract class DocumentConverter : IConverter
    {
        /// <summary>
        /// 转换方法
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public virtual string Convert(DocumentEntity doc)
        {
            return ConvertDocument(doc);
        }

        protected abstract string ConvertDocument(DocumentEntity doc);

        protected void WriteLog(string message)
        {
            if (DocumentConvertConfig.LogEnabled)
            {
                File.AppendAllText(DocumentConvertConfig.LogFile, message + "\r\n");
            }
        }

        [DllImport("User32.dll")]
        public static extern int GetWindowThreadProcessId(IntPtr hWnd, out int processId);
    }
}
