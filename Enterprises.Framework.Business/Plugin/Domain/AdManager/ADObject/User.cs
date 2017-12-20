using System;
using System.Collections.Generic;
using System.DirectoryServices;
using Enterprises.Framework.Plugin.Domain.AdManager.Enum;
using Enterprises.Framework.Plugin.Domain.AdManager.Exception;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using ActiveDs; // Namespace added via ref to C:\Windows\System32\activeds.tlb

namespace Enterprises.Framework.Plugin.Domain.AdManager.ADObject
{
    /// <summary>
    /// AD User 对象。
    /// </summary>
    public class User : BaseObject
    {
        #region PROPERTY Const

        public const string PROPERTY_CN                     = "cn";                             // Common name,string, RDN

        public const string PROPERTY_GENERAL_LASTNAME       = "sn";                             // 常规-姓,string
        public const string PROPERTY_GENERAL_GIVENNAME      = "givenName";                      // 常规-名,string
        public const string PROPERTY_GENERAL_INITIALS       = "initials";                       // 常规-英文缩写,string
        public const string PROPERTY_GENERAL_DISPLAYNAME    = "displayName";                    // 常规-显示名称,string
        public const string PROPERTY_GENERAL_DESCRIPTION    = "description";                    // 常规-描述,string
        public const string PROPERTY_GENERAL_OFFICE         = "physicalDeliveryOfficeName";     // 常规-办公室,string
        public const string PROPERTY_GENERAL_MAIL           = "mail";                           // 常规-电子邮件,string
        public const string PROPERTY_GENERAL_TEL            = "telephoneNumber";                // 常规-电话号码,string
        public const string PROPERTY_GENERAL_HOMEPAGE       = "wWWHomePage";                    // 常规-网页,string

        public const string PROPERTY_ADDRESS_COUNTRY        = "co";                             // 地址-国家/地区,string
        public const string PROPERTY_ADDRESS_COUNTRYAB      = "c";                              // 地址-国家/地区缩写,string
        public const string PROPERTY_ADDRESS_COUNTRYCODE    = "countryCode";                    // 地址-国家/地区编码,int；中国依次属性值为CHINA,CN,156
        public const string PROPERTY_ADDRESS_PROVINCE       = "st";                             // 地址-省/自治区,string
        public const string PROPERTY_ADDRESS_CITY           = "l";                              // 地址-市/县,string
        public const string PROPERTY_ADDRESS_STREET         = "streetAddress";                  // 地址-街道,string
        public const string PROPERTY_ADDRESS_POSTALCODE     = "postalCode";                     // 地址-邮政编码,string
        public const string PROPERTY_ADDRESS_POSTBOX        = "postOfficeBox";                  // 地址-邮政信箱,string

        public const string PROPERTY_ORGAN_TITLE            = "title";                          // 单位-职务,string
        public const string PROPERTY_ORGAN_DEPARTMENT       = "department";                     // 单位-部门,string
        public const string PROPERTY_ORGAN_COMPANY          = "company";                        // 单位-公司,string
        public const string PROPERTY_ORGAN_UNDERLING        = "directReports";                  // 单位-直接下属,string[]
        public const string PROPERTY_ORGAN_MANAGER          = "manager";                        // 单位-经理,string

        public const string PROPERTY_TEL_PHONE              = "homePhone";                      // 电话-家庭电话,string
        public const string PROPERTY_TEL_PHONEO             = "otherHomePhone";                 // 电话-家庭电话其它,string[]
        public const string PROPERTY_TEL_MOBILE             = "mobile";                         // 电话-移动电话,string
        public const string PROPERTY_TEL_MOBILEO            = "otherMobile";                    // 电话-移动电话其它,string[]
        public const string PROPERTY_TEL_FAX                = "facsimileTelephoneNumber";       // 电话-传真,string
        public const string PROPERTY_TEL_FAXO               = "otherFacsimileTelephoneNumber";  // 电话-传真其它,string[]
        public const string PROPERTY_TEL_IP                 = "ipPhone";                        // 电话-IP电话,string
        public const string PROPERTY_TEL_IPO                = "otherIpPhone";                   // 电话-IP电话其它,string[]
        public const string PROPERTY_TEL_PAGER              = "pager";                          // 电话-寻呼机,string
        public const string PROPERTY_TEL_PAGERO             = "otherPager";                     // 电话-寻呼机其它,string[]
        public const string PROPERTY_TEL_INFO               = "info";                           // 电话-注释,string

        public const string PROPERTY_ACCOUNT_SAM            = "sAMAccountName";                 // 账户-用户登录名(Win2000以前版本)（不含域）,string
        public const string PROPERTY_ACCOUNT_TYPE           = "sAMAccountType";                 // ?,int
        public const string PROPERTY_ACCOUNT_PRINCIPAL      = "userPrincipalName";              // 账户-用户登录名,string；eg:user@domain.local
        public const string PROPERTY_ACCOUNT_EXPIRES        = "accountExpires";                 // ?,?
        public const string PROPERTY_ACCOUNT_CONTROL        = "userAccountControl";             // 账户-账户选项,int
        public const string PROPERTY_ACCOUNT_PWDLASTSET     = "pwdLastSet";                     // 账户-账户选项,下次登录必须修改密码,IADsLargeInteger

        public const string PROPERTY_MEMBEROF_ALL           = "memberOf";                       // 隶属于-组,string[]
        public const string PROPERTY_MEMBEROF_PRIMARY       = "primaryGroupID";                 // 隶属于-主要组ID,int


        public const string PROPERTY_BADPWDCOUNT            = "badPwdCount";                    // ?,int
        public const string PROPERTY_CODEPAGE               = "codePage";                       // ?,int
        public const string PROPERTY_BADPASSWORDTIME        = "badPasswordTime";                // ?
        public const string PROPERTY_LASTLOGOFF             = "lastLogoff";                     // ?
        public const string PROPERTY_LASTLOGON              = "lastLogon";                      // ?
        public const string PROPERTY_LOGONCOUNT             = "logonCount";                     // ?,int

        #endregion


        #region fields

        private string userName;        // sAMAccountName
        private string firstName;       // givenName
        private string lastName;        // sn
        private string initials;        // initials
        private string displayName;     // displayName
        private string office;          // physicalDeliveryOfficeName
        private string title;           // title
        private string manager;         // manager
        private string department;      // department
        private string telephone;       // telephoneNumber
        private string mobile;          // mobile
        private string mail;            // mail
        private string principalName;   // userPrincipalName
        private int userAccountControl; // userAccountControl

        private int? primaryGroupID;    // primaryGroupID
        private string[] memberOf;      // memberOf
        private Int64 pwdLastSet;

        #endregion


        #region properties

        /// <summary>
        /// 用户登录名(Win2000以前版本)（不含域）
        /// </summary>
        public string UserName
        {
            get { return this.userName; }
            set 
            { 
                this.userName = value;
                foreach (char i in Utils.InvalidSAMAccountNameChars)
                {
                    this.userName = this.userName.Replace(i, '_');
                }
            }
        }

        /// <summary>
        /// 名
        /// </summary>
        public string FirstName
        {
            get { return this.firstName; }
            set { this.firstName = value; }
        }

        /// <summary>
        /// 姓
        /// </summary>
        public string LastName
        {
            get { return this.lastName; }
            set { this.lastName = value; }
        }

        /// <summary>
        /// 缩写
        /// </summary>
        public string Initials
        {
            get { return this.initials; }
            set { this.initials = value; }
        }

        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName
        {
            get { return this.displayName; }
            set { this.displayName = value; }
        }

        /// <summary>
        /// 办公室
        /// </summary>
        public string Office
        {
            get { return this.office; }
            set { this.office = value; }
        }

        /// <summary>
        /// 职务
        /// </summary>
        public string Title
        {
            get { return this.title; }
            set { this.title = value; }
        }

        /// <summary>
        /// 经理DN
        /// </summary>
        public string Manager
        {
            get { return this.manager; }
            set { this.manager = value; }
        }

        /// <summary>
        /// 电话
        /// </summary>
        public string Telephone
        {
            get { return this.telephone; }
            set { this.telephone = value; }
        }

        /// <summary>
        /// 移动电话
        /// </summary>
        public string Mobile
        {
            get { return this.mobile; }
            set { this.mobile = value; }
        }

        /// <summary>
        /// 电子邮件
        /// </summary>
        public string Mail
        {
            get { return this.mail; }
            set { this.mail = value; }
        }

        /// <summary>
        /// 用户登录名
        /// </summary>
        public string PrincipalName
        {
            get { return this.principalName; }
            set { this.principalName = value; }
        }

        /// <summary>
        /// UserAccountControl
        /// </summary>
        public int UserAccountControl
        {
            get { return this.userAccountControl; }
        }

        /// <summary>
        /// 是否启用/禁用
        /// </summary>
        public bool Enabled
        {
            set
            {
                if (value)
                {
                    this.userAccountControl = this.userAccountControl & ~(int)AccountOptions.ADS_UF_ACCOUNTDISABLE;
                }
                else
                {
                    this.userAccountControl = this.userAccountControl | (int)AccountOptions.ADS_UF_ACCOUNTDISABLE;
                }
            }
        }

        /// <summary>
        /// 密码是否永不过期
        /// </summary>
        public bool DontExpirePwd
        {
            set
            {
                if (value)
                {
                    this.userAccountControl = this.userAccountControl & ~(int)AccountOptions.ADS_UF_DONT_EXPIRE_PASSWD;
                }
                else
                {
                    this.userAccountControl = this.userAccountControl | (int)AccountOptions.ADS_UF_DONT_EXPIRE_PASSWD;
                }
            }
        }

        /// <summary>
        /// 是否下次登录需要更改密码
        /// </summary>
        public bool MustChangePassword
        {
            get 
            {
                //long num = (pwdLastSet.HighPart << 0x20) + pwdLastSet.LowPart;
                //return (num == 0);
                return (this.pwdLastSet == 0);
            }
        }



        /// <summary>
        /// 获取PrimaryGroup SID
        /// </summary>
        public string PrimaryGroupSID
        {
            get
            {
                if (this.primaryGroupID != null)
                {
                    return GeneratePrimaryGroupSID(this.objectSid, this.primaryGroupID.Value);
                }
                else
                    return null;
            }
        }

        #endregion


        #region maintain methods

        /// <summary>
        /// 添加用户。
        /// </summary>
        /// <param name="locationPath">用户被添加的位置，ADsPath。DN形式，完全转义。</param>
        /// <param name="newUserPassword">用户的密码</param>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        public void Add(string locationPath, string newUserPassword, string userName, string password)
        {
            if (locationPath.IndexOf(ParaMgr.LDAP_IDENTITY) >= 0)
                locationPath = locationPath.Substring(7);

            DirectoryEntry parent = null;
            DirectoryEntry newUser = null;

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
                newUser = parent.Children.Add(rdn, "user");

                Utils.SetProperty(newUser, User.PROPERTY_ACCOUNT_SAM, this.userName);
                Utils.SetProperty(newUser, User.PROPERTY_GENERAL_GIVENNAME, this.firstName);
                Utils.SetProperty(newUser, User.PROPERTY_GENERAL_LASTNAME, this.lastName);
                Utils.SetProperty(newUser, User.PROPERTY_GENERAL_INITIALS, this.initials);
                Utils.SetProperty(newUser, User.PROPERTY_GENERAL_DISPLAYNAME, this.displayName);
                Utils.SetProperty(newUser, User.PROPERTY_GENERAL_OFFICE, this.office);
                Utils.SetProperty(newUser, User.PROPERTY_ORGAN_TITLE, this.title);
                Utils.SetProperty(newUser, User.PROPERTY_ORGAN_MANAGER, this.manager);      // 注意，不能是转义/的DN
                Utils.SetProperty(newUser, User.PROPERTY_ORGAN_DEPARTMENT, this.department);
                Utils.SetProperty(newUser, User.PROPERTY_GENERAL_TEL, this.telephone);
                Utils.SetProperty(newUser, User.PROPERTY_TEL_MOBILE, this.mobile);
                Utils.SetProperty(newUser, User.PROPERTY_GENERAL_MAIL, this.mail);
                Utils.SetProperty(newUser, User.PROPERTY_ACCOUNT_PRINCIPAL, this.principalName);
                Utils.SetProperty(newUser, User.PROPERTY_ACCOUNT_CONTROL, this.userAccountControl);

                Utils.SetProperty(newUser, User.PROPERTY_ACCOUNT_PWDLASTSET, -1);           // 取消用户下次登陆时必须更改密码（默认为0）

                newUser.CommitChanges();

                // reload
                this.Parse(newUser);

                newUser.Invoke("SetPassword", new object[] { newUserPassword });            // 在CommitChanges之后才能成功调用
                newUser.CommitChanges();

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
                if (newUser != null)
                {
                    newUser.Close();
                    newUser.Dispose();
                }
            }
        }

        /// <summary>
        /// 添加用户，使用默认用户身份标识。
        /// </summary>
        /// <param name="locationPath">用户被添加的位置，ADsPath</param>
        /// <param name="newUserPassword">用户的密码</param>
        public void Add(string locationPath, string newUserPassword)
        {
            this.Add(locationPath, newUserPassword, this.iUserName, this.iPassword);
        }


        /// <summary>
        /// 更新用户。
        /// </summary>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        public void Update(string userName, string password)
        {
            DirectoryEntry de = null;
            try
            {
                de = ADManager.GetByGuid(this.guid, userName, password);

                Utils.SetProperty(de, User.PROPERTY_ACCOUNT_SAM, this.userName);                    // 可以更改
                Utils.SetProperty(de, User.PROPERTY_GENERAL_GIVENNAME, this.firstName);
                Utils.SetProperty(de, User.PROPERTY_GENERAL_LASTNAME, this.lastName);
                Utils.SetProperty(de, User.PROPERTY_GENERAL_INITIALS, this.initials);
                Utils.SetProperty(de, User.PROPERTY_GENERAL_DISPLAYNAME, this.displayName);
                Utils.SetProperty(de, User.PROPERTY_GENERAL_OFFICE, this.office);
                Utils.SetProperty(de, User.PROPERTY_ORGAN_TITLE, this.title);
                Utils.SetProperty(de, User.PROPERTY_ORGAN_MANAGER, this.manager);                   // 注意，不能是转义/的DN
                Utils.SetProperty(de, User.PROPERTY_ORGAN_DEPARTMENT, this.department);
                Utils.SetProperty(de, User.PROPERTY_GENERAL_TEL, this.telephone);
                Utils.SetProperty(de, User.PROPERTY_TEL_MOBILE, this.mobile);
                Utils.SetProperty(de, User.PROPERTY_GENERAL_MAIL, this.mail);
                Utils.SetProperty(de, User.PROPERTY_ACCOUNT_PRINCIPAL, this.principalName);         // 可以更改
                Utils.SetProperty(de, User.PROPERTY_ACCOUNT_CONTROL, this.userAccountControl);

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
        /// 更新用户，使用默认用户身份标识。
        /// </summary>
        public void Update()
        {
            Update(this.iUserName, this.iPassword);
        }


        /// <summary>
        /// 删除用户。
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
        /// 删除用户。
        /// </summary>
        public void Remove()
        {
            this.Remove(this.iUserName, this.iPassword);
        }


        /// <summary>
        /// 设置用户密码。
        /// </summary>
        /// <param name="newPassword">新密码。</param>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        public void SetPassword(string newPassword, string userName, string password)
        {
            ADManager.SetUserPassword(this.guid, newPassword, userName, password);
        }

        /// <summary>
        /// 设置用户密码，使用默认用户身份标识。
        /// </summary>
        /// <param name="newPassword">新密码。</param>
        public void SetPassword(string newPassword)
        {
            ADManager.SetUserPassword(this.guid, newPassword, this.iUserName, this.iPassword);
        }


        /// <summary>
        /// 移动User到指定位置。
        /// </summary>
        /// <param name="newLocationPath">移动到的位置的ADsPath</param>
        /// <param name="mustOU">移动到的位置对应的DirectoryEntry是否必须是组织单位。</param>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        public void Move(string newLocationPath, bool mustOU, string userName, string password)
        {
            DirectoryEntry de = ADManager.GetByGuid(this.guid, userName, password);

            ADManager.MoveUser(de, newLocationPath, mustOU, userName, password);

            this.Parse(de);

            de.Close();
            de.Dispose();

        }

        /// <summary>
        /// 移动User到指定位置，使用默认用户身份标识。
        /// </summary>
        /// <param name="newLocationPath">移动到的位置的ADsPath</param>
        /// <param name="mustOU">移动到的位置对应的DirectoryEntry是否必须是组织单位。</param>
        public void Move(string newLocationPath, bool mustOU)
        {
            Move(newLocationPath, mustOU, this.iUserName, this.iPassword);
        }

        #endregion


        #region .ctors

        /// <summary>
        /// 默认构函数。启用用户。
        /// </summary>
        public User()
        {
            //this.userAccountControl = 544;      // 544 enabled - 546 disabled。默认启用
            this.userAccountControl = 66080;    // 且 密码永不过期
        }

        /// <summary>
        /// 构造函数，根据DirectoryEntry对象进行构造。
        /// </summary>
        /// <param name="entry">DirectoryEntry对象。</param>
        internal User(DirectoryEntry entry)
        {
            if (entry == null)
                throw new ArgumentNullException();

            this.Parse(entry);
        }

        /// <summary>
        /// 构造函数，根据SearchResult对象进行构造。
        /// </summary>
        /// <param name="result">SearchResult对象。</param>
        internal User(SearchResult result)
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
            base.Parse(entry, SchemaClass.user);        // 调用基类方法

            this.userName = Utils.GetProperty(entry, User.PROPERTY_ACCOUNT_SAM);
            this.firstName = Utils.GetProperty(entry, User.PROPERTY_GENERAL_GIVENNAME);
            this.lastName = Utils.GetProperty(entry, User.PROPERTY_GENERAL_LASTNAME);
            this.initials = Utils.GetProperty(entry, User.PROPERTY_GENERAL_INITIALS);
            this.displayName = Utils.GetProperty(entry, User.PROPERTY_GENERAL_DISPLAYNAME);
            this.office = Utils.GetProperty(entry, User.PROPERTY_GENERAL_OFFICE);
            this.title = Utils.GetProperty(entry, User.PROPERTY_ORGAN_TITLE);
            this.manager = Utils.GetProperty(entry, User.PROPERTY_ORGAN_MANAGER);
            this.department = Utils.GetProperty(entry, User.PROPERTY_ORGAN_DEPARTMENT);
            this.telephone = Utils.GetProperty(entry, User.PROPERTY_GENERAL_TEL);
            this.mobile = Utils.GetProperty(entry, User.PROPERTY_TEL_MOBILE);
            this.mail = Utils.GetProperty(entry, User.PROPERTY_GENERAL_MAIL);
            this.principalName = Utils.GetProperty(entry, User.PROPERTY_ACCOUNT_PRINCIPAL);
            this.userAccountControl = Convert.ToInt32(Utils.GetProperty(entry, User.PROPERTY_ACCOUNT_CONTROL));

            string primaryGroupIDStr = Utils.GetProperty(entry, User.PROPERTY_MEMBEROF_PRIMARY);
            if (primaryGroupIDStr != null)
                this.primaryGroupID = int.Parse(primaryGroupIDStr);
            else
                this.primaryGroupID = (int?)null;

            IADsLargeInteger li = (IADsLargeInteger)entry.Properties[User.PROPERTY_ACCOUNT_PWDLASTSET][0];

            this.pwdLastSet = (li.HighPart << 0x20) + li.LowPart;

            if (entry.Properties.Contains(User.PROPERTY_MEMBEROF_ALL))
            {
                List<string> ms = new List<string>();
                foreach (object m in entry.Properties[User.PROPERTY_MEMBEROF_ALL])
                {
                    ms.Add(Utils.EscapeDNBackslashedChar(m.ToString()));        // 转义/
                }
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
            base.Parse(result, SchemaClass.user);        // 调用基类方法

            this.userName = Utils.GetProperty(result, "samaccountname");
            this.firstName = Utils.GetProperty(result, "givenname");
            this.lastName = Utils.GetProperty(result, "sn");
            this.initials = Utils.GetProperty(result, "initials");
            this.displayName = Utils.GetProperty(result, "displayname");
            this.office = Utils.GetProperty(result, "physicaldeliveryofficename");
            this.title = Utils.GetProperty(result, "title");
            this.manager = Utils.GetProperty(result, "manager");
            this.department = Utils.GetProperty(result, "department");
            this.telephone = Utils.GetProperty(result, "telephonenumber");
            this.mobile = Utils.GetProperty(result, "mobile");
            this.mail = Utils.GetProperty(result, "mail");
            this.principalName = Utils.GetProperty(result, "userprincipalname");
            this.userAccountControl = Convert.ToInt32(Utils.GetProperty(result, "useraccountcontrol"));

            string primaryGroupIDStr = Utils.GetProperty(result, "primarygroupid");
            if (primaryGroupIDStr != null)
                this.primaryGroupID = int.Parse(primaryGroupIDStr);
            else
                this.primaryGroupID = (int?)null;

            
            this.pwdLastSet = (Int64)result.Properties["pwdlastset"][0];

            if (result.Properties.Contains("memberof"))
            {
                List<string> ms = new List<string>();
                foreach (object m in result.Properties["memberof"])
                {
                    ms.Add(Utils.EscapeDNBackslashedChar(m.ToString()));        // 转义/
                }
                this.memberOf = ms.ToArray();
            }
            else
                this.memberOf = new string[] { };
        }

        #endregion


        /// <summary>
        /// 获取User所在的位置的Guid
        /// </summary>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        /// <returns></returns>
        public Guid GetLocation(string userName, string password)
        {
            DirectoryEntry de = null;
            DirectoryEntry parent = null;
            try
            {
                de = ADManager.GetByGuid(this.guid);
                parent = de.Parent;

                return parent.Guid;
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
                if (parent != null)
                {
                    parent.Close();
                    parent.Dispose();
                }
            }
        }

        /// <summary>
        /// 获取User所在的位置的Guid，使用默认用户身份标识。
        /// </summary>
        /// <returns></returns>
        public Guid GetLocation()
        {
            return GetLocation(this.iUserName, this.iPassword);
        }

        /// <summary>
        /// 获取User所在的位置的Guid
        /// </summary>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        /// <returns></returns>
        public OU GetOrganization(string userName, string password)
        {
            DirectoryEntry de = null;
            DirectoryEntry parent = null;
            try
            {
                de = ADManager.GetByGuid(this.guid);
                parent = de.Parent;

                if (parent.SchemaClassName == SchemaClass.organizationalUnit.ToString("F"))
                    return new OU(parent);
                else
                    return null;
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
                if (parent != null)
                {
                    parent.Close();
                    parent.Dispose();
                }
            }
        }

        /// <summary>
        /// 获取User所在的位置的Guid，使用默认用户身份标识。
        /// </summary>
        /// <returns></returns>
        public OU GetOrganization()
        {
            return GetOrganization(this.iUserName, this.iPassword);
        }


        /// <summary>
        /// 获取用户DirectoryEntry对象的隶属组的DN。完全转义。
        /// </summary>
        /// <param name="includePrimaryGroup">是否包括PrimaryGroup</param>
        /// <param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        /// <param name="password">用户身份标识--密码。</param>
        /// <returns></returns>
        public List<string> GetMemberOfDN(bool includePrimaryGroup, string userName, string password)
        {
            List<string> dn = new List<string>();

            if (includePrimaryGroup)
            {
                DirectoryEntry primary = ADManager.GetBySid(this.PrimaryGroupSID, userName, password);
                if (primary != null)
                {
                    dn.Add(Utils.EscapeDNBackslashedChar(primary.Properties[BaseObject.PROPERTY_DN].Value.ToString()));

                    primary.Close();
                    primary.Dispose();
                }
            }

            dn.AddRange(this.memberOf);

            return dn;
        }

        /// <summary>
        /// 获取用户DirectoryEntry对象的隶属组的DN，使用默认用户身份标识。完全转义。
        /// </summary>
        /// <param name="includePrimaryGroup">是否包括PrimaryGroup</param>
        /// <returns></returns>
        public List<string> GetMemberOfDN(bool includePrimaryGroup)
        {
            return GetMemberOfDN(includePrimaryGroup, this.iUserName, this.iPassword);
        }


        /// <summary>
        /// 根据用户DirectoryEntry对象的SID和PrimaryGroupID，生成用户的PrimaryGroup的SID
        /// </summary>
        /// <param name="objectSid">用户DirectoryEntry对象的SID</param>
        /// <param name="primaryGroupID">用户的PrimaryGroupID</param>
        /// <returns>用户的PrimaryGroup的SID</returns>
        internal static string GeneratePrimaryGroupSID(byte[] objectSid, int primaryGroupID)
        {
            string sid1 = Utils.ConvertToOctetString(objectSid).Substring(0, 72);
            string sid2 = "";
            for (int i = 0; i <= 3; i++)
            {
                sid2 += String.Format("\\{0:x2}", primaryGroupID & 0xFF);
                primaryGroupID >>= 8;
            }

            return sid1 + sid2;
        }


        // TODO:重新说明
        // 一般来说，如果调用带用户名/密码参数的方法，如果用户名为空，则会默认使用当前进程的身份
        // 而调用不带用户名/密码参数的方法，则会先尝试使用对象的身份（手工设置的）；如果没有，才会默认使用当前进程的身份。





        #region reference

        #region user schema
        /*
        DirectoryEntry("LDAP://CN=User,CN=Schema,CN=Configuration,DC=maodou,DC=com", username, password);
        所有Properties:
        objectClass=top,classSchema,
        cn=User,
        distinguishedName=CN=User,CN=Schema,CN=Configuration,DC=maodou,DC=com,
        instanceType=4,
        whenCreated=2005/8/5 19:59:04,
        whenChanged=2005/8/5 19:59:04,
        uSNCreated=System.__ComObject,
        subClassOf=organizationalPerson,
        governsID=1.2.840.113556.1.5.9,
        mayContain=msDS-SourceObjectDN,msSFU30NisDomain,msSFU30Name,x500uniqueIdentifier,userSMIMECertificate,userPKCS12,uid,secretary,roomNumber,preferredLanguage,photo,labeledURI,jpegPhoto,homePostalAddress,givenName,employeeType,employeeNumber,displayName,departmentNumber,carLicense,audio,
        rDNAttID=cn,
        uSNChanged=System.__ComObject,
        showInAdvancedViewOnly=True,
        adminDisplayName=User,
        adminDescription=User,
        auxiliaryClass=shadowAccount,posixAccount,
        objectClassCategory=1,
        lDAPDisplayName=user,
        name=User,
        objectGUID=System.Byte[],
        schemaIDGUID=System.Byte[],
        systemOnly=False,
        systemPossSuperiors=builtinDomain,organizationalUnit,domainDNS,
        systemMayContain=pager,o,mobile,manager,mail,initials,homePhone,businessCategory,userCertificate,userWorkstations,userSharedFolderOther,userSharedFolder,userPrincipalName,userParameters,userAccountControl,unicodePwd,terminalServer,servicePrincipalName,scriptPath,pwdLastSet,profilePath,primaryGroupID,preferredOU,otherLoginWorkstations,operatorCount,ntPwdHistory,networkAddress,msRASSavedFramedRoute,msRASSavedFramedIPAddress,msRASSavedCallbackNumber,msRADIUSServiceType,msRADIUSFramedRoute,msRADIUSFramedIPAddress,msRADIUSCallbackNumber,msNPSavedCallingStationID,msNPCallingStationID,msNPAllowDialin,mSMQSignCertificatesMig,mSMQSignCertificates,mSMQDigestsMig,mSMQDigests,msIIS-FTPRoot,msIIS-FTPDir,msDS-User-Account-Control-Computed,msDS-Site-Affinity,mS-DS-CreatorSID,msDS-Cached-Membership-Time-Stamp,msDS-Cached-Membership,msDRM-IdentityCertificate,msCOM-UserPartitionSetLink,maxStorage,logonWorkstation,logonHours,logonCount,lockoutTime,localeID,lmPwdHistory,lastLogonTimestamp,lastLogon,lastLogoff,homeDrive,homeDirectory,groupsToIgnore,groupPriority,groupMembershipSAM,dynamicLDAPServer,desktopProfile,defaultClassStore,dBCSPwd,controlAccessRights,codePage,badPwdCount,badPasswordTime,adminCount,aCSPolicyName,accountExpires,
        systemAuxiliaryClass=securityPrincipal,mailRecipient,
        defaultSecurityDescriptor=D:(A;;RPWPCRCCDCLCLORCWOWDSDDTSW;;;DA)(A;;RPWPCRCCDCLCLORCWOWDSDDTSW;;;SY)(A;;RPWPCRCCDCLCLORCWOWDSDDTSW;;;AO)(A;;RPLCLORC;;;PS)(OA;;CR;ab721a53-1e2f-11d0-9819-00aa0040529b;;PS)(OA;;CR;ab721a54-1e2f-11d0-9819-00aa0040529b;;PS)(OA;;CR;ab721a56-1e2f-11d0-9819-00aa0040529b;;PS)(OA;;RPWP;77B5B886-944A-11d1-AEBD-0000F80367C1;;PS)(OA;;RPWP;E45795B2-9455-11d1-AEBD-0000F80367C1;;PS)(OA;;RPWP;E45795B3-9455-11d1-AEBD-0000F80367C1;;PS)(OA;;RP;037088f8-0ae1-11d2-b422-00a0c968f939;;RS)(OA;;RP;4c164200-20c0-11d0-a768-00aa006e0529;;RS)(OA;;RP;bc0ac240-79a9-11d0-9020-00c04fc2d4cf;;RS)(A;;RC;;;AU)(OA;;RP;59ba2f42-79a2-11d0-9020-00c04fc2d3cf;;AU)(OA;;RP;77B5B886-944A-11d1-AEBD-0000F80367C1;;AU)(OA;;RP;E45795B3-9455-11d1-AEBD-0000F80367C1;;AU)(OA;;RP;e48d0154-bcf8-11d1-8702-00c04fb96050;;AU)(OA;;CR;ab721a53-1e2f-11d0-9819-00aa0040529b;;WD)(OA;;RP;5f202010-79a5-11d0-9020-00c04fc2d4cf;;RS)(OA;;RPWP;bf967a7f-0de6-11d0-a285-00aa003049e2;;CA)(OA;;RP;46a9b11d-60ae-405a-b7e8-ff8a58d456d2;;S-1-5-32-560)(OA;;WPRP;6db69a1c-9422-11d1-aebd-0000f80367c1;;S-1-5-32-561),
        systemFlags=16,
        defaultHidingValue=False,
        objectCategory=CN=Class-Schema,CN=Schema,CN=Configuration,DC=maodou,DC=com,
        defaultObjectCategory=CN=Person,CN=Schema,CN=Configuration,DC=maodou,DC=com,
        nTSecurityDescriptor=System.__ComObject,
        */
        #endregion

        #region user properity
        /*
objectClass=top,person,organizationalPerson,user,
cn=ou1user1,
sn=ou1,
c=CN,
l=海淀,
st=北京,
title=职务,
description=描述,
postalCode=100083,
postOfficeBox=pox,
physicalDeliveryOfficeName=办公室,
telephoneNumber=电话号码,
facsimileTelephoneNumber=传真,
givenName=user1,
initials=u,
distinguishedName=CN=ou1user1,OU=OU1,DC=maodou,DC=com,
instanceType=4,
whenCreated=2007/8/14 3:43:54,
whenChanged=2007/8/15 16:45:55,
displayName=ou1user1,
uSNCreated=System.__ComObject,
info=电话注释,
uSNChanged=System.__ComObject,
co=中国,
department=部门,
company=公司,
streetAddress=知春路,
otherHomePhone=家庭电话号码其它,
directReports=CN=ou11user2,OU=OU11,OU=OU1,DC=maodou,DC=com,CN=ou11user1,OU=OU11,OU=OU1,DC=maodou,DC=com,
wWWHomePage=网页,
name=ou1user1,
objectGUID=System.Byte[],
userAccountControl=66048,
badPwdCount=0,
codePage=0,
countryCode=156,
badPasswordTime=System.__ComObject,
lastLogoff=System.__ComObject,
lastLogon=System.__ComObject,
pwdLastSet=System.__ComObject,
primaryGroupID=513,
objectSid=System.Byte[],
accountExpires=System.__ComObject,
logonCount=0,
sAMAccountName=ou1user1,
sAMAccountType=805306368,
userPrincipalName=ou1user1@maodou.com,
ipPhone=IP电话,
objectCategory=CN=Person,CN=Schema,CN=Configuration,DC=maodou,DC=com,
mail=mail@126.com,
manager=CN=sherwinzhu,CN=Users,DC=maodou,DC=com,
homePhone=家庭电话,
mobile=移动电话,
pager=寻呼机,
nTSecurityDescriptor=System.__ComObject,
        */
        #endregion

        #endregion
    }
}
