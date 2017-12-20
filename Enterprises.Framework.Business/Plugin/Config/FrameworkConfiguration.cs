using System;
using Enterprises.Framework.Utility;

namespace Enterprises.Framework.Plugin.Config
{
    /// <summary>
    /// 框架全局配置类
    /// </summary>
    [Serializable]
    public class FrameworkConfiguration : ConfigBase
    {
        private static readonly object FResetLockObject = new object();

        private static FrameworkConfiguration _fFrameworkConfig;
        private static DateTime _fFrameworkConfigLastUpdateTime = DateTime.MinValue;

        /// <summary>
        /// 框架级别的配置
        /// </summary>
        public static FrameworkConfiguration FrameworkConfig
        {
            get
            {
                if (_fFrameworkConfig == null)
                {
                    lock (FResetLockObject)
                    {
                        if (_fFrameworkConfig == null)
                        {
                            var frameworkConfigFileName = FileUtility.Combine(GetWebOrAppConfigPath(),"Enterprises.Framework.config");
                            _fFrameworkConfigLastUpdateTime = FileUtility.GetLastWriteTime(frameworkConfigFileName);
                            _fFrameworkConfig = new FrameworkConfiguration(frameworkConfigFileName);
                        }
                    }
                }

                return _fFrameworkConfig;
            }
        }

        /// <summary>
        /// 重置配置文档内容
        /// </summary>
        public static void ResetConfigurations()
        {
            lock (FResetLockObject)
            {
                var frameworkConfigFileName = FileUtility.Combine(GetWebOrAppConfigPath(), "Enterprises.Framework.config");
                var frmLastWriteDate = FileUtility.GetLastWriteTime(frameworkConfigFileName);
                if (frmLastWriteDate > _fFrameworkConfigLastUpdateTime)
                {
                    _fFrameworkConfig = null;
                    _fFrameworkConfigLastUpdateTime = DateTime.MinValue;
                }
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        private FrameworkConfiguration(string configXmlFile)
            : base(configXmlFile)
        {
        }
    }
}