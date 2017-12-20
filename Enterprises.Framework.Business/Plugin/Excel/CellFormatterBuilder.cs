using System;
using Enterprises.Framework.Plugin.Excel.ExcelReport;

namespace Enterprises.Framework.Plugin.Excel
{
    /// EXCEL模板单元格数据格式化器创建者类（独立使用）
    public class CellFormatterBuilder : FormatterBuilder<object>
    {
        protected override ExcelReport.ElementFormatter CreateElementFormatter(ExcelReport.Parameter param, object value)
        {
            return new CellFormatter(param, value);
        }
    }

    /// EXCEL模板单元格数据格式化器创建者类（嵌套使用）
    public class CellFormatterBuilder<T> : FormatterBuilder<T, Func<T, object>>
    {

        protected override EmbeddedFormatter<T> CreateElementFormatter(Parameter param, Func<T, object> value)
        {
            return new CellFormatter<T>(param, value);
        }
    }
}
