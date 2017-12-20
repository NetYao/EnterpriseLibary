using System.IO;
using NPOI.SS.UserModel;

namespace Enterprises.Framework.Plugin.Excel
{
    public static class NPOIHelper
    {
        #region 1.0 加载模板,获取IWorkbook对象

        /// <summary>
        ///     加载模板,获取IWorkbook对象
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static IWorkbook LoadWorkbook(string filePath)
        {
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read)) //读入excel模板
            {
                return WorkbookFactory.Create(fileStream);
            }
        }

        #endregion
    }
}