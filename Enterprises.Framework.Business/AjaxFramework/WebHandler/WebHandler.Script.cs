/*********************************************************************************
 *
 * 功能描述：    将服务端的方法映射成为客户端JAVASCRIPT脚本
 *
 * 作    者：    wzcheng
 *
 * 修改日期：    2010/04/22
 * 
 * 职    责：    免除客户端开发者写脚本调用服务端方法
 *              
 * 特    性：   可用html的<script src="xxxx.ashx"> 这样的方式取得客户端调用代理脚本
 
************************************************************************************/


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
    using System.Resources;
   
    public partial class WebHandler : IHttpHandler, IRequiresSessionState
    {

        /// <summary>
        /// 取得服务的客户端代理的脚本86400
        /// </summary>
        /// <returns></returns>
        [ResponseAnnotation(CacheDuration=0,ResponseFormat=ResponseFormat.JavaScript)]
        public StringBuilder GetSvr()
        {
            Type type = this.GetType();
            
            System.Uri url=HttpContext.Current.Request.Url;

            string script = GetScriptTemplete();
            script = script.Replace("%H_DESC%", "通过jQuery.ajax完成服务端函数调用");
            script = script.Replace("%H_DATE%", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            script = script.Replace("%URL%", url.Query.Length>0?url.ToString().Replace(url.Query, ""):url.ToString());
            script = script.Replace("%CLS%", type.Name);

            StringBuilder scriptBuilder = new StringBuilder(script);

            MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            foreach (MethodInfo m in methods)
            {
                ResponseAnnotationAttribute resAnn = this.GetAnnation(m.Name);

                scriptBuilder.AppendLine("/*----------------------------------------------------------------------------");
                scriptBuilder.AppendLine("功能说明:" + resAnn.Desc);
                scriptBuilder.AppendLine("附加说明:缓存时间 " + resAnn.CacheDuration.ToString() + " 秒");
                scriptBuilder.AppendLine("         输出类型 " + resAnn.ResponseFormat.ToString());
                scriptBuilder.AppendLine("----------------------------------------------------------------------------*/");

                string func = GetFunctionTemplete(m,resAnn.ResponseFormat);
                scriptBuilder.AppendLine(func);

            }

            return scriptBuilder;
        }

        /// <summary>
        /// 取得嵌入的脚本模版
        /// </summary>
        /// <returns></returns>
        private static string GetScriptTemplete()
        {
            Type type = typeof(WebHandler);

            AssemblyName asmName = new AssemblyName(type.Assembly.FullName);

            Stream stream = type.Assembly.GetManifestResourceStream(asmName.Name + ".AjaxFramework.WebHandler.net.js");

            if (stream != null)
            {
                byte[] buffer = new byte[stream.Length];
                int len = stream.Read(buffer, 0, (int)stream.Length);

                string temp = Encoding.UTF8.GetString(buffer, 0, len);

                return temp;
            }
            else
            {
                throw new Exception("模版未找到");
            }
        }

        /// <summary>
        /// 取得函数的模版
        /// </summary>
        /// <param name="method"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        private static string GetFunctionTemplete(MethodInfo method, ResponseFormat format)
        {
            StringBuilder func = new StringBuilder(method.DeclaringType.Name);
            func.Append(".prototype." + method.Name);
            func.Append("=function");

            func.Append("(");
            foreach (ParameterInfo p in method.GetParameters())
            {
                func.Append(p.Name + ",");
            }
            func.Append("callback)");

            func.AppendLine("{");
            {
                func.Append("\tvar args = {");
                foreach (ParameterInfo p in method.GetParameters())
                {
                    func.Append(p.Name + ":" + p.Name + ",");
                }
                func.AppendLine("ajax:'jquery1.4.2'};");
                switch (format)
                {
                    case ResponseFormat.HTML:
                        func.AppendLine("\tvar options={dataType:'html'};");
                        break;
                    case ResponseFormat.XML:
                        func.AppendLine("\tvar options={dataType:'xml'};");
                        break;
                    case  ResponseFormat.JSON:
                        func.AppendLine("\tvar options={dataType:'json'};");
                        break;
                    case ResponseFormat.JavaScript:
                        func.AppendLine("\tvar options={dataType:'script'};");
                        break;
                    default:
                        func.AppendLine("\tvar options={dataType:'test'};");
                        break;
                }
                func.AppendLine("\t$.extend(true,options,{},this.Options);");
                func.AppendFormat("\t$.net.CallWebMethod(options,'{0}', args, callback);", method.Name);
                func.AppendLine();
            }
            func.AppendLine("}\t\t");

            return func.ToString();
        }
    }
}
