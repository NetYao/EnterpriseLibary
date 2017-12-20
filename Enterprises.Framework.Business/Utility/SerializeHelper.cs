using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Xml.Serialization;

namespace Enterprises.Framework.Utility
{
    /// <summary>
    /// 序列化帮助类
    /// </summary>
    public sealed class SerializeHelper
    { 
        #region DataContract序列化
        /// <summary>
        /// DataContract序列化
        /// </summary>
        /// <param name="value"></param>
        /// <param name="knownTypes"></param>
        /// <returns></returns>
        public static string SerializeDataContract(object value, List<Type> knownTypes = null)
        {
            DataContractSerializer dataContractSerializer = new DataContractSerializer(value.GetType(), knownTypes);

            using (MemoryStream ms = new MemoryStream())
            {
                dataContractSerializer.WriteObject(ms, value);
                ms.Seek(0, SeekOrigin.Begin);
                using (StreamReader sr = new StreamReader(ms))
                {
                    return sr.ReadToEnd();
                }
            }
        }
        /// <summary>
        /// DataContract反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static T DeserializeDataContract<T>(string xml)
        {
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(T));
                return (T)serializer.ReadObject(ms);
            }
        }
        #endregion

        #region DataContractJson序列化
        /// <summary>
        ///  DataContractJson序列化
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string SerializeDataContractJson(object value)
        {
            DataContractJsonSerializer dataContractSerializer = new DataContractJsonSerializer(value.GetType());
            using (MemoryStream ms = new MemoryStream())
            {                
                dataContractSerializer.WriteObject(ms, value);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }
        /// <summary>
        ///  DataContractJson反序列化
        /// </summary>
        /// <param name="type"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        public static object DeserializeDataContractJson(Type type, string str)
        {
            DataContractJsonSerializer dataContractSerializer = new DataContractJsonSerializer(type);
            using (MemoryStream ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(str)))
            {
                return dataContractSerializer.ReadObject(ms);
            }
        }
        /// <summary>
        /// DataContractJson反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public T DeserializeDataContractJson<T>(string json)
        {
            DataContractJsonSerializer dataContractSerializer = new DataContractJsonSerializer(typeof(T));
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                return (T)dataContractSerializer.ReadObject(ms);
            }
        }
        #endregion

        #region XmlSerializer序列化
        /// <summary>
        /// 将对象序列化到 XML 文档中和从 XML 文档中反序列化对象。XmlSerializer 使您得以控制如何将对象编码到 XML 中。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string SerializeXml(object value)
        {
            XmlSerializer serializer = new XmlSerializer(value.GetType());
            using (MemoryStream ms = new MemoryStream())
            {
                serializer.Serialize(ms, value);
                ms.Seek(0, SeekOrigin.Begin);
                using (StreamReader sr = new StreamReader(ms))
                {
                    return sr.ReadToEnd();
                }
            }
        }
        /// <summary>
        ///  XmlSerializer反序列化
        /// </summary>
        /// <param name="type"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        public static object DeserializeXml(Type type, string str)
        {
            XmlSerializer serializer = new XmlSerializer(type);
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(str);
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                return serializer.Deserialize(ms);
            }
        }
        #endregion

        #region BinaryFormatter序列化
        /// <summary>
        /// BinaryFormatter序列化
        /// 必须类型必须标记为Serializable
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string SerializeBinaryFormatter(object obj)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                formatter.Serialize(ms,obj);
                byte[] bytes = ms.ToArray();
                obj = formatter.Deserialize(new MemoryStream(bytes));
                //如果是UTF8格式，则反序列化报错。可以用Default格式，不过，建议还是传参为byte数组比较好
                return Encoding.Default.GetString(bytes);
            }
        }

        /// <summary>
        /// BinaryFormatter反序列化
        /// 必须类型必须标记为Serializable
        /// </summary>
        /// <param name="serializedStr"></param>
        /// <returns></returns>
        public static T DeserializeBinaryFormatter<T>(string serializedStr)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            byte[] bytes = Encoding.Default.GetBytes(serializedStr);
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                return (T)formatter.Deserialize(ms);
            }
        }
        #endregion 

        #region SoapFormatter序列化
        /// <summary>
        /// SoapFormatter序列化
        /// 必须类型必须标记为Serializable
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string SerializeSoapFormatter(object obj)
        {
            SoapFormatter formatter = new SoapFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                formatter.Serialize(ms, obj);
                byte[] bytes = ms.ToArray();
                return Encoding.UTF8.GetString(bytes);
            }
        }
        /// <summary>
        /// SoapFormatter反序列化
        /// 必须类型必须标记为Serializable
        /// </summary>
        /// <param name="serializedStr"></param>
        /// <returns></returns>
        public static T DeserializeSoapFormatter<T>(string serializedStr)
        {
            SoapFormatter formatter = new SoapFormatter();
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(serializedStr)))
            {
                return (T)formatter.Deserialize(ms);
            }
        }
        #endregion
    }
}
