using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Enterprises.Framework.Plugin.Excel;


namespace Enterprises.Test.Plugin.Excel
{
    [TestClass]
    public class UnitTest1
    {
        /// <summary>
        /// 测试方法：测试依据模板+DataTable来生成EXCEL
        /// </summary>
        [TestMethod]
        public void TestExportToExcelWithTemplateByDataTable()
        {
            DataTable dt = GetDataTable();//获取数据
            string templateFilePath = AppDomain.CurrentDomain.BaseDirectory + "/excel.xlsx"; //获得EXCEL模板路径
            SheetFormatterContainer formatterContainers = new SheetFormatterContainer(); //实例化一个模板数据格式化容器

            PartFormatterBuilder partFormatterBuilder = new PartFormatterBuilder();//实例化一个局部元素格式化器
            partFormatterBuilder.AddFormatter("Title", "跨越IT学员");//将模板表格中Title的值设置为跨越IT学员
            formatterContainers.AppendFormatterBuilder(partFormatterBuilder);//添加到工作薄格式容器中，注意只有添加进去了才会生效

            CellFormatterBuilder cellFormatterBuilder = new CellFormatterBuilder();//实例化一个单元格格式化器
            cellFormatterBuilder.AddFormatter("rptdate", DateTime.Today.ToString("yyyy-MM-dd HH:mm"));//将模板表格中rptdate的值设置为当前日期
            formatterContainers.AppendFormatterBuilder(cellFormatterBuilder);//添加到工作薄格式容器中，注意只有添加进去了才会生效

            //实例化一个表格格式化器，dt.Select()是将DataTable转换成DataRow[]，name表示的模板表格中第一行第一个单元格要填充的数据参数名
            TableFormatterBuilder<DataRow> tableFormatterBuilder = new TableFormatterBuilder<DataRow>(dt.Select(), "name");
            tableFormatterBuilder.AddFormatters(new Dictionary<string, Func<DataRow, object>>{
                {"name",r=>r["Col1"]},//将模板表格中name对应DataTable中的列Col1
                {"sex",r=>r["Col2"]},//将模板表格中sex对应DataTable中的列Col2
                {"km",r=>r["Col3"]},//将模板表格中km对应DataTable中的列Col3
                {"score",r=>r["Col4"]},//将模板表格中score对应DataTable中的列Col4
                {"result",r=>r["Col5"]}//将模板表格中result对应DataTable中的列Co5
            });
            formatterContainers.AppendFormatterBuilder(tableFormatterBuilder);//添加到工作薄格式容器中，注意只有添加进去了才会生效

            string excelPath = Export.ToExcelWithTemplate(templateFilePath, "table", formatterContainers);
            Assert.IsTrue(File.Exists(excelPath));
        }

        private DataTable GetDataTable()
        {
            DataTable dt = new DataTable();
            for (int i = 1; i <= 8; i++)
            {
                if (i == 4)
                {
                    dt.Columns.Add("Col" + i.ToString(), typeof(double));
                }
                else if (i == 7)
                {
                    dt.Columns.Add("Col" + i.ToString(), typeof(DateTime));
                }
                else if (i == 8)
                {
                    dt.Columns.Add("Col" + i.ToString(), typeof(bool));
                }
                else
                {
                    dt.Columns.Add("Col" + i.ToString(), typeof(string));
                }
            }

            for (int i = 1; i <= 100; i++)
            {
                dt.Rows.Add("Name" + i.ToString(), (i % 2) > 0 ? "男" : "女", "科目" + i.ToString(), i * new Random().Next(1, 5), "待定".PadRight(10, '测'), Guid.NewGuid().ToString("N"), null, null);
            }

            return dt;
        }

        private List<Student> GetStudentList()
        {
            List<Student> studentList = new List<Student>();
            for (int i = 1; i <= 10; i++)
            {
                studentList.Add(new Student
                {
                    Name = "Name" + i.ToString(),
                    Sex = (i % 2) > 0 ? "男" : "女",
                    KM = "科目" + i.ToString(),
                    Score = i * new Random().Next(1, 5),
                    Result = "待定"
                });
            }
            return studentList;
        }

        class Student
        {
            public string Name { get; set; }

            public string Sex { get; set; }

            public string KM { get; set; }

            public double Score { get; set; }

            public string Result { get; set; }
        }



        /// <summary>
        /// 测试方法：测试依据模板+List来生成EXCEL
        /// </summary>
        [TestMethod]
        public void TestExportToExcelWithTemplateByList()
        {
            List<Student> studentList = GetStudentList();//获取数据
            string templateFilePath = AppDomain.CurrentDomain.BaseDirectory + "/excel.xlsx"; //获得EXCEL模板路径
            SheetFormatterContainer formatterContainers = new SheetFormatterContainer(); //实例化一个模板数据格式化容器

            PartFormatterBuilder partFormatterBuilder = new PartFormatterBuilder();//实例化一个局部元素格式化器
            partFormatterBuilder.AddFormatter("Title", "跨越IT学员");//将模板表格中Title的值设置为跨越IT学员
            formatterContainers.AppendFormatterBuilder(partFormatterBuilder);//添加到工作薄格式容器中，注意只有添加进去了才会生效

            CellFormatterBuilder cellFormatterBuilder = new CellFormatterBuilder();//实例化一个单元格格式化器
            cellFormatterBuilder.AddFormatter("rptdate", DateTime.Today.ToString("yyyy-MM-dd HH:mm"));//将模板表格中rptdate的值设置为当前日期
            formatterContainers.AppendFormatterBuilder(cellFormatterBuilder);//添加到工作薄格式容器中，注意只有添加进去了才会生效

            //实例化一个表格格式化器，studentList本身就是可枚举的无需转换，name表示的模板表格中第一行第一个单元格要填充的数据参数名
            TableFormatterBuilder<Student> tableFormatterBuilder = new TableFormatterBuilder<Student>(studentList, "name");
            tableFormatterBuilder.AddFormatters(new Dictionary<string, Func<Student, object>>{
                {"name",r=>r.Name},//将模板表格中name对应Student对象中的属性Name
                {"sex",r=>r.Sex},//将模板表格中sex对应Student对象中的属性Sex
                {"km",r=>r.KM},//将模板表格中km对应Student对象中的属性KM
                {"score",r=>r.Score},//将模板表格中score对应Student对象中的属性Score
                {"result",r=>r.Result}//将模板表格中result对应Student对象中的属性Result
            });
            formatterContainers.AppendFormatterBuilder(tableFormatterBuilder);

            string excelPath = Export.ToExcelWithTemplate(templateFilePath, "table", formatterContainers);
            Assert.IsTrue(File.Exists(excelPath));

        }


        /// <summary>
        /// 测试方法：测试依据模板+DataTable来生成多表格EXCEL（注意：由于ExcelReport框架限制，目前仅支持模板文件格式为：xls）
        /// </summary>
        [TestMethod]
        public void TestExportToRepeaterExcelWithTemplateByDataTable()
        {
            DataTable dt = GetDataTable();//获取数据
            string templateFilePath = AppDomain.CurrentDomain.BaseDirectory + "/excel2.xls"; //获得EXCEL模板路径
            SheetFormatterContainer formatterContainers = new SheetFormatterContainer(); //实例化一个模板数据格式化容器

            //实例化一个可重复表格格式化器，dt.Select()是将DataTable转换成DataRow[]，rpt_begin表示的模板表格开始位置参数名，rpt_end表示的模板表格结束位置参数名
            RepeaterFormatterBuilder<DataRow> tableFormatterBuilder = new RepeaterFormatterBuilder<DataRow>(dt.Select(), "rpt_begin", "rpt_end");
            tableFormatterBuilder.AddFormatters(new Dictionary<string, Func<DataRow, object>>{
                {"sex",r=>r["Col2"]},//将模板表格中sex对应DataTable中的列Col2
                {"km",r=>r["Col3"]},//将模板表格中km对应DataTable中的列Col3
                {"score",r=>r["Col4"]},//将模板表格中score对应DataTable中的列Col4
                {"result",r=>r["Col5"]}//将模板表格中result对应DataTable中的列Co5
            });

            PartFormatterBuilder<DataRow> partFormatterBuilder2 = new PartFormatterBuilder<DataRow>();//实例化一个可嵌套的局部元素格式化器
            partFormatterBuilder2.AddFormatter("name", r => r["Col1"]);//将模板表格中name对应DataTable中的列Col1
            tableFormatterBuilder.AppendFormatterBuilder(partFormatterBuilder2);//添加到可重复表格格式化器中，作为其子格式化器


            CellFormatterBuilder<DataRow> cellFormatterBuilder = new CellFormatterBuilder<DataRow>();//实例化一个可嵌套的单元格格式化器
            cellFormatterBuilder.AddFormatter("rptdate", r => DateTime.Today.ToString("yyyy-MM-dd HH:mm"));//将模板表格中rptdate的值设置为当前日期
            tableFormatterBuilder.AppendFormatterBuilder(cellFormatterBuilder);//添加到可重复表格格式化器中，作为其子格式化器

            formatterContainers.AppendFormatterBuilder(tableFormatterBuilder);//添加到工作薄格式容器中，注意只有添加进去了才会生效

            string excelPath = Export.ToExcelWithTemplate(templateFilePath, "multtable", formatterContainers);
            Assert.IsTrue(File.Exists(excelPath));
        }

        /// <summary>
        /// 测试方法：测试依据复杂模板（含固定表格，可重复表格）+DataTable来生成EXCEL （注意：由于ExcelReport框架限制，目前仅支持模板文件格式为：xls）
        /// </summary>
        [TestMethod]
        public void TestExportToExcelWithTemplateByList2()
        {
            var schoolLevelList = SchoolLevel.GetList();
            var classList = ClassInfo.GetList();
            string templateFilePath = AppDomain.CurrentDomain.BaseDirectory + "/mb.xls"; //获得EXCEL模板路径
            SheetFormatterContainer formatterContainers = new SheetFormatterContainer(); //实例化一个模板数据格式化容器

            PartFormatterBuilder partFormatterBuilder = new PartFormatterBuilder();
            partFormatterBuilder.AddFormatter("school", "跨越小学");
            formatterContainers.AppendFormatterBuilder(partFormatterBuilder);

            TableFormatterBuilder<SchoolLevel> tableFormatterBuilder = new TableFormatterBuilder<SchoolLevel>(schoolLevelList, "lv");//实例化一个表格格式化器
            tableFormatterBuilder.AddFormatters(new Dictionary<string, Func<SchoolLevel, object>>
            {
                {"lv",r=>r.LevelName}, //模板参数与数据源SchoolLevel属性对应关系，下同
                {"clscount",r=>r.ClassCount},
                {"lvmaster",r=>r.Master}
            });
            formatterContainers.AppendFormatterBuilder(tableFormatterBuilder);//添加到工作薄格式容器中，注意只有添加进去了才会生效

            RepeaterFormatterBuilder<ClassInfo> repeaterFormatterBuilder = new RepeaterFormatterBuilder<ClassInfo>(classList, "lv_begin", "lv_end");//实例化一个可重复表格格式化器
            repeaterFormatterBuilder.AddFormatters(new Dictionary<string, Func<ClassInfo, object>> { 
                {"class",r=>r.ClassName}, //模板参数与数据源ClassInfo属性对应关系，下同
                {"stucount",r=>r.StudentCount},
                {"clsmaster",r=>r.Master},
                {"lvitem",r=>r.LevelName}
            });
            formatterContainers.AppendFormatterBuilder(repeaterFormatterBuilder);//添加到工作薄格式容器中，注意只有添加进去了才会生效

            string excelPath = Export.ToExcelWithTemplate(templateFilePath, "school", formatterContainers);
            Assert.IsTrue(File.Exists(excelPath));

        }

        /// <summary>
        /// 测试方法：测试依据复杂模板（含固定表格，可重复表格中嵌套表格）+DataTable来生成EXCEL （注意：由于ExcelReport框架限制，目前仅支持模板文件格式为：xls）
        /// </summary>
        [TestMethod]
        public void TestExportToExcelWithTemplateByList3()
        {
            var schoolLevelList = SchoolLevel.GetList();
            var classList = ClassInfo.GetListWithLevels();

            string templateFilePath = AppDomain.CurrentDomain.BaseDirectory + "/mb1.xls"; //获得EXCEL模板路径
            SheetFormatterContainer formatterContainers = new SheetFormatterContainer(); //实例化一个模板数据格式化容器

            PartFormatterBuilder partFormatterBuilder = new PartFormatterBuilder();
            partFormatterBuilder.AddFormatter("school", "跨越小学");
            formatterContainers.AppendFormatterBuilder(partFormatterBuilder);

            TableFormatterBuilder<SchoolLevel> tableFormatterBuilder = new TableFormatterBuilder<SchoolLevel>(schoolLevelList, "lv");//实例化一个表格格式化器
            tableFormatterBuilder.AddFormatters(new Dictionary<string, Func<SchoolLevel, object>>
            {
                {"lv",r=>r.LevelName}, //模板参数与数据源SchoolLevel属性对应关系，下同
                {"clscount",r=>r.ClassCount},
                {"lvmaster",r=>r.Master}
            });
            formatterContainers.AppendFormatterBuilder(tableFormatterBuilder);//添加到工作薄格式容器中，注意只有添加进去了才会生效

            RepeaterFormatterBuilder<KeyValuePair<string, List<ClassInfo>>> repeaterFormatterBuilder = new RepeaterFormatterBuilder<KeyValuePair<string, List<ClassInfo>>>(classList, "lv_begin", "lv_end");
            repeaterFormatterBuilder.AddFormatter("lvitem", r => r.Key);

            TableFormatterBuilder<KeyValuePair<string, List<ClassInfo>>, ClassInfo> tableFormatterBuilder2 = new TableFormatterBuilder<KeyValuePair<string, List<ClassInfo>>, ClassInfo>(r => r.Value, "class");
            tableFormatterBuilder2.AddFormatter("class", r => r.ClassName);
            tableFormatterBuilder2.AddFormatter("stucount", r => r.StudentCount);
            tableFormatterBuilder2.AddFormatter("clsmaster", r => r.Master);

            repeaterFormatterBuilder.AppendFormatterBuilder(tableFormatterBuilder2);

            formatterContainers.AppendFormatterBuilder(repeaterFormatterBuilder);//添加到工作薄格式容器中，注意只有添加进去了才会生效

            string excelPath = Export.ToExcelWithTemplate(templateFilePath, "school", formatterContainers);
            Assert.IsTrue(File.Exists(excelPath));

        }

        /// <summary>
        /// 测试方法：测试依据复杂模板（多工作薄，且含固定表格，可重复表格）+DataSet来生成EXCEL
        /// </summary>
        [TestMethod]
        public void TestExportToExcelWithTemplateByDataSet()
        {
            var ds = GetDataSet();
            string templateFilePath = AppDomain.CurrentDomain.BaseDirectory + "/mb2.xls"; //获得EXCEL模板路径
            Dictionary<string, SheetFormatterContainer> formatterContainerDic = new Dictionary<string, SheetFormatterContainer>(); //实例化一个模板数据格式化容器数组，包含两个SheetFormatterContainer用于格式化两个工作薄


            #region 创建第一个工作薄格式化容器，并设置相关参数对应关系

            SheetFormatterContainer formatterContainer1 = new SheetFormatterContainer();

            PartFormatterBuilder partFormatterBuilder = new PartFormatterBuilder();
            partFormatterBuilder.AddFormatter("school", "跨越小学");
            formatterContainer1.AppendFormatterBuilder(partFormatterBuilder);

            TableFormatterBuilder<DataRow> tableFormatterBuilder = new TableFormatterBuilder<DataRow>(ds.Tables[0].Select(), "lv");//实例化一个表格格式化器
            tableFormatterBuilder.AddFormatters(new Dictionary<string, Func<DataRow, object>>
            {
                {"lv",r=>r["Col1"]}, //模板参数与数据源DataTable属性对应关系，下同
                {"clscount",r=>r["Col2"]},
                {"lvmaster",r=>r["Col3"]}
            });
            formatterContainer1.AppendFormatterBuilder(tableFormatterBuilder);//添加到工作薄格式容器中，注意只有添加进去了才会生效

            RepeaterFormatterBuilder<DataRow> repeaterFormatterBuilder = new RepeaterFormatterBuilder<DataRow>(ds.Tables[1].Select(), "lv_begin", "lv_end");//实例化一个可重复表格格式化器
            repeaterFormatterBuilder.AddFormatters(new Dictionary<string, Func<DataRow, object>> { 
                {"class",r=>r["Col1"]}, //模板参数与数据源ClassInfo属性对应关系，下同
                {"stucount",r=>r["Col2"]},
                {"clsmaster",r=>r["Col3"]},
                {"lvitem",r=>r["Col4"]}
            });
            formatterContainer1.AppendFormatterBuilder(repeaterFormatterBuilder);//添加到工作薄格式容器中，注意只有添加进去了才会生效

            formatterContainerDic.Add("table1", formatterContainer1);//添加到工作薄格式容器数组中，注意此处的Key值为模板上工作薄的名称，此处即为：table1

            #endregion


            #region 创建第二个工作薄格式化容器，并设置相关参数对应关系

            SheetFormatterContainer formatterContainer2 = new SheetFormatterContainer(); //实例化一个模板数据格式化容器

            PartFormatterBuilder partFormatterBuilder2 = new PartFormatterBuilder();//实例化一个局部元素格式化器
            partFormatterBuilder2.AddFormatter("Title", "跨越IT学员");//将模板表格中Title的值设置为跨越IT学员
            formatterContainer2.AppendFormatterBuilder(partFormatterBuilder2);//添加到工作薄格式容器中，注意只有添加进去了才会生效

            CellFormatterBuilder cellFormatterBuilder2 = new CellFormatterBuilder();//实例化一个单元格格式化器
            cellFormatterBuilder2.AddFormatter("rptdate", DateTime.Today.ToString("yyyy-MM-dd HH:mm"));//将模板表格中rptdate的值设置为当前日期
            formatterContainer2.AppendFormatterBuilder(cellFormatterBuilder2);//添加到工作薄格式容器中，注意只有添加进去了才会生效

            //实例化一个表格格式化器，dt.Select()是将DataTable转换成DataRow[]，name表示的模板表格中第一行第一个单元格要填充的数据参数名
            TableFormatterBuilder<DataRow> tableFormatterBuilder2 = new TableFormatterBuilder<DataRow>(ds.Tables[2].Select(), "name");
            tableFormatterBuilder2.AddFormatters(new Dictionary<string, Func<DataRow, object>>{
                {"name",r=>r["Col1"]},//将模板表格中name对应DataTable中的列Col1
                {"sex",r=>r["Col2"]},//将模板表格中sex对应DataTable中的列Col2
                {"km",r=>r["Col3"]},//将模板表格中km对应DataTable中的列Col3
                {"score",r=>r["Col4"]},//将模板表格中score对应DataTable中的列Col4
                {"result",r=>r["Col5"]}//将模板表格中result对应DataTable中的列Co5
            });
            formatterContainer2.AppendFormatterBuilder(tableFormatterBuilder2);//添加到工作薄格式容器中，注意只有添加进去了才会生效

            formatterContainerDic.Add("table2", formatterContainer2);//添加到工作薄格式容器数组中，注意此处的Key值为模板上工作薄的名称，此处即为：table2

            #endregion

            string excelPath = Export.ToExcelWithTemplate(templateFilePath, formatterContainerDic);
            Assert.IsTrue(File.Exists(excelPath));

        }




        /// <summary>
        /// 测试方法：测试将DataTable导出到EXCEL，无模板
        /// </summary>
        [TestMethod]
        public void TestExportToExcelByDataTable()
        {
            DataTable dt = GetDataTable();
            string excelPath = Export.ToExcel(dt, "导出结果");
            Assert.IsTrue(File.Exists(excelPath));
        }


        /// <summary>
        /// 测试方法：测试将DataTable导出到EXCEL，无模板，且指定导出的列名
        /// </summary>
        [TestMethod]
        public void TestExportToExcelByDataTable2()
        {
            DataTable dt = GetDataTable();
            string[] expColNames = { "Col1", "Col2", "Col3", "Col4", "Col5", "Col7" };
            string excelPath = Export.ToExcel(dt, "导出结果", null, expColNames);
            Assert.IsTrue(File.Exists(excelPath));
        }

        /// <summary>
        /// 测试方法：测试将DataTable导出到EXCEL，无模板，且指定导出的列名，以及导出列名的重命名
        /// </summary>
        [TestMethod]
        public void TestExportToExcelByDataTable3()
        {
            DataTable dt = GetDataTable();
            string[] expColNames = { "Col1", "Col2", "Col3", "Col4", "Col5", "Col7" };
            Dictionary<string, string> expColAsNames = new Dictionary<string, string>() { 
                {"Col1","列一"},
                {"Col2","列二"},
                {"Col3","列三"},
                {"Col4","数字列"},
                {"Col5","列五"},
                {"Col7","日期列"}
            };
            string excelPath = Export.ToExcel(dt, "导出结果", null, expColNames, expColAsNames);
            Assert.IsTrue(File.Exists(excelPath));
        }

        /// <summary>
        /// 测试方法：测试将DataTable导出到EXCEL，无模板，且指定导出列名的重命名
        /// </summary>
        [TestMethod]
        public void TestExportToExcelByDataTable4()
        {
            DataTable dt = GetDataTable();
            Dictionary<string, string> expColAsNames = new Dictionary<string, string>() { 
                {"Col1","列一"},
                {"Col5","列五"}
            };
            string excelPath = Export.ToExcel(dt, "导出结果", null, null, expColAsNames);
            Assert.IsTrue(File.Exists(excelPath));
        }

        /// <summary>
        /// 测试方法：测试将DataTable导出到EXCEL，无模板，且指定导出列名重命名数组（顺序及个数必需与DataTable列集合相同）
        /// </summary>
        [TestMethod]
        public void TestExportToExcelByDataTable5()
        {
            DataTable dt = GetDataTable();
            string[] expColAsNames = { "列一", "列二", "列三", "列四", "列五", "列六" };
            // 或者如下将字符串按逗号拆分得到数组
            //expColAsNames = "列一,列二,列三,列四,列五,列六".Split(',');

            string excelPath = Export.ToExcel(dt, expColAsNames, "导出结果");
            Assert.IsTrue(File.Exists(excelPath));
        }

        /// <summary>
        /// 测试方法：测试将DataTable导出到EXCEL，无模板，且指定某些列的显示格式
        /// </summary>
        [TestMethod]
        public void TestExportToExcelByDataTable6()
        {
            DataTable dt = GetDataTable();
            var colDataFormatDic = new Dictionary<string, string>
            {
                {"Col4","0.000"}, //将Col4列DOUBLE类型的EXCEL对应列格式设置为显示成3位小数（默认为2位小数）
                {"Col7","yyyy-mm-dd"}//将Col7列DateTime类型的EXCEL对应列格式设置为年月日（默认为yyyy/mm/dd hh:mm:ss）
            };
            //更多设置格式可在EXCEL的设置单元格格式中的数字选项卡中的自定义格式列表（若无，可自定义，建议先在EXCEL中测试好格式字符串后再用于程序中）

            string excelPath = Export.ToExcel(dt, "导出结果", colDataFormats: colDataFormatDic);
            Assert.IsTrue(File.Exists(excelPath));
        }


        /// <summary>
        /// 测试方法：测试将DataTable导出到EXCEL，无模板，且指定某些列的显示格式
        /// </summary>
        [TestMethod]
        public void TestExportToExcelByDataTable7()
        {
            DataTable dt = GetDataTable();
            var colDataFormatDic = new Dictionary<string, string>
            {
                {"Col4","¥#,##0.00_);(¥#,##0.00)"}, //将Col4列DOUBLE类型的EXCEL对应列格式设置为显示成包含货币格式，如：¥5.00（默认为2位小数）
                {"Col7","yyyy\"年\"m\"月\"d\"日\";@"}//将Col7列DateTime类型的EXCEL对应列格式设置为中文年月日，如：2015年12月5日（默认为yyyy/mm/dd hh:mm:ss）
            };
            //更多设置格式可在EXCEL的设置单元格格式中的数字选项卡中的自定义格式列表（若无，可自定义，建议先在EXCEL中测试好格式字符串后再用于程序中）

            string excelPath = Export.ToExcel(dt, "导出结果", colDataFormats: colDataFormatDic);
            Assert.IsTrue(File.Exists(excelPath));
        }

        /// <summary>
        /// 测试方法：测试将DataTable导出到多工作薄EXCEL
        /// </summary>
        [TestMethod]
        public void TestExportToExcelByDataTable8()
        {
            DataTable dt = GetDataTable();
            string excelPath = Export.ToExcel(dt, "sheet", sheetSize: 10);//指定每个工作薄显示的记录数

            Assert.IsTrue(File.Exists(excelPath));
        }

        /// <summary>
        /// 测试方法：测试依据模板+DataTable来生成多工作薄的EXCEL
        /// </summary>
        [TestMethod]
        public void TestExportToExcelWithTemplateByDataTable2()
        {
            DataTable dt = GetDataTable();
            string templateFilePath = AppDomain.CurrentDomain.BaseDirectory + "/excel.xls"; //获得EXCEL模板路径


            string excelPath = Export.ToExcelWithTemplate<DataRow>(templateFilePath, "table", dt.Select(), (data) =>
            {

                SheetFormatterContainer formatterContainers = new SheetFormatterContainer(); //实例化一个模板数据格式化容器

                PartFormatterBuilder partFormatterBuilder = new PartFormatterBuilder();//实例化一个局部元素格式化器
                partFormatterBuilder.AddFormatter("Title", "跨越IT学员");//将模板表格中Title的值设置为跨越IT学员
                formatterContainers.AppendFormatterBuilder(partFormatterBuilder);//添加到工作薄格式容器中，注意只有添加进去了才会生效

                CellFormatterBuilder cellFormatterBuilder = new CellFormatterBuilder();//实例化一个单元格格式化器
                cellFormatterBuilder.AddFormatter("rptdate", DateTime.Today.ToString("yyyy-MM-dd HH:mm"));//将模板表格中rptdate的值设置为当前日期
                formatterContainers.AppendFormatterBuilder(cellFormatterBuilder);//添加到工作薄格式容器中，注意只有添加进去了才会生效

                //实例化一个表格格式化器，data是已拆分后的数据源(这里是10条记录)，name表示的模板表格中第一行第一个单元格要填充的数据参数名
                TableFormatterBuilder<DataRow> tableFormatterBuilder = new TableFormatterBuilder<DataRow>(data, "name");//这里的数据源设置：data是重点
                tableFormatterBuilder.AddFormatters(new Dictionary<string, Func<DataRow, object>>{
                {"name",r=>r["Col1"]},//将模板表格中name对应DataTable中的列Col1
                {"sex",r=>r["Col2"]},//将模板表格中sex对应DataTable中的列Col2
                {"km",r=>r["Col3"]},//将模板表格中km对应DataTable中的列Col3
                {"score",r=>r["Col4"]},//将模板表格中score对应DataTable中的列Col4
                {"result",r=>r["Col5"]}//将模板表格中result对应DataTable中的列Co5
            });
                formatterContainers.AppendFormatterBuilder(tableFormatterBuilder);//添加到工作薄格式容器中，注意只有添加进去了才会生效
                return formatterContainers;//返回一个模板数据格式化容器

            }, 10);//注意这里的10表示动态生成新的工作薄，且每个工作薄显示10条记录


            Assert.IsTrue(File.Exists(excelPath));
        }




        /// <summary>
        /// 测试方法：测试将指定的EXCEL数据导入到DataTable
        /// </summary>
        [TestMethod]
        public void TestImportToDataTableFromExcel()
        {
            //null表示由用户选择EXCEL文件路径，data表示要导入的sheet名,0表示数据标题行
            DataTable dt = Import.ToDataTable(null, "data", 0);
            Assert.AreNotEqual(0, dt.Rows.Count);
        }

        /// <summary>
        /// 测试方法：测试将DataGridView数据导出到EXCEL文件，无模板，且不导出隐藏列
        /// </summary>
        [TestMethod]
        public void TestToExcelByDataGridView()
        {
            var grid = GetDataGridViewWithData();
            string excelPath = Export.ToExcel(grid, "导出结果", null, false);
            Assert.IsTrue(File.Exists(excelPath));
        }

        /// <summary>
        /// 测试方法：测试将DataGridView数据导出到EXCEL文件，无模板,改变列的显示位置,导出隐藏列
        /// </summary>
        [TestMethod]
        public void TestToExcelByDataGridView2()
        {
            var grid = GetDataGridViewWithData();
            //模拟改变列的显示位置
            grid.Columns[0].DisplayIndex = 1;
            grid.Columns[1].DisplayIndex = 0;
            string excelPath = Export.ToExcel(grid, "导出结果", null, true);
            Assert.IsTrue(File.Exists(excelPath));
        }


        /// <summary>
        /// 测试方法：测试依据模板+DataTable+图片来生成包含图片的EXCEL
        /// </summary>
        [TestMethod]
        public void TestInsertPic()
        {
            DataTable dt = GetDataTable();//获取数据
            string templateFilePath = AppDomain.CurrentDomain.BaseDirectory + "/excel.xlsx"; //获得EXCEL模板路径
            SheetFormatterContainer formatterContainers = new SheetFormatterContainer(); //实例化一个模板数据格式化容器


            PartFormatterBuilder partFormatterBuilder = new PartFormatterBuilder();//实例化一个局部元素格式化器
            partFormatterBuilder.AddFormatter("Title", "跨越IT学员");//将模板表格中Title的值设置为跨越IT学员d
            formatterContainers.AppendFormatterBuilder(partFormatterBuilder);//添加到工作薄格式容器中，注意只有添加进去了才会生效

            CellFormatterBuilder cellFormatterBuilder = new CellFormatterBuilder();//实例化一个单元格格式化器
            cellFormatterBuilder.AddFormatter("rptdate", DateTime.Today.ToString("yyyy-MM-dd HH:mm"));//将模板表格中rptdate的值设置为当前日期
            formatterContainers.AppendFormatterBuilder(cellFormatterBuilder);//添加到工作薄格式容器中，注意只有添加进去了才会生效

            //实例化一个表格格式化器，dt.Select()是将DataTable转换成DataRow[]，name表示的模板表格中第一行第一个单元格要填充的数据参数名
            TableFormatterBuilder<DataRow> tableFormatterBuilder = new TableFormatterBuilder<DataRow>(dt.Select(), "name");
            tableFormatterBuilder.AddFormatters(new Dictionary<string, Func<DataRow, object>>{
                {"name",r=>r["Col1"]},//将模板表格中name对应DataTable中的列Col1
                {"sex",r=>r["Col2"]},//将模板表格中sex对应DataTable中的列Col2
                {"km",r=>r["Col3"]},//将模板表格中km对应DataTable中的列Col3
                {"score",r=>r["Col4"]},//将模板表格中score对应DataTable中的列Col4
                {"result",r=>r["Col5"]}//将模板表格中result对应DataTable中的列Co5
            });
            formatterContainers.AppendFormatterBuilder(tableFormatterBuilder);//添加到工作薄格式容器中，注意只有添加进去了才会生效


            string picPath = AppDomain.CurrentDomain.BaseDirectory + "\\tz.png";//图片路径

            PictureWithShapeFormatterBuilder pictureBuilder = new PictureWithShapeFormatterBuilder();//实例化一个图片关联图形格式化器
            pictureBuilder.AddFormatter(picPath);//当sheet中只有一个图形时，我们可以省略指定区域，那么默认就是把整个工作薄区域当成一个寻找图形区域，若sheet中包含多个，则应指定区域，如下语句
            //pictureBuilder.AddFormatter(picPath,5,60000, 0, 3, false);//第一个参数为图片路径，中间4个参数为数字型指定图形寻找的工作薄区域（行索引，列索引，索引从0开始计）,最后一个为是否自适应大小，一般不建议使用，除非压缩图片

            //pictureBuilder.AddFormatter(picPath, 5, 60000, 0, 255, false);//图形可能下移，可能右移，那么将结束行设为可能最大值：60000,结束列设为可能最大值：255

            //pictureBuilder.AddFormatter(new PictureWithShapeInfo(picPath, new SheetRange() {MinRow=5,MinColumn=0 },false));//此处只指定开始行与开始列，与上面差不多，但建议使用上面的用法

            formatterContainers.AppendFormatterBuilder(pictureBuilder);

            string excelPath = Export.ToExcelWithTemplate(templateFilePath, "table", formatterContainers);
            Assert.IsTrue(File.Exists(excelPath));
        }


        [TestMethod]
        public void TestInsertPic2()
        {
            string templateFilePath = AppDomain.CurrentDomain.BaseDirectory + "/pic.xls"; //获得EXCEL模板路径
            SheetFormatterContainer formatterContainers = new SheetFormatterContainer(); //实例化一个模板数据格式化容器

            string picPath = AppDomain.CurrentDomain.BaseDirectory + "\\tz.png";//图片路径

            PictureWithShapeFormatterBuilder pictureBuilder = new PictureWithShapeFormatterBuilder();//实例化一个图片关联图形格式化器
            pictureBuilder.AddFormatter(picPath);
            formatterContainers.AppendFormatterBuilder(pictureBuilder);

            string excelPath = Export.ToExcelWithTemplate(templateFilePath, "pic", formatterContainers);
            Assert.IsTrue(File.Exists(excelPath));
        }




        private DataGridView GetDataGridViewWithData()
        {
            var grid = new DataGridView();
            var dt = GetDataTable();
            foreach (DataColumn col in dt.Columns)
            {
                bool v = col.Ordinal > 4 ? false : true;
                grid.Columns.Add(new DataGridViewTextBoxColumn() { DataPropertyName = col.ColumnName, HeaderText = "列名" + col.ColumnName, Visible = v, ValueType = col.DataType });
            }
            foreach (DataRow row in dt.Rows)
            {
                ArrayList values = new ArrayList();
                foreach (DataColumn col in dt.Columns)
                {
                    values.Add(row[col]);
                }
                grid.Rows.Add(values.ToArray());
            }
            return grid;
        }


        private DataSet GetDataSet()
        {
            DataTable dt1 = new DataTable("SchoolLevel");
            for (int i = 1; i <= 3; i++)
            {
                if (i == 2)
                {
                    dt1.Columns.Add("Col" + i.ToString(), typeof(int));
                }
                else
                {
                    dt1.Columns.Add("Col" + i.ToString(), typeof(string));
                }
            }

            var rnd = new Random();
            for (int i = 1; i <= 9; i++)
            {
                dt1.Rows.Add(i.ToString() + "年级", (i + rnd.Next(1, 8)) * 5, "牛" + i.ToString());
            }

            DataTable dt2 = new DataTable("ClassInfo");
            for (int i = 1; i <= 4; i++)
            {
                if (i == 3)
                {
                    dt2.Columns.Add("Col" + i.ToString(), typeof(int));
                }
                else
                {
                    dt2.Columns.Add("Col" + i.ToString(), typeof(string));
                }
            }

            string[] fNames = { "张", "李", "赵", "谢" };
            for (int i = 1; i <= 9; i++)
            {
                for (int n = 1; n <= 10; n++)
                {
                    dt2.Rows.Add(i.ToString() + "年级", string.Format("{0}-{1}班", i, n), (i + n) * 5, fNames[rnd.Next(0, 3)] + "某" + i.ToString());
                }
            }


            DataTable dt3 = GetDataTable();

            var ds = new DataSet();
            ds.Tables.AddRange(new[] { dt1, dt2, dt3 });
            return ds;
        }


    }
}
