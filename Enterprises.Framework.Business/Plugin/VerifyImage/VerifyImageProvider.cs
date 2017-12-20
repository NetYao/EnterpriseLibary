using System;
using System.Collections;

namespace Enterprises.Framework.VerifyImage
{
    /// <summary>
    /// 验证码图片创建类
    /// </summary>
    public class VerifyImageProvider
    {
        private static readonly Hashtable Instance = new Hashtable();
        private static readonly object LockHelper = new object();

        /// <summary>
        /// 获取验证码的类实例（如可以输入JpegImage）
        /// </summary>
        /// <param name="assemlyName">用于区分库文件的名称</param>
        /// <returns></returns>
        public static IVerifyImage GetInstance(string assemlyName)
        {
            if (!Instance.ContainsKey(assemlyName))
            {
                lock (LockHelper)
                {
                    if (!Instance.ContainsKey(assemlyName))
                    {
                        IVerifyImage p = null;
                        try
                        {
                            p = (IVerifyImage)Activator.CreateInstance(Type.GetType(string.Format("Enterprises.Framework.VerifyImage.{0}, Enterprises.Framework", assemlyName), false, true));
                        }
                        catch
                        {
                            p = new Enterprises.Framework.VerifyImage.VerifyImage();
                        }

                        Instance.Add(assemlyName, p);
                    }
                }
            }

            return (IVerifyImage)Instance[assemlyName];
        }
    }
}
