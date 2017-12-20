using System;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Enterprises.Framework.Utility
{
    /// <summary>
    /// 字符串帮助类
    /// </summary>
    public class StringHelper
    {

        /// <summary>
        /// 获得字符串的长度,一个汉字的长度为1
        /// </summary>
        public static int GetStringLength(string s)
        {
            if (!string.IsNullOrEmpty(s))
                return Encoding.Default.GetBytes(s).Length;

            return 0;
        }

        #region 分割字符串

        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <param name="sourceStr">源字符串</param>
        /// <param name="splitStr">分隔字符串</param>
        /// <returns></returns>
        public static string[] SplitString(string sourceStr, string splitStr)
        {
            if (string.IsNullOrEmpty(sourceStr) || string.IsNullOrEmpty(splitStr))
                return new string[] { };

            if (sourceStr.IndexOf(splitStr, System.StringComparison.Ordinal) == -1)
                return new[] { sourceStr };

            if (splitStr.Length == 1)
                return sourceStr.Split(splitStr[0]);
            return Regex.Split(sourceStr, Regex.Escape(splitStr), RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <param name="sourceStr">源字符串</param>
        /// <returns></returns>
        public static string[] SplitString(string sourceStr)
        {
            return SplitString(sourceStr, ",");
        }

        #endregion

        #region 截取字符串

        /// <summary>
        /// 截取字符串
        /// </summary>
        /// <param name="sourceStr">源字符串</param>
        /// <param name="startIndex">开始位置的索引</param>
        /// <param name="length">子字符串的长度</param>
        /// <returns></returns>
        public static string SubString(string sourceStr, int startIndex, int length)
        {
            if (!string.IsNullOrEmpty(sourceStr))
            {
                if (sourceStr.Length >= (startIndex + length))
                    return sourceStr.Substring(startIndex, length);
                return sourceStr.Substring(startIndex);
            }

            return "";
        }

        /// <summary>
        /// 截取字符串
        /// </summary>
        /// <param name="sourceStr">源字符串</param>
        /// <param name="length">子字符串的长度</param>
        /// <returns></returns>
        public static string SubString(string sourceStr, int length)
        {
            return SubString(sourceStr, 0, length);
        }

        #endregion

        #region 移除前导/后导字符串

        /// <summary>
        /// 移除前导字符串
        /// </summary>
        /// <param name="sourceStr">源字符串</param>
        /// <param name="trimStr">移除字符串</param>
        /// <returns></returns>
        public static string TrimStart(string sourceStr, string trimStr)
        {
            return TrimStart(sourceStr, trimStr, true);
        }

        /// <summary>
        /// 移除前导字符串
        /// </summary>
        /// <param name="sourceStr">源字符串</param>
        /// <param name="trimStr">移除字符串</param>
        /// <param name="ignoreCase">是否忽略大小写</param>
        /// <returns></returns>
        public static string TrimStart(string sourceStr, string trimStr, bool ignoreCase)
        {
            if (string.IsNullOrEmpty(sourceStr))
                return string.Empty;

            if (string.IsNullOrEmpty(trimStr) || !sourceStr.StartsWith(trimStr, ignoreCase, CultureInfo.CurrentCulture))
                return sourceStr;

            return sourceStr.Remove(0, trimStr.Length);
        }

        /// <summary>
        /// 移除后导字符串
        /// </summary>
        /// <param name="sourceStr">源字符串</param>
        /// <param name="trimStr">移除字符串</param>
        /// <returns></returns>
        public static string TrimEnd(string sourceStr, string trimStr)
        {
            return TrimEnd(sourceStr, trimStr, true);
        }

        /// <summary>
        /// 移除后导字符串
        /// </summary>
        /// <param name="sourceStr">源字符串</param>
        /// <param name="trimStr">移除字符串</param>
        /// <param name="ignoreCase">是否忽略大小写</param>
        /// <returns></returns>
        public static string TrimEnd(string sourceStr, string trimStr, bool ignoreCase)
        {
            if (string.IsNullOrEmpty(sourceStr))
                return string.Empty;

            if (string.IsNullOrEmpty(trimStr) || !sourceStr.EndsWith(trimStr, ignoreCase, CultureInfo.CurrentCulture))
                return sourceStr;

            return sourceStr.Substring(0, sourceStr.Length - trimStr.Length);
        }

        /// <summary>
        /// 移除前导和后导字符串
        /// </summary>
        /// <param name="sourceStr">源字符串</param>
        /// <param name="trimStr">移除字符串</param>
        /// <returns></returns>
        public static string Trim(string sourceStr, string trimStr)
        {
            return Trim(sourceStr, trimStr, true);
        }

        /// <summary>
        /// 移除前导和后导字符串
        /// </summary>
        /// <param name="sourceStr">源字符串</param>
        /// <param name="trimStr">移除字符串</param>
        /// <param name="ignoreCase">是否忽略大小写</param>
        /// <returns></returns>
        public static string Trim(string sourceStr, string trimStr, bool ignoreCase)
        {
            if (string.IsNullOrEmpty(sourceStr))
                return string.Empty;

            if (string.IsNullOrEmpty(trimStr))
                return sourceStr;

            if (sourceStr.StartsWith(trimStr, ignoreCase, CultureInfo.CurrentCulture))
                sourceStr = sourceStr.Remove(0, trimStr.Length);

            if (sourceStr.EndsWith(trimStr, ignoreCase, CultureInfo.CurrentCulture))
                sourceStr = sourceStr.Substring(0, sourceStr.Length - trimStr.Length);

            return sourceStr;
        }

        #endregion

        /// <summary>
        /// 获得指定顺序的字符在字符串中的位置索引
        /// </summary>
        /// <param name="s">字符串</param>
        /// <param name="order">顺序</param>
        /// <returns></returns>
        public static int IndexOf(string s, int order)
        {
            return IndexOf(s, '-', order);
        }

        /// <summary>
        /// 获得指定顺序的字符在字符串中的位置索引
        /// </summary>
        /// <param name="s">字符串</param>
        /// <param name="c">字符</param>
        /// <param name="order">顺序</param>
        /// <returns></returns>
        public static int IndexOf(string s, char c, int order)
        {
            int length = s.Length;
            for (int i = 0; i < length; i++)
            {
                if (c == s[i])
                {
                    if (order == 1)
                        return i;
                    order--;
                }
            }
            return -1;
        }


        /// <summary>
        /// 去除字符串首尾处的空格、回车、换行符、制表符
        /// </summary>
        public static string TbbrTrim(string str)
        {
            if (!string.IsNullOrEmpty(str))
                return str.Trim().Trim('\r').Trim('\n').Trim('\t');
            return string.Empty;
        }

        //空格、回车、换行符、制表符正则表达式
        private static readonly Regex TbbrRegex = new Regex(@"\s*|\t|\r|\n", RegexOptions.IgnoreCase);
        
        /// <summary>
        /// 去除字符串中的空格、回车、换行符、制表符
        /// </summary>
        public static string ClearTbbr(string str)
        {
            if (!string.IsNullOrEmpty(str))
                return TbbrRegex.Replace(str, "");

            return string.Empty;
        }

        /// <summary>
        /// 删除字符串中的空行
        /// </summary>
        /// <returns></returns>
        public static string DeleteNullOrSpaceRow(string s)
        {
            if (string.IsNullOrEmpty(s))
                return "";

            string[] tempArray = StringHelper.SplitString("\r\n");
            var result = new StringBuilder();
            foreach (string item in tempArray)
            {
                if (!string.IsNullOrWhiteSpace(item))
                    result.AppendFormat("{0}\r\n", item);
            }
            if (result.Length > 0)
                result.Remove(result.Length - 2, 2);
            return result.ToString();
        }

        /// <summary>
        ///获得邮箱提供者
        /// </summary>
        /// <param name="email">邮箱</param>
        /// <returns></returns>
        public static string GetEmailProvider(string email)
        {
            int index = email.LastIndexOf('@');
            if (index > 0)
                return email.Substring(index + 1);
            return string.Empty;
        }

        /// <summary>
        /// 转义正则表达式
        /// </summary>
        public static string EscapeRegex(string s)
        {
            string[] oList = { "\\", ".", "+", "*", "?", "{", "}", "[", "^", "]", "$", "(", ")", "=", "!", "<", ">", "|", ":" };
            string[] eList = { "\\\\", "\\.", "\\+", "\\*", "\\?", "\\{", "\\}", "\\[", "\\^", "\\]", "\\$", "\\(", "\\)", "\\=", "\\!", "\\<", "\\>", "\\|", "\\:" };
            for (int i = 0; i < oList.Length; i++)
                s = s.Replace(oList[i], eList[i]);
            return s;
        }

        /// <summary>
        /// 隐藏邮箱
        /// </summary>
        public static string HideEmail(string email)
        {
            int index = email.LastIndexOf('@');

            if (index == 1)
                return "*" + email.Substring(index);
            if (index == 2)
                return email[0] + "*" + email.Substring(index);

            StringBuilder sb = new StringBuilder();
            sb.Append(email.Substring(0, 2));
            int count = index - 2;
            while (count > 0)
            {
                sb.Append("*");
                count--;
            }
            sb.Append(email.Substring(index));
            return sb.ToString();
        }

        /// <summary>
        /// 隐藏手机
        /// </summary>
        public static string HideMobile(string mobile)
        {
            return mobile.Substring(0, 3) + "*****" + mobile.Substring(8);
        }


        /// <summary>
        /// 人民币转换
        /// </summary>
        /// <param name="moneySource"></param>
        /// <returns></returns>
        public static string DoubleToChnMoney(double moneySource)
        {
            Int64 money = Convert.ToInt64(moneySource * 100);
            string[] cstr = { "零", "壹", "贰", "叁", "肆", "伍", "陆", "柒", "捌", "玖" };
            string[] wstr = { "分", "角", "圆", "拾", "佰", "仟", "万", "拾", "佰", "仟", "亿", "拾", "佰", "仟", "兆", "拾", "佰", "仟" };
            string moneyStr = money.ToString(CultureInfo.InvariantCulture);
            string bigMoney = "";
            int di, dj = moneyStr.Length;
            for (di = 0; di < dj; di++)
            {
                int tm = Convert.ToInt16(moneyStr.Substring(dj - di - 1, 1));
                bigMoney += wstr[di] + cstr[tm];
            }
            string bm = string.Empty;
            foreach (char a in bigMoney.ToCharArray())
            {
                bm = a + bm;
            }
            for (di = 0; di < dj; di++)
            {
                bm = bm.Replace("零仟零佰零拾零圆", "圆");
                bm = bm.Replace("零仟零佰零拾零万", "万");
                bm = bm.Replace("零零", "零");
                bm = bm.Replace("零圆", "圆");
                bm = bm.Replace("零拾", "零");
                bm = bm.Replace("零佰", "零");
                bm = bm.Replace("零仟", "零");
                bm = bm.Replace("零万", "万");
                bm = bm.Replace("零亿", "亿");
                bm = bm.Replace("零角零分", "整");
                bm = bm.Replace("零分", "");
                bm = bm.Replace("零角", "");
            }
            return bm;
        }

        /// <summary>
        /// 人民币转换
        /// </summary>
        /// <param name="moneySource"></param>
        /// <returns></returns>
        public static string StringToChnMoney(string moneySource)
        {
            double tM = Convert.ToDouble(moneySource);
            Int64 money = Convert.ToInt64(tM * 100);
            string[] cstr = { "零", "壹", "贰", "叁", "肆", "伍", "陆", "柒", "捌", "玖" };
            string[] wstr = { "分", "角", "圆", "拾", "佰", "仟", "万", "拾", "佰", "仟", "亿", "拾", "佰", "仟", "兆", "拾", "佰", "仟" };
            string moneyStr = money.ToString(CultureInfo.InvariantCulture);
            string bigMoney = "";
            int di, dj = moneyStr.Length;
            for (di = 0; di < dj; di++)
            {
                int tm = Convert.ToInt16(moneyStr.Substring(dj - di - 1, 1));
                bigMoney += wstr[di] + cstr[tm];
            }
            string bm = string.Empty;
            foreach (char a in bigMoney.ToCharArray())
            {
                bm = a + bm;
            }
            for (di = 0; di < dj; di++)
            {
                bm = bm.Replace("零仟零佰零拾零圆", "圆");
                bm = bm.Replace("零仟零佰零拾零万", "万");
                bm = bm.Replace("零零", "零");
                bm = bm.Replace("零圆", "圆");
                bm = bm.Replace("零拾", "零");
                bm = bm.Replace("零佰", "零");
                bm = bm.Replace("零仟", "零");
                bm = bm.Replace("零万", "万");
                bm = bm.Replace("零亿", "亿");
                bm = bm.Replace("零角零分", "整");
                bm = bm.Replace("零分", "");
                bm = bm.Replace("零角", "");
            }
            return bm;
        }
    }
}
