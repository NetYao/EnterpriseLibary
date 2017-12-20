namespace Enterprises.Framework.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    /// <summary>
    ///  字符串扩展
    /// </summary>
    public static class StringExtensions
    {
        private static readonly char[] ValidSegmentChars = "/?#[]@\"^{}|`<>\t\r\n\f ".ToCharArray();

        /// <summary>
        /// 字符串全是某个字符组成.
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="chars"></param>
        /// <returns></returns>
        public static bool All(this string subject, params char[] chars)
        {
            if (!string.IsNullOrEmpty(subject))
            {
                if ((chars == null) || (chars.Length == 0))
                {
                    return false;
                }
                Array.Sort<char>(chars);
                for (int i = 0; i < subject.Length; i++)
                {
                    char ch = subject[i];
                    if (Array.BinarySearch<char>(chars, ch) < 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 是否存在某些字符串
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="chars"></param>
        /// <returns></returns>
        public static bool Any(this string subject, params char[] chars)
        {
            if ((!string.IsNullOrEmpty(subject) && (chars != null)) && (chars.Length != 0))
            {
                Array.Sort<char>(chars);
                for (int i = 0; i < subject.Length; i++)
                {
                    char ch = subject[i];
                    if (Array.BinarySearch<char>(chars, ch) >= 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 驼峰处理
        /// </summary>
        /// <param name="camel"></param>
        /// <returns></returns>
        public static string CamelFriendly(this string camel)
        {
            if (string.IsNullOrWhiteSpace(camel))
            {
                return "";
            }

            StringBuilder builder = new StringBuilder(camel);
            for (int i = camel.Length - 1; i > 0; i--)
            {
                char ch = builder[i];
                if (('A' <= ch) && (ch <= 'Z'))
                {
                    builder.Insert(i, ' ');
                }
            }
            return builder.ToString();
        }

        /// <summary>
        /// 字符串过长追加省略号
        /// </summary>
        /// <param name="text"></param>
        /// <param name="characterCount"></param>
        /// <returns></returns>
        public static string Ellipsize(this string text, int characterCount)
        {
            return text.Ellipsize(characterCount, "&#160;&#8230;", false);
        }

        /// <summary>
        /// 字符串过长追加符合
        /// </summary>
        /// <param name="text">字符串</param>
        /// <param name="characterCount">截取长度</param>
        /// <param name="ellipsis">替换符合（如：省略号）</param>
        /// <param name="wordBoundary">单词边界</param>
        /// <returns></returns>
        public static string Ellipsize(this string text, int characterCount, string ellipsis, bool wordBoundary = false)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return "";
            }
            if ((characterCount < 0) || (text.Length <= characterCount))
            {
                return text;
            }
            int num = characterCount;
            while ((characterCount > 0) && text[characterCount - 1].IsLetter())
            {
                characterCount--;
            }
            while ((characterCount > 0) && text[characterCount - 1].IsSpace())
            {
                characterCount--;
            }
            if ((characterCount == 0) && !wordBoundary)
            {
                characterCount = num;
            }
            return (text.Substring(0, characterCount) + ellipsis);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string HtmlClassify(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return "";
            }
            string str = text.CamelFriendly();
            char[] chArray = new char[str.Length];
            int length = 0;
            bool flag = false;
            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];
                if (c.IsLetter() || (char.IsDigit(c) && (length > 0)))
                {
                    if ((flag && (i != 0)) && (length > 0))
                    {
                        chArray[length++] = '-';
                    }
                    chArray[length++] = char.ToLowerInvariant(c);
                    flag = false;
                }
                else
                {
                    flag = true;
                }
            }
            return new string(chArray, 0, length);
        }

        /// <summary>
        /// 是否是字母
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool IsLetter(this char c)
        {
            return ((('A' <= c) && (c <= 'Z')) || (('a' <= c) && (c <= 'z')));
        }

        /// <summary>
        /// 是否是空格
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool IsSpace(this char c)
        {
            if (((c != '\r') && (c != '\n')) && ((c != '\t') && (c != '\f')))
            {
                return (c == ' ');
            }
            return true;
        }

        /// <summary>
        /// 是否有效URL
        /// </summary>
        /// <param name="segment"></param>
        /// <returns></returns>
        public static bool IsValidUrlSegment(this string segment)
        {
            return !segment.Any(ValidSegmentChars);
        }

        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string RemoveDiacritics(this string name)
        {
            string str = name.Normalize(NormalizationForm.FormD);
            var builder = new StringBuilder();
            foreach (char ch in str)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
                {
                    builder.Append(ch);
                }
            }
            return builder.ToString().Normalize(NormalizationForm.FormC);
        }

        /// <summary>
        /// 去除HTML标签
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string RemoveTags(this string html)
        {
            if (string.IsNullOrEmpty(html))
            {
                return string.Empty;
            }
            char[] chArray = new char[html.Length];
            int length = 0;
            bool flag = false;
            for (int i = 0; i < html.Length; i++)
            {
                char ch = html[i];
                switch (ch)
                {
                    case '<':
                    {
                        flag = true;
                        continue;
                    }
                    case '>':
                    {
                        flag = false;
                        continue;
                    }
                }
                if (!flag)
                {
                    chArray[length++] = ch;
                }
            }
            return new string(chArray, 0, length);
        }

        /// <summary>
        /// 换行处理
        /// </summary>
        /// <param name="text"></param>
        /// <param name="replacement"></param>
        /// <returns></returns>
        public static string ReplaceNewLinesWith(this string text, string replacement)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                return text.Replace("\r\n", "\r\r").Replace("\n", string.Format(replacement, "\r\n")).Replace("\r\r", string.Format(replacement, "\r\n"));
            }
            return string.Empty;
        }

        /// <summary>
        /// 去除字符串相关字符
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static string Strip(this string subject, Func<char, bool> predicate)
        {
            char[] chArray = new char[subject.Length];
            int length = 0;
            for (int i = 0; i < subject.Length; i++)
            {
                char arg = subject[i];
                if (!predicate(arg))
                {
                    chArray[length++] = arg;
                }
            }
            return new string(chArray, 0, length);
        }

        /// <summary>
        /// 去除字符串相关字符
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="stripped"></param>
        /// <returns></returns>
        public static string Strip(this string subject, params char[] stripped)
        {
            if (((stripped == null) || (stripped.Length == 0)) || string.IsNullOrEmpty(subject))
            {
                return subject;
            }
            char[] chArray = new char[subject.Length];
            int length = 0;
            for (int i = 0; i < subject.Length; i++)
            {
                char ch = subject[i];
                if (Array.IndexOf<char>(stripped, ch) < 0)
                {
                    chArray[length++] = ch;
                }
            }
            return new string(chArray, 0, length);
        }

        /// <summary>
        /// ToByteArray
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static byte[] ToByteArray(this string hex)
        {
            return Enumerable.Range(0, hex.Length).Where<int>(delegate (int x) {
                return (0 == (x % 2));
            }).Select<int, byte>(delegate (int x) {
                return Convert.ToByte(hex.Substring(x, 2), 0x10);
            }).ToArray<byte>();
        }

        /// <summary>
        /// 16进制字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ToHexString(this byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", "");
        }

        /// <summary>
        /// 转为安全命名
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string ToSafeName(this string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return string.Empty;
            }
            name = name.RemoveDiacritics();
            name = name.Strip(delegate (char c) {
                return (((c != '_') && (c != '-')) && !c.IsLetter()) && !char.IsDigit(c);
            });
            name = name.Trim();
            while ((name.Length > 0) && !name[0].IsLetter())
            {
                name = name.Substring(1);
            }
            if (name.Length > 0x80)
            {
                name = name.Substring(0, 0x80);
            }
            return name;
        }

        /// <summary>
        /// 字符串替换
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static string Translate(this string subject, char[] from, char[] to)
        {
            if (string.IsNullOrEmpty(subject))
            {
                return subject;
            }
            if ((from == null) || (to == null))
            {
                throw new ArgumentNullException();
            }
            if (from.Length != to.Length)
            {
                throw new ArgumentNullException("from", "Parameters must have the same length");
            }
            Dictionary<char, char> dictionary = new Dictionary<char, char>(from.Length);
            for (int i = 0; i < from.Length; i++)
            {
                dictionary[from[i]] = to[i];
            }
            char[] chArray = new char[subject.Length];
            for (int j = 0; j < subject.Length; j++)
            {
                char key = subject[j];
                if (dictionary.ContainsKey(key))
                {
                    chArray[j] = dictionary[key];
                }
                else
                {
                    chArray[j] = key;
                }
            }
            return new string(chArray);
        }
    }
}

