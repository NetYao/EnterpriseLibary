/****************************************************************************
 *
 * 功能描述：    命名的属性类
 *
 * 作    者：    wzcheng
 *
 * 修改日期：    2009/03/02
 * 
 * 职    责：    该类用于将属性和数据库字段映射使用，并且和传统的ORM采用的
 *               ColumnMappingAttribute相比，该类实现ORM不采用反射技术，争取
 *               运行时控制更简单和快速
 *              
 * 特    性：    
 *  
*****************************************************************************/

namespace Enterprises.Framework
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Xml.Serialization;

    /// <summary>
    /// 命名的属性类
    /// </summary>
    /// <typeparam name="T">通用基本类型</typeparam>
    [Serializable]
    public class NamedProperty<T>
    {
        /// <summary>
        /// 属性名
        /// </summary>
        private string _name;

        /// <summary>
        /// 属性默认值
        /// </summary>
        private T _defaultValue;

        /// <summary>
        /// 私有构造
        /// </summary>
        /// <param name="name">属性名称</param>
        /// <param name="defaultVlaue">属性默认值</param>
        private NamedProperty(string name, T defaultVlaue)
        {
            this._name = name;
            this._defaultValue = defaultVlaue;
        }

        /// <summary>
        /// 属性的名称
        /// </summary>
        public string Name
        {
            get { return this._name; }
        }

        /// <summary>
        /// 属性的默认值
        /// </summary>
        public T DefaultValue
        {
            get { return this._defaultValue; }
        }

        /// <summary>
        /// 创建一个命名属性对象
        /// </summary>
        /// <param name="name">属性名称</param>
        /// <param name="defaultVlaue">属性默认值</param>
        /// <returns>命名属性对象</returns>
        public static NamedProperty<T> Create(string name, T defaultVlaue)
        {
            return new NamedProperty<T>(name, defaultVlaue);
        }

        /// <summary>
        /// 重写命名属性类对象的判断函数
        /// </summary>
        /// <param name="obj">命名属性类对象</param>
        /// <returns>true:对象属性名称相等;false:对象属性名称不等;</returns>
        public override bool Equals(object obj)
        {
            return (obj as NamedProperty<T>).Name.Equals(this.Name);
        }

        /// <summary>
        /// 重写命名属性类对象的获取哈希码函数
        /// </summary>
        /// <returns>命名属性类对象的属性名称的哈希码</returns>
        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }
    }
}
