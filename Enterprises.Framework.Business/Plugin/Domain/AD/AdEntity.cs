using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Exchange.WebServices.Data;

namespace Enterprises.Framework.Plugin.Domain.AD
{
    public class OuEntity
    {
        /// <summary>
        /// 全路径 如："LDAP://10.45.9.11/ou=离职员工,dc=ops,dc=net"
        /// </summary>
        public string LdapPath { get; set; }
        /// <summary>
        /// 组织单元名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 组织单元描述
        /// </summary>
        public string Descrption { get; set; }

        /// <summary>
        /// DC 信息（如：dc=ops,dc=net）也称根
        /// </summary>
        public string DcContainer { get; set; }
    }

    /// <summary>
    /// AD 帐号相关对象
    /// </summary>
    public class AdEntity
    {

        /// <summary>
        /// 如pphbh.com
        /// </summary>
        public string Ldap { get; set; }
        /// <summary>
        /// 组织OU
        /// </summary>
        public string OrganizationalUnit { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 登入名称 如：BG208418@pphbh.net
        /// </summary>
        public string UserPrincipalName { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Alias { get; set; }
        /// <summary>
        /// 账户
        /// </summary>
        public string SamAccountName { get; set; }
        /// <summary>
        /// 邮箱数据库
        /// </summary>
        public string Database { get; set; }
        /// <summary>
        /// 显示名
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 姓
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// 名字
        /// </summary>
        public string LastName { get; set; }
        /// <summary>
        /// 电话号码
        /// </summary>

        public string Phone { get; set; }


        public AdEntity GetInstance(PSObject psObject)
        {
            var adentity=new AdEntity();
            if (psObject.Properties["SamAccountName"] != null)
            {
                adentity.SamAccountName = psObject.Properties["SamAccountName"].Value.ToString();
            }
            else
            {
                return null;
            }

            if (psObject.Properties["Phone"] != null)
            {
                adentity.Phone = psObject.Properties["Phone"].Value.ToString();
            }

            if (psObject.Properties["LastName"] != null)
            {
                adentity.LastName = psObject.Properties["LastName"].Value.ToString();
            }

            if (psObject.Properties["FirstName"] != null)
            {
                adentity.FirstName = psObject.Properties["FirstName"].Value.ToString();
            }

            if (psObject.Properties["Name"] != null)
            {
                adentity.Name = psObject.Properties["Name"].Value.ToString();
            }

            if (psObject.Properties["Description"] != null)
            {
                adentity.Description = psObject.Properties["Description"].Value.ToString();
            }

            if (psObject.Properties["DisplayName"] != null)
            {
                adentity.DisplayName = psObject.Properties["DisplayName"].Value.ToString();
            }

            if (psObject.Properties["Alias"] != null)
            {
                adentity.Alias = psObject.Properties["Alias"].Value.ToString();
            }

            if (psObject.Properties["UserPrincipalName"] != null)
            {
                adentity.UserPrincipalName = psObject.Properties["UserPrincipalName"].Value.ToString();
            }

            if (psObject.Properties["OrganizationalUnit"] != null)
            {
                adentity.OrganizationalUnit = psObject.Properties["OrganizationalUnit"].Value.ToString();
            }

            return adentity;
        }
    }

    /// <summary>
    /// 邮件组
    /// </summary>
    public class ExchangeMailGroup
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// 组织OU 如：pphbh\RD
        /// </summary>
        public string OrganizationalUnit { get; set; }
        /// <summary>
        /// AD 服务器
        /// </summary>
        public string DomainController { get; set; }

        /// <summary>
        /// 显示名
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 人员 chris@contoso.com,michelle@contoso.com,laura@contoso.com,julia@contoso.com
        /// </summary>
        public string Members { get; set; }
        /// <summary>
        /// 类型 Distribution 通讯组   Security   安全组
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// 唯一名称 一般等于Name
        /// </summary>
        public string SamAccountName { get; set; }

        public ExchangeMailGroup GetInstance(PSObject psObject)
        {
            var entity = new ExchangeMailGroup();
            if (psObject.Properties["SamAccountName"] != null)
            {
                entity.SamAccountName = psObject.Properties["SamAccountName"].Value.ToString();
            }
            else
            {
                return null;
            }

            if (psObject.Properties["Notes"] != null)
            {
                entity.Notes = psObject.Properties["Notes"].Value.ToString();
            }

            if (psObject.Properties["Type"] != null)
            {
                entity.Type = psObject.Properties["Type"].Value.ToString();
            }

            if (psObject.Properties["DisplayName"] != null)
            {
                entity.DisplayName = psObject.Properties["DisplayName"].Value.ToString();
            }

            if (psObject.Properties["Name"] != null)
            {
                entity.Name = psObject.Properties["Name"].Value.ToString();
            }

            if (psObject.Properties["Alias"] != null)
            {
                entity.Alias = psObject.Properties["Alias"].Value.ToString();
            }

            if (psObject.Properties["OrganizationalUnit"] != null)
            {
                entity.OrganizationalUnit = psObject.Properties["OrganizationalUnit"].Value.ToString();
            }

            return entity;
        }
    }

    /// <summary>
    /// Exchange管理员配置
    /// </summary>
    public class ExchangeAdminConfig
    {
        /// <summary>
        /// 如pphbh.com
        /// </summary>
        public string Ldap { get; set; }
        /// <summary>
        /// 远程服务器ID或者域名
        /// </summary>
        public string ServerIpOrDomain { get; set; }
        /// <summary>
        /// 端口
        /// </summary>
        public int SSlPoint { get; set; }
        /// <summary>
        /// 管理员密码
        /// </summary>
        public string AdminPwd { get; set; }

        /// <summary>
        /// 管理员帐号
        /// </summary>
        public string AdminAccount { get; set; }
        /// <summary>
        /// 域服务（多域情况制定一台域控服务器名称）
        /// </summary>

        public string DomainController { get; set; }

        /// <summary>
        /// 获取默认实例
        /// </summary>
        /// <returns></returns>
        public ExchangeAdminConfig GetDefaultInstance(string ladp)
        {
            Ldap = ladp;
            if (ladp.ToLower() == "ops.net")
            {
                return new ExchangeAdminConfig
                {
                    ServerIpOrDomain = "ex-cas1.ops.net",
                    SSlPoint = 443,
                    AdminPwd = "pphbh@com",
                    AdminAccount = @"ops\administrator"
                };
            }

            return new ExchangeAdminConfig
            {
                ServerIpOrDomain = ConfigurationManager.AppSettings["ExchangeDomainIp"],
                SSlPoint = int.Parse(ConfigurationManager.AppSettings["ExchangeSSlPoint"]),
                AdminAccount = ConfigurationManager.AppSettings["ExchangeAdmin"],
                DomainController = ConfigurationManager.AppSettings["DomainController"],
                AdminPwd = ConfigurationManager.AppSettings["ExchangePwd"]
            };
        }
    }


    /// <summary>
    /// Ad管理员配置
    /// </summary>
    public class AdAdminConfig
    {
        /// <summary>
        /// Ladp 如："pphbh.net",
        /// </summary>
        public string Ladp { get; set; }
        /// <summary>
        /// 域服务器 域名或者ip地址
        /// </summary>
        public string ServerIpOrDomain { get; set; }

        /// <summary>
        /// DCPath 如： "DC=pphbh,DC=net"
        /// </summary>
        public string DcContainer { get; set; }


        /// <summary>
        /// 管理员密码
        /// </summary>
        public string AdminPwd { get; set; }

        /// <summary>
        /// 管理员帐号
        /// </summary>
        public string AdminAccount { get; set; }

        /// <summary>
        /// 默认OU
        /// </summary>
        public string DefaultOu { get; set; }

        /// <summary>
        /// 获取默认实例
        /// </summary>
        /// <returns></returns>
        public AdAdminConfig GetDefaultInstance(string ladp)
        {
            if (ladp.ToLower() == "ops.net")
            {
                return new AdAdminConfig
                {
                    Ladp = "ops.net",
                    ServerIpOrDomain = "10.45.9.11",
                    DcContainer = "DC=ops,DC=net",
                    AdminPwd = "pphbh@com",
                    AdminAccount = @"ops\administrator"
                };
            }

            return new AdAdminConfig
            {
                Ladp = ladp,
                ServerIpOrDomain = "pphbh.net",
                DcContainer = "DC=pphbh,DC=net",
                AdminPwd = ConfigurationManager.AppSettings["AdAdminPwd"],
                AdminAccount = ConfigurationManager.AppSettings["AdAdminAccount"],
            };
        }
    }


    public class KeyValue
    {
        public string Key { get; set; }

        public string Value { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public KeyValue()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public KeyValue(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }

    /// <summary>
    /// 会议信息
    /// </summary>
    public class MeetInfoEntity
    {
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndDate { get; set; }
        /// <summary>
        /// 主题
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// 发布者
        /// </summary>
        public string Publisher { get; set; }
    }

    /// <summary>
    /// 会议室信息
    /// </summary>
    public class MeetRoomEntity
    {
        /// <summary>
        /// 查询开始时间
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// 查询结束时间
        /// </summary>

        public DateTime EndDate { get; set; }
        /// <summary>
        /// 会议室名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 会议室邮箱
        /// </summary>
        public string MeetEmail { get; set; }
        /// <summary>
        /// 会议记录
        /// </summary>
        public List<MeetInfoEntity> Meetings { get; set; }
    }

    /// <summary>  
    /// 会议属性  
    /// </summary>  
    public class AppointmentProperty
    {
        /// <summary>  
        /// 会议ID  
        /// </summary>  
        public ItemId ID { get; set; }
        /// <summary>  
        /// 会议标题  
        /// </summary>  
        public string Subject { get; set; }
        /// <summary>  
        /// 会议内容  
        /// </summary>  
        public string Body { get; set; }
        /// <summary>  
        /// 开始时间  
        /// </summary>  
        public DateTime Start { get; set; }
        /// <summary>  
        /// 结束时间  
        /// </summary>  
        public DateTime End { get; set; }
        /// <summary>  
        /// 会议地点  
        /// </summary>  
        public string Location { get; set; }
        /// <summary>  
        /// 与会人员（含会议室邮箱）  
        /// </summary>  
        public List<string> Attendees { get; set; }

        
    }
}
