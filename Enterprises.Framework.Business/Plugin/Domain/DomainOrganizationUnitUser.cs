using System;

namespace Enterprises.Framework.Domain
{
    /// <summary>
    /// 组织单元中域用户信息
    /// </summary>
    [Serializable]
    public class DomainOrganizationUnitUser
    {
        /// <summary>
        /// 组织单元
        /// </summary>
        public DomainOrganizationUnit OU { get; set; }

        /// <summary>
        /// 当前组织单元下的直接用户列表（不包括子级组织单元的用户）
        /// </summary>
        public DomainUserCollection Users { get; set; }
    }
}