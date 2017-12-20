using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Enterprises.Framework.Utility;


namespace SerializerSample
{
    class Program
    {
        static void Main(string[] args)
        {
            Person p1 = new Person(1001,"Tom");
            Person p1Temp;

            SaleOrder so = new SaleOrder() { DocNo = "SO2001", ProductName = "Tomato", Price = 10 };

            MO mo = new MO() { DocNo="MO2001",ProductName="Car" };

            string serializedStr;


            //DataContract序列化
            serializedStr= SerializeHelper.SerializeDataContract(p1);
            Console.WriteLine(serializedStr);

            //DataContractJson序列化
            serializedStr = SerializeHelper.SerializeDataContractJson(p1);
            Console.WriteLine(serializedStr);

            //Xml序列化
            Console.WriteLine("Xml序列化");
            serializedStr = SerializeHelper.SerializeXml(p1);
            Console.WriteLine(serializedStr);

            serializedStr = SerializeHelper.SerializeXml(mo);
            Console.WriteLine(serializedStr);

            //BinaryFormater序列化
            serializedStr = SerializeHelper.SerializeBinaryFormatter(so);
            Console.WriteLine(serializedStr);

            //自定义序列化
            serializedStr = SerializeHelper.SerializeBinaryFormatter(mo);
            Console.WriteLine(serializedStr);
   
            //SoapFormater序列化
            serializedStr = SerializeHelper.SerializeSoapFormatter(so);
            Console.WriteLine(serializedStr);


            Student s1 = new Student() { Name="Jim"};
            s1.Account = new Account() { AccountNumber = 20040201, Money = 1000 };

            //DataContract序列化
            serializedStr = SerializeHelper.SerializeDataContract(s1);
            Console.WriteLine(serializedStr);
            //反序列化
            Student s1Temp = SerializeHelper.DeserializeDataContract<Student>(serializedStr);
            if (s1Temp != null)
            {
                Console.WriteLine(string.Format("DeserializeDataContract：{0}", s1Temp.Name));
            }

            //DataContractJson序列化
            serializedStr = SerializeHelper.SerializeDataContractJson(s1);
            Console.WriteLine(serializedStr);
            //反序列化
            s1Temp = SerializeHelper.DeserializeDataContractJson(typeof(Student),serializedStr) as Student;
            if (s1Temp != null)
            {
                Console.WriteLine(string.Format("DeserializeDataContract：{0}", s1Temp.Name));
            }

            //BinaryFormater序列化
            serializedStr = SerializeHelper.SerializeBinaryFormatter(so);
            Console.WriteLine(serializedStr);

            //反序列化
            SaleOrder so1Temp = SerializeHelper.DeserializeBinaryFormatter<SaleOrder>(serializedStr);
            if (s1Temp != null)
            {
                Console.WriteLine(string.Format("DeserializeDataContract：{0}", so1Temp.DocNo));
            }

            //SoapFormater序列化
            serializedStr = SerializeHelper.SerializeSoapFormatter(so);
            Console.WriteLine(serializedStr);

            //反序列化
            so1Temp = SerializeHelper.DeserializeSoapFormatter<SaleOrder>(serializedStr);
            if (s1Temp != null)
            {
                Console.WriteLine(string.Format("DeserializeDataContract：{0}", so1Temp.DocNo));
            }


            Console.WriteLine("Input any key to exit.");
            Console.ReadKey();
        }
    }
}
