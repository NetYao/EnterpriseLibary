namespace Enterprises.Framework.Utility
{
    using System;
    using System.Web;

    /// <summary>
    /// HttpRequest操作类
    /// </summary>
    public static class HttpRequestExtensions
    {
        /// <summary>
        /// 判断本地URL
        /// </summary>
        /// <param name="request"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool IsLocalUrl(this HttpRequestBase request, string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return false;
            }

            if (url.StartsWith("~/"))
            {
                return true;
            }

            if (url.StartsWith("//") || url.StartsWith(@"/\"))
            {
                return false;
            }

            if (url.StartsWith("/"))
            {
                return true;
            }

            try
            {
                Uri uri = new Uri(url);
                return uri.Authority.Equals(request.Headers["Host"], StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        ///  域名+虚拟目录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string ToApplicationRootUrlString(this HttpRequest request)
        {
            return string.Format("{0}://{1}{2}", request.Url.Scheme, request.Headers["Host"], (request.ApplicationPath == "/") ? string.Empty : request.ApplicationPath);
        }

        /// <summary>
        /// 域名+虚拟目录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string ToApplicationRootUrlString(this HttpRequestBase request)
        {
            return string.Format("{0}://{1}{2}", request.Url.Scheme, request.Headers["Host"], (request.ApplicationPath == "/") ? string.Empty : request.ApplicationPath);
        }

        /// <summary>
        /// 域名
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string ToRootUrlString(this HttpRequest request)
        {
            return string.Format("{0}://{1}", request.Url.Scheme, request.Headers["Host"]);
        }

        /// <summary>
        /// 域名
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string ToRootUrlString(this HttpRequestBase request)
        {
            return string.Format("{0}://{1}", request.Url.Scheme, request.Headers["Host"]);
        }

        /// <summary>
        /// 获取页面地址
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string ToUrlString(this HttpRequest request)
        {
            return string.Format("{0}://{1}{2}", request.Url.Scheme, request.Headers["Host"], request.RawUrl);
        }

        /// <summary>
        /// 获取页面地址
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string ToUrlString(this HttpRequestBase request)
        {
            return string.Format("{0}://{1}{2}", request.Url.Scheme, request.Headers["Host"], request.RawUrl);
        }


    }
}

