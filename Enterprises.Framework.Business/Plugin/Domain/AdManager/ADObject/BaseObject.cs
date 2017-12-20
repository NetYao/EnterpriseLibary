
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using Enterprises.Framework.Plugin.Domain.AdManager.Enum;
using Enterprises.Framework.Plugin.Domain.AdManager.Exception;

namespace Enterprises.Framework.Plugin.Domain.AdManager.ADObject
{
    /// <summary>
    /// 定义AD对象的基类。
    /// </summary>
    public class BaseObject
    {
        #region PROPERTY Const

        public const string PROPERTY_OBJECTCLASS    = "objectClass";            // 类型,string[]；eg:user is {"top","person","organizationalPerson","user"}
        public const string PROPERTY_OBJECTCATEGORY = "objectCategory";         // 类别,string；eg:user is "CN=Person,CN=Schema,CN=Configuration,DC=maodou,DC=com"

        public const string PROPERTY_NAME           = "name";                   // RDN,不能通过直接赋值修改,string

        public const string PROPERTY_OBJECTGUID     = "objectGUID";             // GUID,byte[16]。按字节转化为16进制，即NativeGuid，可用于与AD绑定；可转换为Guid结构。
        public const string PROPERTY_OBJECTSID      = "objectSid";              // SID,byte[28]
        public const string PROPERTY_DN             = "distinguishedName";      // DN,string

        public const string PROPERTY_WHENCREATED    = "whenCreated";            // 创建时间,DateTime
        public const string PROPERTY_WHENCHANGED    = "whenChanged";            // 更新时间,DateTime
        public const string PROPERTY_INSTANCETYPE   = "instanceType";           // int；eg:user is 4
        public const string PROPERTY_USNCREATED     = "uSNCreated";             // ?,IADsLargeInteger
        public const string PROPERTY_USNCHANGED     = "uSNChanged";             // ?,IADsLargeInteger
        public const string PROPERTY_SECURITY       = "nTSecurityDescriptor";   // ?,?

        #endregion


        #region fields

        /// <summary>
        /// Guid
        /// </summary>
        protected Guid guid;
        /// <summary>
        /// SID
        /// </summary>
        protected byte[] objectSid;
        //Distinguished Name
        private string dn;
        /// <summary>
        /// Object Class
        /// </summary>
        protected string[] objectClass;
        /// <summary>
        /// ObjectCategory DN
        /// </summary>
        protected string objectCategory;
        /// <summary>
        /// Name
        /// </summary>
        protected string name;
        /// <summary>
        /// SchemaClass Name（对应的枚举）
        /// </summary>
        protected SchemaClass schema;
        // path
        private string path;

        private DateTime whenCreated;

        // guid:227451df-dbe1-4988-9ed6-1523cdfcece9
        // nativeguid:df517422e1db88499ed61523cdfcece9
        #endregion


        #region properity

        /// <summary>
        /// Guid
        /// </summary>
        public Guid Guid
        {
            get { return guid; }
            set { guid = value; }
        }

        /// <summary>
        /// NativeGuid，用于与LDAP绑定查询（按GUID）
        /// </summary>
        public string NativeGuid
        {
            get { return Utils.ConvertGuidToNativeGuid(this.guid); }
        }

        /// <summary>
        /// ObjectSID
        /// </summary>
        public byte[] ObjectSid
        {
            get { return objectSid; }
            set { objectSid = value; }
        }

        /// <summary>
        /// ObjectSID的16进制字符串形式，用于与LDAP绑定查询（按ObjectSid）
        /// </summary>
        public string OctetObjectSid
        {
            get { return Utils.ConvertToOctetString(objectSid); }
        }

        /// <summary>
        /// Distinguished Name。已转义反斜杠/字符
        /// </summary>
        /// <remarks>不允许直接设置。</remarks>
        public string Dn
        {
            get { return dn; }
            set 
            { 
                //dn = value; 
            }
        }

        /// <summary>
        /// ADsPath，The fully qualified path 
        /// </summary>
        public string Path
        {
            get { return this.path; }
        }

        /// <summary>
        /// 由DN构成的ADsPath
        /// </summary>
        public string DNPath
        {
            get { return ParaMgr.LDAP_IDENTITY + this.dn; }
        }

        /// <summary>
        /// Name
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// SchemaClassName
        /// </summary>
        public string SchemaClassName
        {
            get
            {
                return this.schema.ToString("F");
            }
        }

        /// <summary>
        /// ObjectCategory Name
        /// </summary>
        public string ObjectCategory
        {
            get
            {
                return Utils.GetRDNValue(this.objectCategory);
            }
        }

        /// <summary>
        /// 获取创建时间
        /// </summary>
        public DateTime WhenCreated
        {
            get
            {
                return this.whenCreated;
            }
        }

        #endregion


        #region .ctors

        /// <summary>
        /// 默认构造函数，不允许直接创建对象。
        /// </summary>
        protected BaseObject()
        { }

        #endregion


        /// <summary>
        /// 解析DirectoryEntry对象。
        /// </summary>
        /// <param name="entry">DirectoryEntry对象。</param>
        protected virtual void Parse(DirectoryEntry entry)
        {
            this.ParsePrivate(entry);
        }

        private void ParsePrivate(DirectoryEntry entry)
        {
            this.guid = entry.Guid;
            this.dn = Utils.EscapeDNBackslashedChar(Utils.GetProperty(entry, BaseObject.PROPERTY_DN));     // 转义反斜杠'/'字符
            this.path = entry.Path;
            this.name = Utils.GetProperty(entry, BaseObject.PROPERTY_NAME);

            List<string> ocList = new List<string>();
            foreach (object oc in (object[])(entry.Properties[BaseObject.PROPERTY_OBJECTCLASS].Value))
            {
                ocList.Add(oc.ToString());
            }
            this.objectClass = ocList.ToArray();
            this.objectCategory = Utils.GetProperty(entry, BaseObject.PROPERTY_OBJECTCATEGORY);

            this.schema = (SchemaClass)(System.Enum.Parse(typeof(SchemaClass), entry.SchemaClassName));

            // OU 没有objectSid
            if (schema == SchemaClass.user || schema == SchemaClass.group || schema == SchemaClass.builtinDomain ||
                schema == SchemaClass.computer || schema == SchemaClass.domainDNS)
                this.objectSid = (byte[])(entry.Properties[BaseObject.PROPERTY_OBJECTSID].Value);
            else
                this.objectSid = new byte[] { };
        }

        /// <summary>
        /// 解析DirectoryEntry对象。
        /// </summary>
        /// <param name="entry">DirectoryEntry对象。</param>
        /// <param name="demandSchema">要求的对象SchemaClass，以便检验对象类型。</param>
        protected void Parse(DirectoryEntry entry, SchemaClass demandSchema)
        {
            // this.Parse(entry);      // ERROR -- 如何调用this的

            this.ParsePrivate(entry);

            if (this.schema != demandSchema)
            {
                entry.Close();
                entry.Dispose();

                throw new SchemaClassException("对象类型不是" + demandSchema.ToString("F"));
            }
        }

        /// <summary>
        /// 解析SearchResult对象。
        /// </summary>
        /// <param name="result">SearchResult对象。应当包括必要的属性。</param>
        /// <param name="demandSchema">要求的对象SchemaClass，以便检验对象类型。</param>
        protected void Parse(SearchResult result, SchemaClass demandSchema)
        {
            string nativeGuid = "";
            foreach (byte g in (byte[])result.Properties["objectguid"][0])
            {
                nativeGuid += g.ToString("x2");
            }
            this.guid = Utils.ConvertNativeGuidToGuid(nativeGuid);
            this.dn = Utils.EscapeDNBackslashedChar(Utils.GetProperty(result, "distinguishedname"));     // 转义反斜杠'/'字符

            this.path = Utils.GetProperty(result, "adspath");
            this.name = Utils.GetProperty(result, "name");

            List<string> ocList = new List<string>();
            foreach (object oc in result.Properties["objectclass"])
            {
                ocList.Add(oc.ToString());
            }
            this.objectClass = ocList.ToArray();
            this.objectCategory = Utils.GetProperty(result, "objectcategory");

            this.schema = SchemaClass.none;
            foreach(string oc in this.objectClass)
            {
                // 暂时只分析这三个
                switch(oc)
                {
                    case "organizationalUnit":
                        this.schema = SchemaClass.organizationalUnit;
                        break;
                    case "group":
                        this.schema = SchemaClass.group;
                        break;
                    case "user":
                        this.schema = SchemaClass.user;
                        break;

                }
                if (this.schema != SchemaClass.none)
                    break;
            }

            // OU 没有objectSid
            if (schema == SchemaClass.user || schema == SchemaClass.group)
                this.objectSid = (byte[])(result.Properties["objectsid"][0]);
            else
                this.objectSid = new byte[] { };


            this.whenCreated = DateTime.Parse(Utils.GetProperty(result, "whenCreated"));

            if (this.schema != demandSchema)
            {
                throw new SchemaClassException("对象类型不是" + demandSchema.ToString("F"));
            }
        }

        ///<summary>
        ///移动DirectoryEntry到指定位置。
        ///</summary>
        ///<param name="newLocationPath">移动到的位置的ADsPath</param>
        ///<param name="userName">用户身份标识--用户名。为空时使用默认用户身份标识。</param>
        ///<param name="password">用户身份标识--密码。</param>
        public virtual void Move(string newLocationPath, string userName, string password)
        {
            DirectoryEntry de = ADManager.GetByDN(this.dn, userName, password);     // 必须用DN -- 见ADManager.Move方法说明

            ADManager.Move(de, newLocationPath, userName, password);

            this.Parse(de);

            de.Close();
            de.Dispose();
        }

        /// <summary>
        /// 移动DirectoryEntry到指定位置，使用默认用户身份标识。
        /// </summary>
        /// <param name="newLocationPath">移动到的位置的ADsPath</param>
        public virtual void Move(string newLocationPath)
        {
            this.Move(newLocationPath, null, null);
        }


        #region Identity

        /// <summary>
        /// 设置执行对象操作时使用的默认用户身份标识
        /// </summary>
        /// <param name="userName">用户身份标识--用户名。</param>
        /// <param name="password">用户身份标识--密码。</param>
        public void SetIdentity(string userName, string password)
        {
            this.hasIdentity = true;
            this.iUserName = userName;
            this.iPassword = password;
        }

        /// <summary>
        /// 清除执行对象操作时使用的默认用户身份标识
        /// 清除时使用执行进程的身份标识。
        /// </summary>
        public void ClearIdentity()
        {
            this.hasIdentity = false;
            this.iUserName = null;
            this.iPassword = null;
        }
        protected bool hasIdentity = false;
        protected string iUserName = null;
        protected string iPassword = null;

        #endregion
  
    }
}
