
namespace Enterprises.Framework.Plugin.Domain.AdManager.Enum
{
    /// <summary>
    /// 登录结果
    /// </summary>
    public enum LoginResult
    {
        /// <summary>
        /// 正常
        /// </summary>
        LOGIN_OK = 0,
        /// <summary>
        /// 用户不存在
        /// </summary>
        LOGIN_USER_DOT_EXIST,
        /// <summary>
        /// 账户被禁用
        /// </summary>
        LOGIN_USER_ACCOUNT_INACTIVE,
        /// <summary>
        /// 密码不正确
        /// </summary>
        LOGIN_USER_PASSWORD_INCORRECT
    }
}
