using System;
using System.Runtime.Serialization;

namespace Enterprises.Framework
{
    /// <summary>
    /// 键值对
    /// </summary>
    [DataContract]
    [Serializable]
    public class KeyValueItem
    {
        private string _fKey = string.Empty;
        private string _fValue = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public KeyValueItem()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public KeyValueItem(string key,string value)
        {
            _fKey = key;
            _fValue = value;
        }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Key
        {
            get
            {
                return _fKey;
            }
            set
            {
                _fKey = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Value
        {
            get
            {
                return _fValue;
            }
            set
            {
                _fValue = value;
            }
        }

        /// <summary>
        /// 返回Value[Key]结构的字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}[{1}]", Value, Key);
        }
    }
}