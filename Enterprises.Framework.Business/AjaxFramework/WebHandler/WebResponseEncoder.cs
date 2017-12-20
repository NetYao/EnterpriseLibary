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

    #region 编码器

    /// <summary>
    /// 序列化的事件代理
    /// </summary>
    /// <param name="srcObj"></param>
    /// <returns></returns>
    public delegate string OnSerializeHandler(WebResponseEncoder defaultEncoder,object srcObj);


    /// <summary>
    /// 抽象的编码器
    /// </summary>
    public abstract class WebResponseEncoder
    {
        protected HttpContext _context;


        /// <summary>
        /// 序列化之前的事件
        /// </summary>
        protected OnSerializeHandler _onSerialize;


        /// <summary>
        /// 序列化之前的事件
        /// </summary>
        internal event OnSerializeHandler OnSerialize
        {
            add
            {
                this._onSerialize += value;
            }
            remove
            {
                this._onSerialize -= value;
            }
        }

        public static WebResponseEncoder CreateInstance(HttpContext context, ResponseFormat format)
        {
            switch (format)
            {
                case ResponseFormat.JSON:
                    return new JsonEncoder(context);
                case ResponseFormat.XML:
                    throw new NotImplementedException();
                case ResponseFormat.JavaScript:
                    throw new NotImplementedException();
                case ResponseFormat.HTML:
                    return new HtmlEncoder(context);
                default:
                    throw new NotImplementedException();
            }

        }

        protected WebResponseEncoder(HttpContext context)
        {
            List<string> accepts = new List<string>();

            accepts.AddRange(context.Request.AcceptTypes);

            this._context = context;
        }

        /// <summary>
        /// 取得输出对象的类型
        /// </summary>
        public abstract string MimeType { get; }

        /// <summary>
        /// 序列化输出对象
        /// </summary>
        /// <param name="response"></param>
        /// <param name="srcOjb"></param>
        /// <returns></returns>
        public abstract string Serialize(object srcOjb);

        /// <summary>
        /// 将输出对象写到输出流中
        /// </summary>
        /// <param name="srcOjb"></param>
        public virtual void Write(object srcOjb)
        {
            HttpResponse response = this._context.Response;

            string str = this.Serialize(srcOjb);

            response.ContentEncoding = this._context.Request.ContentEncoding;

            response.ContentType = this.MimeType;

            response.Write(str);
        }
    }

    #region JSON方式的编码器
    /// <summary>
    /// JSON方式的编码器
    /// </summary>
    internal class JsonEncoder : WebResponseEncoder
    {
        public JsonEncoder(HttpContext context)
            : base(context)
        {

        }


        public override string MimeType
        {
            get { return "application/json"; }
        }

        public override string Serialize(object srcOjb)
        {
            if (base._onSerialize != null)
            {
                string result = base._onSerialize(this,srcOjb);

                return result;
            }

            if (srcOjb == null)
            {
                return "{}";
            }
            else
            {
                StringBuilder sb = new StringBuilder();

                JavaScriptSerializer jsonSer = new JavaScriptSerializer();

                jsonSer.Serialize(srcOjb, sb);

                return sb.ToString();
            }
        }

    }
    #endregion

    #region HTML编码器
    /// <summary>
    /// HTML编码器
    /// </summary>
    internal class HtmlEncoder : WebResponseEncoder
    {

        public HtmlEncoder(HttpContext context)
            : base(context)
        { }

        public override string MimeType
        {
            get { return @"text/html"; }
        }


        public override string Serialize(object srcOjb)
        {
            if (base._onSerialize != null)
            {
                string result = base._onSerialize(this,srcOjb);

                return result;
            }

            if (srcOjb == null)
            {
                return "<html><body><h3>服务器执行成功!</h3><body></html>";
            }
            else
            {
                StringBuilder sb = new StringBuilder();

                JavaScriptSerializer jsonSer = new JavaScriptSerializer();

                jsonSer.Serialize(srcOjb, sb);

                return "<html><body><div id='result'>" + sb.ToString() + "</div><body></html>";
            }
        }
    }
    #endregion

    #region JQueryScriptEncoder编码器

    /// <summary>
    /// JQueryScriptEncoder编码器
    /// </summary>
    internal class JQueryScriptEncoder : WebResponseEncoder
    {

        public JQueryScriptEncoder(HttpContext context)
            : base(context)
        { }

        public override string MimeType
        {
            get { return @"text/javascript"; }
        }

        public override string Serialize(object srcOjb)
        {
            if (base._onSerialize != null)
            {
                string result = base._onSerialize(this, srcOjb);

                return result;
            }

            if (srcOjb == null)
            {
                return "";
            }
            else
            {
                return srcOjb.ToString();
            }
        }
    }

    #endregion
    #endregion

    
}
