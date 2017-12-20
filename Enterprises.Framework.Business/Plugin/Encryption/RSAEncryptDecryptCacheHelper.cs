using System;
using System.Collections.Generic;

namespace Enterprises.Framework.Plugin.Encryption
{
    /// <summary>
    /// 通过RSA加密解密的缓存类
    /// </summary>
    [Serializable]
    public static class RsaEncryptDecryptCacheHelper
    {
        private static readonly Dictionary<string, RsaEncryptDecryptKeyInfo> FRsaKeys = new Dictionary<string, RsaEncryptDecryptKeyInfo>();

        /// <summary>
        /// 增加新项
        /// </summary>
        /// <returns></returns>
        public static string AddNewRsaEncryptDecryptKeyInfo()
        {
            var keyInfo = new RsaEncryptDecryptKeyInfo();
            FRsaKeys.Add(keyInfo.Base64PublicKey, keyInfo);
            return keyInfo.Base64PublicKey;
        }

        /// <summary>
        /// 获取已有项
        /// </summary>
        /// <param name="publicKey"></param>
        /// <returns></returns>
        public static RsaEncryptDecryptKeyInfo GetRsaEncryptDecryptKeyInfo(string publicKey)
        {
            if (FRsaKeys.ContainsKey(publicKey))
            {
                return FRsaKeys[publicKey];
            }
            return null;
        }

    }
}