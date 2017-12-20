namespace Enterprises.Framework.Plugin.Office.Converters
{
    /// <summary>
    /// 文档书签转换服务接口
    /// </summary>
    public interface IConverter
    {
        /// <summary>
        /// 转换方法
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        string Convert(DocumentEntity doc);
    }
}
