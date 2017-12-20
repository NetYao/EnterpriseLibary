using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Enterprises.Framework.Domain;
using Org.BouncyCastle.Asn1;

namespace Enterprises.Framework.Plugin.Domain.AD
{
    /// <summary>
    /// AD 帮助类，主要结合一些实际场景进行创建AD相关组织，组,人员等
    /// 一个通用demo
    /// </summary>
    public class AdHelper
    {
        private AdAdminConfig AdAdminConfig{ get; set; }

        private ExchangeAdminConfig ExchangeAdminConfig { get; set; }

        readonly OuMethodsMangement _ouMethodsSvc=new OuMethodsMangement();
        readonly AdMethodsAccountManagement _accountSvc;

        public AdHelper(AdAdminConfig adAdminConfig, ExchangeAdminConfig exchangeAdminConfig)
        {
            AdAdminConfig = adAdminConfig;
            ExchangeAdminConfig = exchangeAdminConfig;
            _accountSvc = new AdMethodsAccountManagement(AdAdminConfig);
        }

        public void SyncOuAndUsers(List<AdDepartment> depts,List<AdUser> users,string rootName="")
        {
            var rootContainer = !string.IsNullOrEmpty(rootName) ? CreateRootOu(rootName) : $"LDAP://{AdAdminConfig.ServerIpOrDomain}/{AdAdminConfig.DcContainer}";

            var parentDepts = depts.Where(p => p.ParentId == 0).ToList();
            foreach (var item in parentDepts)
            {
                if (_ouMethodsSvc.CreateOuChilren(rootContainer, item.Name, AdAdminConfig))
                {
                    var ldapPath = _ouMethodsSvc.CreateOuPath(rootContainer, item.Name, AdAdminConfig);
                    CreateOuUsers(ldapPath, users, item.Id);
                    CreateSubOuAndUsers(depts, users, ldapPath, item.Id);
                }
            }
        }

        private void CreateOuUsers(string ldapPath, List<AdUser> users, int deptId)
        {
            var deptUsers = users.Where(p => p.DepartmentId == deptId).ToList();
            foreach (var item in deptUsers)
            {
                var newUser=new DomainUser();
                newUser.User = new DomainUser.UserInfo()
                {
                    Description = item.UserName,
                    UserName = item.UserName,
                    OfficeEMailAddress = item.Email
                };
                newUser.Account = new DomainUser.AccountInfo()
                {
                    Password = "aa@123456",
                    UserAccount = item.UserCode,
                    SamAccountName = item.UserCode,
                    UserPrincipalName = $"{item.UserCode}@{AdAdminConfig.Ladp}"
                };

                DomainUtility.InsertUser(ldapPath, newUser,new DomainUser {
                    Account =
                        new DomainUser.AccountInfo() { UserAccount = AdAdminConfig.AdminAccount, Password = AdAdminConfig.AdminPwd }
                });
            }
        }

        private void CreateSubOuAndUsers(List<AdDepartment> depts, List<AdUser> users, string childrenContainer, int deptId)
        {
            var parentDepts = depts.Where(p => p.ParentId == deptId).ToList();
            foreach (var item in parentDepts)
            {
                if (_ouMethodsSvc.CreateOuChilren(childrenContainer, item.Name, AdAdminConfig))
                {
                    var ldapPath = _ouMethodsSvc.CreateOuPath(childrenContainer, item.Name, AdAdminConfig);
                    CreateOuUsers(ldapPath, users, item.Id);
                    CreateSubOuAndUsers(depts, users, ldapPath, item.Id);
                }
            }
        }

        private string CreateRootOu(string rootName)
        {
            var path = _ouMethodsSvc.CreateOuPath("", rootName, AdAdminConfig);
            if (!_ouMethodsSvc.CreateOu(path, AdAdminConfig))
            {
                throw new Exception($"创建{path}失败.");
            }

            return path;
        }

    }

    public class AdDepartment
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        public int ParentId { get; set; }
        public string Descrption { get; set; }
    }

    public class AdUser
    {
        public string Id { get; set; }
        public string UserName { get; set; }

        public string UserCode { get; set; }
        public string Email { get; set; }

        public int DepartmentId { get; set; }
    }
}
