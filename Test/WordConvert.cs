using System;
using System.Data;
using System.Globalization;
using Enterprises.Framework.Plugin.Office.Converters;
using Enterprises.Framework.Plugin.Office.Converters.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace Test
{
    /// <summary>
    /// word 文档书签转换测试
    /// </summary>
    public class WordConvert
    {
        public void WordConvertTest()
        {
            var dataSource = new DataSource
                {
                    {"QuotationNo", new DataItem {Type = (int) RenderMode.Plain, Value = "QR140507000"}},
                };
           
            var item = new DataItem {Type = (int) RenderMode.Plain, Value = "2014-05-07"};
            dataSource.Add("Date", item);
            item = new DataItem {Type = (int) RenderMode.Plain, Value = "7623"};
            dataSource.Add("CustomerCode", item);
            item = new DataItem {Type = (int) RenderMode.Plain, Value = string.Empty};
            dataSource.Add("VenderCode", item);
            item = new DataItem {Type = (int) RenderMode.Plain, Value = "Microsoft Corporation"};
            dataSource.Add("CustomerName", item);
            item = new DataItem {Type = (int) RenderMode.Plain, Value = "Jon Aswad"};
            dataSource.Add("To", item);
            item = new DataItem {Type = (int) RenderMode.Plain, Value = "张振龙[Zhenlong Zhang]"};
            dataSource.Add("From", item);
            item = new DataItem {Type = (int) RenderMode.Plain, Value = "One  Microsoft Way,ASHBURN,VA,20147"};
            dataSource.Add("ToAddress", item);
            dataSource.Add("FromAddress", DataItem.NewItem(RenderMode.Plain, "No.98-5,  Rd.19  Hangzhou  Eco.&  Tec.  Dev. Zone Hangzhou"));
            dataSource.Add("ToTel", DataItem.NewItem(RenderMode.Plain, "+1(802)8647358 "));
            dataSource.Add("FromTel", DataItem.NewItem(RenderMode.Plain, "+86-571-86714425-323"));
            dataSource.Add("ToFax", DataItem.NewItem(RenderMode.Plain, ""));
            dataSource.Add("FromFax", DataItem.NewItem(RenderMode.Plain, "+86-571-86714426-229"));
            var data = new DataTable();
            var column = new DataColumn("No_0") { DataType = typeof(int) };
            data.Columns.Add(column);
            column = new DataColumn("Item_0") { DataType = typeof(string) };
            data.Columns.Add(column);
            column = new DataColumn("Desc_0") { DataType = typeof(string) };
            data.Columns.Add(column);
            column = new DataColumn("Qty_0") { DataType = typeof(double) };
            data.Columns.Add(column);
            column = new DataColumn("UP_0") { DataType = typeof(double) };
            data.Columns.Add(column);
            column = new DataColumn("CUR_0") { DataType = typeof(string) };
            data.Columns.Add(column);
            column = new DataColumn("AMT_0") { DataType = typeof(double) };
            data.Columns.Add(column);
            var random = new Random();
            double sum = 0.0;
            for (int i = 0; i < 32; i++)
            {
                double value = random.NextDouble();
                DataRow row = data.NewRow();
                row["No_0"] = i + 1;
                row["Item_0"] = string.Format("APEA30{0}", i + 1);
                row["Desc_0"] = string.Format("SHIELD-EMI,FENCE,FRONTSIDE,1{0}", i + 1);
                row["Qty_0"] = 1.0000;
                row["UP_0"] = value;
                row["CUR_0"] = "USD";
                row["AMT_0"] = value;
                sum += value;
                data.Rows.Add(row);
            }

            dataSource.Add("Quotation", DataItem.NewItem(RenderMode.List, data));
            dataSource.Add("Remark", DataItem.NewItem(RenderMode.Plain, @"* This quotation is set up based on current design on May-7-2014.                 
            * This quotation is flexible according to the change of design in the future.                 
            *   Exchange rate: 1 USD = 6.2 RMB. This quotation will be updated if currency fluctuates more than 2%."));
            dataSource.Add("PriceTerm", DataItem.NewItem(RenderMode.Plain, "DAP Suzhou"));
            dataSource.Add("Payment", DataItem.NewItem(RenderMode.Plain, "T/T 30 days"));
            dataSource.Add("LeadTime", DataItem.NewItem(RenderMode.Plain, ""));
            dataSource.Add("Amt_Sum", DataItem.NewItem(RenderMode.Plain, sum.ToString(CultureInfo.InvariantCulture)));
            dataSource.Add("Cur_Sum", DataItem.NewItem(RenderMode.Plain, "USD"));
            dataSource.Add("Origin", DataItem.NewItem(RenderMode.Plain, "P.R.Chinas"));
            dataSource.Add("Packing", DataItem.NewItem(RenderMode.Plain, "Standard bulk packing"));
            dataSource.Add("Beneficiary", DataItem.NewItem(RenderMode.Plain, "Hangzhou Amphenol Phoenix Telecom Parts Co., Ltd."));
            dataSource.Add("AccountNo", DataItem.NewItem(RenderMode.Plain, "3753 5834 2400"));
            dataSource.Add("Name", DataItem.NewItem(RenderMode.Plain, "Bank Of China Hangzhou Economical & Technological Development Area Sub-Branch"));
            dataSource.Add("BankAddress", DataItem.NewItem(RenderMode.Plain, "NO 3 Road Hangzhou Economic And Technological Development Zone Hangzhou China"));
            dataSource.Add("SwiftCode", DataItem.NewItem(RenderMode.Plain, "BKCHCNBJ910"));
            dataSource.Add("PM", DataItem.NewItem(RenderMode.Image, "D:\\Template\\zhenglong.zhang.png"));
            
            var serializer = new JsonSerializer();
            serializer.Converters.Add(new DataSourceJsonConverter());
            serializer.Converters.Add(new DataItemJsonConverter());
            JObject obj = JObject.FromObject(dataSource, serializer);
            string s = obj.ToString();
           

            var doc = new DocumentEntity
                {
                    ID = Guid.NewGuid(),
                    TemplateName = "模板.docx",
                    OutputType = (int) (OutputType.PDF | OutputType.Word),
                    DataSource = s
                };
            
            // 测试文档转换
            var converter = new SimpleConverter();
            converter.Convert(doc);

            // 再生产环境可以调用如下语句直接加入数据库，然后增加文档转换服务自定转换
            //DocumentService.Instance.AddDocument(doc);
            //Enterprises.Framework.Plugin.Office.Converters.DocumentConvert service = new Enterprises.Framework.Plugin.Office.Converters.DocumentConvert();
            //service.Process();
        }
    }
}
