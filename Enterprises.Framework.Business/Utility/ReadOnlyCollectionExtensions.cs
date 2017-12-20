namespace Enterprises.Framework.Utility
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    /// <summary>
    /// 只读集合
    /// </summary>
    public static class ReadOnlyCollectionExtensions
    {
        /// <summary>
        /// 转为只读集合
        /// </summary>
        /// <param name="enumerable"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IList<T> ToReadOnlyCollection<T>(this IEnumerable<T> enumerable)
        {
            return new ReadOnlyCollection<T>(enumerable.ToList<T>());
        }
    }
}

