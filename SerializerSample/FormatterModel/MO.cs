using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace SerializerSample
{
    /// <summary>
    /// 定义自己的序列化和反序列化
    /// 但是必须加标记
    /// </summary>
    [Serializable()]
    public class MO : ISerializable 
    {
        public string DocNo { get; set; }

        public string ProductName { get; set; }

        public MO() { }

        /// <summary>
        /// 反序列化需要,这个是必须的
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public MO(SerializationInfo info, StreamingContext context)
        {
            DocNo = info.GetString("DocNo");
        }

        /// <summary>
        /// 定义自己的序列化
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("DocNo",this.DocNo);
        }
    }
}
