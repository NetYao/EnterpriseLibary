using System;
using System.Runtime.Serialization;

namespace Enterprises.Framework.Plugin.Office.Converters
{
    /// <summary>
    /// 文档转换实体
    /// </summary>
    [DataContract]
    public class DocumentEntity
    {
        /// <summary>
        /// 文档唯一标识
        /// </summary>
        [DataMember]
        public Guid ID { get; set; }
        /// <summary>
        /// 文档模板名称
        /// </summary>
        [DataMember]
        public string TemplateName { get; set; }
        /// <summary>
        /// 输出类型
        /// </summary>
        [DataMember]
        public int OutputType { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        [DataMember]
        public int Status { get; set; }
        /// <summary>
        /// 文档路径
        /// </summary>
        [DataMember]
        public string DocumentAddress { get; set; }
        /// <summary>
        /// 转换异常信息
        /// </summary>
        [DataMember]
        public string InfoMessage { get; set; }
        /// <summary>
        /// 转换时间
        /// </summary>
        [DataMember]
        public DateTime CreateTime { get; set; }
       
        /// <summary>
        /// word书签数据源json字符串
        /// </summary>
        [DataMember]
        public string DataSource { get; set; }
    }
}
