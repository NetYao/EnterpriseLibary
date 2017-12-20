using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Enterprises.Framework.Domain
{
    /// <summary>
    /// AD 用户选项
    /// </summary>
    public enum ADAccountOptions
    {
        UF_TEMP_DUPLICATE_ACCOUNT = 0x0100,

        UF_NORMAL_ACCOUNT = 0x0200,

        UF_INTERDOMAIN_TRUST_ACCOUNT = 0x0800,

        UF_WORKSTATION_TRUST_ACCOUNT = 0x1000,

        UF_SERVER_TRUST_ACCOUNT = 0x2000,

        UF_DONT_EXPIRE_PASSWD = 0x10000,

        UF_SCRIPT = 0x0001,

        UF_ACCOUNTDISABLE = 0x0002,

        UF_HOMEDIR_REQUIRED = 0x0008,

        UF_LOCKOUT = 0x0010,

        UF_PASSWD_NOTREQD = 0x0020,

        UF_PASSWD_CANT_CHANGE = 0x0040,

        UF_ACCOUNT_LOCKOUT = 0X0010,

        UF_ENCRYPTED_TEXT_PASSWORD_ALLOWED = 0X0080
    }

    /// <summary>
    /// 登入结果
    /// </summary>
    public enum LoginResult
    {
        /// <summary>
        /// 登入正常
        /// </summary>
        LOGIN_OK = 0,

        /// <summary>
        /// 登入用户不存在
        /// </summary>
        LOGIN_USER_DOESNT_EXIST,

        /// <summary>
        /// 登入用户禁用
        /// </summary>
        LOGIN_USER_ACCOUNT_INACTIVE
    }
}

