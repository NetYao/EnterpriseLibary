/****************************************************************************
 *
 * 功能描述：   为了适应前端的ajax请求开发的处理机制
 *
 * 作    者：   wzcheng http://www.cnblogs.com/wzcheng/archive/2010/05/20/1739810.html
 *
 * 修改日期：   2010/04/16 ,2010/04/21
 * 
 * 职    责：   负责将业务逻辑和系统的逻辑分开，使得开发者只关注业务部分的代码 
 *               
 *              
 * 特    性：   它可以适应POST和GET两种方式的请求，无论客户端参试是通过
 *              URL传递还是通过JSON对象。
 * 
 * 例如服务端代码如下：
 * 
 *  public class Handler1 : WebHandler
    {
        public object  GetData(int page, int rows, string sort, string order)
        {
            var obj = new { total = 239, rows = new List<object>() };

            obj.rows.Add(new { code = "001", name = "Name 1", addr = "Address 1", col4 = "col4 data" });
            obj.rows.Add(new { code = "002", name = "Name 1", addr = "Address 2", col4 = "col4 data" });

            return obj;
        }
    }
 * 
 * 客户端可通过如下两种方式请求：
 * 
 * GET:
 * 
 * URL :..\Handler1.ashx\GetData?page=1&rows=2&sort=abc&order=desc
 * 
 * 
 * ------------------------------------------------------------------------
 * POST:
 * 
 * PostUrl:   ..\Handler1.ashx\GetData
 *
 * PostData:  {page:1,rows:2,sort:'abc',order:'desc'}
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

    #region Web后台处理器
    /// <summary>
    /// Web后台处理器
    /// </summary>
    public partial class WebHandler : IHttpHandler, IRequiresSessionState
    {
        #region IHttpHandler Members

        public bool IsReusable
        {
            get { return true; }
        }

        /// <summary>
        /// 处理客户端请求
        /// </summary>
        /// <param name="context"></param>
        public void ProcessRequest(HttpContext context)
        {
             string contextMethod="";

            #region 解码阶段
             WebRequestDecoder decoder = null;
            
            try
            {
                decoder = WebRequestDecoder.CreateInstance(context);
            }
            catch (NotImplementedException)
            {
                decoder=this.OnDecodeResolve(context); 

                if (decoder==null)
                    throw new NotSupportedException("无法为当前的请求提供适当的解码器");
            }

            contextMethod = decoder.LogicalMethod;
            #endregion

            #region 调用阶段
            object result = this.Invoke(contextMethod, decoder.Deserialize());

            ResponseAnnotationAttribute resAnn = this.GetAnnation(contextMethod);

            #endregion

            #region 编码阶段
            WebResponseEncoder encoder = null;
            try
            {
                encoder = WebResponseEncoder.CreateInstance(context, resAnn.ResponseFormat);
            }
            catch (NotImplementedException)
            {
                encoder = this.OnEncoderResolve(context, contextMethod, resAnn);

                if (encoder == null)
                    throw new NotSupportedException("无法为当前的请求提供适当的编码器");
            }

            #region 处理化自定义的序列化机制
            OnSerializeHandler handler = this.OnGetCustomerSerializer(contextMethod);

            if (handler != null)
                encoder.OnSerialize += handler;
            #endregion
            #endregion

            #region 回复阶段
            encoder.Write(result);

            InitializeCachePolicy(context, resAnn);

            #endregion
        }

        /// <summary>
        /// 取得当前的Session
        /// </summary>
        protected HttpSessionState Session
        {
            get { return HttpContext.Current.Session; }
        }

        /// <summary>
        /// 取得当前的Application
        /// </summary>
        protected HttpApplicationState Application
        {
            get { return HttpContext.Current.Application; }
        }

     
        /// <summary>
        /// 取得自定义的序列化处理器。如果重写该方法，并且返回值不为空，则该返回值方法取代默认的编码器的序列化方法
        /// </summary>
        /// <param name="contextMethod"></param>
        /// <returns></returns>
        protected virtual OnSerializeHandler OnGetCustomerSerializer(string contextMethod)
        {
            return null;
        }

        /// <summary>
        /// 当默认的编码器配置无法提供的时候，根据上下文解析新的编码器
        /// </summary>
        /// <param name="context"></param>
        /// <param name="contextMethod"></param>
        /// <param name="resAnn"></param>
        /// <returns></returns>
        protected virtual WebResponseEncoder OnEncoderResolve(HttpContext context, string contextMethod, ResponseAnnotationAttribute resAnn)
        {
            if (contextMethod == "GetSvr")
            {
                return new JQueryScriptEncoder(context);
            }
            return null;
        }

        /// <summary>
        /// 当默认的解码器配置无法提供的时候，根据上下文解析新的解码器
        /// </summary>
        /// <param name="context"></param>
        /// <param name="resAnn"></param>
        /// <returns></returns>
        protected virtual WebRequestDecoder OnDecodeResolve(HttpContext context)
        {
            return new SimpleUrlDecoder(context);
        }

        /// <summary>
        /// 取得方法的返回值标注属性
        /// </summary>
        /// <param name="methodName"></param>
        /// <returns></returns>
        private ResponseAnnotationAttribute GetAnnation(string methodName)
        {
            MethodInfo methInfo = this.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);

            ResponseAnnotationAttribute[] resAnns = (ResponseAnnotationAttribute[])methInfo.GetCustomAttributes(typeof(ResponseAnnotationAttribute), false);

            ResponseAnnotationAttribute ann = null;

            if (resAnns.Length == 0)
                ann = ResponseAnnotationAttribute.Default;
            else
                ann = resAnns[0];

            ann = (ann as ICloneable).Clone() as ResponseAnnotationAttribute;

            ParameterInfo[] ps = methInfo.GetParameters();

            if (ps != null)
                ann.Params = ps.Length;


            return ann;
        }

        /// <summary>
        /// 调用本地的方法
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private object Invoke(string methodName, Dictionary<string, object> args)
        {
            MethodInfo methInfo = this.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);

            if (methInfo == null)
            {
                throw new Exception("指定的逻辑方法不存在");
            }
          
            object[] parameters = this.GetArguments(methodName, args);

            object result = methInfo.Invoke(this, parameters);

            return result;
        }

        /// <summary>
        /// 取得参数列表
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private object[] GetArguments(string methodName, Dictionary<string, object> data)
        {
            MethodInfo methInfo = this.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);

            List<object> args = new List<object>();

            if (methInfo != null)
            {
                ParameterInfo[] paraInfos = methInfo.GetParameters();

                foreach (ParameterInfo p in paraInfos)
                {
                    object obj = data[p.Name];

                    if (null != obj)
                    {
                        args.Add(Convert.ChangeType(obj, p.ParameterType));
                    }
                }
            }

            return args.ToArray();
        }

        /// <summary>
        /// 设置缓存策略
        /// </summary>
        /// <param name="context"></param>
        /// <param name="resAnn"></param>
        private static void InitializeCachePolicy(HttpContext context, ResponseAnnotationAttribute resAnn)
        {
            int cacheDuration = resAnn.CacheDuration;

            if (cacheDuration > 0)
            {
                context.Response.Cache.SetCacheability(HttpCacheability.Server);
                context.Response.Cache.SetExpires(DateTime.Now.AddSeconds(
                                                 (double)cacheDuration));
                context.Response.Cache.SetSlidingExpiration(false);
                context.Response.Cache.SetValidUntilExpires(true);

                if (resAnn.Params > 0)
                {
                    context.Response.Cache.VaryByParams["*"] = true;
                }
                else
                {
                    context.Response.Cache.VaryByParams.IgnoreParams = true;
                }
            }
            else
            {
                context.Response.Cache.SetNoServerCaching();
                context.Response.Cache.SetMaxAge(TimeSpan.Zero);
            }
        }

        #endregion
    }

    #endregion

}
