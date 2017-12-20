using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.DirectoryServices;
using System.Net;
using System.Xml;
using System.Data;
using System.Runtime.Serialization;

namespace Enterprises.Framework.Domain
{
    /// <summary>
    /// 域用户信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class DomainUser : ICloneable
    {
        #region Sub Classes

        /// <summary>
        /// 用户(常规)
        /// </summary>
        [Serializable]
        [DataContract]
        public class UserInfo : ICloneable
        {
            /// <summary>
            /// 姓
            /// </summary>
            public string LastName = string.Empty;

            /// <summary>
            /// 名
            /// </summary>
            public string GivenName = string.Empty;


            /// <summary>
            /// 姓名
            /// </summary>
            public string UserName = string.Empty;

            /// <summary>
            /// 英文缩写
            /// </summary>
            public string Initials = string.Empty;

            /// <summary>
            /// 显示名称
            /// </summary>
            public string DisplayName = string.Empty;

            /// <summary>
            /// 描述
            /// </summary>
            public string Description = string.Empty;

            /// <summary>
            /// 办公室
            /// </summary>
            public string PhysicalDeliveryOfficeName = string.Empty;

            /// <summary>
            /// 办公室电话号码
            /// </summary>
            public string OfficeTelephoneNumber = string.Empty;

            /// <summary>
            /// 办公用电子邮件
            /// </summary>
            public string OfficeEMailAddress = string.Empty;

            /// <summary>
            /// 网页
            /// </summary>
            public string HomePage = string.Empty;

            /// <summary>
            /// 名称
            /// </summary>
            public string Name = string.Empty;

            #region ICloneable Members

            public object Clone()
            {
                return this.MemberwiseClone();
            }

            #endregion
        }

        /// <summary>
        /// 公司
        /// </summary>
        [Serializable]
        [DataContract]
        public class CompanyInfo : ICloneable
        {
            /// <summary>
            /// 公司
            /// </summary>
            public string CompanyName = string.Empty;

            /// <summary>
            /// 部门
            /// </summary>
            public string Department = string.Empty;

            /// <summary>
            /// 职务
            /// </summary>
            public string Title = string.Empty;

            /// <summary>
            /// 上级经理的登陆帐号（UserPrincipalName）
            /// </summary>
            public string Manager = string.Empty;

            /// <summary>
            /// 上级经理的LDAP路径
            /// </summary>
            public string ManagerPath = string.Empty;

            #region ICloneable Members

            public object Clone()
            {
                return this.MemberwiseClone();
            }

            #endregion

        }

        /// <summary>
        /// 地址
        /// </summary>
        [Serializable]
        [DataContract]
        public class AddressInfo : ICloneable
        {
            /// <summary>
            /// 国家
            /// </summary>
            [Serializable]
            public class CountryInfo : ICloneable
            {
                /// <summary>
                /// 国家缩写
                /// </summary>
                public string CountryAB = string.Empty;

                /// <summary>
                /// 国家代码
                /// </summary>
                public string CountryCode = string.Empty;

                /// <summary>
                /// 国家名称
                /// </summary>
                public string CountryName = string.Empty;

                public override string ToString()
                {
                    return string.Format("{0}", CountryName);
                }

                #region ICloneable Members

                public object Clone()
                {
                    return this.MemberwiseClone();
                }

                #endregion
            }

            /// <summary>
            /// 国家
            /// </summary>
            public CountryInfo Country = new CountryInfo();

            /// <summary>
            /// 省/自治区
            /// </summary>
            public string Province = string.Empty;

            /// <summary>
            /// 市/县
            /// </summary>
            public string City = string.Empty;

            /// <summary>
            /// 街道
            /// </summary>
            public string Street = string.Empty;

            /// <summary>
            /// 邮政信箱
            /// </summary>
            public string PostOfficeBox = string.Empty;

            /// <summary>
            /// 邮政编码
            /// </summary>
            public string PostCode = string.Empty;

            /// <summary>
            /// 地址全称（包括国家、省份、城市、街道）
            /// </summary>
            public string FullAddress
            {
                get
                {
                    string pc = string.Empty;
                    if (PostCode.Trim() != string.Empty)
                    {
                        pc = string.Format("邮编：{0}", PostCode);
                    }
                    return string.Format("{0}{1}{2}{3}，{4}", Country, Province, City, Street,pc);
                }
            }

            #region ICloneable Members

            public object Clone()
            {
                AddressInfo ai = this.MemberwiseClone() as AddressInfo;
                if (this.Country != null)
                {
                    ai.Country = this.Country.Clone() as CountryInfo;
                }
                return ai;
            }

            #endregion

        }

        /// <summary>
        /// 电话
        /// </summary>
        [Serializable]
        [DataContract]
        public class TelephoneInfo : ICloneable
        {
            /// <summary>
            /// 家庭电话
            /// </summary>
            public string HomePhone = string.Empty;

            /// <summary>
            /// 手机
            /// </summary>
            public string Mobile = string.Empty;

            /// <summary>
            /// 传真
            /// </summary>
            public string Fax = string.Empty;

            /// <summary>
            /// 寻呼机
            /// </summary>
            public string Pager = string.Empty;

            /// <summary>
            /// IP电话
            /// </summary>
            public string IPPhone = string.Empty;

            #region ICloneable Members

            public object Clone()
            {
                return this.MemberwiseClone();
            }

            #endregion
        }

        /// <summary>
        /// 账户
        /// </summary>
        [Serializable]
        [DataContract]
        public class AccountInfo : ICloneable
        {
            /// <summary>
            /// SAM登陆帐号，这个是为了兼容以前Win2000以前的版本而使用的
            /// </summary>
            public string SamAccountName = string.Empty;

            /// <summary>
            ///  SAM登陆帐号类型，这个是为了兼容以前Win2000以前的版本而使用的
            /// </summary>
            public string SAMAccountType = string.Empty;

            /// <summary>
            /// [获取]用户的域账号，格式为：Domain\UserCode
            /// </summary>
            public string DomainUserAccount
            {
                get
                {
                    return string.Format("{0}\\{1}", Domain, UserAccount);
                }
            }


            /// <summary>
            /// 域名（前缀即可），从UserPrincipalName中解析出来，比如administrator@thinkpad.Enterprises.Framework.com，则只取thinkpad
            /// </summary>
            public string Domain
            {
                get
                {
                    if (UserPrincipalName.IndexOf("\\") != -1)
                    {
                        // Domain\User 取Domain
                        string fullDomain = UserPrincipalName.Substring(0, UserPrincipalName.IndexOf("\\"));
                        if (fullDomain.IndexOf(".") != -1)
                        {
                            return fullDomain.Substring(0, fullDomain.IndexOf("."));
                        }
                        return fullDomain;
                    }
                    else if (UserPrincipalName.IndexOf("@") != -1)
                    {
                        //administrator@thinkpad.Enterprises.Framework.com，则只取thinkpad
                        string fullDomain = UserPrincipalName.Remove(0, UserPrincipalName.IndexOf("@") + 1);
                        if (fullDomain.IndexOf(".") != -1)
                        {
                            return fullDomain.Substring(0, fullDomain.IndexOf("."));
                        }
                        return fullDomain;
                    }
                    return string.Empty;
                }
            }

            private string fUserAccount = string.Empty;
            /// <summary>
            /// [获取/设置]登陆帐号，从UserPrincipalName中解析出来，比如administrator@thinkpad.Enterprises.Framework.com，则只取administrator
            /// </summary>
            public string UserAccount
            {
                get
                {
                    if (fUserAccount == string.Empty || fUserAccount == null)
                    {
                        if (UserPrincipalName.IndexOf("\\") != -1)
                        {
                            fUserAccount = UserPrincipalName.Remove(0, UserPrincipalName.IndexOf("\\") + 1);
                        }
                        else if (UserPrincipalName.IndexOf("@") != -1)
                        {
                            fUserAccount = UserPrincipalName.Substring(0, UserPrincipalName.IndexOf("@"));
                        }
                        else
                        {
                            fUserAccount = UserPrincipalName;
                        }
                    }
                    else
                    {
                        if (fUserAccount.IndexOf("\\") != -1)
                        {
                            fUserAccount = fUserAccount.Remove(0, fUserAccount.IndexOf("\\") + 1);
                        }
                        else if (fUserAccount.IndexOf("@") != -1)
                        {
                            fUserAccount = fUserAccount.Substring(0, fUserAccount.IndexOf("@"));
                        }
                    }
                    return fUserAccount;
                }
                set
                {
                    fUserAccount = value;
                    if (fUserAccount != null)
                    {
                        if (fUserAccount.IndexOf("\\") != -1)
                        {
                            fUserAccount = fUserAccount.Remove(0, fUserAccount.IndexOf("\\") + 1);
                        }
                        else if (fUserAccount.IndexOf("@") != -1)
                        {
                            fUserAccount = fUserAccount.Substring(0, fUserAccount.IndexOf("@"));
                        }
                    }
                }
            }

            /// <summary>
            /// 登陆账号全称,包括AD域名，比如administrator@thinkpad.Enterprises.Framework.com
            /// </summary>
            public string UserPrincipalName = string.Empty;

            /// <summary>
            /// 账户选项(是否激活 1激活，0禁用)
            /// </summary>
            public string UserAccountControl = string.Empty;

            /// <summary>
            /// 账户是否有效
            /// </summary>
            public bool IsEnabled
            {
                get
                {
                    if (string.IsNullOrEmpty(UserAccountControl)) return true;
                    return DomainUtility.IsAccountActive(Convert.ToInt32(UserAccountControl));
                }
            }

            /// <summary>
            /// 账户密码
            /// </summary>
            public string Password = string.Empty;

            /// <summary>
            /// 创建时间
            /// </summary>
            public DateTime WhenCreated;

            /// <summary>
            /// 上次修改密码时间
            /// </summary>
            public DateTime PasswordLastSet;

            #region ICloneable Members

            public object Clone()
            {
                return this.MemberwiseClone();
            }

            #endregion
        }

        /// <summary>
        /// 对象信息
        /// </summary>
        [Serializable]
        [DataContract]
        public class ObjectInfo : ICloneable
        {
            /// <summary>
            /// objectCategory
            /// </summary>
            public string ObjectCategory = string.Empty;

            /// <summary>
            /// objectClass
            /// </summary>
            public string ObjectClass = string.Empty;

            /// <summary>
            /// objectGUID
            /// </summary>
            public Guid ObjectGuid = Guid.Empty;

            /// <summary>
            /// objectSid
            /// </summary>
            public string ObjectSid = string.Empty;

            /// <summary>
            /// primaryGroupID
            /// </summary>
            public string PrimaryGroupID = string.Empty;

            #region ICloneable Members

            public object Clone()
            {
                return this.MemberwiseClone();
            }

            #endregion
        }

        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public DomainUser()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userAccount"></param>
        /// <param name="password"></param>
        /// <param name="ldapDnsName"></param>
        public DomainUser(string userAccount, string password, string ldapDnsName)
        {
            Account.UserPrincipalName = string.IsNullOrEmpty(ldapDnsName) ? string.Format("{0}", userAccount) : string.Format("{0}@{1}", userAccount, ldapDnsName);

            Account.Password = password;
            LDAPDNSName = ldapDnsName;
        }

        /// <summary>
        /// 用户
        /// </summary>
        public UserInfo User = new UserInfo();

        /// <summary>
        /// 公司
        /// </summary>
        public CompanyInfo Company = new CompanyInfo();

        /// <summary>
        /// 地址
        /// </summary>
        public AddressInfo Address = new AddressInfo();

        /// <summary>
        /// 电话
        /// </summary>
        public TelephoneInfo Telephone = new TelephoneInfo();

        /// <summary>
        /// 账户
        /// </summary>
        public AccountInfo Account = new AccountInfo();

        /// <summary>
        /// 对象
        /// </summary>
        public ObjectInfo Object = new ObjectInfo();

        /// <summary>
        /// 当前用户的LDAP路径
        /// </summary>
        public string LDAPCurrentUserPath = string.Empty;

        /// <summary>
        /// 公司名称（即根组织单元名称，比如：超级软件公司）
        /// </summary>
        public string LDAPCompanyName = string.Empty;

        /// <summary>
        /// 当前域的DNS名称（比如：demoDomain.com）
        /// </summary>
        public string LDAPDNSName = string.Empty;

        /// <summary>
        /// 获取当前用户所处的根组织单元的LDAP路径(即公司所在的OU路径）
        /// </summary>
        /// <returns></returns>
        public string LDAPCompanyPath
        {
            get
            {
                string comName = !string.IsNullOrEmpty(LDAPCompanyName) ? LDAPCompanyName.Trim() : DomainConfiguration.LDAPCompanyName;

                string dnsName = !string.IsNullOrEmpty(LDAPDNSName) ? LDAPDNSName.Trim() : DomainConfiguration.LDAPDNSName;

                string path = DomainUtility.GetLdapCompanyPathByDnsNameAndCompanyName(dnsName, comName);
                return path;
            }
        }

        /// <summary>
        /// 获取当前用户所属域的LDAP路径
        /// </summary>
        /// <returns></returns>
        public string LDAPRootPath
        {
            get
            {
                string dnsName;
                dnsName = !string.IsNullOrEmpty(LDAPDNSName) ? LDAPDNSName.Trim() : DomainConfiguration.LDAPDNSName;

                string path = DomainUtility.GetLdapPathByDnsName(dnsName);
                return path;
            }
        }

        /// <summary>
        /// [获取]域名的前缀，方便组合访问帐号（从LDAPDNSName中解析出来，比如thinkpad.Enterprises.Framework.com解析出来的结果为：thinkpad）
        /// </summary>
        public string LdapDomainPrefixName
        {
            get
            {
                string ldapDnsName = LDAPDNSName;
                if (string.IsNullOrEmpty(ldapDnsName))
                {
                    ldapDnsName = DomainConfiguration.LDAPDNSName;
                }
                if (ldapDnsName.IndexOf(".", StringComparison.Ordinal) != -1)
                {
                    return ldapDnsName.Substring(0, ldapDnsName.IndexOf(".", StringComparison.Ordinal));
                }
                return ldapDnsName;
            }
        }

        /// <summary>
        /// 修改当前域用户指定的属性信息(因为有些属性不需要修改的或者不能修改的)
        /// 选择属性名时，可以用类DomainUserPropertities中的对象来标示
        /// </summary>
        /// <param name="domainUserPropertityNames"></param>
        public void ModifyInfo(string[] domainUserPropertityNames)
        {
            ModifyInfo(domainUserPropertityNames, null);
        }

        /// <summary>
        /// 修改当前域用户指定的属性信息(因为有些属性不需要修改的或者不能修改的)
        /// 选择属性名时，可以用类DomainUserPropertities中的对象来标示
        /// </summary>
        /// <param name="domainUserPropertityNames"></param>
        /// <param name="adminUser">拥有权限的用户（要有用户名和密码）</param>
        public void ModifyInfo(string[] domainUserPropertityNames,DomainUser adminUser)
        {
            if (domainUserPropertityNames == null)
            {
                throw new ArgumentNullException("domainUserPropertityNames");
            }

            using (DirectoryEntry userEntry = DomainUtility.GetUserEntry(LDAPCurrentUserPath, this, adminUser))
            {
                foreach (string propertityName in domainUserPropertityNames)
                {
                    string formatPropertyName = propertityName.ToUpper().Trim();

                    #region Address

                    if (formatPropertyName == DomainUserPropertities.Address.City.ToUpper().Trim())
                    {
                        if (string.IsNullOrEmpty(Address.City))
                        {
                            continue;
                        }
                        userEntry.Properties[DomainUserPropertities.Address.City].Value = Address.City.Trim();
                        continue;
                    }
                    if (formatPropertyName == DomainUserPropertities.Address.Country.CountryAB.ToUpper().Trim())
                    {
                        if (string.IsNullOrEmpty(Address.Country.CountryAB))
                        {
                            continue;
                        }
                        userEntry.Properties[DomainUserPropertities.Address.Country.CountryAB].Value = Address.Country.CountryAB.Trim();
                        continue;
                    }
                    if (formatPropertyName == DomainUserPropertities.Address.Country.CountryCode.ToUpper().Trim())
                    {
                        if (string.IsNullOrEmpty(Address.Country.CountryCode))
                        {
                            continue;
                        }
                        userEntry.Properties[DomainUserPropertities.Address.Country.CountryCode].Value = Address.Country.CountryCode.Trim();
                        continue;
                    }
                    if (formatPropertyName == DomainUserPropertities.Address.Country.CountryName.ToUpper().Trim())
                    {
                        if (string.IsNullOrEmpty(Address.Country.CountryName))
                        {
                            continue;
                        }
                        userEntry.Properties[DomainUserPropertities.Address.Country.CountryName].Value = Address.Country.CountryName.Trim();
                        continue;
                    }
                    if (formatPropertyName == DomainUserPropertities.Address.PostCode.ToUpper().Trim())
                    {
                        if (string.IsNullOrEmpty(Address.PostCode))
                        {
                            continue;
                        }
                        userEntry.Properties[DomainUserPropertities.Address.PostCode].Value = Address.PostCode.Trim();
                        continue;
                    }
                    if (formatPropertyName == DomainUserPropertities.Address.PostOfficeBox.ToUpper().Trim())
                    {
                        if (string.IsNullOrEmpty(Address.PostOfficeBox))
                        {
                            continue;
                        }
                        userEntry.Properties[DomainUserPropertities.Address.PostOfficeBox].Value = Address.PostOfficeBox.Trim();
                        continue;
                    }
                    if (formatPropertyName == DomainUserPropertities.Address.Province.ToUpper().Trim())
                    {
                        if (string.IsNullOrEmpty(Address.Province))
                        {
                            continue;
                        }
                        userEntry.Properties[DomainUserPropertities.Address.Province].Value = Address.Province.Trim();
                        continue;
                    }
                    if (formatPropertyName == DomainUserPropertities.Address.Street.ToUpper().Trim())
                    {
                        if (string.IsNullOrEmpty(Address.Street))
                        {
                            continue;
                        }
                        userEntry.Properties[DomainUserPropertities.Address.Street].Value = Address.Street.Trim();
                        continue;
                    }

                    #endregion

                    #region Company

                    if (formatPropertyName == DomainUserPropertities.Company.CompanyName.ToUpper().Trim())
                    {
                        if (string.IsNullOrEmpty(Company.CompanyName))
                        {
                            continue;
                        }
                        userEntry.Properties[DomainUserPropertities.Company.CompanyName].Value = Company.CompanyName.Trim();
                        continue;
                    }
                    if (formatPropertyName == DomainUserPropertities.Company.Department.ToUpper().Trim())
                    {
                        if (string.IsNullOrEmpty(Company.Department))
                        {
                            continue;
                        }
                        userEntry.Properties[DomainUserPropertities.Company.Department].Value = Company.Department.Trim();
                        continue;
                    }
                    if (formatPropertyName == DomainUserPropertities.Company.Manager.ToUpper().Trim())
                    {
                        if (string.IsNullOrEmpty(Company.Manager))
                        {
                            continue;
                        }
                        userEntry.Properties[DomainUserPropertities.Company.Manager].Value = Company.Manager.Trim();
                        continue;
                    }
                    if (formatPropertyName == DomainUserPropertities.Company.Title.ToUpper().Trim())
                    {
                        if (string.IsNullOrEmpty(Company.Title))
                        {
                            continue;
                        }
                        userEntry.Properties[DomainUserPropertities.Company.Title].Value = Company.Title.Trim();
                        continue;
                    }

                    #endregion

                    #region Telephone

                    if (formatPropertyName == DomainUserPropertities.Telephone.Fax.ToUpper().Trim())
                    {
                        if (string.IsNullOrEmpty(Telephone.Fax))
                        {
                            continue;
                        }
                        userEntry.Properties[DomainUserPropertities.Telephone.Fax].Value = Telephone.Fax.Trim();
                        continue;
                    }

                    if (formatPropertyName == DomainUserPropertities.Telephone.HomePhone.ToUpper().Trim())
                    {
                        if (string.IsNullOrEmpty(Telephone.HomePhone))
                        {
                            continue;
                        }
                        userEntry.Properties[DomainUserPropertities.Telephone.HomePhone].Value = Telephone.HomePhone.Trim();
                        continue;
                    }

                    if (formatPropertyName == DomainUserPropertities.Telephone.Mobile.ToUpper().Trim())
                    {
                        if (string.IsNullOrEmpty(Telephone.Mobile))
                        {
                            continue;
                        }
                        userEntry.Properties[DomainUserPropertities.Telephone.Mobile].Value = Telephone.Mobile.Trim();
                        continue;
                    }

                    if (formatPropertyName == DomainUserPropertities.Telephone.Pager.ToUpper().Trim())
                    {
                        if (string.IsNullOrEmpty(Telephone.Pager))
                        {
                            continue;
                        }
                        userEntry.Properties[DomainUserPropertities.Telephone.Pager].Value = Telephone.Pager.Trim();
                        continue;
                    }

                    if (formatPropertyName == DomainUserPropertities.Telephone.IPPhone.ToUpper().Trim())
                    {
                        if (string.IsNullOrEmpty(Telephone.IPPhone))
                        {
                            continue;
                        }
                        userEntry.Properties[DomainUserPropertities.Telephone.IPPhone].Value = Telephone.IPPhone.Trim();
                        continue;
                    }

                    #endregion

                    #region User

                    if (formatPropertyName == DomainUserPropertities.User.Description.ToUpper().Trim())
                    {
                        if (string.IsNullOrEmpty(User.Description))
                        {
                            continue;
                        }
                        userEntry.Properties[DomainUserPropertities.User.Description].Value = User.Description.Trim();
                        continue;
                    }

                    if (formatPropertyName == DomainUserPropertities.User.DisplayName.ToUpper().Trim())
                    {
                        if (string.IsNullOrEmpty(User.DisplayName))
                        {
                            continue;
                        }
                        userEntry.Properties[DomainUserPropertities.User.DisplayName].Value = User.DisplayName.Trim();
                        continue;
                    }

                    if (formatPropertyName == DomainUserPropertities.User.GivenName.ToUpper().Trim())
                    {
                        if (string.IsNullOrEmpty(User.GivenName))
                        {
                            continue;
                        }
                        userEntry.Properties[DomainUserPropertities.User.GivenName].Value = User.GivenName.Trim();
                        continue;
                    }

                    if (formatPropertyName == DomainUserPropertities.User.HomePage.ToUpper().Trim())
                    {
                        if (string.IsNullOrEmpty(User.HomePage))
                        {
                            continue;
                        }
                        userEntry.Properties[DomainUserPropertities.User.HomePage].Value = User.HomePage.Trim();
                        continue;
                    }

                    if (formatPropertyName == DomainUserPropertities.User.Initials.ToUpper().Trim())
                    {
                        if (string.IsNullOrEmpty(User.Initials))
                        {
                            continue;
                        }
                        userEntry.Properties[DomainUserPropertities.User.Initials].Value = User.Initials.Trim();
                        continue;
                    }

                    if (formatPropertyName == DomainUserPropertities.User.LastName.ToUpper().Trim())
                    {
                        if (string.IsNullOrEmpty(User.LastName))
                        {
                            continue;
                        }
                        userEntry.Properties[DomainUserPropertities.User.LastName].Value = User.LastName.Trim();
                        continue;
                    }

                    if (formatPropertyName == DomainUserPropertities.User.Mail.ToUpper().Trim())
                    {
                        if (string.IsNullOrEmpty(User.OfficeEMailAddress))
                        {
                            continue;
                        }
                        userEntry.Properties[DomainUserPropertities.User.Mail].Value = User.OfficeEMailAddress.Trim();
                        continue;
                    }

                    ////Name属性好像不能直接修改，它是RDN属性。
                    //if (formatPropertyName == DomainUserPropertities.User.Name.ToUpper().Trim())
                    //{
                    //    userEntry.Properties[DomainUserPropertities.User.Name].Value = User.Name;
                    //    continue;
                    //}

                    if (formatPropertyName == DomainUserPropertities.User.PhysicalDeliveryOfficeName.ToUpper().Trim())
                    {
                        if (string.IsNullOrEmpty(User.PhysicalDeliveryOfficeName))
                        {
                            continue;
                        }
                        userEntry.Properties[DomainUserPropertities.User.PhysicalDeliveryOfficeName].Value = User.PhysicalDeliveryOfficeName.Trim();
                        continue;
                    }

                    if (formatPropertyName == DomainUserPropertities.User.TelephoneNumber.ToUpper().Trim())
                    {
                        if (string.IsNullOrEmpty(User.OfficeTelephoneNumber))
                        {
                            continue;
                        }
                        userEntry.Properties[DomainUserPropertities.User.TelephoneNumber].Value = User.OfficeTelephoneNumber.Trim();
                        continue;
                    }


                    if (formatPropertyName == DomainUserPropertities.User.UserName.ToUpper().Trim())
                    {
                        if (string.IsNullOrEmpty(User.UserName))
                        {
                            continue;
                        }
                        userEntry.Properties[DomainUserPropertities.User.UserName].Value = User.UserName.Trim();
                        continue;
                    }

                    #endregion

                    throw new Exception(string.Format("不支持的属性更新：PropertityName = {0}.", propertityName));
                }

                userEntry.CommitChanges();
            }

        }

        /// <summary>
        /// 创建导出表的架构
        /// </summary>
        /// <returns></returns>
        public static DataTable ToDataTableSchema()
        {
            DataTable dt = new DataTable("USER");

            #region 导出表的字段信息

            
            dt.Columns.Add(new DataColumn(DomainUserPropertities.User.DisplayName.ToUpper()) { Caption = DomainUserPropertitiesDescription.User.DisplayName });
            dt.Columns.Add(new DataColumn(DomainUserPropertities.User.UserName.ToUpper()) { Caption = DomainUserPropertitiesDescription.User.UserName });
            dt.Columns.Add(new DataColumn(DomainUserPropertities.User.Name.ToUpper()) { Caption = DomainUserPropertitiesDescription.User.Name });            
            dt.Columns.Add(new DataColumn(DomainUserPropertities.User.GivenName.ToUpper()) { Caption = DomainUserPropertitiesDescription.User.GivenName });                        
            dt.Columns.Add(new DataColumn(DomainUserPropertities.User.LastName.ToUpper()) { Caption = DomainUserPropertitiesDescription.User.LastName });
            dt.Columns.Add(new DataColumn(DomainUserPropertities.User.Initials.ToUpper()) { Caption = DomainUserPropertitiesDescription.User.Initials });

            dt.Columns.Add(new DataColumn(DomainUserPropertities.User.Mail.ToUpper()) { Caption = DomainUserPropertitiesDescription.User.Mail });            
            dt.Columns.Add(new DataColumn(DomainUserPropertities.User.TelephoneNumber.ToUpper()) { Caption = DomainUserPropertitiesDescription.User.TelephoneNumber });
            
            dt.Columns.Add(new DataColumn(DomainUserPropertities.Telephone.Mobile.ToUpper()) { Caption = DomainUserPropertitiesDescription.Telephone.Mobile });
            dt.Columns.Add(new DataColumn(DomainUserPropertities.Telephone.Fax.ToUpper()) { Caption = DomainUserPropertitiesDescription.Telephone.Fax });
            dt.Columns.Add(new DataColumn(DomainUserPropertities.Company.CompanyName.ToUpper()) { Caption = DomainUserPropertitiesDescription.Company.CompanyName });
            dt.Columns.Add(new DataColumn(DomainUserPropertities.Company.Department.ToUpper()) { Caption = DomainUserPropertitiesDescription.Company.Department });
            dt.Columns.Add(new DataColumn(DomainUserPropertities.Company.Title.ToUpper()) { Caption = DomainUserPropertitiesDescription.Company.Title });
            dt.Columns.Add(new DataColumn(DomainUserPropertities.Company.Manager.ToUpper()) { Caption = DomainUserPropertitiesDescription.Company.Manager });
            
            dt.Columns.Add(new DataColumn(DomainUserPropertities.User.HomePage.ToUpper()) { Caption = DomainUserPropertitiesDescription.User.HomePage });
            dt.Columns.Add(new DataColumn(DomainUserPropertities.User.PhysicalDeliveryOfficeName.ToUpper()) { Caption = DomainUserPropertitiesDescription.User.PhysicalDeliveryOfficeName });
            dt.Columns.Add(new DataColumn(DomainUserPropertities.User.Description.ToUpper()) { Caption = DomainUserPropertitiesDescription.User.Description });

            dt.Columns.Add(new DataColumn(DomainUserPropertities.Account.UserAccountControl.ToUpper()) { Caption = DomainUserPropertitiesDescription.Account.UserAccountControl });
            dt.Columns.Add(new DataColumn(DomainUserPropertities.Account.UserPrincipalName.ToUpper()) { Caption = DomainUserPropertitiesDescription.Account.UserPrincipalName });            
            
            dt.Columns.Add(new DataColumn(DomainUserPropertities.Company.ManagerPath.ToUpper()) { Caption = DomainUserPropertitiesDescription.Company.ManagerPath });
            
            dt.Columns.Add(new DataColumn(DomainUserPropertities.Address.City.ToUpper()) { Caption = DomainUserPropertitiesDescription.Address.City });
            dt.Columns.Add(new DataColumn(DomainUserPropertities.Address.Country.CountryAB.ToUpper()) { Caption = DomainUserPropertitiesDescription.Address.Country.CountryAB });
            dt.Columns.Add(new DataColumn(DomainUserPropertities.Address.Country.CountryCode.ToUpper()) { Caption = DomainUserPropertitiesDescription.Address.Country.CountryCode });
            dt.Columns.Add(new DataColumn(DomainUserPropertities.Address.Country.CountryName.ToUpper()) { Caption = DomainUserPropertitiesDescription.Address.Country.CountryName });
            dt.Columns.Add(new DataColumn(DomainUserPropertities.Address.PostCode.ToUpper()) { Caption = DomainUserPropertitiesDescription.Address.PostCode });
            dt.Columns.Add(new DataColumn(DomainUserPropertities.Address.PostOfficeBox.ToUpper()) { Caption = DomainUserPropertitiesDescription.Address.PostOfficeBox });
            dt.Columns.Add(new DataColumn(DomainUserPropertities.Address.Province.ToUpper()) { Caption = DomainUserPropertitiesDescription.Address.Province });
            dt.Columns.Add(new DataColumn(DomainUserPropertities.Address.Street.ToUpper()) { Caption = DomainUserPropertitiesDescription.Address.Street });

           
            
            dt.Columns.Add(new DataColumn(DomainUserPropertities.Telephone.HomePhone.ToUpper()) { Caption = DomainUserPropertitiesDescription.Telephone.HomePhone });
            
            dt.Columns.Add(new DataColumn(DomainUserPropertities.Telephone.Pager.ToUpper()) { Caption = DomainUserPropertitiesDescription.Telephone.Pager });
            dt.Columns.Add(new DataColumn(DomainUserPropertities.Telephone.IPPhone.ToUpper()) { Caption = DomainUserPropertitiesDescription.Telephone.IPPhone });


            dt.Columns.Add(new DataColumn(DomainUserPropertities.Account.PasswordLastSet.ToUpper()) { Caption = DomainUserPropertitiesDescription.Account.PasswordLastSet });
            dt.Columns.Add(new DataColumn(DomainUserPropertities.Account.SAMAccountName.ToUpper()) { Caption = DomainUserPropertitiesDescription.Account.SAMAccountName });
            dt.Columns.Add(new DataColumn(DomainUserPropertities.Account.SAMAccountType.ToUpper()) { Caption = DomainUserPropertitiesDescription.Account.SAMAccountType });            
            dt.Columns.Add(new DataColumn(DomainUserPropertities.Account.WhenCreated.ToUpper()) { Caption = DomainUserPropertitiesDescription.Account.WhenCreated });

            dt.Columns.Add(new DataColumn(DomainUserPropertities.Object.ObjectCategory.ToUpper()) { Caption = DomainUserPropertitiesDescription.Object.ObjectCategory });
            dt.Columns.Add(new DataColumn(DomainUserPropertities.Object.ObjectClass.ToUpper()) { Caption = DomainUserPropertitiesDescription.Object.ObjectClass });
            dt.Columns.Add(new DataColumn(DomainUserPropertities.Object.ObjectGuid.ToUpper()) { Caption = DomainUserPropertitiesDescription.Object.ObjectGuid });
            dt.Columns.Add(new DataColumn(DomainUserPropertities.Object.ObjectSid.ToUpper()) { Caption = DomainUserPropertitiesDescription.Object.ObjectSid });
            dt.Columns.Add(new DataColumn(DomainUserPropertities.Object.PrimaryGroupID.ToUpper()) { Caption = DomainUserPropertitiesDescription.Object.PrimaryGroupID });

            #endregion

            return dt;
        }

         /// <summary>
        /// 创建导出表的架构
        /// </summary>
        /// <returns></returns>
        public DataTable ToDataTable()
        {
            DataTable dt = ToDataTableSchema();
            DataRow dr = dt.NewRow();
            FillDataRow(dr);
            dt.Rows.Add(dr);
            return dt;
        }

        /// <summary>
        /// 用AD用户信息填充数据行
        /// </summary>
        /// <param name="dr"></param>
        internal void FillDataRow(DataRow dr)
        {
            DomainUser user = this;
            dr[DomainUserPropertities.Account.PasswordLastSet] = user.Account.PasswordLastSet;
            dr[DomainUserPropertities.Account.SAMAccountName] = user.Account.SamAccountName;
            dr[DomainUserPropertities.Account.SAMAccountType] = user.Account.SAMAccountType;
            dr[DomainUserPropertities.Account.UserAccountControl] = user.Account.UserAccountControl;
            dr[DomainUserPropertities.Account.UserPrincipalName] = user.Account.UserPrincipalName;
            dr[DomainUserPropertities.Account.WhenCreated] = user.Account.WhenCreated;

            dr[DomainUserPropertities.Address.City] = user.Address.City;
            dr[DomainUserPropertities.Address.Country.CountryAB] = user.Address.Country.CountryAB;
            dr[DomainUserPropertities.Address.Country.CountryCode] = user.Address.Country.CountryCode;
            dr[DomainUserPropertities.Address.Country.CountryName] = user.Address.Country.CountryName;
            dr[DomainUserPropertities.Address.PostCode] = user.Address.PostCode;
            dr[DomainUserPropertities.Address.PostOfficeBox] = user.Address.PostOfficeBox;
            dr[DomainUserPropertities.Address.Province] = user.Address.Province;
            dr[DomainUserPropertities.Address.Street] = user.Address.Street;

            dr[DomainUserPropertities.Company.CompanyName] = user.Company.CompanyName;
            dr[DomainUserPropertities.Company.Department] = user.Company.Department;
            dr[DomainUserPropertities.Company.Manager] = user.Company.Manager;
            dr[DomainUserPropertities.Company.ManagerPath] = user.Company.ManagerPath;

            dr[DomainUserPropertities.Company.Title] = user.Company.Title;

            dr[DomainUserPropertities.Telephone.Fax] = user.Telephone.Fax;
            dr[DomainUserPropertities.Telephone.HomePhone] = user.Telephone.HomePhone;
            dr[DomainUserPropertities.Telephone.Mobile] = user.Telephone.Mobile;
            dr[DomainUserPropertities.Telephone.Pager] = user.Telephone.Pager;
            dr[DomainUserPropertities.Telephone.IPPhone] = user.Telephone.IPPhone;

            dr[DomainUserPropertities.User.Description] = user.User.Description;
            dr[DomainUserPropertities.User.DisplayName] = user.User.DisplayName;
            dr[DomainUserPropertities.User.GivenName] = user.User.GivenName;
            dr[DomainUserPropertities.User.HomePage] = user.User.HomePage;
            dr[DomainUserPropertities.User.Initials] = user.User.Initials;
            dr[DomainUserPropertities.User.LastName] = user.User.LastName;
            dr[DomainUserPropertities.User.Mail] = user.User.OfficeEMailAddress;
            dr[DomainUserPropertities.User.Name] = user.User.Name;
            dr[DomainUserPropertities.User.PhysicalDeliveryOfficeName] = user.User.PhysicalDeliveryOfficeName;
            dr[DomainUserPropertities.User.TelephoneNumber] = user.User.OfficeTelephoneNumber;
            dr[DomainUserPropertities.User.UserName] = user.User.UserName;

            dr[DomainUserPropertities.Object.ObjectCategory] = user.Object.ObjectCategory;
            dr[DomainUserPropertities.Object.ObjectClass] = user.Object.ObjectClass;
            dr[DomainUserPropertities.Object.ObjectGuid] = user.Object.ObjectGuid;
            dr[DomainUserPropertities.Object.ObjectSid] = user.Object.ObjectSid;
            dr[DomainUserPropertities.Object.PrimaryGroupID] = user.Object.PrimaryGroupID;
        }



        #region ICloneable Members

        public object Clone()
        {
            var du = this.MemberwiseClone() as DomainUser;
            if (this.Account != null)
            {
                du.Account = this.Account.Clone() as AccountInfo;
            }
            if (this.Address != null)
            {
                du.Address = this.Address.Clone() as AddressInfo;
            }
            if (this.Company != null)
            {
                du.Company = this.Company.Clone() as CompanyInfo;
            }
            if (this.Object != null)
            {
                du.Object = this.Object.Clone() as ObjectInfo;
            }
            if (this.Telephone != null)
            {
                du.Telephone = this.Telephone.Clone() as TelephoneInfo;
            }
            if (this.User != null)
            {
                du.User = this.User.Clone() as UserInfo;
            }

            return du;
        }

        #endregion
    }

    /// <summary>
    /// 针对不同公司的用户域账号信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class CompanyDomainUserInfo
    {
        /// <summary>
        /// 管理员账号
        /// </summary>
        [DataMember]
        public string AdminAccount { get; set; }

        /// <summary>
        /// 管理员密码
        /// </summary>
        [DataMember]
        public string AdminPassword { get; set; }

        /// <summary>
        /// 公司的域
        /// </summary>
        [DataMember]
        public string DomainLdapPath { get; set; }
    }
}
