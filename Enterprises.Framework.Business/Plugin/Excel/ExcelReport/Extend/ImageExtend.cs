

using System.Drawing;
using System.IO;

namespace Enterprises.Framework.Plugin.Excel.ExcelReport
{
    public static class ImageExtend
    {
        /// <summary>
        /// 将图片转换为字节数组
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static byte[] ToBuffer(this Image img)
        {
            using (var ms = new MemoryStream())
            {
                img.Save(ms, img.RawFormat);
                ms.Flush();
                ms.Position = 0;
                return ms.GetBuffer();
            }
        }
    }
}