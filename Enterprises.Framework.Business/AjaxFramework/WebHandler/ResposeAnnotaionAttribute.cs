/****************************************************************************
 *
 * 功能描述：    为了适应前端的ajax请求开发的处理机制
 *
 * 作    者：    wzcheng
 *
 * 修改日期：    2010/04/16,2010/04/21
 *  
*****************************************************************************/


namespace Enterprises.Framework
{
    using System;
    using System.Collections.Generic;
    using System.Web;
    using System.Reflection;
    using System.IO;
    using System.Text;
    using System.Collections.Specialized;
    using System.Web.Script.Serialization;
    using System.Web.SessionState;

    #region 返回结果注解

    /// <summary>
    /// 返回的数据类型类型
    /// </summary>
    public enum ResponseFormat
    {
        JSON,

        HTML,

        XML,

        JavaScript,

        /// <summary>
        /// 杂项，相当于任何不确定的格式
        /// </summary>
        Misc
    }


    /// <summary>
    /// 返回值处理的标注属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ResponseAnnotationAttribute : Attribute, ICloneable
    {
        private ResponseFormat _responseFormat;

        private int _caccheDuration = 0;

        private int _params = 0;

        private string _desc = "略";

        /// <summary>
        /// 默认的返回值处理标注
        /// </summary>
        internal static ResponseAnnotationAttribute Default = new ResponseAnnotationAttribute();

        /// <summary>
        /// 设置或取得服务端缓存的时间，单位为秒。默认为0，即没有任何缓存。
        /// </summary>
        public int CacheDuration
        {
            get { return this._caccheDuration; }
            set { this._caccheDuration = value; }
        }


        /// <summary>
        /// 设置或取得返回值类型
        /// </summary>
        public ResponseFormat ResponseFormat
        {
            get { return this._responseFormat; }
            set { this._responseFormat = value; }
        }

        /// <summary>
        /// 设置或取得方法的参数个数
        /// </summary>
        internal int Params
        {
            get { return this._params; }
            set { this._params = value; }
        }

            /// <summary>
        /// 设置或取得方法描述信息
        /// </summary>
        public string  Desc
        {
            get { return this._desc; }
            set { this._desc = value; }
        }

        #region ICloneable Members

        public object Clone()
        {
            ResponseAnnotationAttribute obj = new ResponseAnnotationAttribute();

            obj.ResponseFormat = this.ResponseFormat;
            obj.Params = this.Params;
            obj.CacheDuration = this._caccheDuration;
             obj.Desc = this._desc;
            return obj;
        }

        #endregion
    }

    #endregion
}
