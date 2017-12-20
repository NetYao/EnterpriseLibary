using Microsoft.VisualStudio.TestTools.UnitTesting;
using Enterprises.Framework.Plugin.Domain.AD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Enterprises.Framework.Domain;

namespace Enterprises.Framework.Plugin.Domain.AD.Tests
{
    [TestClass()]
    public class AdTest
    {
        OuMethodsMangement svc = new OuMethodsMangement();
       
        AdAdminConfig config = new AdAdminConfig().GetDefaultInstance("ops.net");
        ExchangeAdminConfig exconfig = new ExchangeAdminConfig().GetDefaultInstance("ops.net");


        [TestMethod()]
        public void GetRootContainerTest()
        {
            Console.Write(svc.GetRootContainer("LDAP://10.45.9.11/ou=HR,ou=离职员工,dc=ops,dc=net"));
            Console.Write(svc.GetContainer("LDAP://10.45.9.11/ou=HR,ou=离职员工,dc=ops,dc=net"));
        }


        [TestMethod]
        public void TestMethod1()
        {
            Console.Write(svc.Rename("LDAP://10.45.9.11/ou=HR,ou=离职员工,dc=ops,dc=net", "RD", config));
        }

        [TestMethod()]
        public void GetOuLevelTest()
        {
            var list = svc.GetOuLevels("LDAP://10.45.9.11/ou=HR,Ou=新政,ou=离职员工,ou=AllUsers,dc=ops,dc=net");
            foreach (var item in list)
            {
                Console.WriteLine(item);
            }

            var list2 = svc.GetOuLevels("LDAP://10.45.9.11/ou=离职员工,DC=ops,DC=net");
            foreach (var item in list2)
            {
                Console.WriteLine(item);
            }
        }

        [TestMethod()]
        public void IsExistsOuTest()
        {
            Console.WriteLine(svc.IsExistsOu("LDAP://10.45.9.11/ou=离职员工,dc=ops,dc=net", config));
            Console.WriteLine(svc.IsExistsOu("LDAP://10.45.9.11/dc=ops,dc=net", config));
            Console.WriteLine(svc.IsExistsOu("LDAP://10.45.9.11/ou=RD,ou=离职员工,dc=ops,dc=net", config));
            Console.WriteLine(svc.IsExistsOu("LDAP://10.45.9.11/ou=HT,ou=离职员工,dc=ops,dc=net", config));

        }

        [TestMethod()]
        public void CreateOuTest()
        {
            Console.WriteLine(svc.CreateOu("LDAP://10.45.9.11/ou=HR,ou=总部事业部,ou=离职员工,dc=ops,dc=net", config));
        }

        [TestMethod()]
        public void MoveToTest()
        {
            svc.MoveTo("LDAP://10.45.9.11/cn=Yong Luo,ou=110_Marketing,ou=pphbh.com,dc=ops,dc=net",
                "LDAP://10.45.9.11/ou=110_Marketing,ou=离职员工,dc=ops,dc=net", "", config);
        }


        [TestMethod()]
        public void InsertUserTest()
        {
            DomainUser newUser=new DomainUser();
            newUser.User=new DomainUser.UserInfo()
            {
                UserName = "Liu dehua",
                Name = "liu dehua",
                DisplayName = "liu dehua",
                Description = "刘德华",
                GivenName = "DeHua",
                LastName = "Liu",
                OfficeEMailAddress = "liudehua@ops.net"
            };
            newUser.Account=new DomainUser.AccountInfo()
            {
               UserAccount = @"BG00012@ops.net",
               Password = "aa@123456",
               SamAccountName = "BG00012",
               UserPrincipalName = "BG00012@ops.net"
            };

            DomainUtility.InsertUser("LDAP://10.45.9.11/ou=102_GMO,ou=pphbh.com,dc=ops,dc=net", newUser,
                new DomainUser()
                {
                    Account =
                        new DomainUser.AccountInfo() {UserAccount = config.AdminAccount, Password = config.AdminPwd}
                });
        }

        [TestMethod()]
        public void EnableMailBox()
        {
            var entity=new AdEntity() {SamAccountName = "BG00012",Alias = "liudehua" };
            ExchangePowerShellHelper.EnableUserMailBox(entity, exconfig);
        }

        [TestMethod()]
        public void NewDistributionGroupTest()
        {
            var entity = new ExchangeMailGroup { SamAccountName = "RDManager", Alias = "RDManager" , DisplayName = "RD部门经理",Name = "RD部门经理",OrganizationalUnit = "ops.net/pphbh.com/103_RD"};
            ExchangePowerShellHelper.NewDistributionGroup(entity,null, exconfig);
        }

        [TestMethod()]
        public void AddDistributionGroupMemberTest()
        {
            List<string> members=new List<string>() { "BG316923", "BG316921", "BG306923" };
            ExchangePowerShellHelper.AddDistributionGroupMember("RDManager", "BG316923", exconfig);
        }

        [TestMethod()]
        public void GetDistributionGroupMemberTest()
        {
            var users=ExchangePowerShellHelper.GetDistributionGroupMember("RDManager", exconfig);
            Console.Write(Newtonsoft.Json.JsonConvert.SerializeObject(users));
        }

        [TestMethod()]
        public void EnableDistributionGroup()
        {
            var entity = new ExchangeMailGroup { SamAccountName = "RD_QD", Alias = "RD_QD", DisplayName = "RD前端UI移动小组",Notes = "高级工程师", Name = "RD_QD" };
            var users = ExchangePowerShellHelper.EnableDistributionGroup(entity, exconfig);
            Console.Write(Newtonsoft.Json.JsonConvert.SerializeObject(users));
        }

        [TestMethod()]
        public void ConverToLdapPath()
        {
            var result = svc.FilePathConverToLdapPath("ht.ppfbh.net/pphbh.com/103_RD", "hzdc1.pphbh.net");
            Console.Write(result);
        }

        [TestMethod()]
        public void LdapPathConverToFilePath()
        {
            var result = svc.LdapPathConverToFilePath("LDAP://hzdc1.pphbh.net/ou=前端UI,ou=103_RD,OU=pphbh.COM,DC=HT,dc=800bset,dc=net");
            Console.Write(result);
        }

        [TestMethod()]
        public void CreateOuPath()
        {
            var result = svc.CreateOuPath("LDAP://hzdc1.pphbh.net/ou=103_RD,OU=pphbh.COM,DC=HT,dc=800bset,dc=net","前端工程师");
            var result2 = svc.CreateOuPath("","前端工程师",config);
            Console.WriteLine(result);
            Console.WriteLine(result2);
        }

        [TestMethod()]
        public void CreateNewUser()
        {
            DomainUser newUser = new DomainUser();
            newUser.User = new DomainUser.UserInfo()
            {
                UserName = "User2",
                Name = "User2",
                DisplayName = "User2",
                Description = "User2",
                GivenName = "User2",
                LastName = "User2",
                OfficeEMailAddress = "User2@ops.net"
            };

            newUser.Account = new DomainUser.AccountInfo()
            {
                UserAccount = @"BM00012@ops.net",
                Password = "aa@123456",
                SamAccountName = "BM00012",
                UserPrincipalName = "BM00012@ops.net"
            };

            AdMethodsAccountManagement adsvc = new AdMethodsAccountManagement(config);
            var user = adsvc.CreateNewUser("LDAP://10.45.9.11/ou=DeptName2,ou=DeptName,ou=离职员工,dc=ops,dc=net", newUser);
           
        }


        [TestMethod()]
        public void SyncOuAndUsers()
        {
            AdHelper adHelper = new AdHelper(config, exconfig);
            var depts = GetDepts();
            var users = GetUsers();
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(depts));
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(users));
            adHelper.SyncOuAndUsers(depts, users,"离职员工");
        }

        private List<AdUser> GetUsers()
        {
            var result = new List<AdUser>();
            for (int i=1;i<10;i++)
            {
                var depId=new Random().Next(1,10);
                result.Add(new AdUser
                {
                    DepartmentId = depId,
                    Email = $"user{i}@ops.net",
                    UserCode = $"BM0002{i}",
                    UserName = $"user{i}"
                });
                
                Thread.Sleep(30);
            }

            return result;

        }

        private List<AdDepartment> GetDepts()
        {
            var result = new List<AdDepartment>();
            for (int i = 1; i < 3; i++)
            {
                result.Add(new AdDepartment
                {
                    Id = i,
                    Name = "DeptName"+i,
                    ParentId = 0
                });
            }

            for (int i = 3; i < 6; i++)
            {
                result.Add(new AdDepartment
                {
                    Id = i,
                    Name = "DeptName" + i,
                    ParentId = new Random().Next(1, 3)
                });

                Thread.Sleep(30);
            }

            for (int i = 6; i <= 9; i++)
            {
                result.Add(new AdDepartment
                {
                    Id = i,
                    Name = "DeptName" + i,
                    ParentId = new Random().Next(3, 6)
                });

                Thread.Sleep(30);
            }

            result.Add(new AdDepartment
            {
                Id = 10,
                Name = "DeptName" +10,
                ParentId = 6
            });

            return result;
        }
    }
}