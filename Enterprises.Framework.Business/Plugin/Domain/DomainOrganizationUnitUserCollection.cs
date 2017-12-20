using System;
using System.Collections;
using System.Data;

namespace Enterprises.Framework.Domain
{
    /// <summary>
    /// 组织单元中域用户信息
    /// </summary>
    [Serializable]
    public class DomainOrganizationUnitUserCollection : CollectionBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ouUser"></param>
        /// <returns></returns>
        public DomainOrganizationUnitUserCollection Add(DomainOrganizationUnitUser ouUser)
        {
            if (!List.Contains(ouUser))
            {
                List.Add(ouUser);
            }
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ouUser"></param>
        /// <returns></returns>
        public DomainOrganizationUnitUserCollection Remove(DomainOrganizationUnitUser ouUser)
        {
            if (List.Contains(ouUser))
            {
                List.Remove(ouUser);
            }
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ouUser"></param>
        /// <returns></returns>
        public bool Contains(DomainOrganizationUnitUser ouUser)
        {
            return List.Contains(ouUser);
        }

        /// <summary>
        ///  [获取]DomainUser对象
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public DomainOrganizationUnitUser this[int index]
        {
            get
            {
                if (index >= 0 && index < this.List.Count)
                {
                    return (DomainOrganizationUnitUser)List[index];
                }
                throw new IndexOutOfRangeException(string.Format("索引[index={0}]超出了数组界限", index));
            }
        }

        /// <summary>
        /// 把当前组织单元和用户集合转换成数据表
        /// </summary>
        public DataTable ToDataTable()
        {            
            DataTable dtOUSchema = DomainOrganizationUnit.ToDataTableSchema(); 
            DataTable dtUserSchema = DomainUser.ToDataTableSchema();  

            DataTable dtResult = new DataTable();
            dtResult.TableName = "组织单元用户表";
            foreach (DataColumn col in dtOUSchema.Columns)
            {
                col.ColumnName = "OU_" + col.ColumnName;
                dtResult.Columns.Add(new DataColumn(){ColumnName = col.ColumnName,Caption = col.Caption});
            }
            foreach (DataColumn col in dtUserSchema.Columns)
            {
                col.ColumnName = "USER_" + col.ColumnName;
                dtResult.Columns.Add(new DataColumn(){ColumnName = col.ColumnName,Caption = col.Caption});
            }
            

            foreach (DomainOrganizationUnitUser ouUser in List)
            {
                DataTable dtResultTemp = dtResult.Clone();

                DataTable dtOU = ouUser.OU.ToDataTable();
                DataTable dtUsers = ouUser.Users.ToDataTable();

                if (dtUsers.Rows.Count == 0)
                {
                    continue;
                }

                foreach (DataRow drUser in dtUsers.Rows)
                {
                    DataRow drNew = dtResultTemp.NewRow();

                    foreach (DataRow drOU in dtOU.Rows)
                    {
                        foreach (DataColumn col in dtOU.Columns)
                        {
                            drNew["OU_" + col.ColumnName] = drOU[col];
                        }
                    }
                    
                    foreach (DataColumn col in dtUsers.Columns)
                    {
                        drNew["USER_" + col.ColumnName] = drUser[col];
                    }                   

                    dtResultTemp.Rows.Add(drNew);
                }

                dtResult.Merge(dtResultTemp);
            }

            return dtResult;
        }

        
    }
}