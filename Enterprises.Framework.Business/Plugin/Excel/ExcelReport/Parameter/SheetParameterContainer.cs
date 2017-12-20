using System.Collections.Generic;
using Enterprises.Framework.Plugin.Excel.ExcelReport.Exceptions;

namespace Enterprises.Framework.Plugin.Excel.ExcelReport
{
    public class SheetParameterContainer
    {
        private List<Parameter> _parameterList = new List<Parameter>();
        public string SheetName { set; get; }

        public List<Parameter> ParameterList
        {
            set { _parameterList = value; }
            get { return _parameterList; }
        }

        /// <summary>
        ///     参数
        /// </summary>
        /// <param name="name">参数名</param>
        /// <returns>参数</returns>
        public Parameter this[string name]
        {
            get
            {
                foreach (Parameter parameter in _parameterList)
                {
                    if (parameter.Name.Equals(name))
                    {
                        return parameter;
                    }
                }
                throw new ExcelReportTemplateException("parameter is not exists");
            }
            set
            {
                bool isExist = false;
                foreach (Parameter parameter in _parameterList)
                {
                    if (parameter.Name.Equals(name))
                    {
                        isExist = true;
                        parameter.RowIndex = value.RowIndex;
                        parameter.ColumnIndex = value.ColumnIndex;
                    }
                }
                if (!isExist)
                {
                    _parameterList.Add(value);
                }
            }
        }
    }
}