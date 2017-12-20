using NPOI.SS.UserModel;

namespace Enterprises.Framework.Plugin.Excel.Extend
{
    /// <summary>
    /// Row扩展方法
    /// </summary>
    public static class RowExtend
    {
        #region 1.0 清除单元格内容

        /// <summary>
        ///     清除内容
        /// </summary>
        /// <param name="row">行</param>
        /// <returns>行</returns>
        public static IRow ClearContent(this IRow row)
        {
            foreach (ICell cell in row.Cells)
            {
                cell.SetCellValue(string.Empty);
            }
            return row;
        }

        #endregion
    }
}