using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.DirectoryServices;
using System.Net;

namespace Enterprises.Framework.Domain
{
    /// <summary>
    /// 域用户的属性的中文名称
    /// </summary>
    [Serializable]
    public class DomainUserPropertitiesDescription
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
            public static readonly string LastName = "姓";

            /// <summary>
            /// 名
            /// </summary>
            public static readonly string GivenName = "名";

            /// <summary>
            /// 姓名
            /// </summary>
            public static readonly string UserName = "姓名";

            /// <summary>
            /// 名称
            /// </summary>
            public static readonly string Name = "名称";

            /// <summary>
            /// 英文缩写
            /// </summary>
            public static readonly string Initials = "英文缩写";

            /// <summary>
            /// 显示名称
            /// </summary>
            public static readonly string DisplayName = "显示名称";

            /// <summary>
            /// 描述
            /// </summary>
            public static readonly string Description = "描述";

            /// <summary>
            /// 办公室
            /// </summary>
            public static readonly string PhysicalDeliveryOfficeName = "办公室";

            /// <summary>
            /// 电话号码
            /// </summary>
            public static readonly string TelephoneNumber = "电话号码";

            /// <summary>
            /// 电子邮件
            /// </summary>
            public static readonly string Mail = "电子邮件";

            /// <summary>
            /// 网页
            /// </summary>
            public static readonly string HomePage = "网页地址";
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
            public static readonly string CompanyName = "公司";

            /// <summary>
            /// 部门
            /// </summary>
            public static readonly string Department = "部门";

            /// <summary>
            /// 职务
            /// </summary>
            public static readonly string Title = "职务";

            /// <summary>
            /// 上级经理
            /// </summary>
            public static readonly string Manager = "上级经理";

            /// <summary>
            /// 上级经理的LDAP路径
            /// </summary>
            public static readonly string ManagerPath = "上级经理的LDAP路径";

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
                public static readonly string CountryAB = "国家缩写";

                /// <summary>
                /// 国家代码
                /// </summary>
                public static readonly string CountryCode = "国家代码";

                /// <summary>
                /// 国家名称
                /// </summary>
                public static readonly string CountryName = "国家名称";
            }

            /// <summary>
            /// 省/自治区
            /// </summary>
            public static readonly string Province = "省或自治区";

            /// <summary>
            /// 市/县
            /// </summary>
            public static readonly string City = "市或县";

            /// <summary>
            /// 街道
            /// </summary>
            public static readonly string Street = "街道";

            /// <summary>
            /// 邮政信箱
            /// </summary>
            public static readonly string PostOfficeBox = "邮政信箱";

            /// <summary>
            /// 邮政编码
            /// </summary>
            public static readonly string PostCode = "邮政编码";

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
            public static readonly string HomePhone = "家庭电话";

            /// <summary>
            /// 手机
            /// </summary>
            public static readonly string Mobile = "手机";

            /// <summary>
            /// 传真
            /// </summary>
            public static readonly string Fax = "传真";

            /// <summary>
            /// 寻呼机
            /// </summary>
            public static string Pager = "寻呼机";

            /// <summary>
            /// IP电话
            /// </summary>
            public static string IPPhone = "IP电话";
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
            public static readonly string SAMAccountName = "SAM登陆帐号";

            /// <summary>
            ///  SAM登陆帐号类型，这个是为了兼容以前Win2000以前的版本而使用的
            /// </summary>
            public static readonly string SAMAccountType = "SAM登陆帐号类型";

            /// <summary>
            /// 登陆账号全称,包括AD域名，比如zhangyong@thinkpad.Enterprises.Framework.com
            /// </summary>
            public static readonly string UserPrincipalName = "登陆账号全称";

            /// <summary>
            /// 账户选项
            /// </summary>
            public static readonly string UserAccountControl = "账户选项";

            /// <summary>
            /// 创建时间
            /// </summary>
            public static readonly string WhenCreated = "创建时间";

            /// <summary>
            /// 上次修改密码时间
            /// </summary>
            public static readonly string PasswordLastSet = "上次修改密码时间";

            /// <summary>
            /// 用户密码
            /// </summary>
            public static readonly string UserPassword = "用户密码";
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

