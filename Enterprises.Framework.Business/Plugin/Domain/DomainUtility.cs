using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Net;
using System.Data;
using System.Web;
using System.Security.Principal;
using System.Security.Permissions;
using Enterprises.Framework.Utility;


namespace Enterprises.Framework.Domain
{
    /// <summary>
    /// 域认证相关的辅助函数
    /// </summary>
    public static class DomainUtility
    {
        /// <summary>
        /// LDAP路径前缀
        /// </summary>
        private const string LdapPrefix = "LDAP://";

        /// <summary>
        /// 通过网络验证账户合法性
        /// </summary>
        public const int LOGON32_LOGON_INTERACTIVE = 2; //通过网络验证账户合法性 

        /// <summary>
        /// 使用默认的Windows 2000/NT NTLM验证方式  
        /// </summary>
        public const int LOGON32_PROVIDER_DEFAULT = 0; //使用默认的Windows 2000/NT NTLM验证方式  

        /// <summary>
        /// 映射函数LogonUser.调用Win32API Import advapi32.dll
        /// </summary>
        /// <param name="lpszUsername"></param>
        /// <param name="lpszDomain"></param>
        /// <param name="lpszPassword"></param>
        /// <param name="dwLogonType"></param>
        /// <param name="dwLogonProvider"></param>
        /// <param name="phToken"></param>
        /// <returns></returns>
        [DllImport("advapi32.dll", CharSet = CharSet.Auto)]
        private static extern bool LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

        /// <summary>
        /// 验证Token
        /// </summary>
        /// <param name="hToken"></param>
        /// <param name="impersonationLevel"></param>
        /// <param name="hNewToken"></param>
        /// <returns></returns>
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private extern static int DuplicateToken(IntPtr hToken, int impersonationLevel, ref IntPtr hNewToken);


        /// <summary>
        /// 采用Windows 2000/NT NTLM验证方式验证本地域账号登陆的合法性，如果帐号密码不正确，返回false，正确则返回true
        /// </summary>
        /// <param name="domain">AD域名称</param>
        /// <param name="account">域账号</param>
        /// <param name="password">登录密码</param>
        /// <returns>成功与否</returns>
        public static bool ValidateLocalDomainAccount(string domain, string account, string password)
        {
            IntPtr tokenHandle = IntPtr.Zero;
            bool checkResult = LogonUser(account, domain, password, LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, ref tokenHandle);
            return checkResult;
        }

        /// <summary>
        /// 验证用户名和密码是否正确
        /// </summary>
        /// <param name="ldapPath"></param>
        /// <param name="userCode">用户账户，不需要带前缀</param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool IsUserValid(string ldapPath,string userCode, string password)
        {
            #region userCode如果有前缀，去掉

            if (userCode == null)
            {
                throw new ArgumentNullException("userCode");
            }

            if (userCode.IndexOf("\\", StringComparison.Ordinal) != -1)
            {
                userCode = userCode.Remove(0, userCode.IndexOf("\\", StringComparison.Ordinal) + 1).Trim();
            }

            #endregion

            using (var deUser = new DirectoryEntry(ldapPath, userCode, password, AuthenticationTypes.Secure))
            {
                try
                {
                    // The NativeObject call on the DirectoryEntry object entry is an attempt to bind to the object in the directory.
                    // Since this call forces authentication, you will get an error if the user does not exist.
                    // If the user is a valid user in the domain, the call will succeed.
                    Object native = deUser.NativeObject;
                    return true;
                }
                catch(Exception ex)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 账户是否激活
        /// </summary>
        /// <param name="userAccount"></param>
        /// <returns></returns>
        public static bool IsAccountActive(string userAccount)
        {
            return IsAccountActive(userAccount, null);
        }

        /// <summary>
        /// 账户是否激活
        /// </summary>
        /// <param name="userAccount"></param>
        /// <param name="adminUser"></param>
        /// <returns></returns>
        public static bool IsAccountActive(string userAccount,DomainUser adminUser)
        {
            DomainUser user = GetDomainUserByAccount(userAccount, adminUser);
            if (user == null)
            {
                throw new Exception(string.Format("没有在AD中找到用户：{0}", userAccount));
            }
            return IsAccountActive(int.Parse(user.Account.UserAccountControl));
        }

        /// <summary>
        /// 账户是否激活
        /// </summary>
        /// <param name="userAccountControl"></param>
        /// <returns></returns>
        public static bool IsAccountActive(int userAccountControl)
        {
            int userAccountControlDisabled = Convert.ToInt32(ADAccountOptions.UF_ACCOUNTDISABLE);
            int flagExists = userAccountControl & userAccountControlDisabled;

            //if a match is found, then the disabled flag exists within the control flags
            return flagExists <= 0;
        }

        /// <summary>
        /// 模拟用户，如果成功，返回模拟用户的上下文引用，否则返回null
        /// </summary>
        /// <param name="accountName"></param>
        /// <param name="domain"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static WindowsImpersonationContext ImpersonateValidUser(string accountName, string password, string domain)
        {
            IntPtr token = IntPtr.Zero;
            IntPtr tokenDuplicate = IntPtr.Zero;

            if (LogonUser(accountName, domain, password, LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, ref token))
            {
                if (DuplicateToken(token, 2, ref tokenDuplicate) != 0)
                {
                    WindowsIdentity tempWindowsIdentity = new WindowsIdentity(tokenDuplicate);
                    WindowsImpersonationContext impersonationContext = tempWindowsIdentity.Impersonate();
                    return impersonationContext;
                }
                return null;
            }
            return null;
        }

        /// <summary>
        /// 取消用户模拟
        /// </summary>
        /// <param name="impersonationContext">模拟用户上下文引用</param>
        public static void UndoImpersonation(WindowsImpersonationContext impersonationContext)
        {
            if (impersonationContext != null)
            {
                impersonationContext.Undo();
            }
        }

        /// <summary>
        /// 验证指定的域账号是否存在，存在返回true，否则false
        /// </summary>        
        /// <param name="ldapString">LDAP路径</param>
        /// <param name="account">需要验证的帐号</param>
        /// <returns></returns>
        public static bool ExistDomainAccount(string ldapString, string account)
        {
            return ExistDomainAccount(ldapString,account, null);
        }


        /// <summary>
        /// 验证指定的域账号是否存在，存在返回true，否则false
        /// </summary>        
        /// <param name="ldapString">LDAP路径</param>
        /// <param name="account">需要验证的帐号</param>
        /// <param name="adminUser">管理帐号</param>        
        /// <returns></returns>
        public static bool ExistDomainAccount(string ldapString, string account, DomainUser adminUser)
        {
            try
            {
                if (string.IsNullOrEmpty(account))
                {
                    throw new ArgumentNullException("account");
                }

                account = account.Trim();
                string noPreFixAccount = account.IndexOf("\\", StringComparison.Ordinal) != -1 ? account.Remove(0, account.IndexOf("\\",StringComparison.Ordinal) + 1) : account;

                using (var entry = new DirectoryEntry(ldapString))
                {
                    if (adminUser != null)
                    {
                        entry.Username = adminUser.Account.UserAccount;
                        entry.Password = adminUser.Account.Password;
                    }

                    using (var searcher = new DirectorySearcher(entry))
                    {
                        searcher.Filter = string.Format("(sAMAccountname={0})", noPreFixAccount);
                        searcher.SearchScope = SearchScope.Subtree;
                        try
                        {
                            SearchResult result = searcher.FindOne();

                            bool exist = result != null;
                            return exist;
                        }
                        catch (Exception ex)
                        {
                            var exNew = new Exception(string.Format("没有找到用户：{0},LDAP路径为:{1}.", account, ldapString), ex);
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var exNew = new Exception(string.Format("判断用户是否存在出现异常，用户：{0},LDAP路径为:{1}.", account, ldapString), ex);
                return false;
            }
        }

        /// <summary>
        /// 根据LDAP路径获取用户的UserPrincipalName
        /// </summary>
        /// <param name="ldapString"></param>
        /// <returns></returns>
        public static string GetUserPrincipalNameByLdap(string ldapString)
        {
            return GetUserPrincipalNameByLdap(ldapString, null);
        }

        /// <summary>
        /// 根据LDAP路径获取用户的UserPrincipalName
        /// </summary>
        /// <param name="ldapString"></param>
        /// <param name="adminUser"></param>
        /// <returns></returns>
        public static string GetUserPrincipalNameByLdap(string ldapString, DomainUser adminUser)
        {
            if (string.IsNullOrEmpty(ldapString))
            {
                throw new Exception("ldapString不能为空");
            }
            ldapString = ldapString.Trim();
            if (!ldapString.ToUpper().StartsWith(LdapPrefix))
            {
                ldapString = LdapPrefix + ldapString;
            }
            using (var entry = new DirectoryEntry(ldapString))
            {
                if (adminUser != null)
                {
                    entry.Username = adminUser.Account.UserAccount;
                    entry.Password = adminUser.Account.Password;
                }
                string userPrincipalName = entry.Properties[DomainUserPropertities.Account.UserPrincipalName][0].ToString();
                return userPrincipalName;
            }
        }

        /// <summary>
        /// 根据用户的LDAP路径查找用户对象
        /// </summary>
        /// <param name="userLdapString">这个是具体到需要查找的人员的LDAP路径</param>
        /// <returns></returns>
        public static DomainUser GetDomainUser(string userLdapString)
        {
            return GetDomainUser(userLdapString, null);
        }

        /// <summary>
        /// 根据用户的LDAP路径查找用户对象
        /// </summary>
        /// <param name="userLdapString">这个是具体到需要查找的人员的LDAP路径</param>        
        /// <param name="adminUser"></param>
        /// <returns></returns>
        public static DomainUser GetDomainUser(string userLdapString, DomainUser adminUser)
        {
            //这里因为传入的userLDAPString已经明确到人了，所以后面采用SearchScope.Base方式
            DomainUserCollection users = GetDomainUsers(userLdapString, string.Empty, SearchScope.Base, adminUser);
            if (users.Count == 1)
            {
                return users[0];
            }
            if (users.Count == 0)
            {
                throw new Exception(string.Format("没有找到用户：{0}", userLdapString));
            }
            throw new Exception(string.Format("用户的LDAP路径错误：{0}", userLdapString));
        }


        /// <summary>
        /// 根据用户名在配置的域中查找用户
        /// </summary>
        /// <param name="userAccount"></param>
        /// <returns></returns>
        public static DomainUser GetDomainUserByAccount(string userAccount)
        {
            return GetDomainUserByAccount(userAccount, null);
        }

        /// <summary>
        /// 根据用户名在配置的域中查找用户
        /// </summary>
        /// <param name="userAccount"></param>        
        /// <param name="adminUser"></param>
        /// <returns></returns>
        public static DomainUser GetDomainUserByAccount(string userAccount, DomainUser adminUser)
        {
            return GetDomainUserByAccount(userAccount, DomainConfiguration.LDAPRootPath, adminUser);
        }

        /// <summary>
        /// 根据用户名在配置的域中查找用户
        /// </summary>
        /// <param name="userAccount"></param>
        /// <param name="ldapRootPath"></param>
        /// <param name="adminUser"></param>
        /// <returns></returns>
        public static DomainUser GetDomainUserByAccount(string userAccount,string ldapRootPath, DomainUser adminUser)
        {
            if (string.IsNullOrEmpty(userAccount))
            {
                throw new ArgumentNullException("userAccount");
            }

            string account = userAccount.Trim();
            if (account.IndexOf("\\", StringComparison.Ordinal) != -1)
            {
                account = account.Remove(0, account.IndexOf("\\", StringComparison.Ordinal) + 1);
            }
            string filter = string.Format("{0}={1}", DomainUserPropertities.Account.SAMAccountName, account);

            DomainUserCollection users = GetDomainUsers(ldapRootPath, filter, SearchScope.Subtree, adminUser);
            if (users.Count == 1)
            {
                DomainUser result = users[0];
                if (string.IsNullOrEmpty(result.Account.UserPrincipalName))
                {
                    result.Account.UserPrincipalName = string.Format("{0}@{1}", result.Account.UserAccount, DomainConfiguration.LDAPDNSName);
                }
                return result;
            }
            throw new Exception(string.Format("没有找到用户：{0}", userAccount));
        }

        /// <summary>
        /// 获取指定LDAP下的用户列表，过滤条件为当前目录以及子目录.
        /// 该方法仅在当前采用域账号登陆的情况下有效
        /// </summary>
        /// <param name="ldapString">LDAP://OU=TrustUsers,DC=adtrust,DC=local格式的地址</param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static DomainUserCollection GetDomainUsers(string ldapString, string filter)
        {
            return GetDomainUsers(ldapString, filter, SearchScope.Subtree);
        }

        /// <summary>
        /// 从ldapString指定的OU开始，逐级向下查找每个单元下的用户，最后汇总返回所有用户。
        /// </summary>
        /// <param name="ldapString"></param>
        /// <param name="filter"></param>
        /// <param name="includeAllSubDept"></param>
        /// <param name="adminUser"></param>
        /// <returns></returns>
        public static DomainUserCollection GetDomainUsersByOrganizationUnits(string ldapString, string filter, bool includeAllSubDept, DomainUser adminUser)
        {
            //先取根目录下的
            DomainUserCollection users = GetDomainUsers(ldapString, filter, SearchScope.OneLevel, adminUser);

            //再追加子目录下的
            var allOus = GetSubOrganizationalUnits(ldapString, includeAllSubDept,adminUser);
            foreach (var ou in allOus)
            {
                DomainUserCollection subUsers = GetDomainUsers(ou.Path, filter, SearchScope.OneLevel,adminUser);
                foreach (DomainUser subUser in subUsers)
                {
                    if (!users.Contains(subUser.Account.UserPrincipalName))
                    {
                        users.Add(subUser);
                    }
                }
            }
            return users;
        }

        /// <summary>
        /// 获取指定LDAP下的用户列表，可以指定过滤条件。
        /// 该方法仅在当前采用域账号登陆的情况下有效
        /// </summary>
        /// <param name="ldapString">LDAP连接串，会搜索改路径下的所有用户</param>
        /// <param name="filter">过滤条件，以()包围，或者为string.Empty</param>
        /// <param name="searchScope"></param>
        /// <returns></returns>
        public static DomainUserCollection GetDomainUsers(string ldapString, string filter, SearchScope searchScope)
        {
            return GetDomainUsers(ldapString, filter, searchScope, null);
        }

        /// <summary>
        /// 获取指定LDAP下的用户列表
        /// </summary>
        /// <param name="ldapString">LDAP连接串，会搜索改路径下的所有用户</param>
        /// <param name="filter">过滤条件，以()包围，或者为string.Empty</param>
        /// <param name="adminUser">域管理员</param>
        /// <param name="searchScope"></param>
        /// <returns></returns>
        public static DomainUserCollection GetDomainUsers(string ldapString, string filter, SearchScope searchScope, DomainUser adminUser)
        {
            using (var entry = new DirectoryEntry(ldapString))
            {
                if (adminUser != null)
                {
                    entry.Username = adminUser.Account.UserAccount;
                    entry.Password = adminUser.Account.Password;
                }
                using (var searcher = new DirectorySearcher(entry))
                {
                    /*
                        使用轻量目录访问协议 (LDAP) 并使用 DirectorySearcher 对 Active Directory 层次结构进行搜索和执行查询。LDAP 是系统提供的唯一支持目录搜索的 Active Directory 服务接口 (ADSI) 提供程序。管理员可以创建、更改和删除在层次结构中找到的对象。有关更多信息，请参见“Visual Studio.NET 中的 Active Directory 对象简介”。
                        在创建 DirectorySearcher 的实例时，通过 SearchRoot 属性指定要检索的根目录和要检索的可选的属性列表。还可以设置属性以确定： 
                        (1) 是否要在本地计算机上缓存搜索结果。将 CacheResults 属性设置为 true，以在本地计算机上存储目录信息。只有在调用 DirectoryEntry.CommitChanges 方法时，才对此本地缓存进行更新，并提交到 Active Directory。 
                        (2) 在搜索上要花费的时间。使用 ServerTimeLimit 属性指定搜索可持续的时间长度。 
                        (3) 是否只检索属性名称。将 PropertyNamesOnly 属性设置为 true，就会只检索已经为其赋值的属性的名称。 
                        (4) 是否执行分页搜索。设置 PageSize 属性，以指定在分页搜索中返回的最大对象数。如果不想执行分页搜索，请将 PageSize 属性设置为其默认值零。 
                        (5) 要返回的项的最大数量。设置 SizeLimit 属性以指定要返回的项的最大数量。若将 SizeLimit 属性设置为其默认值零，则会将它设置为服务器确定的默认值 1000 项。 
                            注意   如果返回的项的最大数量和时间限制超过服务器上设置的限制，则服务器设置覆盖组件设置。
                     */
                    
                    searcher.SizeLimit = 5000;
                    searcher.SearchScope = searchScope;
                    string searchFilter;
                    if (filter == string.Empty)
                    {
                        searchFilter = string.Format("(objectClass=user)");
                    }
                    else
                    {
                        if (filter.StartsWith("(") && filter.EndsWith(")"))
                        {
                            searchFilter = string.Format("(&(objectClass=user){0})", filter);
                        }
                        else
                        {
                            searchFilter = string.Format("(&(objectClass=user)({0}))", filter);
                        }
                    }
                    searcher.Filter = searchFilter;                    

                    #region Properties To Load

                    searcher.PropertiesToLoad.Add(DomainUserPropertities.Account.PasswordLastSet);
                    searcher.PropertiesToLoad.Add(DomainUserPropertities.Account.SAMAccountName);
                    searcher.PropertiesToLoad.Add(DomainUserPropertities.Account.SAMAccountType);
                    searcher.PropertiesToLoad.Add(DomainUserPropertities.Account.UserAccountControl);
                    searcher.PropertiesToLoad.Add(DomainUserPropertities.Account.UserPrincipalName);
                    searcher.PropertiesToLoad.Add(DomainUserPropertities.Account.WhenCreated);


                    searcher.PropertiesToLoad.Add(DomainUserPropertities.Address.City);
                    searcher.PropertiesToLoad.Add(DomainUserPropertities.Address.Country.CountryAB);
                    searcher.PropertiesToLoad.Add(DomainUserPropertities.Address.Country.CountryCode);
                    searcher.PropertiesToLoad.Add(DomainUserPropertities.Address.Country.CountryName);
                    searcher.PropertiesToLoad.Add(DomainUserPropertities.Address.PostCode);
                    searcher.PropertiesToLoad.Add(DomainUserPropertities.Address.PostOfficeBox);
                    searcher.PropertiesToLoad.Add(DomainUserPropertities.Address.Province);
                    searcher.PropertiesToLoad.Add(DomainUserPropertities.Address.Street);

                    searcher.PropertiesToLoad.Add(DomainUserPropertities.Company.CompanyName);
                    searcher.PropertiesToLoad.Add(DomainUserPropertities.Company.Department);
                    searcher.PropertiesToLoad.Add(DomainUserPropertities.Company.Manager);
                    searcher.PropertiesToLoad.Add(DomainUserPropertities.Company.Title);

                    searcher.PropertiesToLoad.Add(DomainUserPropertities.Telephone.Fax);
                    searcher.PropertiesToLoad.Add(DomainUserPropertities.Telephone.HomePhone);
                    searcher.PropertiesToLoad.Add(DomainUserPropertities.Telephone.Mobile);
                    searcher.PropertiesToLoad.Add(DomainUserPropertities.Telephone.Pager);
                    searcher.PropertiesToLoad.Add(DomainUserPropertities.Telephone.IPPhone);

                    searcher.PropertiesToLoad.Add(DomainUserPropertities.User.DisplayName);
                    searcher.PropertiesToLoad.Add(DomainUserPropertities.User.Description);
                    searcher.PropertiesToLoad.Add(DomainUserPropertities.User.GivenName);
                    searcher.PropertiesToLoad.Add(DomainUserPropertities.User.HomePage);
                    searcher.PropertiesToLoad.Add(DomainUserPropertities.User.Initials);
                    searcher.PropertiesToLoad.Add(DomainUserPropertities.User.LastName);
                    searcher.PropertiesToLoad.Add(DomainUserPropertities.User.Mail);
                    searcher.PropertiesToLoad.Add(DomainUserPropertities.User.Name);
                    searcher.PropertiesToLoad.Add(DomainUserPropertities.User.PhysicalDeliveryOfficeName);
                    searcher.PropertiesToLoad.Add(DomainUserPropertities.User.TelephoneNumber);
                    searcher.PropertiesToLoad.Add(DomainUserPropertities.User.UserName);

                    searcher.PropertiesToLoad.Add(DomainUserPropertities.Object.ObjectCategory);
                    searcher.PropertiesToLoad.Add(DomainUserPropertities.Object.ObjectClass);
                    searcher.PropertiesToLoad.Add(DomainUserPropertities.Object.ObjectGuid);
                    searcher.PropertiesToLoad.Add(DomainUserPropertities.Object.ObjectSid);
                    searcher.PropertiesToLoad.Add(DomainUserPropertities.Object.PrimaryGroupID);


                    #endregion

                    var users = new DomainUserCollection();
                    SearchResultCollection src = null;
                    try
                    {
                        try
                        {
                            src = searcher.FindAll();
                        }
                        catch (Exception ex)
                        {
                            var exNew = new Exception(string.Format("GetDomainUsers Error : LdapString={0};Filter={1};SearchScope={2}", ldapString, filter, searchScope), ex);                            
                            throw exNew;
                        }
                        foreach (SearchResult sr in src)
                        {
                            var user = new DomainUser {LDAPCurrentUserPath = sr.Path};

                            #region Domain User properties

                            #region DomainUserPropertities.Account

                            if (sr.Properties.Contains(DomainUserPropertities.Account.PasswordLastSet))
                            {
                                long lastSetDate = long.Parse(sr.Properties[DomainUserPropertities.Account.PasswordLastSet][0].ToString());
                                if (lastSetDate != 0)
                                {
                                    user.Account.PasswordLastSet = DateTime.FromFileTime(lastSetDate);
                                }
                                else
                                {
                                    user.Account.PasswordLastSet = DateTime.MinValue;
                                }
                            }
                            if (sr.Properties.Contains(DomainUserPropertities.Account.SAMAccountName))
                            {
                                user.Account.SamAccountName = sr.Properties[DomainUserPropertities.Account.SAMAccountName][0].ToString();
                            }
                            if (sr.Properties.Contains(DomainUserPropertities.Account.SAMAccountType))
                            {
                                user.Account.SAMAccountType = sr.Properties[DomainUserPropertities.Account.SAMAccountType][0].ToString();
                            }
                            if (sr.Properties.Contains(DomainUserPropertities.Account.UserAccountControl))
                            {
                                user.Account.UserAccountControl = sr.Properties[DomainUserPropertities.Account.UserAccountControl][0].ToString();
                            }
                            if (sr.Properties.Contains(DomainUserPropertities.Account.UserPrincipalName))
                            {
                                user.Account.UserPrincipalName = sr.Properties[DomainUserPropertities.Account.UserPrincipalName][0].ToString();
                            }
                            else
                            {
                                user.Account.UserPrincipalName = user.Account.SamAccountName;
                            }
                            if (sr.Properties.Contains(DomainUserPropertities.Account.WhenCreated))
                            {
                                string createDate = sr.Properties[DomainUserPropertities.Account.WhenCreated][0].ToString();
                                user.Account.WhenCreated = DateTime.Parse(createDate);
                            }

                            #endregion

                            #region DomainUserPropertities.Address

                            if (sr.Properties.Contains(DomainUserPropertities.Address.City))
                            {
                                user.Address.City = sr.Properties[DomainUserPropertities.Address.City][0].ToString();
                            }

                            if (sr.Properties.Contains(DomainUserPropertities.Address.Country.CountryAB))
                            {
                                user.Address.Country.CountryAB = sr.Properties[DomainUserPropertities.Address.Country.CountryAB][0].ToString();
                            }

                            if (sr.Properties.Contains(DomainUserPropertities.Address.Country.CountryCode))
                            {
                                user.Address.Country.CountryCode = sr.Properties[DomainUserPropertities.Address.Country.CountryCode][0].ToString();
                            }

                            if (sr.Properties.Contains(DomainUserPropertities.Address.Country.CountryName))
                            {
                                user.Address.Country.CountryName = sr.Properties[DomainUserPropertities.Address.Country.CountryName][0].ToString();
                            }

                            if (sr.Properties.Contains(DomainUserPropertities.Address.PostCode))
                            {
                                user.Address.PostCode = sr.Properties[DomainUserPropertities.Address.PostCode][0].ToString();
                            }
                            if (sr.Properties.Contains(DomainUserPropertities.Address.PostOfficeBox))
                            {
                                user.Address.PostOfficeBox = sr.Properties[DomainUserPropertities.Address.PostOfficeBox][0].ToString();
                            }

                            if (sr.Properties.Contains(DomainUserPropertities.Address.Province))
                            {
                                user.Address.Province = sr.Properties[DomainUserPropertities.Address.Province][0].ToString();
                            }

                            if (sr.Properties.Contains(DomainUserPropertities.Address.Street))
                            {
                                user.Address.Street = sr.Properties[DomainUserPropertities.Address.Street][0].ToString();
                            }

                            #endregion

                            #region DomainUserPropertities.Company

                            if (sr.Properties.Contains(DomainUserPropertities.Company.CompanyName))
                            {
                                user.Company.CompanyName = sr.Properties[DomainUserPropertities.Company.CompanyName][0].ToString();
                            }

                            if (sr.Properties.Contains(DomainUserPropertities.Company.Department))
                            {
                                user.Company.Department = sr.Properties[DomainUserPropertities.Company.Department][0].ToString();
                            }

                            if (sr.Properties.Contains(DomainUserPropertities.Company.Manager))
                            {
                                string managerPath = sr.Properties[DomainUserPropertities.Company.Manager][0].ToString().Trim();
                                if (!managerPath.ToUpper().Trim().StartsWith(LdapPrefix))
                                {
                                    managerPath = LdapPrefix + managerPath.Trim();
                                }

                                user.Company.ManagerPath = managerPath;

                                //根据这个Path再去找到它的登陆信息
                                if (managerPath != string.Empty)
                                {
                                    user.Company.Manager = GetUserPrincipalNameByLdap(managerPath, adminUser);
                                }
                            }

                            if (sr.Properties.Contains(DomainUserPropertities.Company.Title))
                            {
                                user.Company.Title = sr.Properties[DomainUserPropertities.Company.Title][0].ToString();
                            }

                            #endregion

                            #region DomainUserPropertities.Telephone

                            if (sr.Properties.Contains(DomainUserPropertities.Telephone.Fax))
                            {
                                user.Telephone.Fax = sr.Properties[DomainUserPropertities.Telephone.Fax][0].ToString();
                            }

                            if (sr.Properties.Contains(DomainUserPropertities.Telephone.HomePhone))
                            {
                                user.Telephone.HomePhone = sr.Properties[DomainUserPropertities.Telephone.HomePhone][0].ToString();
                            }

                            if (sr.Properties.Contains(DomainUserPropertities.Telephone.Mobile))
                            {
                                user.Telephone.Mobile = sr.Properties[DomainUserPropertities.Telephone.Mobile][0].ToString();
                            }

                            if (sr.Properties.Contains(DomainUserPropertities.Telephone.Pager))
                            {
                                user.Telephone.Pager = sr.Properties[DomainUserPropertities.Telephone.Pager][0].ToString();
                            }

                            if (sr.Properties.Contains(DomainUserPropertities.Telephone.IPPhone))
                            {
                                user.Telephone.IPPhone = sr.Properties[DomainUserPropertities.Telephone.IPPhone][0].ToString();
                            }

                            #endregion

                            #region DomainUserPropertities.User

                            if (sr.Properties.Contains(DomainUserPropertities.User.Description))
                            {
                                user.User.Description = sr.Properties[DomainUserPropertities.User.Description][0].ToString();
                            }


                            if (sr.Properties.Contains(DomainUserPropertities.User.DisplayName))
                            {
                                user.User.DisplayName = sr.Properties[DomainUserPropertities.User.DisplayName][0].ToString();
                            }

                            if (sr.Properties.Contains(DomainUserPropertities.User.GivenName))
                            {
                                user.User.GivenName = sr.Properties[DomainUserPropertities.User.GivenName][0].ToString();
                            }

                            if (sr.Properties.Contains(DomainUserPropertities.User.HomePage))
                            {
                                user.User.HomePage = sr.Properties[DomainUserPropertities.User.HomePage][0].ToString();
                            }

                            if (sr.Properties.Contains(DomainUserPropertities.User.Initials))
                            {
                                user.User.Initials = sr.Properties[DomainUserPropertities.User.Initials][0].ToString();
                            }

                            if (sr.Properties.Contains(DomainUserPropertities.User.LastName))
                            {
                                user.User.LastName = sr.Properties[DomainUserPropertities.User.LastName][0].ToString();
                            }

                            if (sr.Properties.Contains(DomainUserPropertities.User.Mail))
                            {
                                user.User.OfficeEMailAddress = sr.Properties[DomainUserPropertities.User.Mail][0].ToString();
                            }

                            if (sr.Properties.Contains(DomainUserPropertities.User.Name))
                            {
                                user.User.Name = sr.Properties[DomainUserPropertities.User.Name][0].ToString();
                            }

                            if (sr.Properties.Contains(DomainUserPropertities.User.PhysicalDeliveryOfficeName))
                            {
                                user.User.PhysicalDeliveryOfficeName = sr.Properties[DomainUserPropertities.User.PhysicalDeliveryOfficeName][0].ToString();
                            }

                            if (sr.Properties.Contains(DomainUserPropertities.User.TelephoneNumber))
                            {
                                user.User.OfficeTelephoneNumber = sr.Properties[DomainUserPropertities.User.TelephoneNumber][0].ToString();
                            }

                            if (sr.Properties.Contains(DomainUserPropertities.User.UserName))
                            {
                                user.User.UserName = sr.Properties[DomainUserPropertities.User.UserName][0].ToString();
                            }

                            #endregion

                            #region DomainUserPropertities.Object

                            if (sr.Properties.Contains(DomainUserPropertities.Object.ObjectCategory))
                            {
                                user.Object.ObjectCategory = sr.Properties[DomainUserPropertities.Object.ObjectCategory][0].ToString();
                            }
                            if (sr.Properties.Contains(DomainUserPropertities.Object.ObjectClass))
                            {
                                user.Object.ObjectClass = sr.Properties[DomainUserPropertities.Object.ObjectClass][0].ToString();
                            }
                            if (sr.Properties.Contains(DomainUserPropertities.Object.ObjectGuid))
                            {
                                user.Object.ObjectGuid = new Guid((byte[])(sr.Properties[DomainUserPropertities.Object.ObjectGuid][0]));
                            }
                            if (sr.Properties.Contains(DomainUserPropertities.Object.ObjectSid))
                            {
                                user.Object.ObjectSid = EncodingUtility.ToBase64FromBytes((byte[])(sr.Properties[DomainUserPropertities.Object.ObjectSid][0]));
                            }

                            #endregion

                            #endregion

                            users.Add(user);
                        }
                    }
                    finally
                    {
                        if (src != null)
                        {
                            src.Dispose();
                        }
                    }
                    return users;
                }
            }
        }

        /// <summary>
        /// 插入新用户到指定的LDAP目录中，同时更新newUser对象的Path属性
        /// </summary>
        /// <param name="ldapPath"></param>
        /// <param name="newUser"></param>
        /// <param name="adminUser"></param>
        public static void InsertUser(string ldapPath, DomainUser newUser, DomainUser adminUser)
        {
            if (newUser.User.UserName == null || newUser.User.UserName.Trim() == string.Empty)
            {
                throw new Exception("newUser.User.UserName不允许为空");
            }

            if (newUser.Account.UserAccount == null || newUser.Account.UserAccount.Trim() == string.Empty)
            {
                throw new Exception("newUser.Account.UserAccount不允许为空");
            }

            if (string.IsNullOrEmpty(newUser.Account.Password))
            {
                throw new Exception("newUser.Account.Password不允许为空");
            }

            string userAccount = newUser.Account.UserAccount;

            if (userAccount.IndexOf("\\", StringComparison.Ordinal) != -1)
            {
                newUser.Account.UserAccount = userAccount.Trim();
                if (string.IsNullOrEmpty(newUser.Account.UserPrincipalName))
                {
                    newUser.Account.UserPrincipalName = string.Format("{0}@{1}", userAccount.Trim().Remove(0, userAccount.Trim().IndexOf("\\", StringComparison.Ordinal) + 1), DomainConfiguration.LDAPDNSName);
                }
            }
            else
            {
                newUser.Account.UserAccount = string.Format("{0}\\{1}", DomainConfiguration.LDAPDomainPrefixName, userAccount);
                if (string.IsNullOrEmpty(newUser.Account.UserPrincipalName))
                {
                    newUser.Account.UserPrincipalName = string.Format("{0}@{1}", userAccount.Trim(), DomainConfiguration.LDAPDNSName);
                }
            }

            try
            {              

                using (var entry = new DirectoryEntry(ldapPath, adminUser.Account.UserAccount, adminUser.Account.Password))
                {
                    string adName = string.Format("{0}={1}", DomainUserPropertities.User.UserName, newUser.User.UserName.Trim());

                    //创建用户
                    using (DirectoryEntry newUserEntry = entry.Children.Add(adName, DirectoryObjectType.User.ToString()))
                    {
                        newUser.LDAPCurrentUserPath = newUserEntry.Path;
                        SetAdUserEntry(newUser, newUserEntry);
                        newUserEntry.CommitChanges();
                    }
                }
                EnableUser(ldapPath, newUser, adminUser);
                ModifyPassword(ldapPath, newUser, newUser.Account.Password, adminUser);
            }
            catch (Exception ex)
            {
                var exNew = new Exception(string.Format("InsertUser失败，可能存在重复的账号，或者存在重复的用户名，userAccount = '{0}.UserPrincipalName = '{1}'.userName = '{2}'. ldapPath  = '{3}'.",
                    userAccount,newUser.Account.UserPrincipalName,newUser.User.UserName, ldapPath), ex);
                Console.Write(exNew);
                throw exNew;
            }
        }

        /// <summary>
        /// 判断用户是否禁用，如果禁用，返回true，否则返回false
        /// </summary>
        /// <param name="ldapPath"></param>
        /// <param name="newUser"></param>
        /// <param name="adminUser"></param>
        public static bool UserDisabled(string ldapPath, DomainUser newUser, DomainUser adminUser)
        {
            using (DirectoryEntry entry = GetUserEntry(ldapPath, newUser, adminUser))
            {
                //启用用户
                var userAccountControl = (int)entry.Properties[DomainUserPropertities.Account.UserAccountControl].Value;

                int userAccountControlDisabled = Convert.ToInt32(DomainUserAccountPropertities.ADS_UF_ACCOUNTDISABLE);
                int flagExists = userAccountControl & userAccountControlDisabled;

                return flagExists <= 0;
            }
        }

        /// <summary>
        /// 判断用户密码是否过期，如果过期，返回true，否则返回false
        /// </summary>
        /// <param name="ldapPath"></param>
        /// <param name="newUser"></param>
        /// <param name="adminUser"></param>
        public static bool UserPasswordExpired(string ldapPath, DomainUser newUser, DomainUser adminUser)
        {
            using (DirectoryEntry entry = GetUserEntry(ldapPath, newUser, adminUser))
            {
                //启用用户
                var userAccountControl = (int)entry.Properties[DomainUserPropertities.Account.UserAccountControl].Value;

                int userAccountControlPasswordExpired = Convert.ToInt32(DomainUserAccountPropertities.ADS_UF_PASSWORD_EXPIRED);
                int flagExists = userAccountControl & userAccountControlPasswordExpired;

                return flagExists <= 0;
            }
        }

        /// <summary>
        /// 启用用户
        /// </summary>
        /// <param name="ldapPath"></param>
        /// <param name="newUser"></param>
        /// <param name="adminUser"></param>
        public static void EnableUser(string ldapPath, DomainUser newUser, DomainUser adminUser)
        {
            if (newUser == null)
            {
                throw new ArgumentNullException("newUser");
            }

            using (DirectoryEntry entry = GetUserEntry(ldapPath, newUser, adminUser))
            {
                //启用用户
                entry.Properties[DomainUserPropertities.Account.UserAccountControl].Value = DomainUserAccountPropertities.ADS_UF_NORMAL_ACCOUNT | DomainUserAccountPropertities.ADS_UF_DONT_EXPIRE_PASSWD | DomainUserAccountPropertities.ADS_UF_PASSWD_NOTREQD;
                entry.CommitChanges();
            }
        }

        /// <summary>
        /// 启用用户
        /// </summary>
        /// <param name="userAccount"></param>
        public static void EnableUser(string userAccount)
        {
            string ldapPath = DomainConfiguration.LDAPRootPath;
            var newUser = new DomainUser(userAccount, userAccount, DomainConfiguration.LDAPDNSName);
            DomainUser adminUser = DomainConfiguration.GetAdminUser();
            EnableUser(ldapPath, newUser, adminUser);
        }

        /// <summary>
        /// 禁用用户
        /// </summary>
        /// <param name="ldapPath"></param>
        /// <param name="newUser"></param>
        /// <param name="adminUser"></param>
        public static void DisableUser(string ldapPath, DomainUser newUser, DomainUser adminUser)
        {
            using (DirectoryEntry entry = GetUserEntry(ldapPath, newUser, adminUser))
            {
                //禁用用户
                entry.Properties[DomainUserPropertities.Account.UserAccountControl].Value = DomainUserAccountPropertities.ADS_UF_NORMAL_ACCOUNT | DomainUserAccountPropertities.ADS_UF_DONT_EXPIRE_PASSWD | DomainUserAccountPropertities.ADS_UF_ACCOUNTDISABLE;
                entry.CommitChanges();
            }
        }

        /// <summary>
        /// 启用用户
        /// </summary>
        /// <param name="userAccount"></param>
        public static void DisableUser(string userAccount)
        {
            string ldapPath = DomainConfiguration.LDAPRootPath;
            var newUser = new DomainUser(userAccount, userAccount, DomainConfiguration.LDAPDNSName);
            DomainUser adminUser = DomainConfiguration.GetAdminUser();
            DisableUser(ldapPath, newUser, adminUser);
        }

        /// <summary>
        /// 标示用户密码永不过期
        /// </summary>
        /// <param name="ldapPath"></param>
        /// <param name="newUser"></param>
        /// <param name="adminUser"></param>
        public static void SetUserPasswordNotExpired(string ldapPath, DomainUser newUser, DomainUser adminUser)
        {
            using (DirectoryEntry entry = GetUserEntry(ldapPath, newUser, adminUser))
            {
                //标示用户密码永不过期
                entry.Properties[DomainUserPropertities.Account.UserAccountControl].Value = DomainUserAccountPropertities.ADS_UF_DONT_EXPIRE_PASSWD | DomainUserAccountPropertities.ADS_UF_DONT_EXPIRE_PASSWD;
                entry.CommitChanges();
            }
        }

     

        /// <summary>
        /// 删除AD中的域用户，如果是在Users容器，就用cn=Users，如果在组织单元中，就用ou=***
        /// </summary>
        /// <param name="ldapPath"></param>
        /// <param name="deleteUser"></param>
        /// <param name="adminUser"></param>
        [DirectoryServicesPermission(SecurityAction.Assert)]
        public static void DeleteUser(string ldapPath, DomainUser deleteUser, DomainUser adminUser)
        {
            using (DirectoryEntry userEntry = GetUserEntry(ldapPath, deleteUser, adminUser))
            {
                DirectoryEntry deParent = userEntry.Parent;
                deParent.Children.Remove(userEntry);
                deParent.CommitChanges();
                deParent.Dispose();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ldapPath"></param>
        /// <param name="modifyUser"></param>
        /// <param name="domainUserPropertityNames"></param>
        public static void ModifyUser(string ldapPath, DomainUser modifyUser,string[] domainUserPropertityNames)
        {
            ModifyUser(ldapPath, modifyUser,domainUserPropertityNames, DomainConfiguration.GetAdminUser());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ldapPath"></param>
        /// <param name="modifyUser"></param>
        /// <param name="domainUserPropertityNames"></param>
        /// <param name="adminUser"></param>
        public static void ModifyUser(string ldapPath, DomainUser modifyUser,string[] domainUserPropertityNames, DomainUser adminUser)
        {
            modifyUser.ModifyInfo(domainUserPropertityNames, adminUser);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ldapPath"></param>
        /// <param name="moveUser"></param>
        /// <param name="toUnit"></param>
        public static void ModifyUserOrganizationUnit(string ldapPath, DomainUser moveUser, DomainOrganizationUnit toUnit)
        {
            ModifyUserOrganizationUnit(ldapPath, moveUser, toUnit, null);
        }

        /// <summary>
        /// 修改用户所属的组织单元
        /// </summary>
        /// <param name="ldapPath"></param>
        /// <param name="moveUser"></param>
        /// <param name="toUnit"></param>
        /// <param name="adminUser"></param>
        public static void ModifyUserOrganizationUnit(string ldapPath, DomainUser moveUser, DomainOrganizationUnit toUnit, DomainUser adminUser)
        {
            using (DirectoryEntry toUnitEntry = GetDomainOrganizationUnitEntry(toUnit, adminUser))
            {
                using (DirectoryEntry moveUserEntry = GetUserEntry(ldapPath, moveUser, adminUser))
                {
                    moveUserEntry.MoveTo(toUnitEntry);
                    toUnitEntry.CommitChanges();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromUnit"></param>
        /// <param name="toUnit"></param>
        [DirectoryServicesPermission(SecurityAction.Assert)]
        public static void MoveOrganizationUnit(DomainOrganizationUnit fromUnit, DomainOrganizationUnit toUnit)
        {
            MoveOrganizationUnit(fromUnit, toUnit, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromUnit"></param>
        /// <param name="toUnit"></param>
        /// <param name="adminUser"></param>
        [DirectoryServicesPermission(SecurityAction.Assert)]
        public static void MoveOrganizationUnit(DomainOrganizationUnit fromUnit, DomainOrganizationUnit toUnit, DomainUser adminUser)
        {
            using (DirectoryEntry toUnitEntry = GetDomainOrganizationUnitEntry(toUnit, adminUser))
            {
                using (DirectoryEntry fromUnitEntry = GetDomainOrganizationUnitEntry(fromUnit, adminUser))
                {
                    fromUnitEntry.MoveTo(toUnitEntry);
                    toUnitEntry.CommitChanges();

                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modifyUnit"></param>
        /// <param name="ouNewName"></param>
        [DirectoryServicesPermission(SecurityAction.Assert)]
        public static void ModifyOrganizationUnit(DomainOrganizationUnit modifyUnit, string ouNewName)
        {
            ModifyOrganizationUnit(modifyUnit, ouNewName, null);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="modifyUnit"></param>
        /// <param name="ouNewName"></param>
        /// <param name="adminUser"></param>
        [DirectoryServicesPermission(SecurityAction.Assert)]
        public static void ModifyOrganizationUnit(DomainOrganizationUnit modifyUnit,string ouNewName, DomainUser adminUser)
        {
            using (DirectoryEntry modifyUnitEntry = GetDomainOrganizationUnitEntry(modifyUnit, adminUser))
            {
                modifyUnitEntry.Rename("ou=" + ouNewName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="adminUser"></param>
        /// <returns></returns>
        public static DirectoryEntry GetDomainOrganizationUnitEntry(DomainOrganizationUnit unit, DomainUser adminUser)
        {
            if (unit == null)
            {
                throw new ArgumentNullException("unit");
            }

            if (string.IsNullOrEmpty(unit.Path))
            {
                throw new ArgumentNullException("unit");
            }

            DirectoryEntry entry;

            if (adminUser == null)
            {
                entry = new DirectoryEntry(unit.Path);
            }
            else
            {
                if (string.IsNullOrEmpty(adminUser.Account.UserAccount))
                {
                    throw new Exception("adminUser.Account.UserAccount不能为空");
                }
                if (string.IsNullOrEmpty(adminUser.Account.Password))
                {
                    throw new Exception("adminUser.Account.Password不能为空");
                }
                entry = new DirectoryEntry(unit.Path, adminUser.Account.UserAccount, adminUser.Account.Password);
            }
            return entry;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modifyUser"></param>
        /// <param name="modifyUserEntry"></param>
        private static void SetAdUserEntry(DomainUser modifyUser, DirectoryEntry modifyUserEntry)
        {
            if (!string.IsNullOrEmpty(modifyUser.Account.UserAccount))
            {
                modifyUserEntry.Properties[DomainUserPropertities.Account.SAMAccountName].Value = modifyUser.Account.UserAccount.Trim();
            }

            if (!string.IsNullOrEmpty(modifyUser.Account.UserPrincipalName))
            {
                modifyUserEntry.Properties[DomainUserPropertities.Account.UserPrincipalName].Value = modifyUser.Account.UserPrincipalName.Trim();
            }

            if (modifyUser.Account.Password != null)
            {
                modifyUserEntry.Properties[DomainUserPropertities.Account.UserPassword].Value = modifyUser.Account.Password;
            }

            if (!string.IsNullOrEmpty(modifyUser.User.Name))
            {
                modifyUserEntry.Properties[DomainUserPropertities.User.Name].Value = modifyUser.User.Name.Trim();
            }

            if (!string.IsNullOrEmpty(modifyUser.User.GivenName))
            {
                modifyUserEntry.Properties[DomainUserPropertities.User.GivenName].Value = modifyUser.User.GivenName.Trim();
            }

            if (!string.IsNullOrEmpty(modifyUser.User.LastName))
            {
                modifyUserEntry.Properties[DomainUserPropertities.User.LastName].Value = modifyUser.User.LastName.Trim();
            }

            if (!string.IsNullOrEmpty(modifyUser.User.OfficeEMailAddress))
            {
                modifyUserEntry.Properties[DomainUserPropertities.User.Mail].Value = modifyUser.User.OfficeEMailAddress.Trim();
            }

            if (!string.IsNullOrEmpty(modifyUser.User.DisplayName))
            {
                modifyUserEntry.Properties[DomainUserPropertities.User.DisplayName].Value = modifyUser.User.DisplayName.Trim();
            }

            if (!string.IsNullOrEmpty(modifyUser.User.Description))
            {
                modifyUserEntry.Properties[DomainUserPropertities.User.Description].Value = modifyUser.User.Description.Trim();
            }

            if (!string.IsNullOrEmpty(modifyUser.Telephone.Mobile))
            {
                modifyUserEntry.Properties[DomainUserPropertities.Telephone.Mobile].Value = modifyUser.Telephone.Mobile.Trim();
            }

            if (!string.IsNullOrEmpty(modifyUser.Telephone.Fax))
            {
                modifyUserEntry.Properties[DomainUserPropertities.Telephone.Fax].Value = modifyUser.Telephone.Fax.Trim();
            }

            if (!string.IsNullOrEmpty(modifyUser.Telephone.Pager))
            {
                modifyUserEntry.Properties[DomainUserPropertities.Telephone.Pager].Value = modifyUser.Telephone.Pager.Trim();
            }

            if (!string.IsNullOrEmpty(modifyUser.Telephone.IPPhone))
            {
                modifyUserEntry.Properties[DomainUserPropertities.Telephone.IPPhone].Value = modifyUser.Telephone.IPPhone.Trim();
            }

            if (!string.IsNullOrEmpty(modifyUser.Address.Street))
            {
                modifyUserEntry.Properties[DomainUserPropertities.Address.Street].Value = modifyUser.Address.Street.Trim();
            }

            if (!string.IsNullOrEmpty(modifyUser.Company.Department))
            {
                modifyUserEntry.Properties[DomainUserPropertities.Company.Department].Value = modifyUser.Company.Department.Trim();
            }

            if (!string.IsNullOrEmpty(modifyUser.Company.Title))
            {
                modifyUserEntry.Properties[DomainUserPropertities.Company.Title].Value = modifyUser.Company.Title.Trim();
            }

            if (!string.IsNullOrEmpty(modifyUser.User.OfficeTelephoneNumber))
            {
                modifyUserEntry.Properties[DomainUserPropertities.User.TelephoneNumber].Value = modifyUser.User.OfficeTelephoneNumber.Trim();
            }

            if (!string.IsNullOrEmpty(modifyUser.Telephone.HomePhone))
            {
                modifyUserEntry.Properties[DomainUserPropertities.Telephone.HomePhone].Value = modifyUser.Telephone.HomePhone.Trim();
            }

        }


        /// <summary>
        /// 修改用户密码
        /// </summary>
        /// <param name="ldapPath">LDAP目录</param>
        /// <param name="modifyUser">需要修改的用户</param>
        /// <param name="newPassword">新密码</param>
        [DirectoryServicesPermission(SecurityAction.Assert)]
        public static void ModifyPassword(string ldapPath, DomainUser modifyUser, string newPassword)
        {
            ModifyPassword(ldapPath, modifyUser, newPassword, null);
        }

        /// <summary>
        /// 修改用户密码
        /// </summary>
        /// <param name="ldapPath">LDAP目录</param>
        /// <param name="adminUser">管理帐号</param>
        /// <param name="modifyUser">需要修改的用户</param>
        /// <param name="newPassword">新密码</param>
        [DirectoryServicesPermission(SecurityAction.Assert)]
        public static void ModifyPassword(string ldapPath, DomainUser modifyUser, string newPassword, DomainUser adminUser)
        {
            using (DirectoryEntry userEntry = GetUserEntry(ldapPath, modifyUser, adminUser))
            {
                try
                {
                    userEntry.Invoke("SetPassword", new object[] { newPassword });
                }
                catch (Exception ex)
                {
                    var exNew = new Exception("修改密码错误。如果账户设置了密码复杂度策略，则需要确保密码中包含大小写字母、数字等。（如下是系统管理员信息：如果出现了'Access Denied'错误，可能是权限不够。如果程序部署在IIS中，要确保IIS的应用程序池运行帐户有权限去操作AD信息）", ex);
                    throw exNew;
                }

            }
        }

        /// <summary>
        /// 获取用户对应的DirectoryEntry，没有就返回null
        /// </summary>
        /// <param name="ldapPath"></param>
        /// <param name="newUser"></param>
        /// <returns></returns>
        public static DirectoryEntry GetUserEntry(string ldapPath, DomainUser newUser)
        {
            return GetUserEntry(ldapPath, newUser, null);
        }

        /// <summary>
        /// 获取用户对应的DirectoryEntry，没有就返回null
        /// </summary>
        /// <param name="ldapPath"></param>
        /// <param name="adminUser"></param>
        /// <param name="newUser"></param>
        /// <returns></returns>
        public static DirectoryEntry GetUserEntry(string ldapPath, DomainUser newUser, DomainUser adminUser)
        {
            if (newUser == null)
            {
                throw new ArgumentNullException("newUser");
            }
            DirectoryEntry entry = null;
            try
            {
                if (adminUser == null)
                {
                    entry = new DirectoryEntry(ldapPath);
                }
                else
                {
                    if (string.IsNullOrEmpty(adminUser.Account.UserAccount))
                    {
                        throw new Exception("adminUser.Account.UserAccount不允许为空");
                    }
                    if (string.IsNullOrEmpty(adminUser.Account.Password))
                    {
                        throw new Exception("adminUser.Account.Password不允许为空");
                    }

                    entry = new DirectoryEntry(ldapPath, adminUser.Account.UserAccount, adminUser.Account.Password);
                }

                using (var searcher = new DirectorySearcher(entry))
                {
                    if (string.IsNullOrEmpty(newUser.Account.UserAccount))
                    {
                        throw new Exception("newUser.Account.UserAccount不允许为空");
                    }

                    //这里一定不能加单引号，语法规则就是如此
                    searcher.Filter = string.Format("(sAMAccountname={0})", newUser.Account.UserAccount);

                    searcher.SearchScope = SearchScope.Subtree;


                    #region Properties To Load

                    searcher.PropertiesToLoad.Add(DomainUserPropertities.Account.PasswordLastSet);
                    searcher.PropertiesToLoad.Add(DomainUserPropertities.Account.SAMAccountName);
                    searcher.PropertiesToLoad.Add(DomainUserPropertities.Account.SAMAccountType);
                    searcher.PropertiesToLoad.Add(DomainUserPropertities.Account.UserAccountControl);
                    searcher.PropertiesToLoad.Add(DomainUserPropertities.Account.UserPrincipalName);
                    searcher.PropertiesToLoad.Add(DomainUserPropertities.Account.WhenCreated);


                    searcher.PropertiesToLoad.Add(DomainUserPropertities.Address.City);
                    searcher.PropertiesToLoad.Add(DomainUserPropertities.Address.Country.CountryAB);
                    searcher.PropertiesToLoad.Add(DomainUserPropertities.Address.Country.CountryCode);
                    searcher.PropertiesToLoad.Add(DomainUserPropertities.Address.Country.CountryName);
                    searcher.PropertiesToLoad.Add(DomainUserPropertities.Address.PostCode);
                    searcher.PropertiesToLoad.Add(DomainUserPropertities.Address.PostOfficeBox);
                    searcher.PropertiesToLoad.Add(DomainUserPropertities.Address.Province);
                    searcher.PropertiesToLoad.Add(DomainUserPropertities.Address.Street);

                    searcher.PropertiesToLoad.Add(DomainUserPropertities.Company.CompanyName);
                    searcher.PropertiesToLoad.Add(DomainUserPropertities.Company.Department);
                    searcher.PropertiesToLoad.Add(DomainUserPropertities.Company.Manager);
                    searcher.PropertiesToLoad.Add(DomainUserPropertities.Company.Title);

                    searcher.PropertiesToLoad.Add(DomainUserPropertities.Telephone.Fax);
                    searcher.PropertiesToLoad.Add(DomainUserPropertities.Telephone.HomePhone);
                    searcher.PropertiesToLoad.Add(DomainUserPropertities.Telephone.Mobile);
                    searcher.PropertiesToLoad.Add(DomainUserPropertities.Telephone.Pager);
                    searcher.PropertiesToLoad.Add(DomainUserPropertities.Telephone.IPPhone);

                    searcher.PropertiesToLoad.Add(DomainUserPropertities.User.DisplayName);
                    searcher.PropertiesToLoad.Add(DomainUserPropertities.User.Description);
                    searcher.PropertiesToLoad.Add(DomainUserPropertities.User.GivenName);
                    searcher.PropertiesToLoad.Add(DomainUserPropertities.User.HomePage);
                    searcher.PropertiesToLoad.Add(DomainUserPropertities.User.Initials);
                    searcher.PropertiesToLoad.Add(DomainUserPropertities.User.LastName);
                    searcher.PropertiesToLoad.Add(DomainUserPropertities.User.Mail);
                    searcher.PropertiesToLoad.Add(DomainUserPropertities.User.Name);
                    searcher.PropertiesToLoad.Add(DomainUserPropertities.User.PhysicalDeliveryOfficeName);
                    searcher.PropertiesToLoad.Add(DomainUserPropertities.User.TelephoneNumber);
                    searcher.PropertiesToLoad.Add(DomainUserPropertities.User.UserName);

                    searcher.PropertiesToLoad.Add(DomainUserPropertities.Object.ObjectCategory);
                    searcher.PropertiesToLoad.Add(DomainUserPropertities.Object.ObjectClass);
                    searcher.PropertiesToLoad.Add(DomainUserPropertities.Object.ObjectGuid);
                    searcher.PropertiesToLoad.Add(DomainUserPropertities.Object.ObjectSid);
                    searcher.PropertiesToLoad.Add(DomainUserPropertities.Object.PrimaryGroupID);


                    #endregion

                    SearchResult result = searcher.FindOne();
                    if (result == null)
                    {
                        if (adminUser == null)
                        {
                            throw new Exception(string.Format("获取ldapPath下的用户对象失败，ldapPath='{0}', userAccount='{1}'", ldapPath, newUser.Account.UserAccount));
                        }
                        else
                        {
                            throw new Exception(string.Format("获取ldapPath下的用户对象失败，ldapPath='{0}', userAccount='{1}',adminUserAccount='{2}'", ldapPath, newUser.Account.UserAccount,adminUser.Account.UserAccount));
                        }
                    }

                    DirectoryEntry userEntry = result.GetDirectoryEntry();
                    return userEntry;
                }
            }
            finally
            {
                if (entry != null)
                {
                    entry.Dispose();
                }
            }
        }

        /// <summary>
        /// 在指定的LDAP路径下新建组织单元
        /// </summary>
        /// <param name="ldapPath"></param>
        /// <param name="ouName"></param>
        /// <param name="adminUser"></param>
        public static DomainOrganizationUnit InsertOrganizationUnit(string ldapPath, string ouName, DomainUser adminUser)
        {
            if (string.IsNullOrEmpty(ldapPath))
            {
                throw new ArgumentNullException("ldapPath");
            }

            if (string.IsNullOrEmpty(ouName))
            {
                throw new ArgumentNullException("ouName");
            }


            var result = new DomainOrganizationUnit();
            
            DirectoryEntry entry = null;
            try
            {
                if (adminUser == null)
                {
                    entry = new DirectoryEntry(ldapPath);
                }
                else
                {
                    if (string.IsNullOrEmpty(adminUser.Account.UserAccount))
                    {
                        throw new Exception("adminUser.Account.UserAccount不能为空");
                    }
                    if (string.IsNullOrEmpty(adminUser.Account.Password))
                    {
                        throw new Exception("adminUser.Account.Password不能为空");
                    }
                    entry = new DirectoryEntry(ldapPath, adminUser.Account.UserAccount, adminUser.Account.Password);
                }

                DirectoryEntry ouEntry = entry.Children.Add("OU=" + ouName, DirectoryObjectType.OrganizationalUnit.ToString());
                result.Path = ouEntry.Path;
                result.ParentPath = ouEntry.Parent.Path;

                ouEntry.CommitChanges();

            }
            catch (Exception ex)
            {
                var exNew = new Exception(string.Format("Add OrganizationUnit Error: ldapPath = '{0}', ouName = '{1}'.", ldapPath, ouName), ex);
                throw exNew;
            }
            finally
            {
                if (entry != null)
                {
                    entry.Dispose();
                }
            }

            result.Name = ouName;

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ldapPath"></param>
        /// <returns></returns>
        public static bool ExistOrganizationalUnit(string ldapPath)
        {
            return ExistOrganizationalUnit(ldapPath, DomainConfiguration.GetAdminUser());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ldapPath"></param>
        /// <param name="adminUser"></param>
        /// <returns></returns>
        public static bool ExistOrganizationalUnit(string ldapPath,DomainUser adminUser)
        {
            DirectoryEntry entry = null;
            try
            {
                if (adminUser == null)
                {
                    entry = new DirectoryEntry(ldapPath);
                }
                else
                {
                    if (string.IsNullOrEmpty(adminUser.Account.UserAccount))
                    {
                        throw new Exception("adminUser.Account.UserAccount不能为空");
                    }
                    if (string.IsNullOrEmpty(adminUser.Account.Password))
                    {
                        throw new Exception("adminUser.Account.Password不能为空");
                    }
                    entry = new DirectoryEntry(ldapPath, adminUser.Account.UserAccount, adminUser.Account.Password);
                }

                using (var searcher = new DirectorySearcher(entry))
                {
                    searcher.SearchScope = SearchScope.Base;
                    searcher.Filter = string.Format("(objectClass=organizationalUnit)");
                    try
                    {
                        return searcher.FindOne() != null;
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                string adminAccount = string.Empty;
                if(adminUser != null)
                {
                    adminAccount = adminUser.Account.UserAccount;
                }

                var exNew = new Exception(string.Format("ExistOrganizationalUnit Error,ldapPath='{0}',Admin Account='{1}'", ldapPath, adminAccount), ex);
                throw exNew;
            }
            finally
            {
                if (entry != null)
                {
                    entry.Dispose();
                }
            }
        }

        /// <summary>
        /// 获取指定路径的组织单元信息，如果没有找到就返回null
        /// </summary>
        /// <param name="ldapOuPath"></param>
        /// <returns></returns>
        public static DomainOrganizationUnit GetOrganizationalUnit(string ldapOuPath)
        {
            return GetOrganizationalUnit(ldapOuPath, null);
        }

        /// <summary>
        /// 获取指定路径的组织单元信息，如果没有找到就返回null
        /// </summary>
        /// <param name="ldapOuPath"></param>
        /// <param name="adminUser"></param>
        /// <returns></returns>
        public static DomainOrganizationUnit GetOrganizationalUnit(string ldapOuPath, DomainUser adminUser)
        {
            var rootOus = GetSubOrganizationalUnits(ldapOuPath, adminUser, SearchScope.Base);
            if (rootOus == null || rootOus.Count == 0)
            {
                return null;
            }

            return rootOus.FirstOrDefault();
        }

        /// <summary>
        /// 获取指定LDAP下的直接子级组织单元列表，不包括SearchRoot Object
        /// </summary>
        /// <param name="ldapPath"></param>
        /// <param name="includeAllSubOrganizationUnits"></param>
        /// <returns></returns>
        public static List<DomainOrganizationUnit> GetSubOrganizationalUnits(string ldapPath,bool includeAllSubOrganizationUnits)
        {
            return GetSubOrganizationalUnits(ldapPath, includeAllSubOrganizationUnits, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ldapPath"></param>
        /// <param name="adminUser"></param>
        /// <param name="includeAllSubOrganizationUnits"></param>
        /// <returns></returns>
        public static List<DomainOrganizationUnit> GetSubOrganizationalUnits(string ldapPath, bool includeAllSubOrganizationUnits, DomainUser adminUser)
        {
            var rootOus = GetSubOrganizationalUnits(ldapPath, adminUser, SearchScope.OneLevel);
            var result = rootOus.ToList();
            if (includeAllSubOrganizationUnits)
            {
                foreach (var rootOu in rootOus)
                {
                    var subOus = GetSubOrganizationalUnits(rootOu.Path, true, adminUser);
                    foreach (var subOu in subOus)
                    {
                        if (result.Find(item => item.Path == subOu.Path) == null)
                        {
                            result.Add(subOu);
                        }
                    }
                }
            }

            return result;
        }


        /// <summary>
        /// 获取指定LDAP下的直接子级组织单元列表，不包括SearchRoot Object
        /// </summary>
        /// <param name="ldapPath"></param>
        /// <param name="adminUser"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        public static List<DomainOrganizationUnit> GetSubOrganizationalUnits(string ldapPath, DomainUser adminUser,SearchScope scope)
        {
            if (string.IsNullOrEmpty(ldapPath))
            {
                throw new ArgumentNullException("ldapPath");
            }

            var ous = new List<DomainOrganizationUnit>();
            DirectoryEntry entry = null;
            try
            {
                if (adminUser == null)
                {
                    entry = new DirectoryEntry(ldapPath);
                }
                else
                {
                    if (string.IsNullOrEmpty(adminUser.Account.UserAccount))
                    {
                        throw new Exception("adminUser.Account.UserAccount不能为空");
                    }
                    if (string.IsNullOrEmpty(adminUser.Account.Password))
                    {
                        throw new Exception("adminUser.Account.Password不能为空");
                    }
                    entry = new DirectoryEntry(ldapPath, adminUser.Account.UserAccount, adminUser.Account.Password);
                }

                using (var searcher = new DirectorySearcher(entry))
                {
                    searcher.SearchScope = scope;
                    searcher.Filter = string.Format("(objectClass=organizationalUnit)");

                    searcher.PropertiesToLoad.Add(DomainOrganizationUnitPropertities.Address.City);
                    searcher.PropertiesToLoad.Add(DomainOrganizationUnitPropertities.Address.Country.CountryAb);
                    searcher.PropertiesToLoad.Add(DomainOrganizationUnitPropertities.Address.Country.CountryCode);
                    searcher.PropertiesToLoad.Add(DomainOrganizationUnitPropertities.Address.Country.CountryName);
                    searcher.PropertiesToLoad.Add(DomainOrganizationUnitPropertities.Address.PostCode);
                    searcher.PropertiesToLoad.Add(DomainOrganizationUnitPropertities.Address.Province);
                    searcher.PropertiesToLoad.Add(DomainOrganizationUnitPropertities.Address.Street);

                    searcher.PropertiesToLoad.Add(DomainOrganizationUnitPropertities.Object.ObjectCategory);
                    searcher.PropertiesToLoad.Add(DomainOrganizationUnitPropertities.Object.ObjectClass);
                    searcher.PropertiesToLoad.Add(DomainOrganizationUnitPropertities.Object.ObjectGuid);

                    searcher.PropertiesToLoad.Add(DomainOrganizationUnitPropertities.Manager);
                    searcher.PropertiesToLoad.Add(DomainOrganizationUnitPropertities.ModifyTimeStamp);
                    searcher.PropertiesToLoad.Add(DomainOrganizationUnitPropertities.WhenChanged);
                    searcher.PropertiesToLoad.Add(DomainOrganizationUnitPropertities.WhenCreated);
                    searcher.PropertiesToLoad.Add(DomainOrganizationUnitPropertities.Name);


                    using (SearchResultCollection src = searcher.FindAll())
                    {
                        foreach (SearchResult sr in src)
                        {
                            var ou = new DomainOrganizationUnit();

                            if (sr.Properties.Contains(DomainOrganizationUnitPropertities.Name))
                            {
                                ou.Name = sr.Properties[DomainOrganizationUnitPropertities.Name][0].ToString();
                            }
                            ou.Path = sr.Path;

                            #region DomainOrganizationUnitPropertities.Address

                            if (sr.Properties.Contains(DomainOrganizationUnitPropertities.Address.City))
                            {
                                ou.Address.City = sr.Properties[DomainOrganizationUnitPropertities.Address.City][0].ToString();
                            }

                            if (sr.Properties.Contains(DomainOrganizationUnitPropertities.Address.Country.CountryAb))
                            {
                                ou.Address.Country.CountryAB = sr.Properties[DomainOrganizationUnitPropertities.Address.Country.CountryAb][0].ToString();
                            }

                            if (sr.Properties.Contains(DomainOrganizationUnitPropertities.Address.Country.CountryCode))
                            {
                                ou.Address.Country.CountryCode = sr.Properties[DomainOrganizationUnitPropertities.Address.Country.CountryCode][0].ToString();
                            }

                            if (sr.Properties.Contains(DomainOrganizationUnitPropertities.Address.Country.CountryName))
                            {
                                ou.Address.Country.CountryName = sr.Properties[DomainOrganizationUnitPropertities.Address.Country.CountryName][0].ToString();
                            }

                            if (sr.Properties.Contains(DomainOrganizationUnitPropertities.Address.PostCode))
                            {
                                ou.Address.PostCode = sr.Properties[DomainOrganizationUnitPropertities.Address.PostCode][0].ToString();
                            }
                            if (sr.Properties.Contains(DomainOrganizationUnitPropertities.Address.Province))
                            {
                                ou.Address.Province = sr.Properties[DomainOrganizationUnitPropertities.Address.Province][0].ToString();
                            }

                            if (sr.Properties.Contains(DomainOrganizationUnitPropertities.Address.Street))
                            {
                                ou.Address.Street = sr.Properties[DomainOrganizationUnitPropertities.Address.Street][0].ToString();
                            }

                            #endregion

                            #region DomainOrganizationUnitPropertities.Object

                            if (sr.Properties.Contains(DomainOrganizationUnitPropertities.Object.ObjectCategory))
                            {
                                ou.Object.ObjectCategory = sr.Properties[DomainOrganizationUnitPropertities.Object.ObjectCategory][0].ToString();
                            }
                            if (sr.Properties.Contains(DomainOrganizationUnitPropertities.Object.ObjectClass))
                            {
                                ou.Object.ObjectClass = sr.Properties[DomainOrganizationUnitPropertities.Object.ObjectClass][0].ToString();
                            }
                            if (sr.Properties.Contains(DomainOrganizationUnitPropertities.Object.ObjectGuid))
                            {
                                ou.Object.ObjectGuid = new Guid((byte[])(sr.Properties[DomainOrganizationUnitPropertities.Object.ObjectGuid][0]));
                            }

                            #endregion

                            if (sr.Properties.Contains(DomainOrganizationUnitPropertities.Manager))
                            {
                                string managerPath = sr.Properties[DomainOrganizationUnitPropertities.Manager][0].ToString().Trim();
                                if (!managerPath.ToUpper().Trim().StartsWith(LdapPrefix))
                                {
                                    managerPath = LdapPrefix + managerPath.Trim();
                                }

                                ou.ManagerPath = managerPath;

                                //根据这个Path再去找到它的登陆信息
                                if (managerPath != string.Empty)
                                {
                                    ou.Manager = GetDomainUser(ou.ManagerPath, adminUser);
                                }
                            }

                            if (sr.Properties.Contains(DomainOrganizationUnitPropertities.WhenChanged))
                            {
                                ou.WhenChanged = DateTime.Parse(sr.Properties[DomainOrganizationUnitPropertities.WhenChanged][0].ToString());
                            }
                            if (sr.Properties.Contains(DomainOrganizationUnitPropertities.WhenCreated))
                            {
                                ou.WhenCreated = DateTime.Parse(sr.Properties[DomainOrganizationUnitPropertities.WhenCreated][0].ToString());
                            }
                            if (sr.Properties.Contains(DomainOrganizationUnitPropertities.ModifyTimeStamp))
                            {
                                ou.ModifyTimeStamp = DateTime.Parse(sr.Properties[DomainOrganizationUnitPropertities.ModifyTimeStamp][0].ToString());
                            }
                            DirectoryEntry deCurrent = sr.GetDirectoryEntry();
                            ou.ParentPath = deCurrent.Parent.Path;
                            ou.ParentName = deCurrent.Parent.Name;

                            ous.Add(ou);
                        }
                        return ous;
                    }
                }
            }
            catch (Exception ex)
            {
                string adminAccount = string.Empty;
                if (adminUser != null)
                {
                    adminAccount = adminUser.Account.UserAccount;
                }

                var exNew = new Exception(string.Format("GetSubOrganizationalUnits Error,ldapPath='{0}',Admin Account='{1}'", ldapPath, adminAccount), ex);
               
                throw exNew;
            }
            finally
            {
                if (entry != null)
                {
                    entry.Dispose();
                }
            }
        }

        /// <summary>
        /// 删除指定的LDAP目录下的所有子级组织单元。
        /// </summary>
        /// <param name="adminUser">管理帐号</param>
        [DirectoryServicesPermission(SecurityAction.Assert)]
        public static bool DeleteSubOrganizationUnits(DomainUser adminUser)
        {
            string ldapPath = adminUser.LDAPRootPath;
            if (string.IsNullOrEmpty(ldapPath))
            {
                throw new ArgumentNullException("adminUser");
            }

            DirectoryEntry entry = null;
            try
            {
                if (string.IsNullOrEmpty(adminUser.Account.UserAccount))
                {
                    throw new Exception("adminUser.Account.UserAccount不能为空");
                }

                if (string.IsNullOrEmpty(adminUser.Account.Password))
                {
                    throw new Exception("adminUser.Account.Password不能为空");
                }
                entry = new DirectoryEntry(ldapPath, adminUser.Account.UserAccount, adminUser.Account.Password);

                using (var searcher = new DirectorySearcher(entry))
                {
                    searcher.Filter = string.Format("(objectClass=organizationalUnit)");
                    searcher.SearchScope = SearchScope.OneLevel;
                    using (SearchResultCollection src = searcher.FindAll())
                    {
                        foreach (SearchResult sr in src)
                        {
                            using (DirectoryEntry ouEntry = sr.GetDirectoryEntry())
                            {
                                if (!DirectoryEntry.Exists(ouEntry.Path))
                                {
                                    continue;
                                }

                                ouEntry.DeleteTree();
                                ouEntry.CommitChanges();
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                string adminAccount = adminUser.Account.UserAccount;

                var exNew = new Exception(string.Format("DeleteSubOrganizationUnits Error,ldapPath='{0}',Admin Account='{1}'", ldapPath, adminAccount), ex);
                throw exNew;
            }
            finally
            {
                if (entry != null)
                {
                    entry.Dispose();
                }
            }
        }

        /// <summary>
        /// 删除指定的LDAP目录下指定名称的组织单元
        /// </summary>
        /// <param name="ldapPath">LDAP目录</param>
        /// <param name="ouName">组织单元名称</param>
        /// <param name="adminUser">管理帐号</param>
        [DirectoryServicesPermission(SecurityAction.Assert)]
        public static void DeleteSubOrganizationUnit(string ldapPath, string ouName, DomainUser adminUser)
        {
            if (string.IsNullOrEmpty(ldapPath))
            {
                throw new ArgumentNullException("ldapPath");
            }

            if (string.IsNullOrEmpty(ouName))
            {
                throw new ArgumentNullException("ouName");
            }


            DirectoryEntry entry = null;
            try
            {
                if (adminUser == null)
                {
                    entry = new DirectoryEntry(ldapPath);
                }
                else
                {
                    if (string.IsNullOrEmpty(adminUser.Account.UserAccount))
                    {
                        throw new Exception("adminUser.Account.UserAccount不能为空");
                    }
                    if (string.IsNullOrEmpty(adminUser.Account.Password))
                    {
                        throw new Exception("adminUser.Account.Password不能为空");
                    }
                    entry = new DirectoryEntry(ldapPath, adminUser.Account.UserAccount, adminUser.Account.Password);
                }

                using (var searcher = new DirectorySearcher(entry))
                {
                    searcher.Filter = string.Format("(&(objectClass=organizationalUnit)(ou={0}))", ouName);
                    searcher.SearchScope = SearchScope.OneLevel;
                    SearchResult sr = searcher.FindOne();
                    if (sr != null)
                    {
                        using (DirectoryEntry ouEntry = sr.GetDirectoryEntry())
                        {
                            DirectoryEntry deParent = ouEntry.Parent;
                            deParent.Children.Remove(ouEntry);
                            deParent.CommitChanges();
                            deParent.Dispose();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string adminAccount = string.Empty;
                if (adminUser != null)
                {
                    adminAccount = adminUser.Account.UserAccount;
                }

                var exNew = new Exception(string.Format("DeleteSubOrganizationUnit Error,ldapPath='{0}',Admin Account='{1}'", ldapPath, adminAccount), ex);
                throw exNew;
            }
            finally
            {
                if (entry != null)
                {
                    entry.Dispose();
                }
            }
        }

        /// <summary>
        /// 获取加密后的LDAP路径
        /// </summary>
        /// <param name="ldapPath"></param>
        /// <returns></returns>
        public static string EncryptPath(string ldapPath)
        {
            string encryptedPath = EncryptUtility.EncryptByDES(ldapPath, Encoding.Unicode, EncryptUtility.EncryptDesKeys, EncryptUtility.EncryptDesIVs);
            encryptedPath = HttpUtility.UrlEncode(encryptedPath);
            return encryptedPath;
        }

        /// <summary>
        /// 解密LDAP路径
        /// </summary>
        /// <param name="encryptedPath"></param>
        /// <returns></returns>
        public static string GetDecryptPath(string encryptedPath)
        {
            encryptedPath = HttpUtility.UrlDecode(encryptedPath);
            encryptedPath = EncryptUtility.DecryptByDES(encryptedPath, Encoding.Unicode, EncryptUtility.EncryptDesKeys, EncryptUtility.EncryptDesIVs);
            return encryptedPath;
        }

        /// <summary>
        /// 获取域帐号过滤条件，支持模糊查询
        /// </summary>
        /// <param name="domainUserPropertyValues"></param>
        /// <returns></returns>
        public static string GetFilter(Dictionary<string, string> domainUserPropertyValues)
        {
            if (domainUserPropertyValues == null)
            {
                throw new ArgumentNullException("domainUserPropertyValues");
            }
            string filter = string.Empty;
            foreach (KeyValuePair<string, string> kvp in domainUserPropertyValues)
            {
                if (kvp.Value == null || kvp.Value.Trim() == string.Empty)
                {
                    continue;
                }
                string value = kvp.Value.Trim();
                while (value.StartsWith("*"))
                {
                    value = value.Remove(0, 1).Trim();
                }

                while (value.EndsWith("*"))
                {
                    value = value.Substring(0, value.Length - 1).Trim();
                }


                if (value == string.Empty)
                {
                    value = "*";
                }
                else
                {
                    value = "*" + value + "*";
                }

                filter += string.Format("({0}={1})", kvp.Key, value);
            }
            return filter;
        }

        /// <summary>
        /// 根据App.config/Web.config中配置的LDAP信息获取AD中组织架构信息（当前组织和父级组织），如果父级组织为null，表示为根。
        /// </summary>
        /// <returns></returns>
        public static Dictionary<DomainOrganizationUnit, DomainOrganizationUnit> GetAllOrganizations()
        {
            return GetAllOrganizations(null);
        }

        /// <summary>
        /// 根据App.config/Web.config中配置的LDAP信息获取AD中组织架构信息（当前组织和父级组织），如果父级组织为null，表示为根。
        /// </summary>
        /// <returns></returns>
        public static Dictionary<DomainOrganizationUnit, DomainOrganizationUnit> GetAllOrganizations(DomainUser adminUser)
        {
            var orgs = new Dictionary<DomainOrganizationUnit, DomainOrganizationUnit>();
            string ldapComPath = adminUser != null ? adminUser.LDAPCompanyPath : DomainConfiguration.LDAPCompanyPath;
            foreach (DomainOrganizationUnit ou in GetSubOrganizationalUnits(ldapComPath, adminUser, SearchScope.OneLevel))
            {
                orgs.Add(ou, null);
                CollectSubOrganizationalUnits(ou, orgs,adminUser);
            }
            return orgs;
        }

        private static void CollectSubOrganizationalUnits(DomainOrganizationUnit ouParent, Dictionary<DomainOrganizationUnit, DomainOrganizationUnit> result,DomainUser adminUser)
        {
            if (ouParent == null)
            {
                return;
            }
            foreach (DomainOrganizationUnit ouSub in GetSubOrganizationalUnits(ouParent.Path, adminUser,SearchScope.OneLevel))
            {
                result.Add(ouSub, ouParent);
                CollectSubOrganizationalUnits(ouSub, result,adminUser);
            }
        }

        /// <summary>
        /// 获取域公司目录下的每个组织单元的所有用户信息，具体信息由配置文件中的LDAP决定
        /// 一定是公司目录下的组织单元里的列表.
        /// </summary>
        /// <returns></returns>
        public static DomainOrganizationUnitUserCollection GetDomainOrganizationUnitUsers()
        {
            return GetDomainOrganizationUnitUsers(null);
        }

        /// <summary>
        /// 获取域公司目录下的每个组织单元的所有用户信息，具体信息由配置文件中的LDAP决定
        /// </summary>
        /// <returns></returns>
        public static DomainOrganizationUnitUserCollection GetDomainOrganizationUnitUsers(DomainUser adminUser)
        {
            var ouUsers = new DomainOrganizationUnitUserCollection();
            foreach (DomainOrganizationUnit ou in GetAllOrganizations(adminUser).Keys)
            {
                DomainUserCollection users = GetDomainUsers(ou.Path, string.Empty, SearchScope.OneLevel, adminUser);
                if (users == null)
                {
                    continue;
                }

                var ouUser = new DomainOrganizationUnitUser {OU = ou, Users = users};
                ouUsers.Add(ouUser);
            }
            return ouUsers;
        }

        /// <summary>
        /// 获取指定的组织单元名称对应的LDAP路径。组织单元名称列表（如下格式：总部/分公司/二级分公司，以反斜线分割）
        /// </summary>
        /// <param name="ouNames">组织单元名称列表（如下格式：总部/分公司/二级分公司）</param>
        /// <returns></returns>
        public static string GetLDAPPath(string ouNames)
        {
            return GetLDAPPath(ouNames, DomainConfiguration.LDAPDNSName);
        }


        /// <summary>
        /// 获取指定的组织单元名称对应的LDAP路径。组织单元名称列表ouNames（如下格式：总部/分公司/二级分公司，以反斜线分割）.域控的DNS地址dcNames（比如：testAD.microsoft.com，以"."分割)
        /// </summary>
        /// <param name="ouNames">组织单元名称列表（如下格式：总部/分公司/二级分公司）</param>
        /// <param name="dcNames">域控的DNS地址，比如：testAD.microsoft.com，以"."分割</param>
        /// <returns></returns>
        public static string GetLDAPPath(string ouNames, string dcNames)
        {
            var ouPath = new StringBuilder();
            ouPath.Append("LDAP://");
            if (!string.IsNullOrEmpty(ouNames))
            {
                string[] ous = ouNames.Split('/');
                for (int i = ous.Length - 1; i >= 0;i-- )
                {
                    ouPath.Append(string.Format("OU={0},", ous[i]));
                }
            }
            if (dcNames.IndexOf(".", StringComparison.Ordinal) != -1)
            {
                foreach (string dc in dcNames.Split('.'))
                {
                    ouPath.Append(string.Format(",DC={0}", dc));
                }
            }
            else if (dcNames.IndexOf("/", StringComparison.Ordinal) != -1)
            {
                string[] dcs = dcNames.Split('/');
                for(int i = dcs.Length -1;i >=0;i--)
                {
                    ouPath.Append(string.Format(",DC={0}", dcs[i]));
                }
            }

            return ouPath.ToString().Replace(",,",",");
        }

        /// <summary>
        /// 根据上级路径获取组织单元的路径
        /// </summary>
        /// <param name="parentOuPath"></param>
        /// <param name="currentOuName"></param>
        /// <returns></returns>
        public static string GetOrganizationUnitPath(string parentOuPath, string currentOuName)
        {
            if (string.IsNullOrEmpty(parentOuPath))
            {
                throw new ArgumentNullException("parentOuPath");
            }
            if (!parentOuPath.ToUpper().Trim().StartsWith("LDAP://"))
            {
                throw new Exception("parentOUPath必须以LDAP://开头");
            }
            return string.Format("LDAP://OU={0},{1}",currentOuName, parentOuPath.Trim().Remove(0, 7));
        }

       

        /// <summary>
        /// 根据DNS名称获取LDAP路径
        /// </summary>
        /// <param name="ldapDnsName"></param>
        /// <returns></returns>
        public static string GetLdapPathByDnsName(string ldapDnsName)
        {
            string path = ldapDnsName.Split('.').Aggregate("LDAP://", (current, dc) => current + string.Format("DC={0},", dc));
            if (path.EndsWith(","))
            {
                path = path.Substring(0, path.Length - 1);
            }

            return path;
        }

        /// <summary>
        /// 根据DNS和公司名获取公司所处OU的LDAP路径
        /// </summary>
        /// <param name="ldapDnsName"></param>
        /// <param name="ldapCompanyName"></param>
        /// <returns></returns>
        public static string GetLdapCompanyPathByDnsNameAndCompanyName(string ldapDnsName, string ldapCompanyName)
        {
            string path = string.Format("LDAP://OU={0}", ldapCompanyName);


            return ldapDnsName.Split('.').Aggregate(path, (current, dc) => current + string.Format(",DC={0}", dc)); 
        }

       
    }
}

