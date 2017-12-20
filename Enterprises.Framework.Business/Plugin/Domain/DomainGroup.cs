using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Enterprises.Framework.Plugin.Domain
{
    /// <summary>
    /// 用户组
    /// </summary>
    public class DomainGroup
    {
        /// <summary>
        /// OU Path
        /// </summary>
        public string DistinguishedName { get; set; }

        /// <summary>
        /// 组类型
        /// </summary>
        public string GroupCategory { get; set; }


        /// <summary>
        /// 组作用域
        /// </summary>
        public string GroupScope { get; set; }

        /// <summary>
        /// 组名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 对象标记
        /// </summary>
        public string ObjectClass { get; set; }

        /// <summary>
        /// 唯一名称
        /// </summary>
        public string SamAccountName { get; set; }

        /// <summary>
        /// 对象ID
        /// </summary>
        public string ObjectGuid { get; set; }

        /// <summary>
        /// 注释
        /// </summary>
        public string Info { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

    }
}
