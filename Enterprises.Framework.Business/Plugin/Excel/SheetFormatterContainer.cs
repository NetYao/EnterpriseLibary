
using System;
using System.Collections.Generic;
using Enterprises.Framework.Plugin.Excel.ExcelReport;

namespace Enterprises.Framework.Plugin.Excel
{
    /// <summary>
    /// EXCEL工作薄内部格式化器容器类
    /// </summary>
    /// <typeparam name="TDataSource"></typeparam>
    public class SheetFormatterContainer
    {
        private List<IElementFormatterBuilder> FormatterBuilders = null;

        public SheetFormatterContainer()
        {
            FormatterBuilders = new List<IElementFormatterBuilder>();
        }

        public void AppendFormatterBuilder(IFormatterBuilder formatterBuilder)
        {
            if (formatterBuilder is IElementFormatterBuilder)
            {
                FormatterBuilders.Add((IElementFormatterBuilder)formatterBuilder);
            }
            else
            {
                throw new ArgumentException("传入的formatterBuilder必须实现IElementFormatterBuilder接口", "formatterBuilder");
            }
        }

        internal ElementFormatter[] GetFormatters(SheetParameterContainer paramContainer)
        {
            List<ElementFormatter> formatters = new List<ElementFormatter>();
            for (var i = 0; i < FormatterBuilders.Count; i++)
            {
                var builder = FormatterBuilders[i];
                formatters.AddRange(builder.GetElementFormatters(paramContainer));
            }
            return formatters.ToArray();
        }

    }


    /// <summary>
    /// EXCEL工作薄内部格式化器容器类
    /// </summary>
    /// <typeparam name="TDataSource"></typeparam>
    public class SheetFormatterContainer<TDataSource>
    {
        private readonly List<IFormatterBuilder> _formatterBuilders;

        public SheetFormatterContainer()
        {
            _formatterBuilders = new List<IFormatterBuilder>();
        }

        public void AppendFormatterBuilder(IFormatterBuilder formatterBuilder)
        {
            _formatterBuilders.Add(formatterBuilder);
        }

        internal ElementFormatter[] GetFormatters(SheetParameterContainer paramContainer)
        {
            var formatters = new List<ElementFormatter>();
            foreach (var builder in _formatterBuilders)
            {
                if (builder is PartFormatterBuilder)
                {
                    formatters.AddRange((builder as PartFormatterBuilder).GetFormatters(paramContainer));
                }
                else if (builder is CellFormatterBuilder)
                {
                    formatters.AddRange((builder as CellFormatterBuilder).GetFormatters(paramContainer));
                }
                else if (builder is TableFormatterBuilder<TDataSource>)
                {
                    formatters.AddRange((builder as TableFormatterBuilder<TDataSource>).GetFormatters(paramContainer));
                }
            }

            return formatters.ToArray();
        }

    }
}
