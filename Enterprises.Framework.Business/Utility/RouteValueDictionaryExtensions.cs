namespace Enterprises.Framework.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Web.Routing;

    /// <summary>
    /// 路由映射
    /// </summary>
    public static class RouteValueDictionaryExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool Match(this RouteValueDictionary x, RouteValueDictionary y)
        {
            if (x == y)
            {
                return true;
            }
            if ((x == null) || (y == null))
            {
                return false;
            }
            if (x.Count != y.Count)
            {
                return false;
            }
            bool[] source = x.Join<KeyValuePair<string, object>, KeyValuePair<string, object>, string, bool>(y, delegate (KeyValuePair<string, object> kv1) {
                return kv1.Key.ToLowerInvariant();
            }, delegate (KeyValuePair<string, object> kv2) {
                return kv2.Key.ToLowerInvariant();
            }, delegate (KeyValuePair<string, object> kv1, KeyValuePair<string, object> kv2) {
                return StringMatch(kv1.Value, kv2.Value);
            }).ToArray<bool>();
            return (source.All<bool>(delegate (bool b) {
                return b;
            }) && (source.Count<bool>() == x.Count));
        }

        /// <summary>
        ///  合并路由
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static RouteValueDictionary Merge(this RouteValueDictionary dictionary, object values)
        {
            if (values != null)
            {
                return dictionary.Merge(new RouteValueDictionary(values));
            }
            return dictionary;
        }

        /// <summary>
        /// 合并路线
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="dictionaryToMerge"></param>
        /// <returns></returns>
        public static RouteValueDictionary Merge(this RouteValueDictionary dictionary, RouteValueDictionary dictionaryToMerge)
        {
            if (dictionaryToMerge == null)
            {
                return dictionary;
            }
            var dictionary2 = new RouteValueDictionary(dictionary);
            foreach (KeyValuePair<string, object> pair in dictionaryToMerge)
            {
                dictionary2[pair.Key] = pair.Value;
            }
            return dictionary2;
        }

        private static bool StringMatch(object value1, object value2)
        {
            return string.Equals(Convert.ToString(value1, CultureInfo.InvariantCulture), Convert.ToString(value2, CultureInfo.InvariantCulture), StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="routeValues"></param>
        /// <returns></returns>
        public static RouteValueDictionary ToRouteValueDictionary(this IEnumerable<KeyValuePair<string, string>> routeValues)
        {
            if (routeValues == null)
            {
                return null;
            }
            var dictionary = new RouteValueDictionary();
            foreach (KeyValuePair<string, string> pair in routeValues)
            {
                if (pair.Key.EndsWith("-"))
                {
                    dictionary.Add(pair.Key.Substring(0, pair.Key.Length - 1), pair.Value);
                }
                else
                {
                    dictionary.Add(pair.Key, pair.Value);
                }
            }
            return dictionary;
        }
    }
}

