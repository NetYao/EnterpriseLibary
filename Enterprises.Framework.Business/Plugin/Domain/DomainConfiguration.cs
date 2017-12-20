using System;
using System.Collections.Generic;
using System.Text;
using Enterprises.Framework.Utility;

namespace Enterprises.Framework.Domain
{
    /// <summary>
    /// 域配置
    /// </summary>
    public class DomainConfiguration
    {

        /// <summary>
        /// [获取]LDAP的DNS名称，如下格式：thinkpad.Enterprises.Framework.com
        /// </summary>
        public static string LDAPDNSName
        {
            get
            {
                return ConfigUtility.GetFrameworkConfigValue("LDAPDNSName", "LDAP的DNS名称，如下格式：thinkpad.Enterprises.Framework.com");
            }
        }

        /// <summary>
        /// [获取]通过配置文件配置的LDAP前缀（因为有时候会存在别名的情况）
        /// </summary>
        public static string LDAPConfigedDomainPrefixName
        {
            get
            {
                return ConfigUtility.GetFrameworkConfigValue("LDAPConfigedDomainPrefixName", "通过配置文件配置的LDAP前缀（因为有时候会存在别名的情况）", true);
            }
        }

        /// <summary>
        /// [获取]域名的前缀，方便组合访问帐号（从LDAPDNSName中解析出来，比如thinkpad.Enterprises.Framework.com解析出来的结果为：thinkpad）
        /// </summary>
        public static string LDAPDomainPrefixName
        {
            get
            {
                string configedValue = LDAPConfigedDomainPrefixName;
                if (configedValue == string.Empty)
                {
                    string ldapDNSName = LDAPDNSName;
                    if (ldapDNSName.IndexOf(".") != -1)
                    {
                        return ldapDNSName.Substring(0, ldapDNSName.IndexOf("."));
                    }
                    return ldapDNSName;
                }
                return configedValue;
            }
        }


        /// <summary>
        /// [获取]LDAP根路径，如下格式：LDAP://DC=Enterprises.Framework,DC=com。从LDAPDNSName中解析出来
        /// </summary>
        public static string LDAPRootPath
        {
            get
            {
                return DomainUtility.GetLdapPathByDnsName(LDAPDNSName);
            }
        }

        /// <summary>
        /// [获取]LDAP公司名称，即AD中一级组织单元的名称
        /// </summary>
        public static string LDAPCompanyName
        {
            get
            {
                return ConfigUtility.GetFrameworkConfigValue("LDAPCompanyName", "LDAP公司名称，即AD中一级组织单元的名称");
            }
        }

        /// <summary>
        /// [获取]LDAP公司路径，如下格式：LDAP://OU=CompanyName,DC=Enterprises.Framework,DC=com。从LDAPDNSName和LDAPCompanyName中解析出来
        /// </summary>
        public static string LDAPCompanyPath
        {
            get
            {
                return DomainUtility.GetLdapCompanyPathByDnsNameAndCompanyName(LDAPDNSName, LDAPCompanyName);
            }
        }

        

        /// <summary>
        /// [获取]LDAP管理帐号
        /// </summary>
        public static string LDAPAdminAccount
        {
            get
            {
                return ConfigUtility.GetFrameworkConfigValue("LDAPAdminAccount", "LDAP管理帐号");
            }
        }

        /// <summary>
        /// [获取]LDAP管理密码，如果加密的话是采用DES加密，编码格式为Unicode
        /// </summary>
        public static string LDAPAdminPassword
        {
            get
            {
                string password = ConfigUtility.GetFrameworkConfigValue("LDAPAdminPassword", "LDAP管理密码");
                if (LDAPAdminPasswordEncrpyted)
                {
                    password = EncryptUtility.DecryptByDES(password, Encoding.Unicode, EncryptUtility.EncryptTripleKeys, EncryptUtility.EncryptTripleIVs);
                }

                return password;
            }
        }

        /// <summary>
        /// [获取]LDAP管理密码是否已经加密
        /// </summary>
        public static bool LDAPAdminPasswordEncrpyted
        {
            get
            {
                string value = ConfigUtility.GetFrameworkConfigValue("LDAPAdminPasswordEncrpyted", "LDAP管理密码是否已经加密");
                return value.ToUpper().Trim() == "TRUE";
            }
        }

        /// <summary>
        /// 获取管理账户,当前用户没有权限时,可以采用该方法以使用更高权限的账户
        /// </summary>
        /// <returns></returns>
        public static DomainUser GetAdminUser()
        {
            return GetAdminUser(LDAPAdminAccount, LDAPAdminPassword, LDAPDNSName,LDAPCompanyName);
        }

        /// <summary>
        /// 获取管理账户,当前用户没有权限时,可以采用该方法以使用更高权限的账户
        /// </summary>
        /// <returns></returns>
        public static DomainUser GetAdminUser(string adminAccount, string adminPassword, string ldapDNSName,string ldapCompanyName)
        {
            var user = new DomainUser(adminAccount,adminPassword,ldapDNSName);
            user.LDAPCompanyName = ldapCompanyName;
            return user;
        }
        
        /// <summary>
        /// 获取基于当前AD配置用户模拟的账户对象
        /// </summary>
        /// <returns></returns>
        public static ImpersonateUserHelper GetDomainImpersonateUser()
        {
            return ImpersonateUserHelper.NewImpersonateUserHelper(DomainConfiguration.LDAPAdminAccount, DomainConfiguration.LDAPAdminPassword, DomainConfiguration.LDAPDomainPrefixName);
        }

    }
}

