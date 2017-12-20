using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Net;

namespace Enterprises.Framework.Domain
{

    /// <summary>
    /// AD下查询时可用的对象类型
    /// </summary>
    public enum DirectoryObjectType
    {
        /// <summary>
        /// 查询条件是所有的组织单元（OU）
        /// </summary>
        OrganizationalUnit,

        /// <summary>
        /// 查询条件是所有的组（GROUP）
        /// </summary>
        Group,

        /// <summary>
        /// 查询条件是所有的用户（USER）
        /// </summary>
        User
    }
}

