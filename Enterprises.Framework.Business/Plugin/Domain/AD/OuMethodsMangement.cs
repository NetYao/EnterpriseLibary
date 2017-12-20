using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enterprises.Framework.Plugin.Domain.AD
{
    /// <summary>
    /// OU组织单元管理
    /// </summary>
    public class OuMethodsMangement
    {
        /// <summary>
        /// 是否存在OU
        /// </summary>
        /// <param name="path">LDAP://10.45.9.11/ou=离职员工,dc=ops,dc=net</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public bool IsExistsOu(string path,AdAdminConfig config)
        {
            DirectoryEntry directoryEntry = new DirectoryEntry(path);
            directoryEntry.Username = config.AdminAccount;
            directoryEntry.Password = config.AdminPwd;

            bool exists = false;
            // Validate with Guid
            try
            {
                var tmp = directoryEntry.Guid;
                exists = true;
            }
            catch (Exception ex)
            {
                exists = false;
            }

            directoryEntry.Close();
            return exists;
        }

        /// <summary>
        ///  在已知ou下面创建下级ou
        /// </summary>
        /// <param name="ouParentPath">LDAP://10.45.9.11/ou=离职员工,dc=ops,dc=net</param>
        /// <param name="newOu">增加ou信息（Name 必须赋值）</param>
        /// <param name="config">管理员密码配置</param>
        public bool CreateOuChilren(string ouParentPath,OuEntity newOu,AdAdminConfig config)
        {
            try
            {
                DirectoryEntry deBase = new DirectoryEntry(ouParentPath, config.AdminAccount, config.AdminPwd);
                DirectorySearcher ouSrc = new DirectorySearcher(deBase);
                //ouSrc.PropertiesToLoad.Add("ou");
                ouSrc.Filter = $"(OU={newOu.Name})";
                ouSrc.SearchScope = SearchScope.Subtree;
                SearchResult srOu = ouSrc.FindOne();
                if (srOu == null)
                {
                    /* OU Creation
                     */
                    DirectoryEntry anOu = deBase.Children.Add($"OU={newOu.Name}", "organizationalUnit");
                    if (!string.IsNullOrEmpty(newOu.Descrption))
                    {
                        anOu.Properties["description"].Value =newOu.Descrption;
                    }

                    anOu.CommitChanges();
                    anOu.Close();
                    deBase.Close();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
                
            }

            return false;
        }

        /// <summary>
        ///  在已知ou下面创建下级ou
        /// </summary>
        /// <param name="ouParentPath">LDAP://10.45.9.11/ou=离职员工,dc=ops,dc=net</param>
        /// <param name="newOuName">增加ou信息（Name 必须赋值）</param>
        /// <param name="config">管理员密码配置</param>
        public bool CreateOuChilren(string ouParentPath, string newOuName, AdAdminConfig config)
        {
            try
            {
                DirectoryEntry deBase = new DirectoryEntry(ouParentPath, config.AdminAccount, config.AdminPwd);
                DirectorySearcher ouSrc = new DirectorySearcher(deBase);
                //ouSrc.PropertiesToLoad.Add("ou");
                ouSrc.Filter = $"(OU={newOuName})";
                ouSrc.SearchScope = SearchScope.Subtree;
                SearchResult srOu = ouSrc.FindOne();
                if (srOu == null)
                {
                    /* OU Creation
                     */
                    DirectoryEntry anOu = deBase.Children.Add($"OU={newOuName}", "organizationalUnit");
                    anOu.CommitChanges();
                    anOu.Close();
                    deBase.Close();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;

            }

            return false;
        }

        /// <summary>
        /// 根据ou路径判断路径是否创建，递归创建OU tree
        /// oupath=LDAP://10.45.9.11/ou=HR,ou=离职员工,ou=allusers,dc=ops,dc=net
        /// 会创主机建出 ou=allusers,ou=离职员工,ou=hr
        /// </summary>
        /// <param name="ouPath">LDAP://10.45.9.11/ou=离职员工,dc=ops,dc=net</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public bool CreateOu(string ouPath, AdAdminConfig config)
        {
            try
            {
                var paths = GetOuLevels(ouPath);
                int count = paths.Count;
                if (count < 1) return false;
                var notExistsIndex = count - 1;
                var currentPath = paths[notExistsIndex];
                if (!IsExistsOu(currentPath, config))
                {
                    GetExistsLevalIndex(paths, config, ref notExistsIndex);
                    var parentOuPath = paths[notExistsIndex];
                    for (int i= notExistsIndex+1;i<count;i++)
                    {
                        var itemPath = paths[i];
                        var ouName = GetLastOuName(itemPath);
                        var ouEntity=new OuEntity {Name = ouName};
                        var isSuccess=CreateOuChilren(parentOuPath, ouEntity, config);
                        if (!isSuccess)
                        {
                            return false;
                        }

                        parentOuPath = itemPath;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.Write(ex);
                return false;
            }
        }

        /// <summary>
        /// 移动Ldap 
        /// </summary>
        /// <param name="ldapPath">必须是带用户的Ldap路径 如：LDAP://10.45.9.11/cn=Yong Luo,ou=110_Marketing,ou=pphbh.com,dc=ops,dc=net</param>
        /// <param name="toLdapPath">必须OU路径：如：LDAP://10.45.9.11/ou=110_Marketing,ou=离职员工,dc=ops,dc=net</param>
        /// <param name="newName">新的OU名称</param>
        /// <param name="config">配置文件</param>
        /// <returns></returns>
        public bool MoveTo(string ldapPath,string toLdapPath, string newName,AdAdminConfig config)
        {
            try
            {
                var eLocation = new DirectoryEntry(ldapPath, config.AdminAccount, config.AdminPwd);
                if (CreateOu(toLdapPath, config))
                {
                    var nLocation = new DirectoryEntry(toLdapPath, config.AdminAccount, config.AdminPwd);
                    nLocation.AuthenticationType = AuthenticationTypes.Secure;
                    if (string.IsNullOrEmpty(newName))
                    {
                        newName = eLocation.Name;
                    }

                    eLocation.MoveTo(nLocation, newName);
                    nLocation.Close();
                    return true;
                }

                eLocation.Close();
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
            
        }



        /// <summary>
        /// 获取存在的ou在数组中的序号
        /// </summary>
        /// <param name="paths"></param>
        /// <param name="config"></param>
        /// <param name="notExistsIndex"></param>
        private void GetExistsLevalIndex(List<string> paths, AdAdminConfig config, ref int notExistsIndex)
        {
            notExistsIndex = notExistsIndex - 1;
            if (notExistsIndex < 0)
            {
                return;
            }

            var currentPath = paths[notExistsIndex];
            if (!IsExistsOu(currentPath, config))
            {
                GetExistsLevalIndex(paths, config,ref notExistsIndex);
            }
        }

        /// <summary>
        /// 重新OU最后一个名称 
        /// 如：path=LDAP://10.45.9.11/ou=hr,ou=离职员工,dc=ops,dc=net
        /// 执行Rename(path,"RD",config)
        /// 结果：LDAP://10.45.9.11/ou=RD,ou=离职员工,dc=ops,dc=net
        /// </summary>
        /// <param name="path">如：LDAP://10.45.9.11/ou=离职员工,dc=ops,dc=net </param>
        /// <param name="newName">Ou名称</param>
        /// <param name="config"></param>
        public string Rename(string path,string newName,AdAdminConfig config)
        {
            var de = new DirectoryEntry(path, config.AdminAccount, config.AdminPwd);
            de.Rename($"OU={newName}");
            var result= de.Path;
            de.Close();
            return result;
        }

        #region OU 相关帮助类
        /// <summary>
        /// 获取根容器
        /// 如：LDAP://10.45.9.11/ou=离职员工,ou=AllUsers,dc=ops,dc=net 得到 dc=ops,dc=net
        /// </summary>
        /// <param name="ldapFullPath"></param>
        /// <returns></returns>
        public string GetRootContainer(string ldapFullPath)
        {
            var rootDn = string.Empty;
            foreach (var element in ldapFullPath.Split(','))
            {
                if (element.ToLowerInvariant().StartsWith("DC=".ToLowerInvariant()))
                {
                    rootDn += element + ",";
                }
            }

            return rootDn.Substring(0, rootDn.Length - 1);

        }

        /// <summary>
        /// 获取DC的容器 去除LDAP://xxx.xx/
        /// 如：LDAP://10.45.9.11/ou=离职员工,ou=AllUsers,dc=ops,dc=net 得到 ou=AllUsers,dc=ops,dc=net
        /// 
        /// </summary>
        /// <param name="ldapFullPath"></param>
        /// <returns>ou=AllUsers,dc=ops,dc=net</returns>
        public string GetContainer(string ldapFullPath)
        {
            var cn = ldapFullPath.Split(',')[0];
            cn += ",";
            return ldapFullPath.Replace(cn, string.Empty);
        }

        /// <summary>
        /// 获取DC的容器 去除LDAP://xxx.xx/
        /// 如：LDAP://10.45.9.11/ou=离职员工,ou=AllUsers,dc=ops,dc=net 得到 离职员工
        /// 
        /// </summary>
        /// <param name="ldapFullPath"></param>
        /// <returns>离职员工</returns>
        public string GetLastOuName(string ldapFullPath)
        {
            var cn = ldapFullPath.Split(',')[0];
            var index = cn.IndexOf("ou", StringComparison.CurrentCultureIgnoreCase);
            if (index > 0)
            {
                var findex = cn.IndexOf("=", StringComparison.CurrentCultureIgnoreCase);
                return cn.Substring(findex+1);
            }

            return "";
        }

        /// <summary>
        /// 获取DC的容器 去除LDAP://xxx.xx/
        /// 如：LDAP://10.45.9.11/ou=离职员工,ou=AllUsers,dc=ops,dc=net 得到 LDAP://10.45.9.11/
        /// 
        /// </summary>
        /// <param name="ldapFullPath"></param>
        /// <returns></returns>
        public string GetLdapDomain(string ldapFullPath)
        {
            var lastIndex = ldapFullPath.LastIndexOf('/');
            return ldapFullPath.Substring(0, lastIndex + 1);
        }

        /// <summary>
        /// 根据一个ou 路径获取从根到现在的所有ou路径
        /// </summary>
        /// <param name="ouLdapPath"></param>
        /// <returns></returns>
        public List<string> GetOuLevels(string ouLdapPath)
        {
            List<string> reulst = new List<string>();
            reulst.Add(ouLdapPath);
            var domain = GetLdapDomain(ouLdapPath);
            var rootPath = GetRootContainer(ouLdapPath);
            var parentPath = GetContainer(ouLdapPath);
            if (parentPath.ToLower() != rootPath.ToLower())
            {
                reulst.Insert(0, domain + parentPath);
                GetSubOuLevel(parentPath, rootPath, domain, reulst);
            }

            reulst.Insert(0, domain + rootPath);
            return reulst;
        }

        private void GetSubOuLevel(string ouLdapPath, string rootPath,string ldapDomain, List<string> reulst)
        {
            if (ouLdapPath.ToLower() == rootPath.ToLower()) return;
            var parentPath = GetContainer(ouLdapPath);
            if (parentPath.ToLower() == rootPath.ToLower()) return;
            reulst.Insert(0, ldapDomain + parentPath);
            GetSubOuLevel(parentPath, rootPath, ldapDomain, reulst);
        }

        /// <summary>
        /// 将CanonicalName路径转换为Ldap路径
        /// </summary>
        /// <param name="path">CanonicalName 如：ops.net/pphbh/HR</param>
        /// <param name="serverIpOrDomain">10.45.9.11 or dc1.ops.net</param>
        /// <returns>Ldap://xxx/ou=HR,ou=pphbh,dc=ops,dc=net</returns>
        public string FilePathConverToLdapPath(string path,string serverIpOrDomain)
        {
            string[] ouNames = path.Split('/');
            string[] dcNames = ouNames[0].Split('.');
           
            var dc = dcNames.Aggregate("", (current, item) => current + ("dc="+item.ToString() + ","));
            dc = dc.Trim(',');

            var len = ouNames.Length;
            var ou = "";
            for (int i = len-1; i > 0; i--)
            {
                ou += "ou=" + ouNames[i] + ",";
            }
            
            ou = ou.Trim(',');
            return $"LDAP://{serverIpOrDomain}/{ou},{dc}";
        }

        /// <summary>
        /// 将Ldap路径转换成filepath类型的格式
        /// </summary>
        /// <param name="path">CanonicalName 如：Ldap://xxx/ou=HR,ou=pphbh,dc=ops,dc=net</param>
        /// <returns>ops.net/pphbh/HR</returns>
        public string LdapPathConverToFilePath(string path)
        {
            if (path.StartsWith("ldap", StringComparison.CurrentCultureIgnoreCase))
            {
                int index = path.LastIndexOf('/') + 1;
                path = path.Substring(index);
            }

            string dc = "", ou = "";
            foreach (var element in path.Split(','))
            {
                if (element.ToLowerInvariant().StartsWith("DC=".ToLowerInvariant()))
                {
                    var item = element.Substring(element.IndexOf('=') + 1);
                    dc += item + ".";
                }

                if (element.ToLowerInvariant().StartsWith("OU=".ToLowerInvariant()))
                {
                    var item = element.Substring(element.IndexOf('=') + 1);
                    ou = item + "/" + ou;
                }
            }

            dc = dc.Trim('.');
            ou = ou.Trim('/');
            return $"{dc}/{ou}";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentPath">全路径,必须LDAP开头</param>
        /// <param name="ouName"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public string CreateOuPath(string parentPath, string ouName,AdAdminConfig config=null)
        {
            if (string.IsNullOrEmpty(parentPath) && config == null)
            {
                throw new Exception("parentPath与config参数不能同时为空.");
            }

            if (string.IsNullOrEmpty(parentPath))
            {
                return $"LDAP://{config.ServerIpOrDomain}/ou={ouName},{config.DcContainer}";
            }

            string domain = GetLdapDomain(parentPath);
            int index = parentPath.LastIndexOf('/') + 1;
            string ou = parentPath.Substring(index);
            return $"{domain}ou={ouName},{ou}";
        }

        #endregion
    }
}

