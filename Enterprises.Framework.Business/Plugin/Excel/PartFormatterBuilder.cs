
using System;
using Enterprises.Framework.Plugin.Excel.ExcelReport;

namespace Enterprises.Framework.Plugin.Excel
{
    /// EXCEL模板单元格内局部数据格式化器创建者类（独立使用）
    public class PartFormatterBuilder : FormatterBuilder<object>
    {
        protected override ElementFormatter CreateElementFormatter(Parameter param, object value)
        {
            return new PartFormatter(param, value);
        }
    }

    /// EXCEL模板单元格内局部数据格式化器创建者类（嵌套使用）
    public class PartFormatterBuilder<T> : FormatterBuilder<T, Func<T, object>>
    {

        protected override EmbeddedFormatter<T> CreateElementFormatter(Parameter param, Func<T, object> value)
        {
            return new PartFormatter<T>(param, value);
        }
    }

}
