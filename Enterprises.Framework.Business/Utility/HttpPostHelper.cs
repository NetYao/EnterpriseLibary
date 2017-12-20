using System;
using System.IO;
using System.Net;
using System.Text;

namespace Enterprises.Framework.Utility
{
    /// <summary>
    /// 发起Post请求类
    /// </summary>
    public class HttpPostHelper
    {

        /// <summary>
        /// 发起post请求
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="url">post接收地址</param>
        /// <returns>字符串</returns>
        public static string Send(string data, string url)
        {
            return Send(Encoding.GetEncoding("UTF-8").GetBytes(data), url);
        }

        /// <summary>
        /// 原生态发起post请求
        /// </summary>
        /// <param name="data">post数据</param>
        /// <param name="url">post接收地址</param>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        public static string Send(byte[] data, string url)
        {
            Stream responseStream;
            var request = WebRequest.Create(url) as HttpWebRequest;
            if (request == null)
            {
                throw new ApplicationException(string.Format("Invalid url string: {0}", url));
            }

            request.ContentType = "text/xml";
            request.Method = "POST";
            request.ContentLength = data.Length;
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(data, 0, data.Length);
            requestStream.Close();
            try
            {
                responseStream = request.GetResponse().GetResponseStream();
            }
            catch (Exception exception)
            {
                throw;
            }

            string str = "";
            if (responseStream != null)
            {
                using (var reader = new StreamReader(responseStream, Encoding.GetEncoding("UTF-8")))
                {
                    str = reader.ReadToEnd();
                }

                responseStream.Close();
            }

            return str;
        }

        /// <summary>
        /// 利用System.Net WebClient类方法POST DATA
        /// </summary>
        /// <returns></returns>
        public static string Post(string url,string xml)
        {
            var client = new WebClient {Encoding = Encoding.UTF8};
            var fResponse = client.UploadString(url, xml);
            return fResponse;
        }

        /// <summary>
        /// 利用WebClient类方法异步POST DATA
        /// </summary>
        /// <returns>异步没有返回值</returns>
        public static void PostAsync(string url, string xml)
        {
            var client = new WebClient { Encoding = Encoding.UTF8 };
            var uri = new Uri(url);
            client.UploadStringAsync(uri, xml);
        }
    }
}
