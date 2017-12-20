using System;
using System.Collections.Generic;
using System.DirectoryServices;
using Enterprises.Framework.Plugin.Domain.AdManager.Enum;
using Enterprises.Framework.Plugin.Domain.AdManager.Exception;

namespace Enterprises.Framework.Plugin.Domain.AdManager.ADObject
{
    /// <summary>
    /// AD OrganizationalUnit 对象。
    /// </summary>
    public class OU : BaseObject
    {
        #region PROPERTY const

        public const string PROPERTY_OU                     = "ou";                         // Common name,string, RDN

        public const string PROPERTY_DESCRIPTION            = "description";                // 描述
        public const string PROPERTY_MANAGEDBY              = "managedBy";                  // 管理者
        public const string PROPERTY_ADDRESS_COUNTRY        = "co";                         // 地址-国家/地区,string
        public const string PROPERTY_ADDRESS_COUNTRYAB      = "c";                          // 地址-国家/地区缩写,string
        public const string PROPERTY_ADDRESS_COUNTRYCODE    = "countryCode";                // 地址-国家/地区编码,int；中国依次属性值为CHINA,CN,156
        public const string PROPERTY_ADDRESS_PROVINCE       = "st";                         // 地址-省/自治区,string
        public const string PROPERTY_ADDRESS_CITY           = "l";                          // 地址-市/县,string
        public const string PROPERTY_ADDRESS_STREET         = "street";                     // 地址-街道,string
        public const string PROPERTY_ADDRESS_POSTALCODE     = "postalCode";                 // 地址-邮政编码,string

        #endregion


        #region fields

        private string description;
        private string managedBy;

        private OU parent;
        private List<OU> children;

        #endregion


        #region properties

        /// <summary>
        /// 描述
        /// </summary>
        public string Description
        {
            get { return this.description; }
            set { this.description = value; }
        }

        /// <summary>
        /// 管理者DN
        /// </summary>
        public string ManagedBy
        {
            get { return this.managedBy; }
            set { this.managedBy = value; }
        }

        /// <summary>
        /// 父对象
        /// </summary>
        public OU Parent
        {
            get { return this.parent; }
        }

        /// <summary>
        /// 子对象
        /// </summary>
        public List<OU> Children
        {
            get 
            {
                if (this.children == null)
                    this.children = new List<OU>();
                return this.children; 
            }
        }


        #endregion


        #region maintain methods

        /// <summary>
        /// 添加组织单位。
        /// </summary>
        /// <param name="locationPath">组织单位被添加的位置，ADsPath。DN形式。完全转义。</param>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        public void Add(string locationPath, string userName, string password)
        {
            if (locationPath.IndexOf(ParaMgr.LDAP_IDENTITY) >= 0)
                locationPath = locationPath.Substring(7);

            DirectoryEntry parent = null;
            DirectoryEntry newOU = null;

            // 默认位置，在域容器下
            if (String.IsNullOrEmpty(locationPath))
                locationPath = ParaMgr.ADFullPath;

            if (!ADManager.Exists(locationPath))
                throw new EntryNotExistException("指定的位置对象不存在。");

            string rdn = Utils.GenerateRDNOU(this.name);                                    // 使用name做OU
            // 这里的问题是要求DN形式的的Path
            if (ADManager.Exists(Utils.GenerateDN(rdn, locationPath)))
                throw new EntryNotExistException("指定的位置下存在同名对象。");

            try
            {
                parent = ADManager.GetByPath(locationPath, userName, password);
                newOU = parent.Children.Add(rdn, "organizationalUnit");

                Utils.SetProperty(newOU, OU.PROPERTY_DESCRIPTION, this.description);
                Utils.SetProperty(newOU, OU.PROPERTY_MANAGEDBY, this.managedBy);            // 注意，不能是转义/的DN

                newOU.CommitChanges();

                // reload
                this.Parse(newOU);

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
                if (newOU != null)
                {
                    newOU.Close();
                    newOU.Dispose();
                }
            }
        }

        /// <summary>
        /// 添加组织单位，使用默认用户身份标识。
        /// </summary>
        /// <param name="locationPath">组织单位被添加的位置，ADsPath</param>
        public void Add(string locationPath)
        {
            Add(locationPath, this.iUserName, this.iPassword);
        }

        /// <summary>
        /// 更新组织单位。
        /// </summary>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        public void Update(string userName, string password)
        {
            DirectoryEntry de = null;
            try
            {
                de = ADManager.GetByGuid(this.Guid, userName, password);

                Utils.SetProperty(de, OU.PROPERTY_DESCRIPTION, this.description);
                Utils.SetProperty(de, OU.PROPERTY_MANAGEDBY, this.managedBy);       // 注意，不能是转义/的DN

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
        /// 更新组织单位，使用默认用户身份标识。
        /// </summary>
        public void Update()
        {
            Update(this.iUserName, this.iPassword);
        }

        /// <summary>
        /// 删除组织单位。
        /// </summary>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        public void Remove(string userName, string password)
        {
            DirectoryEntry de = null;
            try
            {
                de = ADManager.GetByGuid(this.guid, userName, password);

                if (de.Children.GetEnumerator().MoveNext())
                {
                    throw new ExistChildException("组织单位下存在子对象。");
                }

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
        /// 删除组织单位，使用默认用户身份标识。
        /// </summary>
        public void Remove()
        {
            Remove(this.iUserName, this.iPassword);
        }

        /// <summary>
        /// 更改组织单位名称。
        /// </summary>
        /// <param name="newName">该项的新名称。</param>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        public void Rename(string newName, string userName, string password)
        {
            DirectoryEntry de = null;

            string rdn = Utils.GenerateRDNOU(newName);
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
        /// 更改组织单位名称，使用默认用户身份标识。
        /// </summary>
        /// <param name="newName">该项的新名称。</param>
        public void Rename(string newName)
        {
            Rename(newName, this.iUserName, this.iPassword);
        }

        /// <summary>
        /// 移动OU到指定位置。
        /// </summary>
        /// <param name="newLocationPath">移动到的位置的ADsPath。必须是DN形式，且完全转义。</param>
        /// <param name="mustOU">移动到的位置对应的DirectoryEntry是否必须是组织单位。</param>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        public void Move(string newLocationPath, bool mustOU, string userName, string password)
        {
            DirectoryEntry de = ADManager.GetByDN(this.Dn, userName, password);         // 必须DN -- 见ADManager.MoveOU方法

            ADManager.MoveOU(de, newLocationPath, mustOU, userName, password);

            this.Parse(de);

            de.Close();
            de.Dispose();

        }

        /// <summary>
        /// 移动OU到指定位置，使用默认用户身份标识。
        /// </summary>
        /// <param name="newLocationPath">移动到的位置的ADsPath。必须是DN形式，且完全转义。</param>
        /// <param name="mustOU">移动到的位置对应的DirectoryEntry是否必须是组织单位。</param>
        public void Move(string newLocationPath, bool mustOU)
        {
            Move(newLocationPath, mustOU, this.iUserName, this.iPassword);
        }


        #endregion


        #region ctors
        /// <summary>
        ///  默认构函数。
        /// </summary>
        public OU()
        {
        }

        internal OU(DirectoryEntry entry)
        {
            if (entry == null)
                throw new ArgumentNullException();

            this.Parse(entry);
        }

        /// <summary>
        /// 构造函数，根据SearchResult对象进行构造。
        /// </summary>
        /// <param name="result">SearchResult对象。</param>
        internal OU(SearchResult result)
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
            this.Parse(entry, SchemaClass.organizationalUnit);

            this.description = Utils.GetProperty(entry, OU.PROPERTY_DESCRIPTION);
            this.managedBy = Utils.GetProperty(entry, OU.PROPERTY_MANAGEDBY);
        }

        /// <summary>
        /// 解析SearchResult对象。
        /// </summary>
        /// <param name="result">SearchResult对象。应当包括必要的属性。</param>
        protected void Parse(SearchResult result)
        {
            this.Parse(result, SchemaClass.organizationalUnit);

            this.description = Utils.GetProperty(result, "description");
            this.managedBy = Utils.GetProperty(result, "managedby");
        }

        #endregion


        /// <summary>
        /// 获取组织单位对象的子树。
        /// </summary>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        /// <returns></returns>
        public OU GetSubTree(string userName, string password)
        {
            DirectoryEntry de = null;
            try
            {
                de = ADManager.GetByGuid(this.guid, userName, password);

                this.GetSubTree(de.Path, userName, password);

                return this;
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

        private void GetSubTree(string parentPath, string userName, string password)
        {
            this.children = new List<OU>();
            OU ou = null;
            foreach (DirectoryEntry c in
                    ADManager.Search(null, "organizationalUnit", null, parentPath, SearchScope.OneLevel, userName, password))
            {
                ou = new OU(c);
                ou.parent = this;
                this.children.Add(ou);

                ou.GetSubTree(c.Path, userName, password);

                if (c != null)
                {
                    c.Close();
                    c.Dispose();
                }
            }
        }

        /// <summary>
        /// 获取组织单位对象的子树，使用默认用户身份标识。
        /// </summary>
        /// <returns></returns>
        public OU GetSubTree()
        {
            return GetSubTree(this.iUserName, this.iPassword);
        }

        /// <summary>
        ///  获取组织单位子组织单位。
        /// </summary>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        /// <returns></returns>
        public List<OU> GetChildren(string userName, string password)
        {
            this.children = new List<OU>();
            OU ou = null;
            foreach (DirectoryEntry c in
                    ADManager.Search(null, "organizationalUnit", null, this.Path, SearchScope.OneLevel, userName, password))
            {
                ou = new OU(c);
                ou.parent = this;
                this.children.Add(ou);

                if (c != null)
                {
                    c.Close();
                    c.Dispose();
                }
            }

            return this.children;
        }

        /// <summary>
        ///  获取组织单位子组织单位，使用默认用户身份标识。
        /// </summary>
        /// <returns></returns>
        public List<OU> GetChildren()
        {
            return GetChildren(this.iUserName, this.iPassword);
        }

        /// <summary>
        /// 获取组织单位的父组织单位，
        /// </summary>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        /// <returns></returns>
        public OU GetParent(string userName, string password)
        {
            DirectoryEntry de = null;
            try
            {
                de = ADManager.GetByGuid(this.Guid, userName, password);

                DirectoryEntry parentDe = null;
                try
                {
                    parentDe = de.Parent;
                    if (parent.SchemaClassName == SchemaClass.organizationalUnit.ToString("F"))
                        this.parent = new OU(parentDe);
                    else
                        this.parent = null;

                    
                }
                catch (DirectoryServicesCOMException dsce)
                {
                    this.parent = null;
                    throw dsce;
                }
                finally
                {
                    if (parentDe != null)
                    {
                        parentDe.Close();
                        parentDe.Dispose();
                    }
                }
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

            return this.parent;
        }

        /// <summary>
        /// 获取组织单位的父组织单位，使用默认用户身份标识。
        /// </summary>
        /// <returns></returns>
        public OU GetParent()
        {
            return GetParent(this.iUserName, this.iPassword);
        }



        #region reference

        #region organizationalUnit properity

        /*
objectClass=top,organizationalUnit,
c=CN,
l=海淀,
st=北京,
street=知春路,
ou=OU1,
description=描述,
postalCode=100083,
distinguishedName=OU=OU1,DC=maodou,DC=com,
instanceType=4,
whenCreated=2007/8/14 3:36:01,
whenChanged=2007/8/15 18:38:30,
uSNCreated=System.__ComObject,
uSNChanged=System.__ComObject,
co=中国,
name=OU1,
objectGUID=System.Byte[],
countryCode=156,
managedBy=CN=ou1user1,OU=OU1,DC=maodou,DC=com,
objectCategory=CN=Organizational-Unit,CN=Schema,CN=Configuration,DC=maodou,DC=com,
nTSecurityDescriptor=System.__ComObject,

        */
        #endregion

        #endregion
    }
}
