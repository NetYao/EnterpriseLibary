using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Data;
using System.Runtime.Serialization;
using System.Text;
using System.Runtime.Serialization.Json;
using System.Collections.Specialized;
using Enterprises.Framework;

namespace WebHandlerTest
{
    public class DemoHandler : WebHandler
    {
        [ResponseAnnotation(CacheDuration = 30, Desc = "演示返回动态对象和服务端缓存")]
        public object GetVar(string str,int i)
        {
            var obj = new { total = i, str = str};

            return obj;
        }

        public object GetVar2(string str, int i)
        {
            var obj = new { total = i, str = str };

            return obj;
        }

        /// <summary>
        /// 这里演示了非契约式的数据字典和强类型对象之间的映射
        /// </summary>
        /// <param name="stu"></param>
        /// <returns></returns>
        public string AddStudent(Dictionary<string,object> stu)
        {
           Student student= new Student(stu);

           return "已完成:" + student.SN;
        }

        [ResponseAnnotation(Desc = "取得表格需要的数据源")]
        public object GetData(int page, int rows, string sort, string order)
        {
            var obj = new { total = 2, rows = new List<object>() };

            obj.rows.Add(new { code = "001", name = "Name 1", addr = "Address 1", col4 = "col4 data" });
            obj.rows.Add(new { code = "002", name = "Name 1", addr = "Address 2", col4 = "col4 data" });

            return obj;
        }


        [ResponseAnnotation(ResponseFormat = ResponseFormat.HTML,Desc="演示自定义序列化为HTML")]
        public Person GetPerson(string name, string address, string city)
        {
            Person p = new Person();

            p.Address = address;
            p.City = city;
            p.Name = name;
            

            return p;
        }

        /// <summary>
        /// 这样的设计方式是为了给patial方法后期注入到类型，改变默认的序列化提供基础
        /// </summary>
        /// <param name="contextMethod"></param>
        /// <returns></returns>
        protected override OnSerializeHandler OnGetCustomerSerializer(string contextMethod)
        {
            if (contextMethod == "GetPerson")
                return new OnSerializeHandler(ToHtml);
            else
                return base.OnGetCustomerSerializer(contextMethod);
        }

        private string ToHtml(WebResponseEncoder defEncoder, object srcObj)
        {
            //以下为视图和数据分离的模式

            Person p = srcObj as Person;

            HttpContext.Current.Session["person"] = p;

            StringWriter writer = new StringWriter();
            HttpContext.Current.Server.Execute("PersonView.aspx", writer, false);

            return writer.ToString();
        }
    }

    public class Person
    {
        public string Name = "";

        public string Address = "";

        public string City = "";
    }

    [DataContract]
    [Serializable]
    public class Student :SimpleObject
    {
        
        public static readonly NamedProperty<int> PropAge = NamedProperty<int>.Create("Age", 20);

        public static readonly NamedProperty<string> PropSN = NamedProperty<string>.Create("SN", "000-000");

        public static readonly NamedProperty<string> PropName = NamedProperty<string>.Create("Name", "unknow");

        [DataMember(Order = 1)]
        public string SN
        {
            get { return base.GetValue<string>(PropSN); }

            set { base.SetValue<string>(PropSN, value); }
        }

        [DataMember(Order = 2)]
        public string Name
        {
            get { return base.GetValue<string>(PropName); }

            set { base.SetValue<string>(PropName, value); }
        }

        [DataMember(Order = 3)]
        public int Age
        {
            get { return base.GetValue<int>(PropAge); }

            set { base.SetValue<int>(PropAge, value); }
        }

        public Student(Dictionary<string,object> data):base(data)
        {
            
        }

        public Student():base()
        { }
    }

}
