using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Enterprises.Framework.Utility
{
    /// <summary>
    /// 生成Hash字符串
    /// </summary>
    public class Hash
    {
        private long _hash;

        /// <summary>
        /// 时间Hash
        /// </summary>
        /// <param name="dateTime"></param>
        public void AddDateTime(DateTime dateTime)
        {
            this._hash += dateTime.ToBinary();
        }

        /// <summary>
        /// 字符串Hash
        /// </summary>
        /// <param name="value"></param>
        public void AddString(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                this._hash += GetStringHashCode(value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void AddStringInvariant(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                this.AddString(value.ToLowerInvariant());
            }
        }

        /// <summary>
        /// 类型Hash
        /// </summary>
        /// <param name="type"></param>
        public void AddTypeReference(Type type)
        {
            this.AddString(type.AssemblyQualifiedName);
            this.AddString(type.FullName);
        }

        private static long GetStringHashCode(string s)
        {
            long num = 0x15051505L;
            foreach (char ch in s)
            {
                long hashCode = ch.GetHashCode();
                num = (num + (hashCode << 0x1b)) + hashCode;
            }
            return num;
        }

        /// <summary>
        /// 重写ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Value;
        }


        /// <summary>
        /// 值
        /// </summary>
        public string Value
        {
            get
            {
                return this._hash.ToString("x", CultureInfo.InvariantCulture);
            }
        }
    }
}
