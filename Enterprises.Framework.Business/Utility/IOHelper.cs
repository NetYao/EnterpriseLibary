using System.Web;

namespace Enterprises.Framework.Utility
{
    /// <summary>
    /// IO帮助类
    /// </summary>
    public class IOHelper
    {
        /// <summary>
        /// 获得文件物理路径
        /// </summary>
        /// <returns></returns>
        public static string GetMapPath(string path)
        {
            if (HttpContext.Current != null)
            {
                return HttpContext.Current.Server.MapPath(path);
            }
            else
            {
                return System.Web.Hosting.HostingEnvironment.MapPath(path);
            }
        }

      

      
    }
}
