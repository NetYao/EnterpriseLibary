using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Enterprises.Framework.Utility
{
    public class Base64
    {
        /// <summary>
        /// 将字符串化成Base64编码的数据。
        /// </summary>
        /// <param name="dataString">字符串数据。</param>
        /// <returns>Base64编码数据。</returns>
        public static string Encode(string dataString)
        {
            byte[] base64bytes = Encoding.UTF8.GetBytes(dataString);
            return Convert.ToBase64String(base64bytes);
        }


        /// <summary>
        /// 将Base64编码的数据转化成字符串。
        /// </summary>
        /// <param name="base64String">Base64编码数据。</param>
        /// <returns>字符串数据。</returns>
        public static string Decode(string base64String)
        {
            byte[] bytes = Convert.FromBase64String(base64String);


            return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        }
    }
}
