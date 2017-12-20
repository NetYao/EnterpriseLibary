using System;
using System.DirectoryServices;
using Enterprises.Framework.Plugin.Domain.AdManager.Enum;

namespace Enterprises.Framework.Plugin.Domain.AdManager.ADObject
{
    /// <summary>
    /// AD Domain
    /// </summary>
    public class Domain : BaseObject
    {
        public const string PROPERTY_DC = "dc";                                // Common name,string, RDN
        public const string PROPERTY_DESCRIPTION = "description";              // 描述

        private string description;


        /// <summary>
        /// 描述
        /// </summary>
        public string Description
        {
            get { return description; }
            set { description = value; }
        }


        #region ctors

        /// <summary>
        ///  默认构函数。
        /// </summary>
        public Domain()
        {
        }
        
        /// <summary>
        /// 构造函数，根据DirectoryEntry对象进行构造。
        /// </summary>
        /// <param name="entry">DirectoryEntry对象。</param>
        internal Domain(DirectoryEntry entry)
        {
            if (entry == null)
                throw new ArgumentNullException();

            this.Parse(entry);
        }

        #endregion


        /// <summary>
        /// 解析DirectoryEntry对象。
        /// </summary>
        /// <param name="entry">DirectoryEntry对象。</param>
        protected override void Parse(DirectoryEntry entry)
        {
            base.Parse(entry, SchemaClass.domainDNS);

            this.description = Utils.GetProperty(entry, Domain.PROPERTY_DESCRIPTION);
        }



        #region reference

        #region domainDNS properity

        /* 
         * 
        objectClass=top,domain,domainDNS,
        distinguishedName=DC=maodou,DC=com,
        instanceType=5,
        whenCreated=2007/8/10 8:08:33,
        whenChanged=2007/8/11 2:11:10,
        subRefs=DC=ForestDnsZones,DC=maodou,DC=com,DC=DomainDnsZones,DC=maodou,DC=com,CN=Configuration,DC=maodou,DC=com,
        uSNCreated=System.__ComObject,
        uSNChanged=System.__ComObject,
        name=maodou,
        objectGUID=System.Byte[],
        creationTime=System.__ComObject,
        forceLogoff=System.__ComObject,
        lockoutDuration=System.__ComObject,
        lockOutObservationWindow=System.__ComObject,
        lockoutThreshold=0,
        maxPwdAge=System.__ComObject,
        minPwdAge=System.__ComObject,
        minPwdLength=7,
        modifiedCountAtLastProm=System.__ComObject,
        nextRid=1007,
        pwdProperties=1,
        pwdHistoryLength=24,
        objectSid=System.Byte[],
        serverState=1,
        uASCompat=1,
        modifiedCount=System.__ComObject,
        auditingPolicy=System.Byte[],
        nTMixedDomain=1,
        rIDManagerReference=CN=RID Manager$,CN=System,DC=maodou,DC=com,
        fSMORoleOwner=CN=NTDS Settings,CN=XINXIN-PCS,CN=Servers,CN=Default-First-Site-Name,CN=Sites,CN=Configuration,DC=maodou,DC=com,
        systemFlags=-1946157056,
        wellKnownObjects=System.__ComObject,System.__ComObject,System.__ComObject,System.__ComObject,System.__ComObject,System.__ComObject,System.__ComObject,System.__ComObject,System.__ComObject,System.__ComObject,System.__ComObject,
        objectCategory=CN=Domain-DNS,CN=Schema,CN=Configuration,DC=maodou,DC=com,
        isCriticalSystemObject=True,
        gPLink=[LDAP://CN={31B2F340-016D-11D2-945F-00C04FB984F9},CN=Policies,CN=System,DC=maodou,DC=com;0],
        masteredBy=CN=NTDS Settings,CN=XINXIN-PCS,CN=Servers,CN=Default-First-Site-Name,CN=Sites,CN=Configuration,DC=maodou,DC=com,
        ms-DS-MachineAccountQuota=10,
        msDS-Behavior-Version=0,
        msDS-PerUserTrustQuota=1,
        msDS-AllUsersTrustQuota=1000,
        msDS-PerUserTrustTombstonesQuota=10,
        msDs-masteredBy=CN=NTDS Settings,CN=XINXIN-PCS,CN=Servers,CN=Default-First-Site-Name,CN=Sites,CN=Configuration,DC=maodou,DC=com,
        dc=maodou,
        nTSecurityDescriptor=System.__ComObject,
            
        */

        #endregion

        #endregion

    }
}
