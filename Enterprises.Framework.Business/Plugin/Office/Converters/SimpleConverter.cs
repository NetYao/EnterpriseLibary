using System;
using System.IO;
using DocumentConvert.Infrastructure;
using Enterprises.Framework.Plugin.Office.Converters.Model;
using Newtonsoft.Json;

namespace Enterprises.Framework.Plugin.Office.Converters
{
   
    /// <summary>
    /// 简单的文档转换实例
    /// </summary>
    public class SimpleConverter : DocumentConverter
    {
        protected override string ConvertDocument(DocumentEntity doc)
        {
            object missing = Type.Missing,
                visible = false,
                documentType = Microsoft.Office.Interop.Word.WdDocumentType.wdTypeDocument,
                fileName = Path.Combine(DocumentConvertConfig.TemplatePath, doc.TemplateName),
                objFalse = false;
            Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.ApplicationClass();
            WriteLog("启动word程序！");
            try
            {
                WriteLog(string.Format("加载模板文件！文件名为{0}!", fileName));
                Microsoft.Office.Interop.Word.Document document = wordApp.Documents.Open(
                    ref fileName,
                    ref missing,
                    ref objFalse,
                    ref missing/* Readonly */,
                    ref missing,
                    ref missing,
                    ref missing,
                    ref missing,
                    ref missing,
                    ref documentType/* DocumentEntity Type */,
                    ref missing,
                    ref visible /* Visible */,
                    ref missing,
                    ref missing,
                    ref missing,
                    ref missing);
                if (document == null)
                {
                    WriteLog("目标文档打开失败！");
                }

                DataSource dataSource = null;
                var serializer = new Newtonsoft.Json.JsonSerializer();
                serializer.Converters.Add(new DataSourceJsonConverter());
                serializer.Converters.Add(new DataItemJsonConverter());
                using (var sReader = new StringReader(doc.DataSource))
                {
                    using (JsonReader reader = new JsonTextReader(sReader))
                    {
                        dataSource = serializer.Deserialize<DataSource>(reader);
                    }
                }

                using (var target = new Model.Document(document))
                {
                    target.Instantiate(dataSource);
                    string path = Path.Combine(DocumentConvertConfig.OutputPath, doc.ID.ToString());
                    WriteLog("保存文件!");
                    target.Save(path, (OutputType)doc.OutputType);
                    doc.DocumentAddress = path;
                }

                doc.Status = (int)DocumentStatus.Processed;
                doc.InfoMessage = string.Empty;

                //var result = DocumentService.Instance.UpdateDocument(doc);
                //if (result == null)
                //{
                //    WriteLog("更新数据库状态失败！");
                //}
            }
            catch (Exception ex)
            {
                doc.InfoMessage = ex.ToString();
                doc.Status = (int)DocumentStatus.Failed;
                WriteLog(ex.ToString());
                //DocumentService.Instance.UpdateDocument(doc);
            }
            finally
            {
                wordApp.Quit(ref objFalse, ref missing, ref missing);
                WriteLog("文档处理完毕,退出word程序！");
                System.Runtime.InteropServices.Marshal.ReleaseComObject(wordApp);
                wordApp = null;
            }

            return string.Empty;
        }
    }
}
