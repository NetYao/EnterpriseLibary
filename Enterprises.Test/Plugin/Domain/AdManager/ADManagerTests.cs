using Microsoft.VisualStudio.TestTools.UnitTesting;
using Enterprises.Framework.Plugin.Domain.AdManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Enterprises.Framework.Plugin.Domain.AD;

namespace Enterprises.Framework.Plugin.Domain.AdManager.Tests
{
    [TestClass()]
    public class ADManagerTests
    {
        AdAdminConfig config = new AdAdminConfig().GetDefaultInstance("ops.net");
        [TestMethod()]
        public void MoveOUTest()
        {
            var fpath = "LDAP://10.45.9.11/ou=110_Marketing,ou=pphbh.com,dc=ops,dc=net";
            var toPath = "LDAP://10.45.9.11/ou=110_Marketing,ou=离职员工,dc=ops,dc=net";

            //var f=ADManager.Exists("LDAP://HZBESTDC3.pphbh.net/ou=110_Marketing,ou=pphbh.com,dc=pphbh,dc=net");
            ADManager.MoveOU(fpath, toPath,true, config.AdminAccount,config.AdminPwd);
        }
    }
}