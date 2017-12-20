using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Office.Interop.Word;
using System.Runtime.InteropServices;
using System.IO;

namespace Enterprises.Framework.Plugin.Office.Converters.Model
{
    /// <summary>
    /// Word转换文档
    /// </summary>
    public class Document : IDisposable
    {
        private Microsoft.Office.Interop.Word.Document _document;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="doc"></param>
        public Document(Microsoft.Office.Interop.Word.Document doc)
        {
            _document = doc;
        }

        #region IDisposable Implemention
        private bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Document()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Nothing todo,anyone modify this calss,
                    // check if any managed resource need
                    // dispose and add code here!
                }
                if (_document != null)
                {
                    object objFalse = false, objMissing = Type.Missing;
                    _document.Close(ref objFalse, ref objMissing, ref objMissing);
                    Marshal.ReleaseComObject(_document);
                    _document = null;
                    _disposed = true;
                }
            }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public Microsoft.Office.Interop.Word.Application Application
        {
            get
            {
                return _document.Application;
            }
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="saveType">保存类型</param>
        public void Save(string path, OutputType saveType)
        {
            string fileName = Path.GetFileName(path);
            path = Path.GetDirectoryName(path);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            fileName = Path.Combine(path, fileName);
            switch (saveType)
            {
                case OutputType.PDF:
                    Save(fileName, Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatPDF);
                    break;
                case OutputType.PDF | OutputType.Word:
                    Save(fileName, Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatDocumentDefault);
                    Save(fileName, Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatPDF);
                    break;
                default:
                    Save(fileName, Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatDocumentDefault);
                    break;
            }
        }

        /// <summary>
        /// 转换实例
        /// </summary>
        /// <param name="dataSource"></param>
        public void Instantiate(DataSource dataSource)
        {
            WriteLog("开始替换书签！");
            if (_document == null)
            {
                WriteLog("目标文档不存在，跳过！");
                return;
            }

            Microsoft.Office.Interop.Word.Bookmarks bookmarks = _document.Bookmarks;
            foreach (Microsoft.Office.Interop.Word.Bookmark item in bookmarks)
            {
                if (dataSource.IsSupported(item.Name))
                {
                    DataItem data = dataSource[item.Name];
                    Bookmark bookmark = Bookmark.CreateBookmark(item, this, data.Type);
                    bookmark.Instantiate(data.Value);
                }
            }
        }

        #region Assistant Methods

        protected void WriteLog(string message)
        {
            if (DocumentConvertConfig.LogEnabled)
            {
                File.AppendAllText(DocumentConvertConfig.LogFile, message + "\r\n");
            }
        }

        internal Microsoft.Office.Interop.Word.Table GetClosestTable(Microsoft.Office.Interop.Word.Bookmark bookmark)
        {
            List<Table> tables = new List<Table>();
            foreach (Microsoft.Office.Interop.Word.Table item in _document.Tables)
            {
                if (item.Range.Start <= bookmark.Range.Start && item.Range.End >= bookmark.Range.End)
                {
                    tables.Add(item);
                }
            }

            return tables.OrderByDescending(t => t.Range.Start).FirstOrDefault();
        }

        private void Save(string fileName, Microsoft.Office.Interop.Word.WdSaveFormat format)
        {
            switch (format)
            {
                case Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatDocumentDefault:
                    fileName = string.Format("{0}.docx", fileName);
                    break;
                case Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatPDF:
                    fileName = string.Format("{0}.pdf", fileName);
                    break;
            }
            object objFileName = fileName,
                    objMissing = Type.Missing,
                    objFalse = false,
                    objTrue = true,
                    objFileFormat = format;
            _document.SaveAs(ref objFileName, ref objFileFormat);
            WriteLog(string.Format("文件 {0} 已保存！\r\n",fileName));
        }
        #endregion
    }
}
