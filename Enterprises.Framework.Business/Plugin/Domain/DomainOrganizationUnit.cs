using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.DirectoryServices;
using System.Net;
using System.Xml;
using System.Web;
using System.Data;

namespace Enterprises.Framework.Domain
{
    /// <summary>
    /// 域组织单元信息
    /// </summary>
    [Serializable]
    public class DomainOrganizationUnit
    {

        /// <summary>
        /// 地址
        /// </summary>
        [Serializable]
        public class AddressInfo
        {
            /// <summary>
            /// 国家
            /// </summary>
            public class CountryInfo
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
            /// 邮政编码
            /// </summary>
            public string PostCode = string.Empty;

        }

        /// <summary>
        /// 对象信息
        /// </summary>
        [Serializable]
        public class ObjectInfo
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
        }

        /// <summary>
        /// 域组织单元的名称
        /// </summary>
        public string Name = string.Empty;

        /// <summary>
        /// 域组织单元的路径
        /// </summary>
        public string Path = string.Empty;

        /// <summary>
        /// 域组织单元的上级节点的路径
        /// </summary>
        public string ParentPath = string.Empty;

        /// <summary>
        /// 域组织单元的上级节点的名称
        /// </summary>
        public string ParentName = string.Empty;

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime WhenCreated = DateTime.MinValue;

        /// <summary>
        /// 变动日期
        /// </summary>
        public DateTime WhenChanged = DateTime.MinValue;

        /// <summary>
        /// 组织经理
        /// </summary>
        public DomainUser Manager = null;

        /// <summary>
        /// 组织经理LDAP路径
        /// </summary>
        public string ManagerPath = string.Empty;

        /// <summary>
        /// 修改日期
        /// </summary>
        public DateTime ModifyTimeStamp = DateTime.MinValue;


        /// <summary>
        /// [获取]加密后的LDAP路径，比如有些时候需要在前台传递Path时，可以采用这个方式。
        /// </summary>
        public string EncryptedPath
        {
            get
            {
                return DomainUtility.EncryptPath(Path);
            }
        }

        /// <summary>
        /// 地址
        /// </summary>
        public AddressInfo Address = new AddressInfo();

        /// <summary>
        /// 对象
        /// </summary>
        public ObjectInfo Object = new ObjectInfo();

        /// <summary>
        /// 创建导出表的架构
        /// </summary>
        /// <returns></returns>
        public static DataTable ToDataTableSchema()
        {
            DataTable dt = new DataTable("OU");
            
            #region 导出表的字段信息

            dt.Columns.Add(new DataColumn { ColumnName = DomainOrganizationUnitPropertities.Name, Caption = DomainOrganizationUnitPropertitiesDescription.Name });
            dt.Columns.Add(new DataColumn { ColumnName = DomainOrganizationUnitPropertities.Path, Caption = DomainOrganizationUnitPropertitiesDescription.Path });
            dt.Columns.Add(new DataColumn { ColumnName = DomainOrganizationUnitPropertities.ParentName, Caption = DomainOrganizationUnitPropertitiesDescription.ParentName });
            dt.Columns.Add(new DataColumn { ColumnName = DomainOrganizationUnitPropertities.ParentPath, Caption = DomainOrganizationUnitPropertitiesDescription.ParentPath });
            dt.Columns.Add(new DataColumn { ColumnName = DomainOrganizationUnitPropertities.Manager, Caption = DomainOrganizationUnitPropertitiesDescription.Manager });
            dt.Columns.Add(new DataColumn { ColumnName = DomainOrganizationUnitPropertities.ManagerPath, Caption = DomainOrganizationUnitPropertitiesDescription.ManagerPath });
            
            #endregion

            return dt;
        }
        
        /// <summary>
        /// 把当前对象导出到表
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
        /// 填充数据行
        /// </summary>
        /// <param name="dr"></param>
        internal void FillDataRow(DataRow dr)
        {
            DomainOrganizationUnit ou = this;

            dr[DomainOrganizationUnitPropertities.Name] = ou.Name;
            dr[DomainOrganizationUnitPropertities.Path] = ou.Path;
            dr[DomainOrganizationUnitPropertities.ParentName] = ou.ParentName;
            dr[DomainOrganizationUnitPropertities.ParentPath] = ou.ParentPath;
            dr[DomainOrganizationUnitPropertities.Manager] = ou.Manager;
            dr[DomainOrganizationUnitPropertities.ManagerPath] = ou.ManagerPath;


        }
        
    }
}
