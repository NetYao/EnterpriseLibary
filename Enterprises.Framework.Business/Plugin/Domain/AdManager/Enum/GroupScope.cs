
namespace Enterprises.Framework.Plugin.Domain.AdManager.Enum
{
    /// <summary>
    /// 组类型和作用域
    /// </summary>
    public enum GroupScope
    {
        /// <summary>
        /// 安全组本地域( = -2147483648 + 4)
        /// </summary>
        ADS_GROUP_TYPE_DOMAIN_LOCAL_GROUP = -2147483644,
        /// <summary>
        /// 安全组全局( = -2147483648 + 2)
        /// </summary>
        ADS_GROUP_TYPE_GLOBAL_GROUP = -2147483646,
        ///// <summary>
        ///// 安全组通用
        ///// </summary>
        //ADS_GROUP_TYPE_UNIVERSAL_GROUP = -2147483640,
        ///// <summary>
        ///// 通讯组本地域
        ///// </summary>
        //ADC_GROUP_TYPE_DOMAIN_LOCAL_GROUP = 4,
        ///// <summary>
        ///// 通讯组全局
        ///// </summary>
        //ADC_GROUP_TYPE_GLOBAL_GROUP = 2,
        ///// <summary>
        ///// 通讯组通用
        ///// </summary>
        //ADC_GROUP_TYPE_UNIVERSAL_GROUP = 8

        // 本地内置安全组
        //-2147483643
    }

    /// <summary>
    /// 组类型枚举
    /// </summary>
    public enum ADS_GROUP_TYPE_ENUM
    {
        /// <summary>
        /// 本地域
        /// </summary>
        ADS_GROUP_TYPE_DOMAIN_LOCAL_GROUP = 4,
        /// <summary>
        /// 全局
        /// </summary>
        ADS_GROUP_TYPE_GLOBAL_GROUP = 2,
        /// <summary>
        /// 本地内置
        /// </summary>
        ADS_GROUP_TYPE_LOCAL_GROUP = 4,     
        /// <summary>
        /// 安全
        /// </summary>
        ADS_GROUP_TYPE_SECURITY_ENABLED = -2147483648,      // 32位整数，第一位为1 -- 理解为标记
        /// <summary>
        /// 通用
        /// </summary>
        ADS_GROUP_TYPE_UNIVERSAL_GROUP = 8
    }

}
