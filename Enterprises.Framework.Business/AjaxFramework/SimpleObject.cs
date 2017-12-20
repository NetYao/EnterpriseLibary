/****************************************************************************
 *
 * 功能描述：    简单对象类
 *
 * 作    者：    wzcheng
 *
 * 修改日期：    2009/12/10
 * 
 * 职    责：    
 *              
 * 特    性：    
 *  
*****************************************************************************/

namespace Enterprises.Framework
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Reflection;
    using System.Text;
    using System.Xml.Serialization;
    using System.Data;

    /// <summary>
    /// 简单对象
    /// </summary>
    [Serializable]
    [XmlType(Namespace = "http://gczx/2010/model")]
    [XmlRoot(Namespace = "http://gczx/2010/model")]
    public abstract class SimpleObject
    {
        /// <summary>
        /// 数据字典
        /// </summary>
        private Dictionary<string, object> _properties = null;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="properties">数据字典</param>
        public SimpleObject(Dictionary<string, object> properties)
        {
            if (properties == null)
            {
                throw new ArgumentNullException("数据字典不能为空");
            }

            this._properties = properties;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public SimpleObject()
        {
            this._properties = new Dictionary<string, object>();
        }

        /// <summary>
        /// 取得对象的数据字典
        /// </summary>
        [XmlIgnore]
        public Dictionary<string, object> Properties
        {
            get
            {
                if (this._properties == null)
                {
                    this._properties = new Dictionary<string, object>();
                }

                return this._properties;
            }
        }

        /// <summary>
        /// 将数据表转化为属性键值对字典集
        /// </summary>
        /// <param name="dataTable">数据源表</param>
        /// <returns>属性键值对字典集</returns>
        public static List<Dictionary<string, object>> GetSimpleObjects(DataTable dataTable)
        {
            List<Dictionary<string, object>> diclist = new List<Dictionary<string, object>>();
            foreach (DataRow dr in dataTable.Rows)
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();
                foreach (DataColumn dc in dataTable.Columns)
                {
                    dic.Add(dc.ColumnName, dr[dc.ColumnName]);
                }
                diclist.Add(dic);
            }
            return diclist;
        }

        /// <summary>
        /// 从读取器枚举出属性键值对字典集
        /// </summary>
        /// <param name="reader">数据读取器</param>
        /// <param name="mustCloseReader">接收读取后是否强制关闭</param>
        /// <returns>属性键值对字典枚举集</returns>
        public static IEnumerable<Dictionary<string, object>> EnumerateObjects(SqlDataReader reader, bool mustCloseReader)
        {
            if (reader.IsClosed)
            {
                throw new ArgumentException("reader", "读取器已关闭");
            }

            object[] values = new object[reader.FieldCount];

            while (reader.Read())
            {
                reader.GetValues(values);

                Dictionary<string, object> dic = new Dictionary<string, object>();

                dic.Clear();

                for (int i = 0; i < values.Length; i++)
                {
                    string key = reader.GetName(i);

                    if (!dic.ContainsKey(key))
                    {
                        dic.Add(key, values[i]);
                    }
                }


                yield return dic;
            }

            if (mustCloseReader)
            {
                reader.Close();
            }
        }

        /// <summary>
        /// 从读取器枚举出通用类型对象集
        /// </summary>
        /// <typeparam name="T">通用基本类型</typeparam>
        /// <param name="reader">数据读取器</param>
        /// <param name="mustCloseReader">接收读取后是否强制关闭</param>
        /// <returns>通用类型对象枚举集</returns>
        public static IEnumerable<T> EnumerateObjects<T>(SqlDataReader reader, bool mustCloseReader)
        {
            Type t = typeof(T);

            ConstructorInfo ctlInfo= t.GetConstructor(new Type[] { typeof(Dictionary<string, object>) });

            if (ctlInfo == null)
            {
                throw new Exception("泛型T未提供'Dictionary<string, object>'参数类型的构造函数");
            }
 
            IEnumerable<Dictionary<string, object>> ems = EnumerateObjects(reader, mustCloseReader);

            foreach (Dictionary<string, object> dic in ems)
            {
                object obj = ctlInfo.Invoke(new object[] { dic });

                yield return (T)obj;
            }
        }

        /// <summary>
        /// 设置一个属性的值
        /// </summary>
        /// <typeparam name="T">属性值类型</typeparam>
        /// <param name="propName">属性名称</param>
        /// <param name="propValue">属性值</param>
        public void SetValue<T>(NamedProperty<T> propName, T propValue)
        {
            if (propName == null)
            {
                throw new ArgumentNullException("propName", "参数不能为空");
            }

            if (!this.Properties.ContainsKey(propName.Name))
            {
                this.Properties.Add(propName.Name, propValue == null ? propName.DefaultValue : propValue);
            }
            else
            {
                this.Properties[propName.Name] = propValue == null ? propName.DefaultValue : propValue;
            }
        }

        /// <summary>
        /// 取得属性的值
        /// </summary>
        /// <typeparam name="T">属性值类型</typeparam>
        /// <param name="propName">属性名称</param>
        /// <returns>对象属性值</returns>
        public T GetValue<T>(NamedProperty<T> propName)
        {
            if (propName == null)
            {
                throw new ArgumentNullException("propName", "参数不能为空");
            }

            if (this.Properties.ContainsKey(propName.Name))
            {
                object obj = this.Properties[propName.Name];

                if (obj is DBNull)
                {
                    return propName.DefaultValue;
                }
                else
                {
                    return (T)Convert.ChangeType(this.Properties[propName.Name], typeof(T));
                }
            }
            else
            {
                return propName.DefaultValue;
            }
        }

        /// <summary>
        /// 从数据表枚举属性键值对字典集
        /// </summary>
        /// <param name="dataTable">数据源表</param>
        /// <returns>属性键值对字典枚举集</returns>
        internal static IEnumerable<Dictionary<string, object>> EnumerateObjects(DataTable dataTable)
        {
            foreach (DataRow dr in dataTable.Rows)
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();
                foreach (DataColumn dc in dataTable.Columns)
                {
                    dic.Add(dc.ColumnName, dr[dc.ColumnName]);
                }

                yield return dic;
            }
        }
    }
}