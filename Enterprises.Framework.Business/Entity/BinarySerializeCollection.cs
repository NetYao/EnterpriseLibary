using System;
using System.Data;
using System.Collections;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Enterprises.Framework
{
    /// <summary>
    /// 支持二进制序列化的集合
    /// </summary>
    public class BinarySerializeCollection : CollectionBase, ISerializable, ICloneable
    {
        private SerializationFormat _fRemotingFormat = SerializationFormat.Binary;

        /// <summary>
        /// 构造函数
        /// </summary>
        public BinarySerializeCollection()
        {
        }

        /// <summary>
        /// 为反序列化提供支持
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public BinarySerializeCollection(SerializationInfo info, StreamingContext context)
        {
            _fRemotingFormat = (SerializationFormat)info.GetValue("RemotingFormat", typeof(SerializationFormat));

            var bytes = (byte[])info.GetValue("List", typeof(byte[]));
            using (var serializationStream = new MemoryStream(bytes))
            {
                if (serializationStream.CanSeek)
                {
                    serializationStream.Position = 0;
                }

                var formatter = new BinaryFormatter(null, new StreamingContext(context.State, false));

                var items = (object[])formatter.Deserialize(serializationStream);
                foreach (object item in items)
                {
                    List.Add(item);
                }
            }
        }       

        #region ISerializable 成员

        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("RemotingFormat", _fRemotingFormat);

            //把List转为Array对象然后来序列化

            var items = new object[Count];
            List.CopyTo(items, 0);

            SerializeObjectArray(info, context, items,"List");

        }

        protected void SerializeObjectArray(SerializationInfo info, StreamingContext context, object[] items,string infoValueName)
        {
            var formatter = new BinaryFormatter(null, new StreamingContext(context.State, false));
            using (var serializationStream = new MemoryStream())
            {
                formatter.Serialize(serializationStream, items);
                if (serializationStream.CanSeek)
                {
                    serializationStream.Position = 0;
                }
                info.AddValue(infoValueName, serializationStream.GetBuffer());
            }            
        }

        #endregion

        #region ICloneable Members

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual object Clone()
        {
            var newObject = new BinarySerializeCollection {_fRemotingFormat = _fRemotingFormat};
            foreach (object item in List)
            {
                newObject.List.Add(item);
            }
            return newObject;
        }

        #endregion
    }
}

