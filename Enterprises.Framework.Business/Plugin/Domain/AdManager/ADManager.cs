using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using Enterprises.Framework.Plugin.Domain.AdManager.ADObject;
using Enterprises.Framework.Plugin.Domain.AdManager.Enum;
using Enterprises.Framework.Plugin.Domain.AdManager.Exception;

namespace Enterprises.Framework.Plugin.Domain.AdManager
{
    /// <summary>
    /// 用于进行AD管理里的对象，提供操作AD的静态方法。
    /// </summary>
    /// <remarks>
    /// 这里ADsPath可以是LDAP://DN和LDAP://&lt;GUID&gt;两种形式，但不限于这种形式。
    /// 一般方法均提供2种形式，一种是用参数提供的用户身份标识，一种是用默认的用户身份标识。
    /// 默认用户身份标识取决于当前进程的用户身份标识。
    /// </remarks>
    public class ADManager
    {
        #region 域信息

        /// <summary>
        /// 将友好的域名称（friendly domain name）转换为合格的域名称（fully qualified domain name）。
        /// eg:tb -- > tb.com
        /// </summary>
        /// <param name="friendlyDomainName">友好的域名称（friendly domain name）。
        /// 可以是：
        /// 域控制器的 DNS 名称。
        /// ADAM 服务器的 DNS 名称和 LDAP 端口号（如 adam_instance.fabrikam.com:389）。
        /// 域的 DNS 名称，如 sales.corp.fabrikam.com。
        /// 林的 DNS 名称，如 corp.fabrikam.com。
        /// 应用程序分区的 DNS 名称。
        /// 与服务连接点关联的关键字之一，该服务连接点由配置集的 ADAM 实例注册。</param>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        /// <returns></returns>
        public static string FriendlyDomainToLdapDomain(string friendlyDomainName, string userName, string password)
        {
            string ldapPath;
            try
            {
                DirectoryContext objContext = null;
                if (UseDefaultIdentity(userName, password))
                    objContext = new DirectoryContext(DirectoryContextType.Domain, friendlyDomainName);
                else
                    objContext = new DirectoryContext(DirectoryContextType.Domain, friendlyDomainName, userName, password);

                ldapPath = System.DirectoryServices.ActiveDirectory.Domain.GetDomain(objContext).Name;
            }
            catch (DirectoryServicesCOMException dsce)
            {
                throw dsce;
            }
            return ldapPath;
        }

        /// <summary>
        /// 将友好的域名称（friendly domain name）转换为合格的域名称（fully qualified domain name）。
        /// eg:tb -- > tb.com
        /// </summary>
        /// <param name="friendlyDomainName">友好的域名称（friendly domain name）。
        /// 可以是：
        /// 域控制器的 DNS 名称。
        /// ADAM 服务器的 DNS 名称和 LDAP 端口号（如 adam_instance.fabrikam.com:389）。
        /// 域的 DNS 名称，如 sales.corp.fabrikam.com。
        /// 林的 DNS 名称，如 corp.fabrikam.com。
        /// 应用程序分区的 DNS 名称。
        /// 与服务连接点关联的关键字之一，该服务连接点由配置集的 ADAM 实例注册。</param>
        /// <returns></returns>
        public static string FriendlyDomainToLdapDomain(string friendlyDomainName)
        {
            return FriendlyDomainToLdapDomain(friendlyDomainName, null, null);
        }

        /// <summary>
        /// 获取当前用户上下文的 Forest 对象中的所有域名称。
        /// </summary>
        /// <returns></returns>
        public static List<string> EnumerateDomains()
        {
            List<string> alDomains = new List<string>();
            Forest currentForest = Forest.GetCurrentForest();
            DomainCollection myDomains = currentForest.Domains;

            foreach (System.DirectoryServices.ActiveDirectory.Domain objDomain in myDomains)
            {
                alDomains.Add(objDomain.Name);
            }
            return alDomains;
        }

        /// <summary>
        /// 获取当前用户上下文的 Forest 对象中所有的全局目录。 
        /// </summary>
        /// <returns></returns>
        public static List<string> EnumerateGlobalCatalogs()
        {
            List<string> alGCs = new List<string>();
            Forest currentForest = Forest.GetCurrentForest();
            foreach (GlobalCatalog gc in currentForest.GlobalCatalogs)
            {
                alGCs.Add(gc.Name);
            }
            return alGCs;
        }

        /// <summary>
        /// 获取当前用户身份标识的 Domain 对象中的域控制器。
        /// </summary>
        /// <returns></returns>
        public static List<string> EnumerateDomainControllers()
        {
            List<string> alDcs = new List<string>();
            System.DirectoryServices.ActiveDirectory.Domain domain = System.DirectoryServices.ActiveDirectory.Domain.GetCurrentDomain();
            foreach (DomainController dc in domain.DomainControllers)
            {
                alDcs.Add(dc.Name);
            }
            return alDcs;
        }

        #endregion


        #region Common

        /// <summary>
        /// 检验指定的DirectoryEntry是否存在
        /// </summary>
        /// <param name="path">ADsPath，自动添加LDAP_IDENTITY。完全转义过的。</param>
        /// <returns></returns>
        public static bool Exists(string path)
        {
            if (path.StartsWith(ParaMgr.LDAP_IDENTITY))
                return DirectoryEntry.Exists(path);
            else
                return DirectoryEntry.Exists(ParaMgr.LDAP_IDENTITY + path);

        }

        /// <summary>
        /// 移动DirectoryEntry到指定位置。
        /// </summary>
        /// <param name="objectPath">要移动的DirectoryEntry的ADsPath。必须是DN形式。完全转义过的。</param>
        /// <param name="newLocationPath">移动到的位置的ADsPath。必须是DN形式。完全转义过的。</param>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        /// <remarks>被移动的对象和要移动到的位置对象必须使用DN形式的路径创建，不能使用GUID形式的路径，否则会引发异常。</remarks>
        public static void Move(string objectPath, string newLocationPath, string userName, string password)
        {
            if (!Exists(objectPath))
                throw new EntryNotExistException("需要被移动的对象不存在。");

            DirectoryEntry de = null;
            try
            {
                de = GetByPath(objectPath, userName, password);

                Move(de, newLocationPath, userName, password);
            }
            finally
            {
                if (de != null)
                {
                    de.Close();
                    de.Dispose();
                }
            }
        }

        /// <summary>
        /// 移动DirectoryEntry到指定位置，使用默认用户身份标识。
        /// </summary>
        /// <param name="objectPath">要移动的DirectoryEntry的ADsPath。必须是DN形式。完全转义过的。</param>
        /// <param name="newLocationPath">移动到的位置的ADsPath。必须是DN形式。完全转义过的。</param>
        public static void Move(string objectPath, string newLocationPath)
        {
            Move(objectPath, newLocationPath, null, null);
        }

        /// <summary>
        /// 移动DirectoryEntry到指定位置。
        /// </summary>
        /// <param name="de">要移动的DirectoryEntry对象</param>
        /// <param name="newLocationPath">移动到的位置的ADsPath。必须是DN形式。完全转义过的。</param>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        /// <remarks>被移动的对象和要移动到的位置对象必须使用DN形式的路径创建，不能使用GUID形式的路径，否则会引发异常。</remarks>
        internal static void Move(DirectoryEntry de, string newLocationPath, string userName, string password)
        {
            if (!Exists(newLocationPath))
                throw new EntryNotExistException("移动到的位置对象不存在。");

            DirectoryEntry newLocation = null;
            try
            {
                newLocation = GetByPath(newLocationPath, userName, password);

                string newLocationDN = Utils.EscapeDNBackslashedChar(newLocation.Properties[BaseObject.PROPERTY_DN].Value.ToString());
                string deDN = Utils.EscapeDNBackslashedChar(de.Properties[BaseObject.PROPERTY_DN].Value.ToString());
                if (Exists(Utils.GenerateDN(Utils.GetRDN(deDN), deDN)))
                    throw new SameRDNException("移动到的位置下存在同名对象。");

                de.MoveTo(newLocation);

                de.CommitChanges();
            }
            catch (InvalidOperationException ioe)   // 指定的 DirectoryEntry 不是容器。
            {
                throw new NotContainerException(ioe.Message, ioe);
            }
            catch (DirectoryServicesCOMException dsce)
            {
                throw dsce;
            }
            finally
            {
                if (newLocation != null)
                {
                    newLocation.Close();
                    newLocation.Dispose();
                }
            }
        }


        /// <summary>
        /// 获取应用程序设置的默认域。
        /// </summary>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        /// <returns></returns>
        public static DirectoryEntry GetAppSetDomain(string userName, string password)
        {
            return GetByDN(ParaMgr.ADFullPath, userName, password);
        }

        /// <summary>
        /// 获取应用程序设置的默认域，使用默认用户身份标识。
        /// </summary>
        /// <returns></returns>
        public static DirectoryEntry GetAppSetDomain()
        {
            return GetAppSetDomain(null, null);
        }

        // 根据用户名和密码，判断是否应该使用默认用户身份标识。
        private static bool UseDefaultIdentity(string userName, string password)
        {
            if (String.IsNullOrEmpty(userName))
                return true;
            else
                return false;
        }


        #endregion


        #region Get & Search

        /// <summary>
        /// 获取DirectoryEntry
        /// </summary>
        /// <param name="schema">自定义模式</param>
        /// <param name="objectClass">类型</param>
        /// <param name="objectCategory">类别</param>
        /// <param name="rootDN">根对象DN，null表示整个域。</param>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        /// <returns>如果不存在，返回null。</returns>
        internal static DirectoryEntry Get(string schema, string objectClass, string objectCategory, string rootDN, string userName, string password)
        {
            DirectoryEntry de = GetByDN((String.IsNullOrEmpty(rootDN) ? (ParaMgr.ADFullPath) : rootDN), 
                userName, password);

            DirectorySearcher deSearch = new DirectorySearcher();
            deSearch.SearchRoot = de;
            if (!String.IsNullOrEmpty(objectClass) ||
                !String.IsNullOrEmpty(objectCategory) ||
                !String.IsNullOrEmpty(schema))
            {
                deSearch.Filter = String.Format("(&{0}{1}{2})",
                    ((!String.IsNullOrEmpty(objectClass)) ? String.Format("(objectClass={0})", objectClass) : ""),
                    ((!String.IsNullOrEmpty(objectCategory)) ? String.Format("(objectCategory={0})", objectCategory) : ""),
                    ((!String.IsNullOrEmpty(schema)) ? String.Format("({0})", schema) : "")
                    );
            }
            deSearch.SearchScope = SearchScope.Subtree;
            SearchResult results = deSearch.FindOne();
            var rde = results != null ? GetByPath(results.Path) : null;

            de.Close();
            de.Dispose();

            return rde;
        }

        /// <summary>
        /// 获取DirectoryEntry，使用默认用户身份标识。
        /// </summary>
        /// <param name="schema">自定义模式</param>
        /// <param name="objectClass">类型</param>
        /// <param name="objectCategory">类别</param>
        /// <param name="rootDN">根对象DN，null表示整个域。</param>
        /// <returns>如果不存在，返回null。</returns>
        internal static DirectoryEntry Get(string schema, string objectClass, string objectCategory, string rootDN)
        {
            return Get(schema, objectClass, objectCategory, rootDN, null, null);
        }


        /// <summary>
        /// 查找DirectoryEntry
        /// </summary>
        /// <param name="schema">自定义模式</param>
        /// <param name="objectClass">类型</param>
        /// <param name="objectCategory">类别</param>
        /// <param name="rootPath">根对象ADsPath</param>
        /// <param name="scope">SearchScope</param>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        /// <returns>如果不存在，返回空集合。</returns>
        internal static List<DirectoryEntry> Search(string schema, string objectClass, string objectCategory,
            string rootPath, SearchScope scope, string userName, string password)
        {
            DirectoryEntry de = null;

            if (!String.IsNullOrEmpty(rootPath))
            {
                de = GetByPath(rootPath, userName, password);
            }
            if (de == null)
                de = GetByPath(ParaMgr.ADFullPath, userName, password);


            DirectorySearcher deSearch = new DirectorySearcher();
            if (de != null)
                deSearch.SearchRoot = de;
            if (!String.IsNullOrEmpty(objectClass) ||
                !String.IsNullOrEmpty(objectCategory) ||
                !String.IsNullOrEmpty(schema))
            {
                deSearch.Filter = String.Format("(&{0}{1}{2})",
                    ((!String.IsNullOrEmpty(objectClass)) ? String.Format("(objectClass={0})", objectClass) : ""),
                    ((!String.IsNullOrEmpty(objectCategory)) ? String.Format("(objectCategory={0})", objectCategory) : ""),
                    ((!String.IsNullOrEmpty(schema)) ? String.Format("({0})", schema) : "")
                    );
            }
            deSearch.SearchScope = scope;
            SearchResultCollection results = deSearch.FindAll();

            List<DirectoryEntry> entries = (from SearchResult se in results select se.GetDirectoryEntry()).ToList();

            if (de != null)
            {
                de.Close();
                de.Dispose();
            }

            return entries;

        }

        /// <summary>
        /// 查找DirectoryEntry，使用默认用户身份标识。
        /// </summary>
        /// <param name="schema">自定义模式</param>
        /// <param name="objectClass">类型</param>
        /// <param name="objectCategory">类别</param>
        /// <param name="rootPath">根对象ADsPath</param>
        /// <param name="scope">SearchScope</param>
        /// <returns>如果不存在，返回空集合。</returns>
        internal static List<DirectoryEntry> Search(string schema, string objectClass, string objectCategory, 
            string rootPath, SearchScope scope)
        {
            return Search(schema, objectClass, objectCategory, rootPath, scope, null, null);
        }

        /// <summary>
        /// 查找DirectoryEntry，结果为字符串形式
        /// </summary>
        /// <param name="schema">自定义模式</param>
        /// <param name="objectClass">类型</param>
        /// <param name="objectCategory">类别</param>
        /// <param name="rootPath">根对象ADsPath</param>
        /// <param name="scope">SearchScope</param>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        /// <returns>如果不存在，返回空集合。</returns>
        /// <remarks>包括distinguishedname,objectguid,name,description,adspath,objectcategory,objectclass。
        /// 最后添加了sAMAccountName。</remarks>
        internal static List<string[]> Search2(string schema, string objectClass, string objectCategory,
            string rootPath, SearchScope scope, string userName, string password)
        {
            DirectoryEntry de = null;

            if (!String.IsNullOrEmpty(rootPath))
            {
                de = GetByPath(rootPath, userName, password);
            }
            if (de == null)
                de = GetByPath(ParaMgr.ADFullPath, userName, password);


            DirectorySearcher deSearch = new DirectorySearcher();
            if (de != null)
                deSearch.SearchRoot = de;
            if (!String.IsNullOrEmpty(objectClass) ||
                !String.IsNullOrEmpty(objectCategory) ||
                !String.IsNullOrEmpty(schema))
            {
                deSearch.Filter = String.Format("(&{0}{1}{2})",
                    ((!String.IsNullOrEmpty(objectClass)) ? String.Format("(objectClass={0})", objectClass) : ""),
                    ((!String.IsNullOrEmpty(objectCategory)) ? String.Format("(objectCategory={0})", objectCategory) : ""),
                    ((!String.IsNullOrEmpty(schema)) ? String.Format("({0})", schema) : "")
                    );
            }
            deSearch.SearchScope = scope;
            SearchResultCollection results = deSearch.FindAll();

            List<string[]> entriesProperty = new List<string[]>();
            foreach (SearchResult se in results)
            {
                string nativeGuid = "";
                foreach(byte g in (byte[])se.Properties["objectguid"][0])
                {
                    nativeGuid += g.ToString("x2");
                }
                string oc = "";
                foreach (object c in se.Properties["objectclass"])
                {
                    oc = oc + "," + c.ToString();
                }

                string sAmAccountName = null;
                if (se.Properties.Contains("sAMAccountName"))
                    sAmAccountName = se.Properties["sAMAccountName"][0].ToString();


                entriesProperty.Add(new string[] {
                    se.Properties["distinguishedname"][0].ToString(),
                    Utils.ConvertNativeGuidToGuid(nativeGuid).ToString(),
                    se.Properties["name"][0].ToString(),
                    ((se.Properties["description"].Count > 0) ? se.Properties["description"][0].ToString() : null),
                    se.Properties["adspath"][0].ToString(),
                    se.Properties["objectcategory"][0].ToString(),
                    oc.Substring(1),
                    sAmAccountName
                });
            }

            if (de != null)
            {
                de.Close();
                de.Dispose();
            }

            return entriesProperty;

        }

        /// <summary>
        /// 查找DirectoryEntry，使用默认用户身份标识。结果为字符串形式
        /// </summary>
        /// <param name="schema">自定义模式</param>
        /// <param name="objectClass">类型</param>
        /// <param name="objectCategory">类别</param>
        /// <param name="rootPath">根对象ADsPath</param>
        /// <param name="scope">SearchScope</param>
        /// <returns>如果不存在，返回空集合。</returns>
        /// <remarks>包括distinguishedname,objectguid,name,description,adspath,objectcategory,objectclass</remarks>
        internal static List<string[]> Search2(string schema, string objectClass, string objectCategory,
            string rootPath, SearchScope scope)
        {
            return Search2(schema, objectClass, objectCategory, rootPath, scope, null, null);
        }

        /// <summary>
        /// 查找DirectoryEntry
        /// </summary>
        /// <param name="schema">自定义模式</param>
        /// <param name="objectClass">类型</param>
        /// <param name="objectCategory">类别</param>
        /// <param name="rootPath">根对象ADsPath</param>
        /// <param name="scope">SearchScope</param>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        /// <returns>如果不存在，返回空集合。</returns>
        internal static SearchResultCollection Search3(string schema, string objectClass, string objectCategory,
            string rootPath, SearchScope scope, string userName, string password)
        {
            DirectoryEntry de = null;

            if (!String.IsNullOrEmpty(rootPath))
            {
                de = GetByPath(rootPath, userName, password);
            }
            if (de == null)
                de = GetByPath(ParaMgr.ADFullPath, userName, password);


            DirectorySearcher deSearch = new DirectorySearcher();
            if (de != null)
                deSearch.SearchRoot = de;
            if (!String.IsNullOrEmpty(objectClass) ||
                !String.IsNullOrEmpty(objectCategory) ||
                !String.IsNullOrEmpty(schema))
            {
                deSearch.Filter = String.Format("(&{0}{1}{2})",
                    ((!String.IsNullOrEmpty(objectClass)) ? String.Format("(objectClass={0})", objectClass) : ""),
                    ((!String.IsNullOrEmpty(objectCategory)) ? String.Format("(objectCategory={0})", objectCategory) : ""),
                    ((!String.IsNullOrEmpty(schema)) ? String.Format("({0})", schema) : "")
                    );
            }
            deSearch.SearchScope = scope;
            
            SearchResultCollection results = deSearch.FindAll();

            de.Close();
            de.Dispose();

            return results;
        }

        /// <summary>
        /// 查找DirectoryEntry，使用默认用户身份标识。
        /// </summary>
        /// <param name="schema">自定义模式</param>
        /// <param name="objectClass">类型</param>
        /// <param name="objectCategory">类别</param>
        /// <param name="rootPath">根对象ADsPath</param>
        /// <param name="scope">SearchScope</param>
        /// <returns>如果不存在，返回空集合。</returns>
        internal static SearchResultCollection Search3(string schema, string objectClass, string objectCategory,
            string rootPath, SearchScope scope)
        {
            return Search3(schema, objectClass, objectCategory, rootPath, scope, null, null);
        }

        /// <summary>
        /// 根据DirectoryEntry的Guid得到DirectoryEntry对象。
        /// </summary>
        /// <param name="guid">Guid</param>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        /// <returns>如果不存在，返回null。</returns>
        internal static DirectoryEntry GetByGuid(Guid guid, string userName, string password)
        {
            return GetByPath(Utils.GenerateADsPath(guid), userName, password);
        }

        /// <summary>
        /// 根据DirectoryEntry的Guid得到DirectoryEntry对象，使用默认用户身份标识。
        /// </summary>
        /// <param name="guid">Guid</param>
        /// <returns>如果不存在，返回null。</returns>
        internal static DirectoryEntry GetByGuid(Guid guid)
        {
            return GetByGuid(guid, null,null );
        }


        /// <summary>
        /// 根据DirectoryEntry的SID得到DirectoryEntry对象。
        /// </summary>
        /// <param name="sid">objectSID</param>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        /// <returns>如果不存在，返回null。</returns>
        internal static DirectoryEntry GetBySid(string sid, string userName, string password)
        {
            return Get("objectSID=" + sid, null, null, null, userName, password);
        }

        /// <summary>
        /// 根据DirectoryEntry的SID得到DirectoryEntry对象，使用默认用户身份标识。
        /// </summary>
        /// <param name="sid">objectSID</param>
        /// <returns>如果不存在，返回null。</returns>
        internal static DirectoryEntry GetBySid(string sid)
        {
            return GetBySid(sid, null, null);
        }


        /// <summary>
        /// 根据DirectoryEntry的DN得到DirectoryEntry对象。
        /// </summary>
        /// <param name="dn">DN。完全转义过的。</param>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        /// <returns>如果不存在，返回null。</returns>
        internal static DirectoryEntry GetByDN(string dn, string userName, string password)
        {
            return GetByPath(ParaMgr.LDAP_IDENTITY + dn, userName, password);
        }

        /// <summary>
        /// 根据DirectoryEntry的DN得到DirectoryEntry对象，使用默认用户身份标识。
        /// </summary>
        /// <param name="dn">DN。完全转义过的。</param>
        /// <returns>如果不存在，返回null。</returns>
        internal static DirectoryEntry GetByDN(string dn)
        {
            return GetByDN(dn, null, null);
        }


        /// <summary>
        /// 根据DirectoryEntry的ADsPath得到DirectoryEntry对象。
        /// </summary>
        /// <param name="path">完整的ADsPath，自动添加LDAP_IDENTITY。完全转义过的。</param>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        /// <returns>如果不存在，返回null。</returns>
        /// <returns></returns>
        internal static DirectoryEntry GetByPath(string path, string userName, string password)
        {
            if (Exists(path))
            {
                if (UseDefaultIdentity(userName, password))
                    return new DirectoryEntry((path.StartsWith(ParaMgr.LDAP_IDENTITY)) ? path : (ParaMgr.LDAP_IDENTITY + path));
                return new DirectoryEntry(
                    (path.StartsWith(ParaMgr.LDAP_IDENTITY)) ? path : (ParaMgr.LDAP_IDENTITY + path),
                    userName,
                    password,
                    AuthenticationTypes.Secure);
            }

            return null;
        }

        /// <summary>
        /// 根据DirectoryEntry的ADsPath得到DirectoryEntry对象，使用默认用户身份标识。
        /// </summary>
        /// <param name="path">完整的ADsPath。完全转义过的。</param>
        /// <returns>如果不存在，返回null。</returns>
        /// <returns></returns>
        internal static DirectoryEntry GetByPath(string path)
        {
            return GetByPath(path, null, null);
        }


        #endregion


        #region User

        #region Search

        /// <summary>
        /// 获取指定所有用户。
        /// </summary>
        /// <param name="rootPath">根对象ADsPath，null表示整个域。</param>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        /// <returns>如果不存在，返回null。</returns>
        public static List<User> GetUserAll(string rootPath, string userName, string password)
        {
            List<DirectoryEntry> entries = Search(null, "user", "person", rootPath, SearchScope.Subtree, userName, password);
            List<User> users = new List<User>();
            foreach (DirectoryEntry de in entries)
            {
                users.Add(new User(de));

                de.Close();
                de.Dispose();
            }

            return users;
        }

        /// <summary>
        /// 获取指定所有用户，使用默认用户身份标识。
        /// </summary>
        /// <param name="rootPath">根对象ADsPath，null表示整个域。</param>
        /// <returns>如果不存在，返回null。</returns>
        public static List<User> GetUserAll(string rootPath)
        {
            return GetUserAll(rootPath, null, null);
        }

        /// <summary>
        /// 获取指定所有用户。
        /// </summary>
        /// <param name="rootPath">根对象ADsPath，null表示整个域。</param>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        /// <returns>如果不存在，返回null。</returns>
        /// <remarks>包括distinguishedname,objectguid,name,description,adspath,objectcategory,objectclass。
        /// 最后添加了sAMAccountName。</remarks>
        public static List<string[]> GetUserAllSimple(string rootPath, string userName, string password)
        {
            return Search2(null, "user", "person", rootPath, SearchScope.Subtree, userName, password);
        }

        /// <summary>
        /// 获取指定所有用户，使用默认用户身份标识。
        /// </summary>
        /// <param name="rootPath">根对象ADsPath，null表示整个域。</param>
        /// <returns>如果不存在，返回null。</returns>
        /// <remarks>包括distinguishedname,objectguid,name,description,adspath,objectcategory,objectclass</remarks>
        public static List<string[]> GetUserAllSimple(string rootPath)
        {
            return GetUserAllSimple(rootPath, null, null);
        }

        /// <summary>
        /// 获取指定所有用户。直接解析查询结果，速度较GetUserAll快。
        /// </summary>
        /// <param name="rootPath">根对象ADsPath，null表示整个域。</param>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        /// <returns>如果不存在，返回null。</returns>
        public static List<User> GetUserAllQuick(string rootPath, string userName, string password)
        {
            SearchResultCollection results = Search3(null, "user", "person", rootPath, SearchScope.Subtree, userName, password);

            List<User> users = new List<User>();
            foreach (SearchResult se in results)
            {
                users.Add(new User(se));
            }

            return users;
        }

        /// <summary>
        /// 获取指定所有用户，使用默认用户身份标识。直接解析查询结果，速度较GetUserAll快。
        /// </summary>
        /// <param name="rootPath">根对象ADsPath，null表示整个域。</param>
        /// <returns>如果不存在，返回null。</returns>
        public static List<User> GetUserAllQuick(string rootPath)
        {
            return GetUserAllQuick(rootPath, null, null);
        }

        /// <summary>
        /// 根据userPrincipalName获取Group。
        /// </summary>
        /// <param name="userPrincipalName">userPrincipalName。</param>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        /// <returns>如果不存在，返回null。</returns>
        public static User GetUserByPrincipalName(string userPrincipalName, string userName, string password)
        {
            List<DirectoryEntry> entries = Search("userPrincipalName=" + Utils.Escape4Query(userPrincipalName), 
                "user", "person", null, SearchScope.Subtree, userName, password);
            if (entries.Count == 1)
            {
                DirectoryEntry de = entries[0];

                User user = new User(de);

                de.Close();
                de.Dispose();

                return user;
            }

            return null;
        }

        /// <summary>
        /// 根据sAMAccountName获取User。
        /// </summary>
        /// <param name="sAmAccountName">sAMAccountName。</param>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        /// <returns>如果不存在，返回null。</returns>
        public static User GetUserBySamAccountName(string sAmAccountName, string userName, string password)
        {
            List<DirectoryEntry> entries = Search("sAMAccountName=" + Utils.Escape4Query(sAmAccountName), 
                "user", "person", null, SearchScope.Subtree, userName, password);
            if (entries.Count == 1)
            {
                DirectoryEntry de = entries[0];

                User user = new User(de);

                de.Close();
                de.Dispose();

                return user;
            }

            return null;
        }
        #endregion

        #region Get

        /// <summary>
        /// 根据用户的Guid得到用户对象。
        /// </summary>
        /// <param name="guid">Guid</param>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        /// <returns>如果不存在，返回null。</returns>
        public static User GetUserByGuid(Guid guid, string userName, string password)
        {
            return GetUserByPath(Utils.GenerateADsPath(guid), userName, password);
        }

        /// <summary>
        /// 根据用户的Guid得到用户对象，使用默认用户身份标识。
        /// </summary>
        /// <param name="guid">Guid</param>
        /// <returns>如果不存在，返回null。</returns>
        public static User GetUserByGuid(Guid guid)
        {
            return GetUserByGuid(guid, null, null);
        }

        /// <summary>
        /// 根据用户的DN得到用户对象。
        /// </summary>
        /// <param name="dn">DN。完全转义过的。</param>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        /// <returns>如果不存在，返回null。</returns>
        public static User GetUserByDN(string dn, string userName, string password)
        {
            return GetUserByPath(dn, userName, password);    
        }

        /// <summary>
        /// 根据用户的DN得到用户对象，使用默认用户身份标识。
        /// </summary>
        /// <param name="dn">DN。完全转义过的。</param>
        /// <returns>如果不存在，返回null。</returns>
        public static User GetUserByDN(string dn)
        {
            return GetUserByDN(dn, null, null);
        }

        /// <summary>
        /// 根据用户的ADsPath得到用户对象。
        /// </summary>
        /// <param name="path">ADsPath。完全转义过的。</param>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        /// <returns>如果不存在，返回null。</returns>
        public static User GetUserByPath(string path, string userName, string password)
        {
            DirectoryEntry entry = GetByPath(path, userName, password);
            if (entry != null)
            {
                User user = new User(entry);
                entry.Close();
                entry.Dispose();

                return user;
            }
            return null;
        }

        /// <summary>
        /// 根据用户的ADsPath得到用户对象，使用默认用户身份标识。
        /// </summary>
        /// <param name="path">ADsPath。完全转义过的。</param>
        /// <returns>如果不存在，返回null。</returns>
        public static User GetUserByPath(string path)
        {
            return GetUserByPath(path, null, null);
        }

        #endregion

        #region Password

        /// <summary>
        /// 设置用户密码。
        /// </summary>
        /// <param name="guid">用户DirectoryEntry的Guid。</param>
        /// <param name="newPassword">新密码。</param>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        public static void SetUserPassword(Guid guid, string newPassword, string userName, string password)
        {
            DirectoryEntry de = null;
            try
            {
                de = GetByGuid(guid, userName, password);

                if (de == null)
                    throw new EntryNotExistException("用户对象不存在。");

                if (de.SchemaClassName != SchemaClass.user.ToString("F"))
                    throw new SchemaClassException("对象类型不是" + SchemaClass.user.ToString("F") + "。");

                de.Invoke("SetPassword", new object[] { newPassword });

                de.CommitChanges();
            }
            catch (DirectoryServicesCOMException dsce)
            {
                throw dsce;
            }
            finally
            {
                if (de != null)
                {
                    de.Close();
                    de.Dispose();
                }
            }
        }

        /// <summary>
        /// 设置用户密码，使用默认用户身份标识。
        /// </summary>
        /// <param name="guid">用户DirectoryEntry的Guid。</param>
        /// <param name="newPassword">新密码。</param>
        public static void SetUserPassword(Guid guid, string newPassword)
        {
            SetUserPassword(guid, newPassword, null, null);
        }

        #endregion

        #region Move

        /// <summary>
        /// 移动用户DirectoryEntry到指定位置。
        /// </summary>
        /// <param name="userPath">要移动的用户DirectoryEntry的ADsPath。必须是DN形式。完全转义过的。</param>
        /// <param name="newLocationPath">移动到的位置的ADsPath。必须是DN形式。完全转义过的。</param>
        /// <param name="mustOu">移动到的位置对应的DirectoryEntry是否必须是组织单位。</param>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        public static void MoveUser(string userPath, string newLocationPath, bool mustOu, string userName, string password)
        {
            if (!Exists(userPath))
                throw new EntryNotExistException("需要被移动的对象不存在。");

            DirectoryEntry de = null;
            try
            {
                de = GetByPath(userPath, userName, password);

                MoveUser(de, newLocationPath, mustOu, userName, password);
            }
            finally
            {
                if (de != null)
                {
                    de.Close();
                    de.Dispose();
                }
            }
        }

        /// <summary>
        /// 移动用户DirectoryEntry到指定位置，使用默认用户身份标识。
        /// </summary>
        /// <param name="userPath">要移动的用户DirectoryEntry的ADsPath。必须是DN形式。完全转义过的。</param>
        /// <param name="newLocationPath">移动到的位置的ADsPath。必须是DN形式。完全转义过的。</param>
        /// <param name="mustOu">移动到的位置对应的DirectoryEntry是否必须是组织单位。</param>
        public static void MoveUser(string userPath, string newLocationPath, bool mustOu)
        {
            MoveUser(userPath, newLocationPath, mustOu, null, null);
        }

        /// <summary>
        /// 移动用户DirectoryEntry到指定位置。
        /// </summary>
        /// <param name="user">要移动的用户DirectoryEntry的Guid</param>
        /// <param name="newLocation">移动到的位置的Guid</param>
        /// <param name="mustOu">移动到的位置对应的DirectoryEntry是否必须是组织单位。</param>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        public static void MoveUser(Guid user, Guid newLocation, bool mustOu, string userName, string password)
        {
            MoveUser(GetUserByGuid(user).Dn, GetOUByGuid(newLocation).Dn, mustOu, userName, password);
        }

        /// <summary>
        /// 移动用户DirectoryEntry到指定位置，使用默认用户身份标识。
        /// </summary>
        /// <param name="user">要移动的用户DirectoryEntry的Guid</param>
        /// <param name="newLocation">移动到的位置的Guid</param>
        /// <param name="mustOu">移动到的位置对应的DirectoryEntry是否必须是组织单位。</param>
        public static void MoveUser(Guid user, Guid newLocation, bool mustOu)
        {
            MoveUser(GetUserByGuid(user).Dn, GetOUByGuid(newLocation).Dn, mustOu, null, null);
        }

        /// <summary>
        /// 移动用户DirectoryEntry到指定位置。
        /// </summary>
        /// <param name="de">要移动的用户DirectoryEntry对象。必须是通过DN形式路径得到的对象。</param>
        /// <param name="newLocationPath">移动到的位置的ADsPath。必须是DN形式。完全转义过的。</param>
        /// <param name="mustOu">移动到的位置对应的DirectoryEntry是否必须是组织单位。</param>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        internal static void MoveUser(DirectoryEntry de, string newLocationPath, bool mustOu, string userName, string password)
        {
            if (!Exists(newLocationPath))
                throw new EntryNotExistException("移动到的位置对象不存在。");

            DirectoryEntry newLocation = null;
            try
            {
                newLocation = GetByPath(newLocationPath, userName, password);

                if (de.SchemaClassName != SchemaClass.user.ToString("F"))
                    throw new SchemaClassException("需要被移动的对象类型不是" + SchemaClass.user.ToString("F") + "。");

                if (mustOu && newLocation.SchemaClassName != SchemaClass.organizationalUnit.ToString("F"))
                    throw new SchemaClassException("移动到的位置对象类型不是" + SchemaClass.organizationalUnit.ToString("F") + "。");

                if (Exists(Utils.GetRDNValue(de.Properties[BaseObject.PROPERTY_DN].Value.ToString()) + "," +
                    newLocation.Properties[BaseObject.PROPERTY_DN].Value.ToString()))
                    throw new SameRDNException("移动到的位置下存在同名对象。");

                de.MoveTo(newLocation);
                de.CommitChanges();
            }
            catch (InvalidOperationException ioe)   // 指定的 DirectoryEntry 不是容器。
            {
                throw new NotContainerException(ioe.Message, ioe);
            }
            catch (DirectoryServicesCOMException dsce)
            {
                throw dsce;
            }
            finally
            {
                if (newLocation != null)
                {
                    newLocation.Close();
                    newLocation.Dispose();
                }
            }
        }

        #endregion

        #region MemberOf

        /// <summary>
        /// 获取用户DirectoryEntry对象的PrimaryGroup DirectoryEntry对象。
        /// </summary>
        /// <param name="userPath">用户DirectoryEntry的ADsPath。</param>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        /// <returns>不存在返回null。</returns>
        public static DirectoryEntry GetUserPrimaryGroup(string userPath, string userName, string password)
        {
            DirectoryEntry de = GetByPath(userPath, userName, password);

            if (de == null)
                throw new EntryNotExistException("用户对象不存在。");

            if (de.SchemaClassName != SchemaClass.user.ToString("F"))
                throw new SchemaClassException("对象类型不是" + SchemaClass.user.ToString("F") + "。");

            return GetUserPrimaryGroup(de, userName, password);
        }

        /// <summary>
        /// 获取用户DirectoryEntry对象的PrimaryGroup DirectoryEntry对象，使用默认用户身份标识。
        /// </summary>
        /// <param name="userPath">用户DirectoryEntry的ADsPath。</param>
        /// <returns>不存在返回null。</returns>
        public static DirectoryEntry GetUserPrimaryGroup(string userPath)
        {
            return GetUserPrimaryGroup(userPath, null, null);
        }

        /// <summary>
        /// 获取用户DirectoryEntry对象的PrimaryGroup DirectoryEntry对象。
        /// </summary>
        /// <param name="user">用户DirectoryEntry对象。</param>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        /// <returns>不存在返回null。</returns>
        internal static DirectoryEntry GetUserPrimaryGroup(DirectoryEntry user, string userName, string password)
        {
            string primaryGroupSID = User.GeneratePrimaryGroupSID((byte[])(user.Properties[BaseObject.PROPERTY_OBJECTSID].Value),
                Convert.ToInt32(user.Properties[User.PROPERTY_MEMBEROF_PRIMARY].Value));

            return GetBySid(primaryGroupSID, userName, password);
        }

        /// <summary>
        /// 获取用户DirectoryEntry对象的隶属组的DN。
        /// </summary>
        /// <param name="userPath">用户DirectoryEntry的ADsPath。</param>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        /// <param name="includePrimaryGroup">是否包括PrimaryGroup</param>
        /// <returns>不存在返回空集合。</returns>
        public static List<string> GetUserMemberOfDN(string userPath, string userName, string password, bool includePrimaryGroup)
        {
            DirectoryEntry de = GetByPath(userPath, userName, password);

            if (de == null)
                throw new EntryNotExistException("用户对象不存在。");

            if (de.SchemaClassName != SchemaClass.user.ToString("F"))
                throw new SchemaClassException("对象类型不是" + SchemaClass.user.ToString("F") + "。");

            List<string> dn = new List<string>();

            if (includePrimaryGroup)
            {
                DirectoryEntry primary = GetUserPrimaryGroup(de, userName, password);
                if (primary != null)
                {
                    dn.Add(Utils.EscapeDNBackslashedChar(primary.Properties[BaseObject.PROPERTY_DN].Value.ToString()));

                    primary.Close();
                    primary.Dispose();
                }
            }
            if (de.Properties.Contains(User.PROPERTY_MEMBEROF_ALL))
            {
                foreach (object m in de.Properties[User.PROPERTY_MEMBEROF_ALL])
                {
                    dn.Add(Utils.EscapeDNBackslashedChar(m.ToString()));        // 转义/
                }
            }

            de.Close();
            de.Dispose();

            return dn;
        }

        /// <summary>
        /// 获取用户DirectoryEntry对象的隶属组的DN，使用默认用户身份标识。
        /// </summary>
        /// <param name="userPath">用户DirectoryEntry的ADsPath。</param>
        /// <param name="includePrimaryGroup">是否包括PrimaryGroup</param>
        /// <returns>不存在返回空集合。</returns>
        public static List<string> GetUserMemberOfDN(string userPath, bool includePrimaryGroup)
        {
            return GetUserMemberOfDN(userPath, null, null, includePrimaryGroup);
        }

        #endregion

        #endregion


        #region Group

        #region Search

        /// <summary>
        /// 获取指定所有组。
        /// </summary>
        /// <param name="cn">组CN。</param>
        /// <param name="description">组描述。</param>
        /// <param name="rootPath">根对象ADsPath，null表示整个域。</param>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        /// <returns>如果不存在，返回null。</returns>
        public static List<Group> SearchGroup(string cn, string description, string rootPath, string userName, string password)
        {
            string schema = null;
            if (!String.IsNullOrEmpty(cn) || !String.IsNullOrEmpty(description))
                schema = String.Format("(&{0}{1})", 
                    (!String.IsNullOrEmpty(cn) ? String.Format("(cn=*{0}*)", Utils.Escape4Query(cn)) : "" ),
                    (!String.IsNullOrEmpty(description) ? String.Format("(description=*{0}*)", Utils.Escape4Query(description)) : ""));

            List<DirectoryEntry> entries = Search(schema, "group", null, rootPath, SearchScope.Subtree, userName, password);
            List<Group> groups = new List<Group>();
            foreach (DirectoryEntry de in entries)
            {
                groups.Add(new Group(de));

                de.Close();
                de.Dispose();
            }

            return groups;
        }

        /// <summary>
        /// 获取指定所有组，使用默认用户身份标识。
        /// </summary>
        /// <param name="cn">组CN。</param>
        /// <param name="description">组描述。</param>
        /// <param name="rootPath">根对象ADsPath，null表示整个域。</param>
        /// <returns>如果不存在，返回null。</returns>
        public static List<Group> SearchGroup(string cn, string description, string rootPath)
        {
            return SearchGroup(cn, description, rootPath, null, null);
        }

        /// <summary>
        /// 获取指定所有组。
        /// </summary>
        /// <param name="cn">组CN。</param>
        /// <param name="description">组描述。</param>
        /// <param name="rootPath">根对象ADsPath，null表示整个域。</param>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        /// <returns>如果不存在，返回null。</returns>
        /// <remarks>包括distinguishedname,objectguid,name,description,adspath,objectcategory,objectclass。
        /// 最后添加了sAMAccountName。</remarks>
        public static List<String[]> SearchGroupSimple(string cn, string description, string rootPath, string userName, string password)
        {
            string schema = null;
            if (!String.IsNullOrEmpty(cn) || !String.IsNullOrEmpty(description))
                schema = String.Format("&{0}{1}",
                    (!String.IsNullOrEmpty(cn) ? String.Format("(cn=*{0}*)", Utils.Escape4Query(cn)) : ""),
                    (!String.IsNullOrEmpty(description) ? String.Format("(cn=*{0}*)", Utils.Escape4Query(description)) : ""));

            return Search2(schema, "group", null, rootPath, SearchScope.Subtree, userName, password);
        }

        /// <summary>
        /// 获取指定所有组，使用默认用户身份标识。
        /// </summary>
        /// <param name="cn">组CN。</param>
        /// <param name="description">组描述。</param>
        /// <param name="rootPath">根对象ADsPath，null表示整个域。</param>
        /// <returns>如果不存在，返回null。</returns>
        /// <remarks>包括distinguishedname,objectguid,name,description,adspath,objectcategory,objectclass</remarks>
        public static List<String[]> SearchGroupSimple(string cn, string description, string rootPath)
        {
            return SearchGroupSimple(cn, description, rootPath, null, null);
        }

        /// <summary>
        /// 获取指定所有组。直接解析查询结果，速度较SearchGroup快。
        /// </summary>
        /// <param name="cn">组CN。</param>
        /// <param name="description">组描述。</param>
        /// <param name="rootPath">根对象ADsPath，null表示整个域。</param>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        /// <returns>如果不存在，返回null。</returns>
        public static List<Group> SearchGroupQuick(string cn, string description, string rootPath, string userName, string password)
        {
            string schema = null;
            if (!String.IsNullOrEmpty(cn) || !String.IsNullOrEmpty(description))
                schema = String.Format("&{0}{1}",
                    (!String.IsNullOrEmpty(cn) ? String.Format("(cn=*{0}*)", Utils.Escape4Query(cn)) : ""),
                    (!String.IsNullOrEmpty(description) ? String.Format("(cn=*{0}*)", Utils.Escape4Query(description)) : ""));

            SearchResultCollection results = Search3(schema, "group", null, rootPath, SearchScope.Subtree, userName, password);

            List<Group> groups = new List<Group>();
            foreach (SearchResult se in results)
            {
                groups.Add(new Group(se));
            }

            return groups;
        }

        /// <summary>
        /// 获取指定所有组，使用默认用户身份标识。直接解析查询结果，速度较SearchGroup快。
        /// </summary>
        /// <param name="cn">组CN。</param>
        /// <param name="description">组描述。</param>
        /// <param name="rootPath">根对象ADsPath，null表示整个域。</param>
        /// <returns>如果不存在，返回null。</returns>
        public static List<Group> SearchGroupQuick(string cn, string description, string rootPath)
        {
            return SearchGroupQuick(null,null, rootPath, null, null);
        }

        /// <summary>
        /// 根据sAMAccountName获取Group。
        /// </summary>
        /// <param name="sAmAccountName">sAMAccountName。</param>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        /// <returns>如果不存在，返回null。</returns>
        public static Group GetGroupBySamAccountName(string sAmAccountName, string userName, string password)
        {
            List<DirectoryEntry> entries = Search("sAMAccountName=" + Utils.Escape4Query(sAmAccountName), 
                "group", null, null, SearchScope.Subtree, userName, password);
            if (entries.Count == 1)
            {
                DirectoryEntry de = entries[0];

                Group group = new Group(de);

                de.Close();
                de.Dispose();

                return group;
            }

            return null;
        }

        #endregion

        #region Get

        /// <summary>
        /// 根据用户的Guid得到组对象。
        /// </summary>
        /// <param name="guid">Guid</param>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        /// <returns>如果不存在，返回null。</returns>
        public static Group GetGroupByGuid(Guid guid, string userName, string password)
        {
            return GetGroupByPath(Utils.GenerateADsPath(guid), userName, password);

        }

        /// <summary>
        /// 根据用户的Guid得到组对象，使用默认用户身份标识。
        /// </summary>
        /// <param name="guid">Guid</param>
        /// <returns>如果不存在，返回null。</returns>
        public static Group GetGroupByGuid(Guid guid)
        {
            return GetGroupByGuid(guid, null,null);
        }

        /// <summary>
        /// 根据用户的DN得到用户组。
        /// </summary>
        /// <param name="dn">DN。完全转义过的。</param>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        /// <returns>如果不存在，返回null。</returns>
        public static Group GetGroupByDN(string dn, string userName, string password)
        {
            return GetGroupByPath(dn, userName, password);
        }

        /// <summary>
        /// 根据用户的DN得到组对象，使用默认用户身份标识。
        /// </summary>
        /// <param name="dn">DN。完全转义过的。</param>
        /// <returns>如果不存在，返回null。</returns>
        public static Group GetGroupByDN(string dn)
        {
            return GetGroupByDN(dn, null, null);
        }

        /// <summary>
        /// 根据用户的ADsPath得到组对象。
        /// </summary>
        /// <param name="path">ADsPath。完全转义过的。</param>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        /// <returns>如果不存在，返回null。</returns>
        public static Group GetGroupByPath(string path, string userName, string password)
        {
            DirectoryEntry entry = GetByPath(path, userName, password);
            if (entry != null)
            {
                Group group = new Group(entry);
                entry.Close();
                entry.Dispose();

                return group;
            }
            else
                return null;

            
        }

        /// <summary>
        /// 根据用户的ADsPath得到组对象，使用默认用户身份标识。
        /// </summary>
        /// <param name="path">ADsPath。完全转义过的。</param>
        /// <returns>如果不存在，返回null。</returns>
        public static Group GetGroupByPath(string path)
        {
            return GetGroupByPath(path, null, null);
        }

        #endregion

        #region Rename

        /// <summary>
        /// 更改组DirectoryEntry对象的名称。
        /// </summary>
        /// <param name="groupPath">组DirectoryEntry的ADsPath</param>
        /// <param name="newName">该项的新名称。</param>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        public static void RenameGroup(string groupPath, string newName, string userName, string password)
        {
            DirectoryEntry de = GetByPath(groupPath, userName, password);

            if (de == null)
                throw new EntryNotExistException("组对象不存在。");

            if (de.SchemaClassName != SchemaClass.group.ToString("F"))
                throw new SchemaClassException("对象类型不是" + SchemaClass.group.ToString("F") + "。");

            string dn = Utils.EscapeDNBackslashedChar(de.Properties[BaseObject.PROPERTY_DN].Value.ToString());
            string rdn = Utils.GenerateRDNCN(newName);
            if(Exists(Utils.GenerateDN(rdn, Utils.GetParentDN(dn))))
                throw new SameRDNException("已存在同名对象。");
            try
            {
                de.Rename(rdn);

                de.CommitChanges();
            }
            catch (DirectoryServicesCOMException dsce)
            {
                throw dsce;
            }
            finally
            {
                if (de != null)
                {
                    de.Close();
                    de.Dispose();
                }
            }
        }

        /// <summary>
        /// 更改组DirectoryEntry对象的名称，使用默认用户身份标识。
        /// </summary>
        /// <param name="groupPath">组DirectoryEntry的ADsPath</param>
        /// <param name="newName">该项的新名称。</param>
        public static void RenameGroup(string groupPath, string newName)
        {
            RenameGroup(groupPath, newName);
        }

        #endregion

        #region Member Change

        /// <summary>
        /// 将用户添加到组。
        /// </summary>
        /// <param name="groupPath">组DirectoryEntry的ADsPath。完全转义的。</param>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        /// <param name="userDN">需要添加的用户的DN。完全转义的。</param>
        public static void AddUserToGroup(string groupPath, string userName, string password, params string[] userDN)
        {
            DirectoryEntry de = GetByPath(groupPath, userName, password);

            if (de == null)
                throw new EntryNotExistException("组对象不存在。");

            if (de.SchemaClassName != SchemaClass.group.ToString("F"))
                throw new SchemaClassException("对象类型不是" + SchemaClass.group.ToString("F") + "。");

            // 得到已有的Member
            List<string> ms = new List<string>();
            foreach (object m in de.Properties[Group.PROPERTY_MEMBER])
            {
                ms.Add(Utils.EscapeDNBackslashedChar(m.ToString()));
            }
            ms.Sort();          // 已排序 -- 以便内部使用

            List<string> toAdd = new List<string>();
            foreach (string udn in userDN)
            {
                if (!(ms.BinarySearch(udn) >= 0))
                {
                    if (!toAdd.Exists(delegate(string a ) {return a == udn;}))
                        toAdd.Add(udn);
                }
            }

            try
            {
                foreach (string udn in toAdd)
                {
                    de.Invoke("Add", new object[] { ParaMgr.LDAP_IDENTITY + udn });         // 需要ADsPath
                }

                de.CommitChanges();
            }
            catch (DirectoryServicesCOMException dsce)
            {
                throw dsce;
            }
            finally
            {
                if (de != null)
                {
                    de.Close();
                    de.Dispose();
                }
            }
        }

        /// <summary>
        /// 将用户添加到组，使用默认用户身份标识。
        /// </summary>
        /// <param name="groupPath">组DirectoryEntry的ADsPath。完全转义的。</param>
        /// <param name="userDN">需要添加的用户的DN。</param>
        public static void AddUserToGroup(string groupPath, params string[] userDN)
        {
            AddUserToGroup(groupPath, null,null,userDN);
        }

        /// <summary>
        /// 将用户添加到组。
        /// </summary>
        /// <param name="groupPath">组DirectoryEntry的ADsPath。完全转义的。</param>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        /// <param name="userGuid">需要添加的用户的Guid。</param>
        public static void AddUserToGroup(string groupPath, string userName, string password, params Guid[] userGuid)
        {
            List<string> userDN = new List<string>();
            User user = null;
            foreach(Guid guid in userGuid)
            {
                user = GetUserByGuid(guid);
                if (user != null)
                {
                    userDN.Add(user.Dn);
                }
            }

            AddUserToGroup(groupPath, userName, password, userDN.ToArray());
        }

        /// <summary>
        /// 将用户添加到组，使用默认用户身份标识。
        /// </summary>
        /// <param name="groupPath">组DirectoryEntry的ADsPath。完全转义的。</param>
        /// <param name="userGuid">需要添加的用户的Guid。</param>
        public static void AddUserToGroup(string groupPath, params Guid[] userGuid)
        {
            AddUserToGroup(groupPath, null, null, userGuid);
        }

        /// <summary>
        /// 将用户从组中移除。
        /// </summary>
        /// <param name="groupPath">组DirectoryEntry的ADsPath。完全转义的。</param>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        /// <param name="userDN">需要移除的用户的DN。完全转义的。</param>
        public static void RemoveUserFromGroup(string groupPath, string userName, string password, params string[] userDN)
        {
            DirectoryEntry de = GetByPath(groupPath, userName, password);

            if (de == null)
                throw new EntryNotExistException("组对象不存在。");

            if (de.SchemaClassName != SchemaClass.group.ToString("F"))
                throw new SchemaClassException("对象类型不是" + SchemaClass.group.ToString("F") + "。");

            // 得到已有的Group
            List<string> ms = new List<string>();
            foreach (object m in de.Properties[Group.PROPERTY_MEMBER])
            {
                ms.Add(Utils.EscapeDNBackslashedChar(m.ToString()));
            }
            ms.Sort();          // 已排序 -- 以便内部使用

            List<string> toRemove = new List<string>();
            foreach (string udn in userDN)
            {
                if (ms.BinarySearch(udn) >= 0)
                {
                    if (!toRemove.Exists(delegate(string a) { return a == udn; }))
                        toRemove.Add(udn);
                }
            }

            try
            {
                foreach (string udn in toRemove)
                {
                    de.Invoke("Remove", new object[] { ParaMgr.LDAP_IDENTITY + udn });         // 需要ADsPath
                }

                //de.Invoke("Remove", userDN);        // TODO:是否需要保留转义的/，是否需要ADsPath，like AddUserToGroup

                de.CommitChanges();
            }
            catch (DirectoryServicesCOMException dsce)
            {
                throw dsce;
            }
            finally
            {
                if (de != null)
                {
                    de.Close();
                    de.Dispose();
                }
            }
        }

        /// <summary>
        /// 将用户从组中移除，使用默认用户身份标识。
        /// </summary>
        /// <param name="groupPath">组DirectoryEntry的ADsPath。完全转义的。</param>
        /// <param name="userDN">需要移除的用户的DN。</param>        
        public static void RemoveUserFromGroup(string groupPath, params string[] userDN)
        {
            RemoveUserFromGroup(groupPath, null,null,userDN);
        }

        /// <summary>
        /// 将用户从组中移除。
        /// </summary>
        /// <param name="groupPath">组DirectoryEntry的ADsPath。完全转义的。</param>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        /// <param name="userGuid">需要移除的用户的Guid。</param>
        public static void RemoveUserFromGroup(string groupPath, string userName, string password, params Guid[] userGuid)
        {
            List<string> userDN = new List<string>();
            User user = null;
            foreach(Guid guid in userGuid)
            {
                user = GetUserByGuid(guid);
                if (user != null)
                {
                    userDN.Add(user.Dn);
                }
            }

            RemoveUserFromGroup(groupPath, userName, password, userDN.ToArray());
        }

        /// <summary>
        /// 将用户从组中移除，使用默认用户身份标识。
        /// </summary>
        /// <param name="groupPath">组DirectoryEntry的ADsPath。完全转义的。</param>
        /// <param name="userGuid">需要移除的用户的Guid。</param>
        public static void RemoveUserFromGroup(string groupPath, params Guid[] userGuid)
        {
            RemoveUserFromGroup(groupPath, null, null, userGuid);
        }

        #endregion

        #region MemberOf & Member

        /// <summary>
        /// 获取组的隶属组的DN
        /// </summary>
        /// <param name="groupPath">组DirectoryEntry的ADsPath。完全转义的。</param>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        /// <returns></returns>
        public static List<string> GetGroupMemberOfDN(string groupPath, string userName, string password)
        {
            DirectoryEntry de = GetByPath(groupPath, userName, password);

            if (de == null)
                throw new EntryNotExistException("组对象不存在。");

            if (de.SchemaClassName != SchemaClass.group.ToString("F"))
                throw new SchemaClassException("对象类型不是" + SchemaClass.group.ToString("F") + "。");

            List<string> dn = new List<string>();
            if (de.Properties.Contains(Group.PROPERTY_MEMBEROF))
            {
                foreach (object m in de.Properties[Group.PROPERTY_MEMBEROF])
                {
                    dn.Add(Utils.EscapeDNBackslashedChar(m.ToString()));
                }
            }

            de.Close();
            de.Dispose();

            return dn;
        }

        /// <summary>
        /// 获取组的隶属组的DN，使用默认用户身份标识。
        /// </summary>
        /// <param name="groupPath">组DirectoryEntry的ADsPath。完全转义的。</param>
        /// <returns></returns>
        public static List<string> GetGroupMemberOfDN(string groupPath)
        {
            return GetGroupMemberOfDN(groupPath, null, null);
        }

        /// <summary>
        /// 获取组的成员（仅用户）
        /// </summary>
        /// <param name="groupPath">组DirectoryEntry的ADsPath。完全转义的。</param>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        /// <returns></returns>
        public static List<User> GetGroupUserMember(string groupPath, string userName, string password)
        {
            DirectoryEntry de = GetByPath(groupPath, userName, password);

            if (de == null)
                throw new EntryNotExistException("组对象不存在。");

            if (de.SchemaClassName != SchemaClass.group.ToString("F"))
                throw new SchemaClassException("对象类型不是" + SchemaClass.group.ToString("F") + "。");

            List<User> users = new List<User>();
            string userSchemaClassName = SchemaClass.user.ToString("F");

            if (de.Properties.Contains(Group.PROPERTY_MEMBER))
            {
                foreach (object memberDN in de.Properties[Group.PROPERTY_MEMBER])
                {
                    de = GetByDN(Utils.EscapeDNBackslashedChar(memberDN.ToString()), userName, password);

                    if (de != null)
                    {
                        if (de.SchemaClassName == userSchemaClassName)
                        {
                            users.Add(new User(de));
                        }

                        de.Close();
                        de.Dispose();
                    }
                }
            }

            return users;
        }

        /// <summary>
        /// 获取组的成员（仅用户），使用默认用户身份标识。
        /// </summary>
        /// <param name="groupPath">组DirectoryEntry的ADsPath。完全转义的。</param>
        /// <returns></returns>
        public static List<User> GetGroupUserMember(string groupPath)
        {
            return GetGroupUserMember(groupPath, null, null);
        }

        #endregion

        #endregion


        #region OU

        #region Search

        /// <summary>
        /// 获取指定所有组织单位。
        /// </summary>
        /// <param name="rootPath">根对象ADsPath，null表示整个域。</param>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        /// <returns>如果不存在，返回null。</returns>
        public static List<OU> GetOUAll(string rootPath, string userName, string password)
        {
            List<DirectoryEntry> entries = Search(null, "organizationalUnit", null, rootPath, SearchScope.Subtree, userName, password);
            List<OU> ous = new List<OU>();
            foreach (DirectoryEntry de in entries)
            {
                ous.Add(new OU(de));

                de.Close();
                de.Dispose();
            }

            return ous;
        }

        /// <summary>
        /// 获取指定所有组织单位，使用默认用户身份标识。
        /// </summary>
        /// <param name="rootPath">根对象ADsPath，null表示整个域。</param>
        /// <returns>如果不存在，返回null。</returns>
        public static List<OU> GetOUAll(string rootPath)
        {
            return GetOUAll(rootPath, null, null);
        }

        /// <summary>
        /// 获取指定所有组织单位。
        /// </summary>
        /// <param name="rootPath">根对象ADsPath，null表示整个域。</param>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        /// <returns>如果不存在，返回null。</returns>
        /// <remarks>包括distinguishedname,objectguid,name,description,adspath,objectcategory,objectclass</remarks>
        public static List<String[]> GetOUAllSimple(string rootPath, string userName, string password)
        {
            return Search2(null, "organizationalUnit", null, rootPath, SearchScope.Subtree, userName, password);
        }

        /// <summary>
        /// 获取指定所有组织单位，使用默认用户身份标识。
        /// </summary>
        /// <param name="rootPath">根对象ADsPath，null表示整个域。</param>
        /// <returns>如果不存在，返回null。</returns>
        /// <remarks>包括distinguishedname,objectguid,name,description,adspath,objectcategory,objectclass</remarks>
        public static List<String[]> GetOUAllSimple(string rootPath)
        {
            return GetOUAllSimple(rootPath, null, null);
        }

        /// <summary>
        /// 获取指定所有组织单位。直接解析查询结果，速度较GetUserAll快。
        /// </summary>
        /// <param name="rootPath">根对象ADsPath，null表示整个域。</param>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        /// <returns>如果不存在，返回null。</returns>
        public static List<OU> GetOUAllQuick(string rootPath, string userName, string password)
        {
            SearchResultCollection results = Search3(null, "organizationalUnit", null, rootPath, SearchScope.Subtree, userName, password);

            List<OU> ous = new List<OU>();
            foreach (SearchResult se in results)
            {
                ous.Add(new OU(se));
            }

            return ous;
        }

        /// <summary>
        /// 获取指定所有组织单位，使用默认用户身份标识。直接解析查询结果，速度较GetUserAll快。
        /// </summary>
        /// <param name="rootPath">根对象ADsPath，null表示整个域。</param>
        /// <returns>如果不存在，返回null。</returns>
        public static List<OU> GetOUAllQuick(string rootPath)
        {
            return GetOUAllQuick(rootPath, null, null);
        }

        #endregion

        #region Get

        /// <summary>
        /// 根据组织单位的Guid得到组织单位对象。
        /// </summary>
        /// <param name="guid">Guid</param>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        /// <returns>如果不存在，返回null。</returns>
        public static OU GetOUByGuid(Guid guid, string userName, string password)
        {
            return GetOUByPath(Utils.GenerateADsPath(guid), userName, password);
        }

        /// <summary>
        /// 根据组织单位的Guid得到组织单位对象，使用默认用户身份标识。
        /// </summary>
        /// <param name="guid">Guid</param>
        /// <returns>如果不存在，返回null。</returns>
        public static OU GetOUByGuid(Guid guid)
        {
            return GetOUByGuid(guid, null, null);
        }

        /// <summary>
        /// 根据组织单位的DN得到组织单位对象。
        /// </summary>
        /// <param name="dn">DN。完全转义过的。</param>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        /// <returns>如果不存在，返回null。</returns>
        public static OU GetOUByDN(string dn, string userName, string password)
        {
            return GetOUByPath(dn, userName, password);
        }

        /// <summary>
        /// 根据组织单位的DN得到组织单位对象，使用默认用户身份标识。
        /// </summary>
        /// <param name="dn">DN。完全转义过的。</param>
        /// <returns>如果不存在，返回null。</returns>
        public static OU GetOUByDN(string dn)
        {
            return GetOUByDN(dn, null, null);
        }

        /// <summary>
        /// 根据组织单位的ADsPath得到组织单位对象。
        /// </summary>
        /// <param name="path">ADsPath。完全转义过的。</param>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        /// <returns>如果不存在，返回null。</returns>
        public static OU GetOUByPath(string path, string userName, string password)
        {
            DirectoryEntry entry = GetByPath(path, userName, password);
            if (entry != null)
            {
                OU ou = new OU(entry);
                entry.Close();
                entry.Dispose();

                return ou;
            }
            else
                return null;
        }

        /// <summary>
        /// 根据组织单位的ADsPath得到组织单位对象，使用默认用户身份标识。
        /// </summary>
        /// <param name="path">ADsPath。完全转义过的。</param>
        /// <returns>如果不存在，返回null。</returns>
        public static OU GetOUByPath(string path)
        {
            return GetOUByPath(path, null, null);
        }

        #endregion

        #region Rename

        /// <summary>
        /// 更改组织单位DirectoryEntry对象的名称。
        /// </summary>
        /// <param name="ouPath">组织单位DirectoryEntry的ADsPath。必须是DN形式，且完全转义。</param>
        /// <param name="newName">该项的新名称。</param>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        public static void RenameOU(string ouPath, string newName, string userName, string password)
        {
            DirectoryEntry de = GetByPath(ouPath, userName, password);

            if (de == null)
                throw new EntryNotExistException("组织单位对象不存在。");

            if (de.SchemaClassName != SchemaClass.organizationalUnit.ToString("F"))
                throw new SchemaClassException("对象类型不是" + SchemaClass.organizationalUnit.ToString("F") + "。");

            string dn = Utils.EscapeDNBackslashedChar(de.Properties[BaseObject.PROPERTY_DN].Value.ToString());
            string rdn = Utils.GenerateRDNOU(newName);
            if (Exists(Utils.GenerateDN(rdn, Utils.GetParentDN(dn))))
                throw new SameRDNException("已存在同名对象。");
            try
            {
                de.Rename(rdn);

                de.CommitChanges();
            }
            catch (DirectoryServicesCOMException dsce)
            {
                throw dsce;
            }
            finally
            {
                de.Close();
                de.Dispose();
            }
        }

        /// <summary>
        /// 更改组DirectoryEntry对象的名称，使用默认用户身份标识。
        /// </summary>
        /// <param name="ouPath">组织单位DirectoryEntry的ADsPath。必须是DN形式，且完全转义。</param>
        /// <param name="newName">该项的新名称。</param>
        public static void RenameOU(string ouPath, string newName)
        {
            RenameOU(ouPath, newName, null, null);
        }

        /// <summary>
        /// 更改组织单位DirectoryEntry对象的名称。
        /// </summary>
        /// <param name="ouGuid">组织单位DirectoryEntry的Guid</param>
        /// <param name="newName">该项的新名称。</param>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        public static void RenameOU(Guid ouGuid, string newName, string userName, string password)
        {
            RenameOU(ADManager.GetOUByGuid(ouGuid).Dn, newName, userName, password);
        }

        /// <summary>
        /// 更改组织单位DirectoryEntry对象的名称，使用默认用户身份标识。
        /// </summary>
        /// <param name="ouGuid">组织单位DirectoryEntry的ADsPath</param>
        /// <param name="newName">该项的新名称。</param>
        public static void RenameOU(Guid ouGuid, string newName)
        {
            RenameOU(Utils.GenerateADsPath(ouGuid), newName, null, null);
        }

        #endregion

        #region Move

        /// <summary>
        /// 移动组织单位DirectoryEntry到指定位置。
        /// </summary>
        /// <param name="ouPath">要移动的组织单位DirectoryEntry的ADsPath。必须是DN形式，且完全转义。</param>
        /// <param name="newLocationPath">移动到的位置的ADsPath。必须是DN形式，且完全转义。</param>
        /// <param name="mustOu">移动到的位置对应的DirectoryEntry是否必须是组织单位。</param>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        public static void MoveOU(string ouPath, string newLocationPath, bool mustOu, string userName, string password)
        {
            if (!Exists(ouPath))
                throw new EntryNotExistException("需要被移动的对象不存在。");

            DirectoryEntry de = null;
            try
            {
                de = GetByPath(ouPath, userName, password);

                MoveOU(de, newLocationPath, mustOu, userName, password);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (de != null)
                {
                    de.Close();
                    de.Dispose();
                }
            }
        }

        /// <summary>
        /// 移动组织单位DirectoryEntry到指定位置，使用默认用户身份标识。
        /// </summary>
        /// <param name="ouPath">要移动的组织单位DirectoryEntry的ADsPath</param>
        /// <param name="newLocationPath">移动到的位置的ADsPath</param>
        /// <param name="mustOu">移动到的位置对应的DirectoryEntry是否必须是组织单位。</param>
        public static void MoveOU(string ouPath, string newLocationPath, bool mustOu)
        {
            MoveUser(ouPath, newLocationPath, mustOu, null, null);
        }

        /// <summary>
        /// 移动组织单位DirectoryEntry到指定位置。
        /// </summary>
        /// <param name="ou">要移动的组织单位DirectoryEntry的Guid</param>
        /// <param name="newLocation">移动到的位置的Guid</param>
        /// <param name="mustOu">移动到的位置对应的DirectoryEntry是否必须是组织单位。</param>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        public static void MoveOU(Guid ou, Guid newLocation, bool mustOu, string userName, string password)
        {
            MoveUser(ADManager.GetOUByGuid(ou).Dn,
               ADManager.GetOUByGuid(newLocation).Dn, mustOu, userName, password);
        }

        /// <summary>
        /// 移动组织单位DirectoryEntry到指定位置，使用默认用户身份标识。
        /// </summary>
        /// <param name="ou">要移动的组织单位DirectoryEntry的Guid</param>
        /// <param name="newLocationPath">移动到的位置的Guid</param>
        /// <param name="mustOu">移动到的位置对应的DirectoryEntry是否必须是组织单位。</param>
        public static void MoveOU(Guid ou, Guid newLocationPath, bool mustOu)
        {
            MoveUser(ou, newLocationPath, mustOu, null, null);
        }

        /// <summary>
        /// 移动组织单位DirectoryEntry到指定位置。
        /// </summary>
        /// <param name="de">要移动的组织单位DirectoryEntry对象</param>
        /// <param name="newLocationPath">移动到的位置的ADsPath</param>
        /// <param name="mustOu">移动到的位置对应的DirectoryEntry是否必须是组织单位。</param>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        internal static void MoveOU(DirectoryEntry de, string newLocationPath, bool mustOu, string userName, string password)
        {
            if (!Exists(newLocationPath))
                throw new EntryNotExistException("移动到的位置对象不存在。");

            DirectoryEntry newLocation = null;
            try
            {
                newLocation = GetByPath(newLocationPath, userName, password);

                if (de.SchemaClassName != SchemaClass.organizationalUnit.ToString("F"))
                    throw new SchemaClassException("需要被移动的对象类型不是" + SchemaClass.organizationalUnit.ToString("F") + "。");

                if (mustOu && newLocation.SchemaClassName != SchemaClass.organizationalUnit.ToString("F"))
                    throw new SchemaClassException("移动到的位置对象类型不是" + SchemaClass.organizationalUnit.ToString("F") + "。");

                if (Exists(Utils.GetRDNValue(de.Properties[BaseObject.PROPERTY_DN].Value.ToString()) + "," +
                    newLocation.Properties[BaseObject.PROPERTY_DN].Value.ToString()))
                    throw new SameRDNException("移动到的位置下存在同名对象。");

                de.MoveTo(newLocation);
                de.CommitChanges();
            }
            catch (InvalidOperationException ioe)   // 指定的 DirectoryEntry 不是容器。
            {
                throw new NotContainerException(ioe.Message, ioe);
            }
            catch (DirectoryServicesCOMException dsce)
            {
                throw dsce;
            }
            finally
            {
                if (newLocation != null)
                {
                    newLocation.Close();
                    newLocation.Dispose();
                }
            }
        }

        #endregion

        #region Structure

        /// <summary>
        /// 获取组织单位子树。
        /// </summary>
        /// <param name="ouGuid">组织单位DirectoryEntry的Guid</param>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        /// <returns></returns>
        public OU GetOUSubTree(Guid ouGuid, string userName, string password)
        {
            OU ou = GetOUByGuid(ouGuid);

            if (ou == null)
                throw new EntryNotExistException("组织单位对象不存在。");

            return ou.GetSubTree(userName, password);
        }

        /// <summary>
        /// 获取组织单位子树，使用默认用户身份标识。
        /// </summary>
        /// <param name="ouGuid">组织单位DirectoryEntry的Guid</param>
        /// <returns></returns>
        public OU GetOUSubTree(Guid ouGuid)
        {
            return GetOUSubTree(ouGuid, null, null);
        }

        /// <summary>
        /// 获取组织单位子组织单位。
        /// </summary>
        /// <param name="ouGuid">组织单位DirectoryEntry的Guid</param>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        /// <returns></returns>
        public List<OU> GetOUChildren(Guid ouGuid, string userName, string password)
        {
            OU ou = GetOUByGuid(ouGuid);

            if (ou == null)
                throw new EntryNotExistException("组织单位对象不存在。");

            return ou.GetChildren(userName, password);
        }

        /// <summary>
        /// 获取组织单位子组织单位，使用默认用户身份标识。
        /// </summary>
        /// <param name="ouGuid">组织单位DirectoryEntry的Guid</param>
        /// <returns></returns>
        public List<OU> GetOUChildren(Guid ouGuid)
        {
            return GetOUChildren(ouGuid, null, null);
        }

        /// <summary>
        /// 获取组织单位父组织单位。
        /// </summary>
        /// <param name="ouGuid">组织单位DirectoryEntry的Guid</param>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        /// <returns></returns>
        public OU GetOUParent(Guid ouGuid, string userName, string password)
        {
            OU ou = GetOUByGuid(ouGuid);

            if (ou == null)
                throw new EntryNotExistException("组织单位对象不存在。");

            return ou.GetParent(userName, password);
        }

        /// <summary>
        /// 获取组织单位父组织单位，使用默认用户身份标识。
        /// </summary>
        /// <param name="ouGuid">组织单位DirectoryEntry的Guid</param>
        /// <returns></returns>
        public OU GetOUParent(Guid ouGuid)
        {
            return GetOUParent(ouGuid, null, null);
        }

        #endregion

        #endregion


        /// <summary>
        /// 通过ADsPath获取对象。目前仅限User,OU和Group
        /// </summary>
        /// <param name="path">ADsPath。完全转义过的。</param>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        /// <returns>如果不存在，返回null。</returns>
        public static BaseObject GetObjectByPath(string path, string userName, string password)
        {
            BaseObject baseObject = null;
            DirectoryEntry entry = GetByPath(path, userName, password);
            if (entry != null)
            {
                SchemaClass schema = SchemaClass.none;
                try
                {
                    schema = (SchemaClass)(System.Enum.Parse(typeof(SchemaClass), entry.SchemaClassName));
                    switch (schema)
                    {
                        case SchemaClass.user:
                            baseObject = new User(entry);
                            break;
                        case SchemaClass.group:
                            baseObject = new Group(entry);
                            break;
                        case SchemaClass.organizationalUnit:
                            baseObject = new OU(entry);
                            break;
                    }
                }
                catch
                { }
                
                entry.Close();
                entry.Dispose();

                return baseObject;
            }
            else
                return null;
        }

        /// <summary>
        /// 指定的SAMAccountName用户或组是否存在。
        /// </summary>
        /// <param name="sAmAccountName">sAMAccountName</param>
        /// <param name="an">如果存在，对应的sAMAccountName。</param>
        /// <param name="dn">如果存在，对应的DN。</param>
        /// <param name="precision">true表示完全匹配，false表示前向匹配。</param>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        /// <returns>如果不存在，返回false。</returns>
        public static bool SamAccountNameExists(string sAmAccountName, out string an, out string dn, bool precision,
            string userName, string password)
        {
            an = null;
            dn = null;
            List<DirectoryEntry> entries = Search("sAMAccountName=" + Utils.Escape4Query(sAmAccountName) + "*", null, null, null, SearchScope.Subtree, userName, password);
            if (entries.Count >= 1)
            {
                string schemaClassName = entries[0].SchemaClassName;
                bool valid = ((schemaClassName == SchemaClass.group.ToString("F")) || (schemaClassName == SchemaClass.user.ToString("F")));

                if (valid)
                {
                    an = entries[0].Properties["sAMAccountName"].Value.ToString();
                    if ((precision && (an == sAmAccountName)) || (!precision))
                    {
                        dn = Utils.EscapeDNBackslashedChar(entries[0].Properties[BaseObject.PROPERTY_DN].Value.ToString());
                    }
                    else
                    {
                        an = null;
                        valid = false;
                    }
                    
                }

                entries[0].Close();
                entries[0].Dispose();

                return valid;
            }

            return false;
        }


        #region special

//        /// <summary>
//        /// 获取组和成员的对应关系
//        /// </summary>
//        /// <returns>字典,组的标识符作为键,对应的用户作为值</returns>
//        public static Dictionary<string, List<User>>  GetGroupMembers()
//        {
//            return null;

//            // 应当得到组关系树,然后从根开始处理
//            // 
//            /*
//reuslt : dir -- user

//progress :
//foreach dic
//    get group with perm
//    foreach group
//        get group member
//        save dir -- member(user)


//how to get dic perm group



//how to get group member 
//can get all user
//can get all group
//can get uesr belong group
//can get group belong group

//so create a dictionary,group is key,user is value -- result

//first,get all user -->save in a dic,guid is key(sid?)
//second,get all group-->save in a dic,guid is key(sid?)
//third,foreach user get user belong group, save in result dic
//forth,foreach group get group belong group, get saved group member,then save in result dic
//    -- must from base group--how?
//    -- with a tree, save group belong relation,process from root



//todo,filter user who is not in db        
//             */
//        }

        #endregion

    }
}
