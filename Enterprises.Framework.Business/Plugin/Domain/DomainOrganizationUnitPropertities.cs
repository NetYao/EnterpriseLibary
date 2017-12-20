using System;

namespace Enterprises.Framework.Domain
{
    /// <summary>
    /// 域中组织单元的属性
    /// </summary>
    [Serializable]
    public class DomainOrganizationUnitPropertities
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
                public static string CountryAb = "c";

                /// <summary>
                /// 国家代码
                /// </summary>
                public static string CountryCode = "countryCode";

                /// <summary>
                /// 国家名称
                /// </summary>
                public static string CountryName = "co";
            }

            /// <summary>
            /// 省/自治区
            /// </summary>
            public static string Province = "st";

            /// <summary>
            /// 市/县
            /// </summary>
            public static string City = "I";

            /// <summary>
            /// 街道
            /// </summary>
            public static string Street = "streetAddress";

            /// <summary>
            /// 邮政编码
            /// </summary>
            public static string PostCode = "postalCode";

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
        public static string Name = "name";

        /// <summary>
        /// OU路径
        /// </summary>
        internal static string Path = "Path";

        /// <summary>
        /// 上级OU名称
        /// </summary>
        internal static string ParentName = "ParentName";

        /// <summary>
        /// 上级OU路径
        /// </summary>
        internal static string ParentPath = "ParentPath";

        /// <summary>
        /// 创建日期
        /// </summary>
        public static string WhenCreated = "whenCreated";

        /// <summary>
        /// 变动日期
        /// </summary>
        public static string WhenChanged = "whenChanged";

        /// <summary>
        /// 组织经理
        /// </summary>
        public static string Manager = "managedBy";

        /// <summary>
        /// 组织经理LDAP路径
        /// </summary>
        public static string ManagerPath = "managerPath";

        /// <summary>
        /// 修改日期
        /// </summary>
        public static string ModifyTimeStamp = "modifyTimeStamp";
    }
}
