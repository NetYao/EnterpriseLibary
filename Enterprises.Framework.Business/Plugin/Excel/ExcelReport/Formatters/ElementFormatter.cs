
namespace Enterprises.Framework.Plugin.Excel.ExcelReport
{
    /// <summary>
    /// 元素格式化器接口
    /// </summary>
    public abstract class ElementFormatter
    {
        /// <summary>
        ///     格式化
        /// </summary>
        /// <param name="sheetAdapter">Sheet适配器</param>
        public abstract void Format(SheetAdapter sheetAdapter);
    }
}