using System;
using System.Data;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Enterprises.Framework
{
    /// <summary>
    /// 键值对包装（这样可以通过WCF传递）
    /// </summary>
    [Serializable]
    [DataContract]
    public class KeyValueItemCollectionWrap : IEnumerable
    {
        private List<KeyValueItem> fListItems = null;

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public List<KeyValueItem> ListItems 
        {
            get 
            {
                if (fListItems == null)
                {
                    fListItems = new List<KeyValueItem>();
                }
                return fListItems; 
            } 
            set 
            {
                fListItems = value; 
            } 
        }

        /// <summary>
        /// 计数
        /// </summary>
        public int Count
        {
            get
            {
                return ListItems.Count;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public KeyValueItemCollectionWrap()
        {
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="kvi"></param>
        public List<KeyValueItem> Add(KeyValueItem kvi)
        {
            ListItems.Add(kvi);
            return ListItems;
        }

        public List<KeyValueItem> Add(params KeyValueItem[] items)
        {
            foreach (var item in items)
            {
                ListItems.Add(item);
            }
            return ListItems;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="kvi"></param>
        public List<KeyValueItem> Remove(KeyValueItem kvi)
        {
            ListItems.Remove(kvi);
            return ListItems;
        }

        public bool ContainsValue(string value)
        {
            return ListItems.Find(item => item.Value == value) != null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="kvi"></param>
        /// <returns></returns>
        public bool Contains(KeyValueItem kvi)
        {
            return ListItems.Contains(kvi);
        }

        public bool Contains(string key)
        {
            return ListItems.Find(item => item.Key == key) != null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public KeyValueItem this[string key]
        {
            get
            {
                return ListItems.Find(item => item.Key == key);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public KeyValueItem this[int index]
        {
            get
            {
                return ListItems[index];
            }
        }

        /// <summary>
        /// 把键转成泛型集合
        /// </summary>
        /// <returns></returns>
        public List<string> KeyToStringList()
        {
            List<string> result = new List<string>();
            foreach (var item in ListItems)
            {
                result.Add(item.Key);
            }
            return result;
        }


        /// <summary>
        /// 把键转成GUID类型的泛型集合
        /// </summary>
        /// <returns></returns>
        public List<Guid> KeyToGuidList()
        {
            List<Guid> result = new List<Guid>();
            foreach (var item in ListItems)
            {
                result.Add(new Guid(item.Value));
            }
            return result;
        }

        /// <summary>
        /// 把键转成Object类型的泛型集合
        /// </summary>
        /// <returns></returns>
        public List<object> KeyToObjectList()
        {
            List<object> result = new List<object>();
            foreach (var item in ListItems)
            {
                result.Add(item.Key);
            }
            return result;
        }

        /// <summary>
        /// 把值转成GUID类型的泛型集合
        /// </summary>
        /// <returns></returns>
        public List<Guid> ValueToGuidList()
        {
            List<Guid> result = new List<Guid>();
            foreach (var item in ListItems)
            {
                result.Add(new Guid(item.Value));
            }
            return result;
        }

        /// <summary>
        /// 把值转成Object类型的泛型集合
        /// </summary>
        /// <returns></returns>
        public List<object> ValueToObjectList()
        {
            List<object> result = new List<object>();
            foreach (var item in ListItems)
            {
                result.Add(item.Value);
            }
            return result;
        }

        /// <summary>
        /// 把值转成泛型集合
        /// </summary>
        /// <returns></returns>
        public List<string> ValueToStringList()
        {
            List<string> result = new List<string>();
            foreach (var item in ListItems)
            {
                result.Add(item.Value);
            }
            return result;
        }

        /// <summary>
        /// 获取按照KeyValueItem项中的Value排序的结果集
        /// </summary>
        /// <returns></returns>
        public List<KeyValueItem> OrderByValue()
        {
            List<KeyValueItem> result = new List<KeyValueItem>();
            foreach (var item in ListItems.OrderBy(item => item.Value))
            {
                result.Add(item);
            }
            return result;
        }

        /// <summary>
        /// 获取按照KeyValueItem项中的Key排序的结果集
        /// </summary>
        /// <returns></returns>
        public List<KeyValueItem> OrderByKey()
        {
            List<KeyValueItem> result = new List<KeyValueItem>();
            foreach (var item in ListItems.OrderBy(item => item.Key))
            {
                result.Add(item);
            }
            return result;
        }

        /// <summary>
        /// 排除重复主键值的记录
        /// </summary>
        /// <returns></returns>
        public List<KeyValueItem> Distinct()
        {
            List<KeyValueItem> result = new List<KeyValueItem>();
            foreach (var item in ListItems)
            {
                if (result.Find(m => m.Key == item.Key) == null)
                {
                    result.Add(item);
                }
            }
            return result;
        }

       

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return ListItems.GetEnumerator();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public KeyValueItemCollection ToKeyValueItemCollection()
        {
            KeyValueItemCollection result = new KeyValueItemCollection();
            foreach (var item in ListItems)
            {
                result.Add(item);
            }
            return result;
        }
    }
}