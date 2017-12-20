using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Net;

namespace Enterprises.Framework.Domain
{
    /// <summary>
    /// 域中组织单元的属性
    /// </summary>
    [Serializable]
    public class DomainOrganizationUnitPropertitiesDescription
    {
        /// <summary>
        /// 地址
        /// </summary>
        [Serializable]
        public class Address
        {
            /// <summary>
            /// 国家
            /// </summary>
            public class Country
            {
                /// <summary>
                /// 国家缩写
                /// </summary>
                public static string CountryAb = "国家缩写";

                /// <summary>
                /// 国家代码
                /// </summary>
                public static string CountryCode = "国家代码";

                /// <summary>
                /// 国家名称
                /// </summary>
                public static string CountryName = "国家名称";
            }

            /// <summary>
            /// 省/自治区
            /// </summary>
            public static string Province = "省或自治区";

            /// <summary>
            /// 市/县
            /// </summary>
            public static string City = "市或县";

            /// <summary>
            /// 街道
            /// </summary>
            public static string Street = "街道";

            /// <summary>
            /// 邮政编码
            /// </summary>
            public static string PostCode = "邮政编码";

        }

        /// <summary>
        /// 对象级信息
        /// </summary>
        [Serializable]
        public class Object
        {
            /// <summary>
            /// objectCategory
            /// </summary>
            public static string ObjectCategory = "objectCategory";

            /// <summary>
            /// objectClass
            /// </summary>
            public static string ObjectClass = "objectClass";

            /// <summary>
            /// objectGUID
            /// </summary>
            public static string ObjectGuid = "objectGUID";
        }

        /// <summary>
        /// 名称
        /// </summary>
        public static string Name = "组织单元名称";

        /// <summary>
        /// OU路径
        /// </summary>
        internal static string Path = "组织单元路径";

        /// <summary>
        /// 上级OU名称
        /// </summary>
        internal static string ParentName = "上级组织单元名称";

        /// <summary>
        /// 上级OU路径
        /// </summary>
        internal static string ParentPath = "上级组织单元路径";


        /// <summary>
        /// 创建日期
        /// </summary>
        public static string WhenCreated = "组织单元创建日期";

        /// <summary>
        /// 变动日期
        /// </summary>
        public static string WhenChanged = "组织单元变动日期";

        /// <summary>
        /// 组织经理
        /// </summary>
        public static string Manager = "组织单元的经理";

        /// <summary>
        /// 组织经理LDAP路径
        /// </summary>
        public static string ManagerPath = "组织单元的经理LDAP路径";

        /// <summary>
        /// 修改日期
        /// </summary>
        public static string ModifyTimeStamp = "组织单元修改日期";
    }
}
