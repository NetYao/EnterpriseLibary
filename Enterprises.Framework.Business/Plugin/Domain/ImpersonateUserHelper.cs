using System;
using System.Security.Principal;
using Enterprises.Framework.Utility;

namespace Enterprises.Framework.Domain
{
    /// <summary>
    /// 模拟用户辅助类
    /// </summary>
    [Serializable]
    public class ImpersonateUserHelper : IDisposable
    {
        /// <summary>
        /// 创建新的模拟环境。
        /// 注意采用using(ImpersonateUserHelper helper = ImpersonateUserHelper.NewImpersonateUserHelper()){}方式调用，以确保用完马上释放资源。
        /// </summary>
        /// <returns></returns>
        public static ImpersonateUserHelper NewImpersonateUserHelper()
        {
            string accountName = ConfigUtility.GetFrameworkConfigValue("ImpersonateUserAccount", "配置用户模拟所用的帐号");
            string password = ConfigUtility.GetFrameworkConfigValue("ImpersonateUserPassword", "配置用户模拟所用帐号的密码");
            string domain = ConfigUtility.GetFrameworkConfigValue("ImpersonateUserDomain", "用户模拟所用的帐号的域");
            var helper = new ImpersonateUserHelper(accountName,password,domain);
            return helper;
        }

        /// <summary>
        /// 创建新的模拟环境。
        /// 注意采用using(ImpersonateUserHelper helper = ImpersonateUserHelper.NewImpersonateUserHelper()){}方式调用，以确保用完马上释放资源。
        /// </summary>
        /// <param name="accountName"></param>
        /// <param name="password"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static ImpersonateUserHelper NewImpersonateUserHelper(string accountName, string password, string domain)
        {
            var helper = new ImpersonateUserHelper(accountName,password,domain);
            return helper;
        }

        private WindowsImpersonationContext _fContext;

        /// <summary>
        /// 采用当前机器中用户的账号来模拟
        /// </summary>
        /// <param name="accountName"></param>
        /// <param name="password"></param>
        public ImpersonateUserHelper(string accountName, string password) 
        {
            try
            {
                if (accountName == null)
                {
                    throw new ArgumentNullException("accountName");
                }

                string domain = ".";
                if (accountName.IndexOf("\\", StringComparison.Ordinal) != -1)
                {
                    domain = accountName.Substring(0, accountName.IndexOf("\\", StringComparison.Ordinal));
                    accountName = accountName.Remove(0, accountName.IndexOf("\\", StringComparison.Ordinal) + 1);
                }
                ImpersonateValidUser(accountName, password, domain);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="accountName"></param>
        /// <param name="password"></param>
        /// <param name="domain"></param>
        public ImpersonateUserHelper(string accountName, string password, string domain)
        {
            if (accountName == null)
            {
                throw new ArgumentNullException("accountName");
            }

            if (accountName.IndexOf("\\", StringComparison.Ordinal) != -1)
            {
                throw new Exception(string.Format("accountName不能包含\\：{0}",accountName));
            }
            ImpersonateValidUser(accountName, password, domain);
        }

        private void ImpersonateValidUser(string accountName, string password, string domain)
        {
            _fContext = DomainUtility.ImpersonateValidUser(accountName,password,domain);
            if (_fContext == null)
            {
                throw new Exception(string.Format("帐号[Account={0},Domain={1}]模拟失败", accountName, domain));
            }
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~ImpersonateUserHelper()
        {
            Disposing(false);
        }

        #region IDisposable Members

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            Disposing(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="dispose"></param>
        protected virtual void Disposing(bool dispose)
        {
            if (dispose)
            {
                if (_fContext != null)
                {
                    _fContext.Undo();
                    _fContext.Dispose();
                }
            }
        }

        #endregion

    }
}
