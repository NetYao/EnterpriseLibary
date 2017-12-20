using System;
using System.Collections.Generic;
using System.DirectoryServices;
using Enterprises.Framework.Plugin.Domain.AdManager.Enum;
using Enterprises.Framework.Plugin.Domain.AdManager.Exception;

namespace Enterprises.Framework.Plugin.Domain.AdManager.ADObject
{
    /// <summary>
    /// AD Group 对象。
    /// </summary>
    public class Group : BaseObject
    {
        #region PROPERTY const

        public const string PROPERTY_CN             = "cn";                     // Common name,string, RDN

        public const string PROPERTY_DESCRIPTION    = "description";            // 描述
        public const string PROPERTY_INFO           = "info";                   // 注释
        public const string PROPERTY_MAIL           = "mail";                   // 电子邮件
        public const string PROPERTY_GROUPTYPE      = "groupType";              // 组类型
        public const string PROPERTY_ACCOUNT        = "sAMAccountName";         // 组名（Windows2000前）
        public const string PROPERTY_ACCOUNTYPE     = "sAMAccountType";         // 账户类型
        public const string PROPERTY_MANAGEDBY      = "managedBy";              // 管理者
        public const string PROPERTY_MEMBER         = "member";                 // 成员
        public const string PROPERTY_MEMBEROF       = "memberOf";               // 隶属于


        #endregion


        #region fields

        private string description;
        private string info;
        private string accountName;
        private string[] members;
        private string[] memberOf;

        #endregion


        #region properties

        /// <summary>
        /// 
        /// </summary>
        public string AccountName
        {
            get { return this.accountName; }
            set 
            {
                this.accountName = value;

                foreach (char i in Utils.InvalidSAMAccountNameChars)
                {
                    this.accountName = this.accountName.Replace(i, '_');
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Description
        {
            get { return this.description; }
            set { this.description = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Info
        {
            get { return this.info; }
            set { this.info = value; }
        }

        /// <summary>
        /// 获取所有成员（DN）
        /// </summary>
        /// <remarks>对于系统内置组，成员不仅是用户，还可以是组</remarks>
        public string[] MembersDN
        {
            get { return this.members; }
        }

        #endregion


        #region maintain methods

        /// <summary>
        /// 添加组。
        /// </summary>
        /// <param name="locationPath">组被添加的位置，ADsPath。DN形式，完全转义。</param>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        public void Add(string locationPath, string userName, string password)
        {
            if (locationPath.IndexOf(ParaMgr.LDAP_IDENTITY) >= 0)
                locationPath = locationPath.Substring(7);

            DirectoryEntry parent = null;
            DirectoryEntry newGroup = null;

            // 默认位置，在Users容器下
            if (String.IsNullOrEmpty(locationPath))
                locationPath = "CN=Users," + ParaMgr.ADFullPath;

            if (!ADManager.Exists(locationPath))
                throw new EntryNotExistException("指定的位置对象不存在。");

            string rdn = Utils.GenerateRDNCN(this.name);                                    // 使用name做CN
            // 这里的问题是要求DN形式的的Path
            if (ADManager.Exists(Utils.GenerateDN(rdn, locationPath)))
                throw new EntryNotExistException("指定的位置下存在同名对象。");

            try
            {
                parent = ADManager.GetByPath(locationPath, userName, password);
                newGroup = parent.Children.Add(rdn, "group");                           

                Utils.SetProperty(newGroup, Group.PROPERTY_ACCOUNT, this.accountName);
                Utils.SetProperty(newGroup, Group.PROPERTY_INFO, this.info);
                Utils.SetProperty(newGroup, Group.PROPERTY_DESCRIPTION, this.description);
                Utils.SetProperty(newGroup, Group.PROPERTY_GROUPTYPE, (int)GroupScope.ADS_GROUP_TYPE_GLOBAL_GROUP);

                newGroup.CommitChanges();

                // reload
                this.Parse(newGroup);
            }
            catch (DirectoryServicesCOMException dsce)
            {
                throw dsce;
            }
            finally
            {
                if (parent != null)
                {
                    parent.Close();
                    parent.Dispose();
                }
                if (newGroup != null)
                {
                    newGroup.Close();
                    newGroup.Dispose();
                }
            }
        }

        /// <summary>
        /// 添加组，使用默认用户身份标识。
        /// </summary>
        /// <param name="locationPath">组被添加的位置，ADsPath</param>
        public void Add(string locationPath)
        {
            Add(locationPath, this.iUserName, this.iPassword);
        }


        /// <summary>
        /// 更新组。
        /// </summary>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        public void Update(string userName, string password)
        {
            DirectoryEntry de = null;
            try
            {
                de = ADManager.GetByGuid(this.Guid, userName, password);

                Utils.SetProperty(de, Group.PROPERTY_ACCOUNT, this.accountName);       // 会更改组名
                Utils.SetProperty(de, Group.PROPERTY_INFO, this.info);
                Utils.SetProperty(de, Group.PROPERTY_DESCRIPTION, this.description);

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
        /// 更新组，使用默认用户身份标识。
        /// </summary>
        public void Update()
        {
            Update(this.iUserName, this.iPassword);
        }


        /// <summary>
        /// 删除组。
        /// </summary>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        public void Remove(string userName, string password)
        {
            DirectoryEntry de = null;
            try
            {
                de = ADManager.GetByGuid(this.guid, userName, password);

                de.DeleteTree();

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
        /// 删除组，使用默认用户身份标识。
        /// </summary>
        public void Remove()
        {
            Remove(this.iUserName, this.iPassword);
        }


        /// <summary>
        /// 更改组名称。
        /// </summary>
        /// <param name="newName">该项的新名称。</param>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        public void Rename(string newName, string userName, string password)
        {
            DirectoryEntry de = null;

            string rdn = Utils.GenerateRDNCN(newName);

            if (ADManager.Exists(Utils.GenerateDN(rdn, Utils.GetParentDN(this.Dn))))
                throw new SameRDNException("已存在同名对象。");

            try
            {
                de = ADManager.GetByDN(this.Dn, userName, password);        // 必须是DN形式，且完全转义。

                de.Rename(rdn);

                de.CommitChanges();

                // Reload
                this.Parse(de);
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
        /// 更改组名称，使用默认用户身份标识。
        /// </summary>
        /// <param name="newName">该项的新名称。</param>
        public void Rename(string newName)
        {
            Rename(newName,this.iUserName, this.iPassword);
        }


        /// <summary>
        /// 将用户添加到组。
        /// </summary>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        /// <param name="usersDN">需要添加的用户的DN。</param>
        public void AddUser(string userName, string password, params string[] usersDN)
        {
            DirectoryEntry de = null;
            try
            {
                de = ADManager.GetByGuid(this.guid, userName, password);

                // 过滤 -- 如果添加一个已存在的member，可能会引发异常
                List<string> toAdd = new List<string>();
                foreach (string udn in usersDN)
                {
                    if (!(Array.BinarySearch(this.members, udn) >= 0))
                    {
                        toAdd.Add(udn);
                    }
                }

                foreach (string user in toAdd)
                {
                    de.Properties[Group.PROPERTY_MEMBER].Add(Utils.UnEscapeDNBackslashedChar(user));
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
        /// <param name="usersDN">需要添加的用户的DN。</param>
        public void AddUser(params string[] usersDN)
        {
            AddUser(this.iUserName, this.iPassword, usersDN);
        }


        /// <summary>
        /// 将用户从组中移除。
        /// </summary>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        /// <param name="usersDN">需要移除的用户的DN。</param>
        public void RemoveUser(string userName, string password, params string[] usersDN)
        {
            DirectoryEntry de = null;
            try
            {
                de = ADManager.GetByGuid(this.guid, userName, password);

                // 过滤 -- 如果移除一个不存在的member，会引发异常
                List<string> toRemoves = new List<string>();
                foreach (string user in usersDN)
                {
                    if (Array.BinarySearch(this.members, user) >= 0)
                        toRemoves.Add(user);
                }
                foreach (string user in toRemoves)
                {
                    de.Properties[Group.PROPERTY_MEMBER].Remove(
                        Utils.UnEscapeDNBackslashedChar(user));  // 去除/转义，以便匹配
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
        /// 将用户从组中移除，使用默认用户身份标识。
        /// </summary>
        /// <param name="usersDN">需要移除的用户的DN。</param>
        public void RemoveUser(params string[] usersDN)
        {
            RemoveUser(this.iUserName, this.iPassword, usersDN);
        }

        #endregion


        #region .ctors

        /// <summary>
        ///  默认构函数。
        /// </summary>
        public Group() {}

        /// <summary>
        /// 构造函数，根据DirectoryEntry对象进行构造。
        /// </summary>
        /// <param name="entry">DirectoryEntry对象。</param>
        internal Group(DirectoryEntry entry)
        {
            if (entry == null)
                throw new ArgumentNullException();

            this.Parse(entry);
        }

        /// <summary>
        /// 构造函数，根据SearchResult对象进行构造。
        /// </summary>
        /// <param name="result">SearchResult对象。</param>
        internal Group(SearchResult result)
        {
            if (result == null)
                throw new ArgumentNullException();

            this.Parse(result);
        }

        #endregion


        #region parse

        /// <summary>
        /// 解析DirectoryEntry对象。
        /// </summary>
        /// <param name="entry">DirectoryEntry对象。</param>
        protected override void Parse(DirectoryEntry entry)
        {
            base.Parse(entry, SchemaClass.group);

            this.accountName = Utils.GetProperty(entry, Group.PROPERTY_ACCOUNT);
            this.description = Utils.GetProperty(entry, Group.PROPERTY_DESCRIPTION);
            this.info = Utils.GetProperty(entry, Group.PROPERTY_INFO);
            if (entry.Properties.Contains(Group.PROPERTY_MEMBER))
            {
                List<string> ms = new List<string>();
                foreach (object m in entry.Properties[Group.PROPERTY_MEMBER])
                {
                    ms.Add(Utils.EscapeDNBackslashedChar(m.ToString()));            // 转义/
                }
                ms.Sort();          // 已排序 -- 以便内部使用
                this.members = ms.ToArray() ;
            }
            else
                this.members = new string[] { };
            if (entry.Properties.Contains(Group.PROPERTY_MEMBEROF))
            {
                List<string> ms = new List<string>();
                foreach (object m in entry.Properties[Group.PROPERTY_MEMBEROF])
                {
                    ms.Add(Utils.EscapeDNBackslashedChar(m.ToString()));            // 转义/
                }
                ms.Sort();          // 已排序 -- 以便内部使用
                this.memberOf = ms.ToArray();
            }
            else
                this.memberOf = new string[] { };

        }

        /// <summary>
        /// 解析SearchResult对象。
        /// </summary>
        /// <param name="result">SearchResult对象。应当包括必要的属性。</param>
        protected void Parse(SearchResult result)
        {
            base.Parse(result, SchemaClass.group);

            this.accountName = Utils.GetProperty(result, "samaccountname");
            this.description = Utils.GetProperty(result, "description");
            this.info = Utils.GetProperty(result, "info");
            if (result.Properties.Contains("member"))
            {
                List<string> ms = new List<string>();
                foreach (object m in result.Properties["member"])
                {
                    ms.Add(Utils.EscapeDNBackslashedChar(m.ToString()));            // 转义/
                }
                ms.Sort();          // 已排序 -- 以便内部使用
                this.members = ms.ToArray();
            }
            else
                this.members = new string[] { };
            if (result.Properties.Contains("memberof"))
            {
                List<string> ms = new List<string>();
                foreach (object m in result.Properties["memberof"])
                {
                    ms.Add(Utils.EscapeDNBackslashedChar(m.ToString()));            // 转义/
                }
                ms.Sort();          // 已排序 -- 以便内部使用
                this.memberOf = ms.ToArray();
            }
            else
                this.memberOf = new string[] { };

        }

        #endregion


        /// <summary>
        /// 获取组对象的隶属组的DN。
        /// </summary>
        /// <returns></returns>
        public List<string> GetMemberOfDN()
        {
            // 防止更改
            List<string> dn = new List<string>();
            dn.AddRange(this.memberOf);
            return dn;
        }


        /// <summary>
        /// 获取组对象的成员。
        /// </summary>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        /// <returns></returns>
        public List<DirectoryEntry> GetMembers(string userName, string password)
        {
            List<DirectoryEntry> entries = new List<DirectoryEntry>();

            DirectoryEntry de = null;

            try
            {         
                foreach (string member in this.members)
                {
                    de = ADManager.GetByDN(member, userName, password);

                    if (de != null)
                        entries.Add(de);
                }

                return entries;
            }
            catch (DirectoryServicesCOMException dsce)
            {
                foreach (DirectoryEntry d in entries)
                {
                    if (d != null)
                    {
                        d.Close();
                        d.Dispose();
                    }
                }

                throw dsce;
            }
        }

        /// <summary>
        /// 获取组对象的成员，使用默认用户身份标识。
        /// </summary>
        /// <returns></returns>
        public List<DirectoryEntry> GetMembers()
        {
            return GetMembers(this.iUserName, this.iPassword);
        }


        /// <summary>
        /// 获取组对象的成员。（仅用户）
        /// </summary>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        /// <returns></returns>
        public List<User> GetUserMembers(string userName, string password)
        {
            List<User> users = new List<User>();

            DirectoryEntry de = null;
            string userSchemaClassName = SchemaClass.user.ToString("F");

            try
            {
                foreach (string member in this.members)
                {
                    de = ADManager.GetByDN(member, userName, password);

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

                return users;
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
        /// 获取组对象的成员。（仅用户）
        /// </summary>
        /// <returns></returns>
        public List<User> GetUserMembers()
        {
            return GetUserMembers(this.iUserName, this.iPassword);
        }



        #region reference

        #region group properity
        /*
objectClass=top,group,
cn=ou1group1,
description=描述,
member=CN=ou1user1,OU=OU1,DC=maodou,DC=com,
distinguishedName=CN=ou1group1,OU=OU1,DC=maodou,DC=com,
instanceType=4,
whenCreated=2007/8/14 3:44:35,
whenChanged=2007/8/16 21:11:22,
uSNCreated=System.__ComObject,
memberOf=CN=Administrators,CN=Builtin,DC=maodou,DC=com,
info=注释,
uSNChanged=System.__ComObject,
name=ou1group1,
objectGUID=System.Byte[],
objectSid=System.Byte[],
sAMAccountName=ou1group1,
sAMAccountType=268435456,
managedBy=CN=sherwinzhu,CN=Users,DC=maodou,DC=com,
groupType=-2147483646,
objectCategory=CN=Group,CN=Schema,CN=Configuration,DC=maodou,DC=com,
mail=mail@126.com,
nTSecurityDescriptor=System.__ComObject,

         */

        #endregion

        #endregion
    }
}
