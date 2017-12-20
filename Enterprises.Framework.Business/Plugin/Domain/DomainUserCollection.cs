using System;
using System.Collections;
using System.Data;

namespace Enterprises.Framework.Domain
{
    /// <summary>
    /// 域用户信息集合
    /// </summary>
    [Serializable]
    public class DomainUserCollection : CollectionBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int Add(DomainUser user)
        {
            if (List.Contains(user))
            {
                throw new Exception(string.Format("Has exists the object:{0}", user.Account.UserAccount));
            }
            return List.Add(user);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        public void Remove(DomainUser user)
        {
            if (List.Contains(user))
            {
                List.Remove(user);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool Contains(DomainUser user)
        {
            return List.Contains(user);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userPrincipalName"></param>
        /// <returns></returns>
        public bool Contains(string userPrincipalName)
        {
            foreach (DomainUser item in this)
            {
                if (item.Account.UserPrincipalName == userPrincipalName)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///  [获取]DomainUser对象
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public DomainUser this[int index]
        {
            get
            {
                if (index >= 0 && index < List.Count)
                {
                    return (DomainUser)List[index];
                }
                throw new IndexOutOfRangeException(string.Format("索引[index={0}]超出了数组界限", index));
            }
        }

        /// <summary>
        /// [获取]DomainUser对象
        /// </summary>
        /// <param name="accountName">登陆帐号</param>
        /// <returns></returns>
        public DomainUser this[string accountName]
        {
            get
            {
                foreach (DomainUser user in List)
                {
                    if (user.Account.UserAccount.ToUpper().Trim() == accountName.ToUpper().Trim())
                    {
                        return user;
                    }
                }

                return null;
            }
        }

       
      
       
        /// <summary>
        /// 把AD用户集合转化为DataTable,表名为USER
        /// </summary>
        /// <returns></returns>
        public DataTable ToDataTable()
        {
            DataTable dt = DomainUser.ToDataTableSchema();
            foreach (DomainUser user in List)
            {
                DataRow dr = dt.NewRow();
                user.FillDataRow(dr);
                dt.Rows.Add(dr);
            }
            return dt;
        }

        

        /// <summary>
        /// 修改集合中域用户的指定信息.选择属性名时，可以用类DomainUserPropertities中的对象来标示
        /// </summary>
        /// <param name="domainUserPropertityNames"></param>
        public void ModifyInfo(string[] domainUserPropertityNames)
        {
            foreach (DomainUser user in this)
            {
                user.ModifyInfo(domainUserPropertityNames);
            }
        }
    }
}
