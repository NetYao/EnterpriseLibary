using System;
using System.Security.Cryptography;
using System.Xml.Serialization;
using Enterprises.Framework.Utility;
using Org.BouncyCastle.Math;

namespace Enterprises.Framework.Plugin.Encryption
{
    /// <summary>
    /// 通过RSA加密解密的参数
    /// </summary>
    [Serializable]
    public class RsaEncryptDecryptKeyInfo
    {

        #region Propertities

        /// <summary>
        /// 
        /// </summary>
        public string Exponent { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string Modulus { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string D { get; private set; }


        /// <summary>
        /// 
        /// </summary>
        [XmlIgnore]
        public BigInteger BigExponent
        {
            get
            {
                return new BigInteger(Convert.FromBase64String(Exponent));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [XmlIgnore]
        public BigInteger BigModulus
        {
            get
            {
                return new BigInteger(Convert.FromBase64String(Modulus));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [XmlIgnore]
        public BigInteger BigD
        {
            get
            {
                return new BigInteger(Convert.FromBase64String(D));
            }
        }

        /// <summary>
        /// BigD + BigModulus
        /// </summary>
        [XmlIgnore]
        public string BigEncryptString
        {
            get
            {
                return string.Format("{0}{1}{2}", BigD, BigModulus,1);
            }
        }

        /// <summary>
        /// BigExponent + BigModulus
        /// </summary>
        [XmlIgnore]
        public string BigDecryptString
        {
            get
            {
                return string.Format("{0}{1}{2}", BigExponent, BigModulus, 1);
            }
        }

        /// <summary>
        /// [获取]Base64格式的公钥
        /// </summary>
        [XmlIgnore]
        public string Base64PublicKey
        {
            get
            {
                return EncodingUtility.ToBase64FromUnicode(XmlPublicKey);
            }
        }

        /// <summary>
        /// [获取]Base64格式的私钥
        /// </summary>
        [XmlIgnore]
        public string Base64PrivateKey
        {
            get
            {
                return EncodingUtility.ToBase64FromUnicode(XmlPrivateKey);
            }
        }

        /// <summary>
        /// [获取]XML格式的私钥
        /// </summary>
        public string XmlPrivateKey
        {
            get;
            private set;
        }

        /// <summary>
        /// [获取]XML格式的公钥
        /// </summary>
        public string XmlPublicKey
        {
            get;
            private set;
        }

        #endregion

        /// <summary>
        /// 构造函数，实例化新的加密解密参数
        /// </summary>
        public RsaEncryptDecryptKeyInfo()
        {
            var rsaParams = new CspParameters {Flags = CspProviderFlags.UseMachineKeyStore};
            using (var rsaProvider = new RSACryptoServiceProvider(1024, rsaParams))
            {
                RSAParameters rsaParam = rsaProvider.ExportParameters(true);
                Modulus = Convert.ToBase64String(rsaParam.Modulus);
                D = Convert.ToBase64String(rsaParam.D);
                Exponent = Convert.ToBase64String(rsaParam.Exponent);
                XmlPrivateKey = rsaProvider.ToXmlString(true);
                XmlPublicKey = rsaProvider.ToXmlString(false);
            }
        }

        /// <summary>
        /// 根据公钥实例化对象（公钥由Exponent;Modulus组成）
        /// </summary>
        /// <param name="xmlPublicKey"></param>
        public RsaEncryptDecryptKeyInfo(string xmlPublicKey)
        {
            XmlPublicKey = xmlPublicKey;
            var rsaParams = new CspParameters {Flags = CspProviderFlags.UseMachineKeyStore};
            using (var rsaProvider = new RSACryptoServiceProvider(1024, rsaParams))
            {
                rsaProvider.FromXmlString(xmlPublicKey);
                RSAParameters rsaParam = rsaProvider.ExportParameters(false);
                Exponent = Convert.ToBase64String(rsaParam.Exponent);
                Modulus = Convert.ToBase64String(rsaParam.Modulus);
            }
        }

        /// <summary>
        /// 根据公钥和私钥实例化对象（公钥由Exponent;Modulus组成， 私钥由D;Modulus组成）
        /// </summary>
        /// <param name="xmlPublicKey"></param>
        /// <param name="xmlPrivateKey"></param>
        public RsaEncryptDecryptKeyInfo(string xmlPublicKey,string xmlPrivateKey)
        {
            XmlPublicKey = xmlPublicKey;
            XmlPrivateKey = xmlPrivateKey;
            var rsaParams = new CspParameters {Flags = CspProviderFlags.UseMachineKeyStore};
            using (var rsaProvider = new RSACryptoServiceProvider(1024, rsaParams))
            {
                rsaProvider.FromXmlString(xmlPrivateKey);
                RSAParameters rsaParam = rsaProvider.ExportParameters(true);
                D = Convert.ToBase64String(rsaParam.D);
                Modulus = Convert.ToBase64String(rsaParam.Modulus);
                Exponent = Convert.ToBase64String(rsaParam.Exponent);
            }
        }
    }


}
