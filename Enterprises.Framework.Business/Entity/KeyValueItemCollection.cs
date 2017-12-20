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
    /// 键值对
    /// </summary>
    [Serializable]
    public class KeyValueItemCollection : BinarySerializeCollection
    {
        /// <summary>
        /// 
        /// </summary>
        public KeyValueItemCollection()
        {
        }

        /// <summary>
        /// 为反序列化提供支持
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public KeyValueItemCollection(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="kvi"></param>
        public KeyValueItemCollection Add(KeyValueItem kvi)
        {
            List.Add(kvi);
            return this;
        }

        public KeyValueItemCollection Add(params KeyValueItem[] items)
        {
            if (items == null)
            {
                return this;
            }
            foreach (var item in items)
            {
                Add(item);
            }
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="kvi"></param>
        public KeyValueItemCollection Remove(KeyValueItem kvi)
        {
            if (List.Contains(kvi))
            {
                List.Remove(kvi);
            }
            return this;
        }

        public bool ContainsValue(string value)
        {
            foreach (KeyValueItem item in List)
            {
                if (item.Value == value)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="kvi"></param>
        /// <returns></returns>
        public bool Contains(KeyValueItem kvi)
        {
            return List.Contains(kvi);
        }

        public bool Contains(string key)
        {
            if (key == null)
            {
                key = string.Empty;
            }
            foreach (KeyValueItem item in List)
            {
                if (item.Key.ToUpper().Trim() == key.ToUpper().Trim())
                {
                    return true;
                }
            }
            return false;
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
                if (string.IsNullOrEmpty(key))
                {
                    return null;
                }

                foreach (KeyValueItem kvi in List)
                {
                    if (kvi.Key.ToLower() == key.Trim().ToLower())
                    {
                        return kvi;
                    }
                }
                return null;
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
                if (index < 0 || index > Count)
                {
                    throw new Exception("KeyValueItemCollection集合索引时超出了范围");
                }
                return (KeyValueItem)List[index];
            }
        }

        /// <summary>
        /// 转成泛型集合
        /// </summary>
        /// <returns></returns>
        public List<KeyValueItem> ToList()
        {
            var items = new List<KeyValueItem>();
            foreach (KeyValueItem item in List)
            {
                items.Add(item);
            }
            return items;
        }

        /// <summary>
        /// 把键转成泛型集合
        /// </summary>
        /// <returns></returns>
        public List<string> KeyToStringList()
        {
            var items = new List<string>();
            foreach (KeyValueItem item in List)
            {
                items.Add(item.Key);
            }
            return items;
        }


        /// <summary>
        /// 把键转成GUID类型的泛型集合
        /// </summary>
        /// <returns></returns>
        public List<Guid> KeyToGuidList()
        {
            var items = new List<Guid>();
            foreach (KeyValueItem item in List)
            {
                items.Add(new Guid(item.Key));
            }
            return items;
        }

        /// <summary>
        /// 把键转成Object类型的泛型集合
        /// </summary>
        /// <returns></returns>
        public List<object> KeyToObjectList()
        {
            var items = new List<object>();
            foreach (KeyValueItem item in List)
            {
                items.Add(item.Key);
            }
            return items;
        }

        /// <summary>
        /// 把值转成GUID类型的泛型集合
        /// </summary>
        /// <returns></returns>
        public List<Guid> ValueToGuidList()
        {
            var items = new List<Guid>();
            foreach (KeyValueItem item in List)
            {
                items.Add(new Guid(item.Value));
            }
            return items;
        }

        /// <summary>
        /// 把值转成Object类型的泛型集合
        /// </summary>
        /// <returns></returns>
        public List<object> ValueToObjectList()
        {
            var items = new List<object>();
            foreach (KeyValueItem item in List)
            {
                items.Add(item.Value);
            }
            return items;
        }

        /// <summary>
        /// 把值转成泛型集合
        /// </summary>
        /// <returns></returns>
        public List<string> ValueToStringList()
        {
            var items = new List<string>();
            foreach (KeyValueItem item in List)
            {
                items.Add(item.Value);
            }
            return items;
        }

        /// <summary>
        /// 获取按照KeyValueItem项中的Value排序的结果集
        /// </summary>
        /// <returns></returns>
        public KeyValueItemCollection OrderByValue()
        {
            var kvis = new List<KeyValueItem>();            
            foreach (KeyValueItem kvi in this.List)
            {
                kvis.Add(kvi);
            }
            var kvic = new KeyValueItemCollection();
            foreach (var kvi in kvis.OrderBy(item => item.Value))
            {
                kvic.Add(kvi);
            }            
            return kvic;
        }

        /// <summary>
        /// 获取按照KeyValueItem项中的Key排序的结果集
        /// </summary>
        /// <returns></returns>
        public KeyValueItemCollection OrderByKey()
        {
            var kvis = new List<KeyValueItem>();
            foreach (KeyValueItem kvi in this.List)
            {
                kvis.Add(kvi);
            }
            var kvic = new KeyValueItemCollection();
            foreach (var kvi in kvis.OrderBy(item => item.Key))
            {
                kvic.Add(kvi);
            }
            return kvic;
        }

        /// <summary>
        /// 排除重复主键值的记录
        /// </summary>
        /// <returns></returns>
        public KeyValueItemCollection Distinct()
        {
            var result = new KeyValueItemCollection();
            foreach (KeyValueItem kvi in List)
            {
                bool exist = false;
                foreach (KeyValueItem kviResult in result)
                {
                    if (kvi.Key == kviResult.Key)
                    {
                        exist = true;
                        break;
                    }
                }
                if (exist)
                {
                    continue;
                }
                result.Add(kvi);
            }
            return result;
        }

       

        /// <summary>
        /// 打包现有对象，方便通过WCF传递
        /// </summary>
        /// <returns></returns>
        public KeyValueItemCollectionWrap Wrap()
        {
            var result = new KeyValueItemCollectionWrap();
            result.ListItems = new List<KeyValueItem>();
            foreach (KeyValueItem item in List)
            {
                result.ListItems.Add(item);
            }
            return result;
        }
    }

    
}