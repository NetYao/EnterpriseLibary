using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.DirectoryServices;
using System.Net;

namespace Enterprises.Framework.Domain
{
    /// <summary>
    /// 域用户的属性
    /// </summary>
    [Serializable]
    public class DomainUserPropertities
    {
        /// <summary>
        /// 用户
        /// </summary>
        [Serializable]
        public class User
        {
            /// <summary>
            /// 姓
            /// </summary>
            public static readonly string LastName = "sn";

            /// <summary>
            /// 名
            /// </summary>
            public static readonly string GivenName = "givenName";

            /// <summary>
            /// 姓名
            /// </summary>
            public static readonly string UserName = "cn";

            /// <summary>
            /// 名称
            /// </summary>
            public static readonly string Name = "name";

            /// <summary>
            /// 英文缩写
            /// </summary>
            public static readonly string Initials = "initials";

            /// <summary>
            /// 显示名称
            /// </summary>
            public static readonly string DisplayName = "displayname";

            /// <summary>
            /// 描述
            /// </summary>
            public static readonly string Description = "description";

            /// <summary>
            /// 办公室
            /// </summary>
            public static readonly string PhysicalDeliveryOfficeName = "physicalDeliveryOfficeName";

            /// <summary>
            /// 电话号码
            /// </summary>
            public static readonly string TelephoneNumber = "telephoneNumber";

            /// <summary>
            /// 电子邮件
            /// </summary>
            public static readonly string Mail = "mail";

            /// <summary>
            /// 网页
            /// </summary>
            public static readonly string HomePage = "wWWHomePage";
        }

        /// <summary>
        /// 公司
        /// </summary>
        [Serializable]
        public class Company
        {
            /// <summary>
            /// 公司
            /// </summary>
            public static readonly string CompanyName = "company";

            /// <summary>
            /// 部门
            /// </summary>
            public static readonly string Department = "department";

            /// <summary>
            /// 职务
            /// </summary>
            public static readonly string Title = "title";

            /// <summary>
            /// 上级经理
            /// </summary>
            public static readonly string Manager = "manager";

            /// <summary>
            /// 上级经理的LDAP路径
            /// </summary>
            public static readonly string ManagerPath = "managerPath";

        }

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
                public static readonly string CountryAB = "c";

                /// <summary>
                /// 国家代码
                /// </summary>
                public static readonly string CountryCode = "countryCode";

                /// <summary>
                /// 国家名称
                /// </summary>
                public static readonly string CountryName = "co";
            }

            /// <summary>
            /// 省/自治区
            /// </summary>
            public static readonly string Province = "st";

            /// <summary>
            /// 市/县
            /// </summary>
            public static readonly string City = "I";

            /// <summary>
            /// 街道
            /// </summary>
            public static readonly string Street = "streetAddress";

            /// <summary>
            /// 邮政信箱
            /// </summary>
            public static readonly string PostOfficeBox = "postOfficeBox";

            /// <summary>
            /// 邮政编码
            /// </summary>
            public static readonly string PostCode = "postalCode";

        }

        /// <summary>
        /// 电话
        /// </summary>
        [Serializable]
        public class Telephone
        {
            /// <summary>
            /// 家庭电话
            /// </summary>
            public static readonly string HomePhone = "homePhone";

            /// <summary>
            /// 手机
            /// </summary>
            public static readonly string Mobile = "mobile";

            /// <summary>
            /// 传真
            /// </summary>
            public static readonly string Fax = "facsimileTelephoneNumber";

            /// <summary>
            /// 寻呼机
            /// </summary>
            public static string Pager = "pager";

            /// <summary>
            /// IP电话
            /// </summary>
            public static string IPPhone = "ipphone";
        }

        /// <summary>
        /// 账户
        /// </summary>
        [Serializable]
        public class Account
        {
            /// <summary>
            /// SAM登陆帐号，这个是为了兼容以前Win2000以前的版本而使用的
            /// </summary>
            public static readonly string SAMAccountName = "sAMAccountName";

            /// <summary>
            ///  SAM登陆帐号类型，这个是为了兼容以前Win2000以前的版本而使用的
            /// </summary>
            public static readonly string SAMAccountType = "sAMAccountType";

            /// <summary>
            /// 登陆账号全称,包括AD域名，比如zhangyong@thinkpad.Enterprises.Framework.com
            /// </summary>
            public static readonly string UserPrincipalName = "userPrincipalName";

            /// <summary>
            /// 账户选项
            /// </summary>
            public static readonly string UserAccountControl = "userAccountControl";

            /// <summary>
            /// 创建时间
            /// </summary>
            public static readonly string WhenCreated = "whenCreated";

            /// <summary>
            /// 上次修改密码时间
            /// </summary>
            public static readonly string PasswordLastSet = "pwdLastSet";

            /// <summary>
            /// 
            /// </summary>
            public static readonly string UserPassword = "userPassword";
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
            public static readonly string ObjectCategory = "objectCategory";

            /// <summary>
            /// objectClass
            /// </summary>
            public static readonly string ObjectClass = "objectClass";

            /// <summary>
            /// objectGUID
            /// </summary>
            public static readonly string ObjectGuid = "objectGUID";

            /// <summary>
            /// objectSid
            /// </summary>
            public static readonly string ObjectSid = "objectSid";

            /// <summary>
            /// primaryGroupID
            /// </summary>
            public static readonly string PrimaryGroupID = "primaryGroupID";
        }
    }
}

