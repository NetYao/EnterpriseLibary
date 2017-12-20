using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Security;
using System.Text;

namespace Enterprises.Framework.Plugin.Domain.AD
{
    /// <summary>
    /// PowerShell 相关类
    /// </summary>
    public class ExchangePowerShellHelper
    {
        /// <summary>
        /// 执行本地脚本
        /// </summary>
        /// <param name="scripts"></param>
        /// <returns></returns>
        public static string RunScript(List<string> scripts)
        {
            Runspace runspace = RunspaceFactory.CreateRunspace();

            runspace.Open();

            Pipeline pipeline = runspace.CreatePipeline();
            foreach (var scr in scripts)
            {
                pipeline.Commands.AddScript(scr);
            }
            //返回结果
            var results = pipeline.Invoke();
            if (pipeline.Error.Count > 0)
            {
                List<object> oerror = pipeline.Error.ReadToEnd().ToList();
                StringBuilder sb = new StringBuilder();
                foreach (object o in oerror)
                {
                    sb.AppendLine(o.ToString());
                }
                throw new Exception(sb.ToString());
            }
            runspace.Close();
            StringBuilder stringBuilder = new StringBuilder();
            foreach (PSObject obj in results)
            {
                stringBuilder.AppendLine(obj.ToString());
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// 执行脚本 待Connection信息
        /// </summary>
        /// <param name="scripts"></param>
        /// <param name="connInfo"></param>
        /// <returns></returns>
        public static string RunScript(List<string> scripts, WSManConnectionInfo connInfo)
        {
            var runspace = RunspaceFactory.CreateRunspace(connInfo);
            try
            {
                runspace.Open();

                Pipeline pipeline = runspace.CreatePipeline();

                foreach (var scr in scripts)
                {
                    pipeline.Commands.AddScript(scr);
                }
                //返回结果
                var results = pipeline.Invoke();
                if (pipeline.Error.Count > 0)
                {
                    //List<object> oerror = pipeline.Error.ReadToEnd().ToList();
                    //StringBuilder sb = new StringBuilder();
                    //foreach (object o in oerror)
                    //{
                    //    sb.AppendLine(o.ToString());
                    //}
                    //throw new Exception(sb.ToString());
                }

                StringBuilder stringBuilder = new StringBuilder();
                foreach (PSObject obj in results)
                {
                    stringBuilder.AppendLine(obj.ToString());
                }

                return stringBuilder.ToString();
            }
            finally
            {
                runspace.Close();
            }
        }

        /// <summary>
        /// 执行脚本
        /// </summary>
        /// <param name="scripts"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public static string RunScript(List<string> scripts, List<KeyValue> pars)
        {
            Runspace runspace = RunspaceFactory.CreateRunspace();

            runspace.Open();

            Pipeline pipeline = runspace.CreatePipeline();
            foreach (var scr in scripts)
            {
                pipeline.Commands.AddScript(scr);
            }

            //注入参数
            if (pars != null)
            {
                foreach (var par in pars)
                {
                    runspace.SessionStateProxy.SetVariable(par.Key, par.Value);
                }
            }

            //返回结果
            var results = pipeline.Invoke();
            runspace.Close();
            StringBuilder stringBuilder = new StringBuilder();
            foreach (PSObject obj in results)
            {
                stringBuilder.AppendLine(obj.ToString());
            }
            return stringBuilder.ToString();
        }
        /// <summary>
        /// 执行ExchangePowershell脚本
        /// </summary>
        /// <param name="command"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static string RunExchangePowerShellScript(Command command, ExchangeAdminConfig config)
        {

            //ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;
            SecureString ssRunasPassword = String2SecureString(config.AdminPwd);
            PSCredential credentials = new PSCredential(config.AdminAccount, ssRunasPassword);
            //var connInfo = new WSManConnectionInfo(new Uri($"http://{config.ServerIpOrDomain}/Powershell"),
            //        "http://schemas.microsoft.com/powershell/Microsoft.Exchange",
            //        credentials)
            //{ AuthenticationMechanism = AuthenticationMechanism.Basic };

            // 带证书访问
            var connInfo = new WSManConnectionInfo(true, config.ServerIpOrDomain, config.SSlPoint, "/Powershell", "http://schemas.microsoft.com/powershell/Microsoft.Exchange", credentials);
            connInfo.AuthenticationMechanism = AuthenticationMechanism.Basic;

            //Ignore SSL Errors
            connInfo.SkipCACheck = true;
            connInfo.SkipCNCheck = true;
            connInfo.SkipRevocationCheck = true;
            connInfo.MaximumConnectionRedirectionCount = 5;

            //var connInfo = new WSManConnectionInfo(false, config.ServerIpOrDomain, 80, "/Powershell", "http://schemas.microsoft.com/powershell/Microsoft.Exchange", credentials);
            //connInfo.AuthenticationMechanism = AuthenticationMechanism.Basic;

            var runspace = RunspaceFactory.CreateRunspace(connInfo);
            try
            {
                runspace.Open();
                var pipeline = runspace.CreatePipeline();
                pipeline.Commands.Add(command);
                var results = pipeline.Invoke();
                if (results.Count > 0)
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    foreach (PSObject obj in results)
                    {
                        stringBuilder.AppendLine(obj.ToString());
                    }

                    return stringBuilder.ToString();
                }
                else
                {
                    if (pipeline.Error.Count > 0)
                    {
                        //Console.Write(Newtonsoft.Json.JsonConvert.SerializeObject(pipeline.Error));
                    }

                    return "FAIL";
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex);
                throw;
            }
            finally
            {
                runspace.Close();
            }
        }

        /// <summary>
        /// 执行ExchangePowershell脚本
        /// </summary>
        /// <param name="command"></param>
        /// <param name="config"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public static string RunExchangePowerShellScript(Command command, ExchangeAdminConfig config, out IEnumerable<PSObject> output)
        {

            //ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;
            SecureString ssRunasPassword = String2SecureString(config.AdminPwd);
            PSCredential credentials = new PSCredential(config.AdminAccount, ssRunasPassword);
            //var connInfo = new WSManConnectionInfo(new Uri($"http://{config.ServerIpOrDomain}/Powershell"),
            //        "http://schemas.microsoft.com/powershell/Microsoft.Exchange",
            //        credentials)
            //{ AuthenticationMechanism = AuthenticationMechanism.Basic };

            // 带证书访问
            var connInfo = new WSManConnectionInfo(true, config.ServerIpOrDomain, config.SSlPoint, "/Powershell", "http://schemas.microsoft.com/powershell/Microsoft.Exchange", credentials);
            connInfo.AuthenticationMechanism = AuthenticationMechanism.Basic;

            //Ignore SSL Errors
            connInfo.SkipCACheck = true;
            connInfo.SkipCNCheck = true;
            connInfo.SkipRevocationCheck = true;
            connInfo.MaximumConnectionRedirectionCount = 5;

            //var connInfo = new WSManConnectionInfo(false, config.ServerIpOrDomain, 80, "/Powershell", "http://schemas.microsoft.com/powershell/Microsoft.Exchange", credentials);
            //connInfo.AuthenticationMechanism = AuthenticationMechanism.Basic;

            var runspace = RunspaceFactory.CreateRunspace(connInfo);
            try
            {
                runspace.Open();
                var pipeline = runspace.CreatePipeline();
                pipeline.Commands.Add(command);
                var results = pipeline.Invoke();
                if (results.Count > 0)
                {
                    output = results;
                    StringBuilder stringBuilder = new StringBuilder();
                    foreach (PSObject obj in results)
                    {
                        stringBuilder.AppendLine(obj.ToString());
                    }

                    return stringBuilder.ToString();
                }
                else
                {
                    if (pipeline.Error.Count > 0)
                    {
                        //Console.Write(Newtonsoft.Json.JsonConvert.SerializeObject(pipeline.Error));
                    }

                    output = null;
                    return "FAIL";
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex);
                throw;
            }
            finally
            {
                runspace.Close();
            }
        }

        /// <summary>
        /// 执行ExchangePowershell脚本
        /// </summary>
        /// <param name="scripts"></param>
        /// <param name="config"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public static string RunExchangePowerShellScript(List<string> scripts, ExchangeAdminConfig config, out IEnumerable<PSObject> output)
        {
            SecureString ssRunasPassword = String2SecureString(config.AdminPwd);
            PSCredential credentials = new PSCredential(config.AdminAccount, ssRunasPassword);
            var connInfo = new WSManConnectionInfo(true, config.ServerIpOrDomain, config.SSlPoint, "/Powershell", "http://schemas.microsoft.com/powershell/Microsoft.Exchange", credentials);
            connInfo.AuthenticationMechanism = AuthenticationMechanism.Basic;

            //Ignore SSL Errors
            connInfo.SkipCACheck = true;
            connInfo.SkipCNCheck = true;
            connInfo.SkipRevocationCheck = true;
            connInfo.MaximumConnectionRedirectionCount = 5;

            //var connInfo = new WSManConnectionInfo(false, config.ServerIpOrDomain, 80, "/Powershell", "http://schemas.microsoft.com/powershell/Microsoft.Exchange", credentials);
            //connInfo.AuthenticationMechanism = AuthenticationMechanism.Basic;

            var runspace = RunspaceFactory.CreateRunspace(connInfo);
            try
            {
                runspace.Open();
                var pipeline = runspace.CreatePipeline();
                foreach (var scr in scripts)
                {
                    pipeline.Commands.AddScript(scr);
                }

                var results = pipeline.Invoke();
                if (results.Count > 0)
                {
                    output = results;
                    StringBuilder stringBuilder = new StringBuilder();
                    foreach (PSObject obj in results)
                    {
                        stringBuilder.AppendLine(obj.ToString());
                    }

                    return stringBuilder.ToString();
                }
                else
                {
                    output = null;
                    return "FAIL";
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                runspace.Close();
            }
        }

        /// <summary>
        /// 获取证书
        /// </summary>
        /// <param name="liveIdConnectionUri"></param>
        /// <param name="schemaUri"></param>
        /// <param name="credentials"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public Collection<PSObject> GetUsersUsingBasicAuth(string liveIdConnectionUri, string schemaUri, PSCredential credentials, int count)
        {
            WSManConnectionInfo connectionInfo = new WSManConnectionInfo(
                new Uri(liveIdConnectionUri),
                schemaUri, credentials);
            connectionInfo.AuthenticationMechanism = AuthenticationMechanism.Basic;

            using (Runspace runspace = RunspaceFactory.CreateRunspace(connectionInfo))
            {
                return GetUserInformation(count, runspace);
            }
        }

        public Collection<PSObject> GetUserInformation(int count, Runspace runspace)
        {
            using (PowerShell powershell = PowerShell.Create())
            {
                powershell.AddCommand("Get-Users");
                powershell.AddParameter("ResultSize", count);

                runspace.Open();

                powershell.Runspace = runspace;

                return powershell.Invoke();
            }
        }

       

        /// <summary>
        /// 创建AD帐号并开通Exchange帐号
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static ExcuseResult CreateAdAccountAndOpenExcahnge(AdEntity entity, ExchangeAdminConfig config)
        {
            ExcuseResult er = new ExcuseResult();
            var command = new Command("New-Mailbox");
            command.Parameters.Add("Name", entity.Name);
            command.Parameters.Add("Alias", entity.Alias);
            command.Parameters.Add("UserPrincipalName", entity.UserPrincipalName + "@" + entity.Ldap);
            command.Parameters.Add("SamAccountName", entity.SamAccountName);
            command.Parameters.Add("FirstName", entity.FirstName);
            command.Parameters.Add("LastName", entity.LastName);
            command.Parameters.Add("Password", String2SecureString(entity.Password));
            command.Parameters.Add("ResetPasswordOnNextLogon", false);
            command.Parameters.Add("OrganizationalUnit", $"{entity.Ldap}/{entity.OrganizationalUnit}");
            command.Parameters.Add("Database", entity.Database);
            command.Parameters.Add("DisplayName", entity.DisplayName);

            if (!string.IsNullOrEmpty(config.DomainController))
            {
                command.Parameters.Add("DomainController", config.DomainController);
            }

            try
            {
                string result = RunExchangePowerShellScript(command, config);
                if (!string.IsNullOrEmpty(result) && result != "FAIL")
                {

                    return er.SetSuccessResult();
                }

                return er.SetFailResult();
            }
            catch (Exception ex)
            {
                return er.SetErrorResult(ex.Message);
            }
        }

        private static SecureString String2SecureString(string password)
        {
            SecureString remotePassword = new SecureString();
            for (int i = 0; i < password.Length; i++)
                remotePassword.AppendChar(password[i]);

            return remotePassword;
        }


        /// <summary>
        /// 创建用户邮箱帐号
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static ExcuseResult CreateUserMailBox(AdEntity entity, ExchangeAdminConfig config)
        {
            ExcuseResult er = new ExcuseResult();
            var command = new Command("Enable-Mailbox");
            command.Parameters.Add("Identity", entity.SamAccountName);
            command.Parameters.Add("Alias", entity.Alias);
            command.Parameters.Add("Database", entity.Database);
            command.Parameters.Add("Password", String2SecureString(entity.Password));
            if (!string.IsNullOrEmpty(config.DomainController))
            {
                command.Parameters.Add("DomainController", config.DomainController);
            }

            try
            {
                string result = RunExchangePowerShellScript(command, config);
                if (!string.IsNullOrEmpty(result) && result != "FAIL")
                {
                    return er.SetSuccessResult();
                }

                return er.SetFailResult();
            }
            catch (Exception ex)
            {
                return er.SetErrorResult(ex.Message);
            }
        }

        /// <summary>
        /// 判断是否存在邮箱
        /// </summary>
        /// <param name="userCodeOrEmail">工号或者邮箱</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static ExcuseResult IsExitsUserMailBox(string userCodeOrEmail, ExchangeAdminConfig config)
        {
            ExcuseResult er = new ExcuseResult();
            var command = new Command("Get-Mailbox");
            command.Parameters.Add("Identity", userCodeOrEmail);
            if (!string.IsNullOrEmpty(config.DomainController))
            {
                command.Parameters.Add("DomainController", config.DomainController);
            }

            try
            {
                string result = RunExchangePowerShellScript(command, config);
                if (!string.IsNullOrEmpty(result) && result != "FAIL")
                {
                    return er.SetSuccessResult();
                }

                return er.SetFailResult();
            }
            catch (Exception ex)
            {
                return er.SetErrorResult(ex.Message);
            }
        }


        /// <summary>
        /// 获取用户邮箱
        /// </summary>
        /// <param name="userCode">工号</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static ExcuseResult GetUserMailBox(string userCode, ExchangeAdminConfig config)
        {
            var er = new ExcuseResult();
            List<string> srcpirt = new List<string> { $"Get-MailBox -Identity {userCode} | Select-Object Alias" };
            try
            {
                IEnumerable<PSObject> output;
                string result = RunExchangePowerShellScript(srcpirt, config, out output);
                if (output != null)
                {
                    var psObjects = output as IList<PSObject> ?? output.ToList();
                    PSObject psObject = psObjects[0];
                    foreach (PSPropertyInfo psPropertyInfo in psObject.Properties)
                    {
                        er.Reuslt = psPropertyInfo.Value;
                        return er.SetSuccessResult();
                    }
                }

                return er.SetFailResult();
            }
            catch (Exception ex)
            {
                return er.SetErrorResult(ex.Message);
            }
        }


        /// <summary>
        /// 禁用用户邮箱
        /// </summary>
        /// <param name="userCode">工号</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static ExcuseResult DisableUserMailBox(string userCode, ExchangeAdminConfig config)
        {
            var er = new ExcuseResult();
            List<string> srcpirt = new List<string> { $"Disable-Mailbox {userCode} -Confirm:$False" };
            try
            {
                IEnumerable<PSObject> output;
                string result = RunExchangePowerShellScript(srcpirt, config, out output);
                if (output != null)
                {
                    var psObjects = output as IList<PSObject> ?? output.ToList();
                    PSObject psObject = psObjects[0];
                    foreach (PSPropertyInfo psPropertyInfo in psObject.Properties)
                    {
                        er.Reuslt = psPropertyInfo.Value;
                        return er.SetSuccessResult();
                    }
                }

                return er.SetFailResult();
            }
            catch (Exception ex)
            {
                return er.SetErrorResult(ex.Message);
            }
        }

        /// <summary>
        /// 启用邮箱
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="exConfig"></param>
        public static ExcuseResult EnableUserMailBox(AdEntity entity, ExchangeAdminConfig exConfig)
        {
            if (string.IsNullOrEmpty(entity.SamAccountName) || string.IsNullOrEmpty(entity.Alias))
            {
                throw new Exception("SamAccountName和Alias必须赋值");
            }

            var er = new ExcuseResult();
            var command = new Command("Enable-Mailbox");
            command.Parameters.Add("Identity", entity.SamAccountName);
            command.Parameters.Add("Alias", entity.Alias);
            if (!string.IsNullOrEmpty(entity.Database))
            {
                command.Parameters.Add("Database", entity.Database);
            }

            if (!string.IsNullOrEmpty(entity.Password))
            {
                command.Parameters.Add("Password", String2SecureString(entity.Password));
            }

            if (!string.IsNullOrEmpty(exConfig.DomainController))
            {
                command.Parameters.Add("DomainController", exConfig.DomainController);
            }

            try
            {
                string result = RunExchangePowerShellScript(command, exConfig);
                if (!string.IsNullOrEmpty(result) && result != "FAIL")
                {
                    return er.SetSuccessResult();
                }

                return er.SetFailResult();
            }
            catch (Exception ex)
            {
                return er.SetErrorResult(ex.Message);
            }

        }

        /// <summary>
        /// 创建邮箱组
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="members">可以是邮箱集合，或者是用户的SamAccountName、Name等</param>
        /// <param name="exConfig"></param>
        public static ExcuseResult NewDistributionGroup(ExchangeMailGroup entity,List<string> members, ExchangeAdminConfig exConfig)
        {
            var er = new ExcuseResult();
            var command = new Command("New-DistributionGroup");
            command.Parameters.Add("Name", entity.Name);
            command.Parameters.Add("OrganizationalUnit", entity.OrganizationalUnit);
            if (!string.IsNullOrEmpty(entity.Alias))
            {
                command.Parameters.Add("Alias", entity.Alias);
            }

            if (!string.IsNullOrEmpty(entity.SamAccountName))
            {
                command.Parameters.Add("SamAccountName", entity.SamAccountName);
            }

            if (!string.IsNullOrEmpty(entity.Type))
            {
                command.Parameters.Add("Type", entity.Type);
            }

            if (!string.IsNullOrEmpty(entity.DomainController))
            {
                command.Parameters.Add("DomainController", entity.DomainController);
            }

            if (!string.IsNullOrEmpty(entity.DisplayName))
            {
                command.Parameters.Add("DisplayName", entity.DisplayName);
            }

            if (!string.IsNullOrEmpty(entity.Notes))
            {
                command.Parameters.Add("Notes", entity.Notes);
            }

            if (members!=null && members.Count>0)
            {
                var menbersStr = string.Join(",", members);
                command.Parameters.Add("Members", menbersStr);
            }

            try
            {
                string result = RunExchangePowerShellScript(command, exConfig);
                if (!string.IsNullOrEmpty(result) && result != "FAIL")
                {
                    return er.SetSuccessResult();
                }

                return er.SetFailResult();
            }
            catch (Exception ex)
            {
                return er.SetErrorResult(ex.Message);
            }

        }

        /// <summary>
        /// 删除用户组
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="exConfig"></param>
        /// <returns></returns>
        public static bool RemoveDistributionGroup(string identity,ExchangeAdminConfig exConfig)
        {
            var command = new Command("Remove-DistributionGroup");
            command.Parameters.Add("Identity", identity);
            try
            {
                string result = RunExchangePowerShellScript(command, exConfig);
                if (!string.IsNullOrEmpty(result) && result != "FAIL")
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 增加用户组成员
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="member">成员,只能一个</param>
        /// <param name="exConfig"></param>
        /// <returns></returns>
        public static bool AddDistributionGroupMember(string identity, string member,ExchangeAdminConfig exConfig)
        {
            var command = new Command("Add-DistributionGroupMember");
            command.Parameters.Add("Identity", identity);
            command.Parameters.Add("Member", member);
            try
            {
                string result = RunExchangePowerShellScript(command, exConfig);
                if (!string.IsNullOrEmpty(result) && result != "FAIL")
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 获取用户组成员
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="exConfig"></param>
        /// <returns></returns>
        public static List<AdEntity> GetDistributionGroupMember(string identity, ExchangeAdminConfig exConfig)
        {
            List<AdEntity> users = new List<AdEntity>();
            var command = new Command("Get-DistributionGroupMember");
            command.Parameters.Add("Identity", identity);
            try
            {
                IEnumerable<PSObject> menbers;
                string result = RunExchangePowerShellScript(command, exConfig,out menbers);
                if (!string.IsNullOrEmpty(result) && result != "FAIL")
                {
                    foreach (var item in menbers)
                    {
                        PSObject psObject = item;
                        var user = new AdEntity().GetInstance(psObject);
                        if (user != null)
                        {
                            users.Add(user);
                        }
                    }

                    return users;
                }

                return users;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取用户组
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="exConfig"></param>
        /// <returns></returns>
        public static ExchangeMailGroup GetDistributionGroup(string identity, ExchangeAdminConfig exConfig)
        {
            
            var command = new Command("Get-DistributionGroup");
            command.Parameters.Add("Identity", identity);
            try
            {
                IEnumerable<PSObject> menbers;
                string result = RunExchangePowerShellScript(command, exConfig, out menbers);
                if (!string.IsNullOrEmpty(result) && result != "FAIL")
                {
                    var psObjects = menbers as IList<PSObject> ?? menbers.ToList();
                    PSObject psObject = psObjects[0];
                    var group = new ExchangeMailGroup().GetInstance(psObject);
                    return group;
                }

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 判断是否存在组
        /// </summary>
        /// <param name="identity">组的标识</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static ExcuseResult IsExitsDistributionGroup(string identity, ExchangeAdminConfig config)
        {
            ExcuseResult er = new ExcuseResult();
            var command = new Command("Get-DistributionGroup");
            command.Parameters.Add("Identity", identity);
            if (!string.IsNullOrEmpty(config.DomainController))
            {
                command.Parameters.Add("DomainController", config.DomainController);
            }

            try
            {
                string result = RunExchangePowerShellScript(command, config);
                if (!string.IsNullOrEmpty(result) && result != "FAIL")
                {
                    return er.SetSuccessResult();
                }

                return er.SetFailResult();
            }
            catch (Exception ex)
            {
                return er.SetErrorResult(ex.Message);
            }
        }
        
        /// <summary>
        /// 启用AD邮件组，显示在exchange组中，可以首发邮件
        /// </summary>
        /// <param name="identity">组的标识</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static ExcuseResult EnableDistributionGroup(string identity, ExchangeAdminConfig config)
        {
            ExcuseResult er = new ExcuseResult();
            var command = new Command("Enable-DistributionGroup");
            command.Parameters.Add("Identity", identity);
            if (!string.IsNullOrEmpty(config.DomainController))
            {
                command.Parameters.Add("DomainController", config.DomainController);
            }

            try
            {
                string result = RunExchangePowerShellScript(command, config);
                if (!string.IsNullOrEmpty(result) && result != "FAIL")
                {
                    return er.SetSuccessResult();
                }

                return er.SetFailResult();
            }
            catch (Exception ex)
            {
                return er.SetErrorResult(ex.Message);
            }
        }

        /// <summary>
        /// 启用AD邮件组，显示在exchange组中，可以首发邮件
        /// </summary>
        /// <param name="entity">组的信息</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static ExcuseResult EnableDistributionGroup(ExchangeMailGroup entity, ExchangeAdminConfig config)
        {
            ExcuseResult er = new ExcuseResult();
            var command = new Command("Enable-DistributionGroup");
            command.Parameters.Add("Identity", entity.Name);
            if (!string.IsNullOrEmpty(config.DomainController))
            {
                command.Parameters.Add("DomainController", config.DomainController);
            }

            if (!string.IsNullOrEmpty(entity.Alias))
            {
                command.Parameters.Add("Alias", entity.Alias);
            }

            if (!string.IsNullOrEmpty(entity.DisplayName))
            {
                command.Parameters.Add("DisplayName", entity.DisplayName);
            }

            try
            {
                string result = RunExchangePowerShellScript(command, config);
                if (!string.IsNullOrEmpty(result) && result != "FAIL")
                {
                    return er.SetSuccessResult();
                }

                return er.SetFailResult();
            }
            catch (Exception ex)
            {
                return er.SetErrorResult(ex.Message);
            }
        }
    }

    /// <summary>
    /// 执行状态
    /// </summary>
    public class ExcuseResult
    {
        /// <summary>
        /// 脚本执行成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 是否有错误，或者不存在某个对象
        /// </summary>
        public bool HasError { get; set; }

        /// <summary>
        /// 结果
        /// </summary>
        public object Reuslt { get; set; }

        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 执行成功，有结果
        /// </summary>
        public ExcuseResult SetSuccessResult()
        {
            return new ExcuseResult()
            {
                Success = true,
                HasError = false
            };
        }

        /// <summary>
        /// 执行成功，无结果
        /// </summary>
        public ExcuseResult SetFailResult()
        {
            return new ExcuseResult()
            {
                Success = true,
                HasError = true
            };
        }

        /// <summary>
        /// 执行失败，不可知有误结果
        /// </summary>
        /// <param name="message"></param>
        public ExcuseResult SetErrorResult(string message)
        {
            return new ExcuseResult()
            {
                Success = false,
                HasError = true,
                Message = message
            };
        }
    }
}
