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


    #region 解码器
    /// <summary>
    /// 抽象的解码器
    /// </summary>
    public abstract class WebRequestDecoder
    {
        protected HttpContext _context;


        protected WebRequestDecoder(HttpContext request)
        {
            this._context = request;
        }

        /// <summary>
        /// 创建一个解码器
        /// </summary>
        /// <returns></returns>
        public static WebRequestDecoder CreateInstance(HttpContext context)
        {
            string contentType =context.Request.ContentType.ToLower();

            if (contentType.IndexOf("application/contract") >= 0)
            {
                throw new NotImplementedException();
            }
            else if (contentType.IndexOf("application/json") >= 0)
            {
                return new JsonDecoder(context);
            }
            else if (contentType.IndexOf("application/x-www-form-urlencoded") >= 0)
            {
                return new SimpleUrlDecoder(context);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 取得逻辑的方法名称
        /// </summary>
        public virtual string LogicalMethod
        {
            get
            {
                int segLen = this._context.Request.Url.Segments.Length;

                string methodName = this._context.Request.Url.Segments[segLen - 1];

                return methodName;
            }
        }

        /// <summary>
        /// 反序列化话请求中的数据
        /// </summary>
        public abstract Dictionary<string, object> Deserialize();
    }

    /// <summary>
    /// JSON方式的解码器
    /// </summary>
    internal class JsonDecoder : WebRequestDecoder
    {
        internal JsonDecoder(HttpContext context)
            : base(context)
        {
            if (this._context.Request.HttpMethod.ToUpper() == "GET")
            {
                throw new NotSupportedException("不支持GET请求");
            }
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <returns></returns>
        public override Dictionary<string, object> Deserialize()
        {
            Stream inStr = this._context.Request.InputStream;

            inStr.Position = 0;

            byte[] buffer = new byte[inStr.Length];

            inStr.Read(buffer, 0, (int)inStr.Length);

            Encoding en = this._context.Request.ContentEncoding;

            string str = en.GetString(buffer);

            JavaScriptSerializer jsonSer = new JavaScriptSerializer();
            object obj = jsonSer.DeserializeObject(str);

            Dictionary<string, object> result = obj as Dictionary<string, object>;

            NameValueCollection queryStr = this._context.Request.QueryString;

            foreach (string name in queryStr)
            {
                result.Add(name, queryStr[name]);
            }

            return result;
        }
    }

    /// <summary>
    /// JSONContract方式的解码器
    /// </summary>
    internal class JsonContractDecoder : WebRequestDecoder
    {
        internal JsonContractDecoder(HttpContext context)
            : base(context)
        {
            if (this._context.Request.HttpMethod.ToUpper() == "GET")
            {
                throw new NotSupportedException("不支持GET请求");
            }
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <returns></returns>
        public override Dictionary<string, object> Deserialize()
        {
            Stream inStr = this._context.Request.InputStream;

            inStr.Position = 0;

            byte[] buffer = new byte[inStr.Length];

            inStr.Read(buffer, 0, (int)inStr.Length);

            Encoding en = this._context.Request.ContentEncoding;

            string str = en.GetString(buffer);

            JavaScriptSerializer jsonSer = new JavaScriptSerializer();
            object obj = jsonSer.DeserializeObject(str);

            Dictionary<string, object> result = obj as Dictionary<string, object>;

            NameValueCollection queryStr = this._context.Request.QueryString;

            foreach (string name in queryStr)
            {
                result.Add(name, queryStr[name]);
            }

            return result;
        }
    }

    /// <summary>
    /// 简单格式的URL解码器
    /// </summary>
    internal class SimpleUrlDecoder : WebRequestDecoder
    {
        internal SimpleUrlDecoder(HttpContext context)
            : base(context)
        {

        }

        public override Dictionary<string, object> Deserialize()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            NameValueCollection queryStr = this._context.Request.Params;

            foreach (string name in queryStr)
            {
                if (name == "ALL_HTTP")
                    break;

                if (name == null)
                {
                    continue;
                }
                else
                {
                    data.Add(name, queryStr[name]);
                }
            }

            return data;
        }

        public override string LogicalMethod
        {
            get
            {
                int segLen = this._context.Request.Url.Segments.Length;

                string methodName = this._context.Request.Url.Segments[segLen - 1];

                if (methodName.ToLower().LastIndexOf(".ashx") >= 0)
                    return "GetSvr";
                else
                    return methodName;
            }
        }
    }

    #endregion

}
