using System.Collections.Generic;

namespace Enterprises.Framework.Plugin.Office.Converters.Model
{
    /// <summary>
    /// 书签数据源
    /// </summary>
    public class DataSource:IDictionary<string,DataItem>
    {
        private readonly IDictionary<string, DataItem> _dictionary;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="data"></param>
        public DataSource(IDictionary<string, DataItem> data)
        {
            _dictionary = data;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public DataSource()
        {
            _dictionary = new Dictionary<string, DataItem>();
        }

        /// <summary>
        /// 判断是否存在书签
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool IsSupported(string name)
        {
            return _dictionary.ContainsKey(name);
        }


        /// <summary>
        /// 增加项
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(string key, DataItem value)
        {
            _dictionary[key] = value;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key)
        {
            if (_dictionary.ContainsKey(key))
            {
                _dictionary.Remove(key);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public DataItem this[string key]
        {
            get
            {
                DataItem value;
                if(_dictionary.TryGetValue(key,out value))
                {
                    return value;
                }
                return null;
            }
            set
            {
                Add(key, value);
            }
        }


        public bool ContainsKey(string key)
        {
            return _dictionary.ContainsKey(key);
        }

        public ICollection<string> Keys
        {
            get { return _dictionary.Keys; }
        }

        bool IDictionary<string, DataItem>.Remove(string key)
        {
            return _dictionary.Remove(key);
        }

        public bool TryGetValue(string key, out DataItem value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        public ICollection<DataItem> Values
        {
            get { return _dictionary.Values; }
        }

        public void Add(KeyValuePair<string, DataItem> item)
        {
            _dictionary.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            _dictionary.Clear();
        }

        public bool Contains(KeyValuePair<string, DataItem> item)
        {
            return _dictionary.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, DataItem>[] array, int arrayIndex)
        {

        }

        public int Count
        {
            get { return _dictionary.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<string, DataItem> item)
        {
            return _dictionary.Remove(item.Key);
        }

        public IEnumerator<KeyValuePair<string, DataItem>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }
    }
}
