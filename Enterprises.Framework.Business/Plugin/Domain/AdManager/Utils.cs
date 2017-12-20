using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Text;
using Enterprises.Framework.Plugin.Domain.AdManager.Enum;

namespace Enterprises.Framework.Plugin.Domain.AdManager
{
    /// <summary>
    /// 帮助类
    /// </summary>
    public class Utils
    {
        /// <summary>
        /// 去除ADsPath中的前置“LADP://”
        /// </summary>
        /// <param name="adsPath">ADsPath</param>
        /// <returns></returns>
        internal static string RemoveLDAPPre(string adsPath)
        {
            return adsPath.Substring(7);
        }



        /// <summary>
        /// 设置DirectoryEntry对象的属性
        /// </summary>
        /// <param name="de">DirectoryEntry对象</param>
        /// <param name="propertyName">属性名</param>
        /// <param name="propertyValue">属性值</param>
        internal static void SetProperty(DirectoryEntry de, string propertyName, string propertyValue)
        {
            if (de.Properties.Contains(propertyName))      // 已包括
            {
                if (String.IsNullOrEmpty(propertyValue))
                {
                    de.Properties[propertyName].Value = null;            // 清空
                }
                else
                {
                    de.Properties[propertyName][0] = propertyValue;
                }
            }
            else
            {
                if (!String.IsNullOrEmpty(propertyValue))
                {
                    de.Properties[propertyName].Add(propertyValue);
                }
            }
        }
        
        /// <summary>
        /// 设置DirectoryEntry对象的属性
        /// </summary>
        /// <param name="de">DirectoryEntry对象</param>
        /// <param name="propertyName">属性名</param>
        /// <param name="propertyValue">属性值</param>
        internal static void SetProperty(DirectoryEntry de, string propertyName, int propertyValue)
        {
            if (de.Properties.Contains(propertyName))
            {
                de.Properties[propertyName][0] = propertyValue;
            }
            else
            {
                de.Properties[propertyName].Add(propertyValue);
            }
        }
        
        /// <summary>
        /// 获取DirectoryEntry对象的属性值
        /// </summary>
        /// <param name="de">DirectoryEntry对象</param>
        /// <param name="propertyName">属性名</param>
        /// <returns></returns>
        internal static string GetProperty(DirectoryEntry de, string propertyName)
        {
            if (de.Properties.Contains(propertyName))
            {
                return de.Properties[propertyName][0].ToString();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取SearchResult对象的属性值
        /// </summary>
        /// <param name="result">SearchResult对象</param>
        /// <param name="propertyName">属性名</param>
        /// <returns></returns>
        internal static string GetProperty(SearchResult result, string propertyName)
        {
            if (result.Properties.Contains(propertyName))
            {
                return result.Properties[propertyName][0].ToString();
            }
            else
            {
                return null;
            }
        }



        /// <summary>
        /// 将字节数组转化为16进制形式的字符串
        /// </summary>
        /// <param name="ids">字节数组</param>
        /// <returns></returns>
        public static string ConvertToOctetString(byte[] ids)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in ids)
            {
                sb.AppendFormat(@"\{0:x2}", b);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 将Guid转换为NativeGuid
        /// </summary>
        /// <param name="guid">Guid</param>
        /// <returns></returns>
        public static string ConvertGuidToNativeGuid(Guid guid)
        {
            // GUID默认的ToString()在转换各个字节为16进制时，并不是按照字节顺序逐个进行，而有所调整。
            // NativeGuid要求完全按照字节顺序。
            StringBuilder sb = new StringBuilder();
            byte[] byteArr = guid.ToByteArray();
            foreach (byte b in byteArr)
            {
                sb.AppendFormat(@"{0:x2}", b);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 将NativeGuid转换为Guid
        /// </summary>
        /// <param name="nativeGuid">NativeGuid</param>
        /// <returns></returns>
        public static Guid ConvertNativeGuidToGuid(string nativeGuid)
        {
            StringBuilder sb = new StringBuilder();
            byte[] byteArr = new byte[16];
            for (int i = 0; i < 16;i++ )
            {
                byteArr[i] = byte.Parse(nativeGuid[2 * i].ToString().ToUpper() + nativeGuid[2 * i + 1].ToString().ToUpper(),
                    System.Globalization.NumberStyles.HexNumber);
            }
            return new Guid(byteArr);
        }

        /// <summary>
        /// 将域名转换为DN形式
        /// </summary>
        /// <param name="domainName"></param>
        /// <returns></returns>
        public static string ConvertDomainNameToDN(string domainName)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string dc in domainName.Split('.'))
            {
                sb.AppendFormat("DC={0},", dc.ToUpper());
            }
            return sb.ToString(0, sb.Length - 1);
        }

        /// <summary>
        /// 将DN形式域转换为域名形式
        /// </summary>
        /// <param name="dn"></param>
        /// <returns></returns>
        public static string ConvertDNToDomainName(string dn)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string dc in dn.Split(','))
            {
                sb.AppendFormat("{0}.", dc.Trim().Split('=')[1]);
            }
            return sb.ToString(0, sb.Length - 1);

        }



        /// <summary>
        /// 判断DirectoryEntry对象是否可以作为容器(container,ou,builtinDomain)
        /// </summary>
        /// <param name="de">DirectoryEntry对象</param>
        /// <returns></returns>
        internal static bool CanAsContainer(DirectoryEntry de)
        {
            string schemaClassName = de.SchemaClassName;

            return ((schemaClassName == SchemaClass.organizationalUnit.ToString("F")) ||
                (schemaClassName == SchemaClass.builtinDomain.ToString("F")) ||
                (schemaClassName == SchemaClass.container.ToString("F")));
        }



        /// <summary>
        /// 根据DN得到RDN
        /// </summary>
        /// <param name="dn">DN</param>
        /// <returns></returns>
        public static string GetRDNValue(string dn)
        {
            string part = SplitDN(dn)[0];
            return part.Substring(part.IndexOf('=')+1).Trim();
        }

        /// <summary>
        /// 根据DN得到RDN
        /// </summary>
        /// <param name="dn">DN</param>
        /// <returns></returns>
        public static string GetRDN(string dn)
        {
            return SplitDN(dn)[0];
        }

        /// <summary>
        /// 根据对象的GUID生成ADsPath。LDAP://&lt;GUID=xxxxxxxxxxxxxxxx&gt;形式
        /// </summary>
        /// <param name="guid">对象的GUID。</param>
        /// <returns></returns>
        public static string GenerateADsPath(Guid guid)
        {
            return ParaMgr.LDAP_IDENTITY + "<GUID=" + Utils.ConvertGuidToNativeGuid(guid) + ">";
        }

        /// <summary>
        /// 转义DN。转义'\'符号。
        /// </summary>
        /// <param name="odn">DN。</param>
        /// <returns></returns>
        public static string EscapeDNBackslashedChar(string odn)
        {
            // #+="><,/;\
            return odn.Replace("/", "\\/"); 
        }

        /// <summary>
        /// 转义DN
        /// </summary>
        /// <param name="odn">DN。</param>
        /// <returns></returns>
        /// <remarks>没有转义过的的DN。不能检验重复转义。</remarks>
        internal static string EscapeDN(string odn)
        {
            // #+="><,/;\
            return odn.Replace("\\", "\\\\").Replace("#", "\\#").Replace("+", "\\+").Replace("=", "\\=").Replace("\"", "\\\"").Replace(">", "\\>").Replace("<", "\\<").Replace(",", "\\,").Replace(";", "\\;").Replace("/", "\\/");
        }


        /// <summary>
        /// 反转义DN。反转义'\'符号。
        /// </summary>
        /// <param name="dn"></param>
        /// <returns></returns>
        public static string UnEscapeDNBackslashedChar(string dn)
        {
            // #+="><,/;\
            return dn.Replace("\\/", "/");
        }

        /// <summary>
        /// 反转义DN。
        /// </summary>
        /// <param name="dn">转义过的的DN。</param>
        /// <returns></returns>
        internal static string UnEscapeDN(string dn)
        {
            // #+="><,/;\
            return dn.Replace("\\#", "#").Replace("\\+", "+").Replace("\\=", "=").Replace("\\\"", "\"").Replace("\\>", ">").Replace("\\<", "<").Replace("\\,", ",").Replace("\\;", ";").Replace("\\/", "/").Replace("\\\\", "\\");
        }

        /// <summary>
        /// 分割DN(转义或未转义的)各个部分
        /// </summary>
        /// <param name="dn">DN(转义或未转义的)。</param>
        /// <returns></returns>
        /// <remarks>仅分割，不做任何改变。包括转义结果。</remarks>
        public static string[] SplitDN(string dn)
        {
            List<string> parts = new List<string>();
            int l = 0;
            int i = 0;
            for (; i < dn.Length; i++)
            {
                if (dn[i] == ',' && dn[i - 1] != '\\')
                {
                    parts.Add(dn.Substring(l, i-l));
                    l = i + 1;
                }
            }
            if (l < i - 1)
            {
                string last = dn.Substring(l, i - l);
                if (!String.IsNullOrEmpty(last.Trim()))
                    parts.Add(last.Trim());
            }
            return parts.ToArray();
        }

        public static string GetParentDN(string dn)
        {
            int i = 0;
            for (; i < dn.Length; i++)
            {
                if (dn[i] == ',' && dn[i - 1] != '\\')
                {
                    break;
                }
            }

            if (i == dn.Length)
                return null;
            else
                return dn.Substring(i+1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rdn">rdn。完全转义。(含=号前部分)</param>
        /// <param name="parentDN">父的DN。完全转义。</param>
        /// <returns>DN</returns>
        /// <remarks>返回DN会对字符进行转义。</remarks>
        public static string GenerateDN(string rdn, string parentDN)
        {
            if (!String.IsNullOrEmpty(parentDN))
                return String.Format("{0},{1}",
                    rdn, 
                    parentDN);
            else
                return rdn;
        }

        /// <summary>
        /// 生成RDN。CN=***
        /// </summary>
        /// <param name="cn">RDN值。未转义。</param>
        /// <returns></returns>
        public static string GenerateRDNCN(string cn)
        {
            return String.Format("CN={0}",EscapeDN(cn));
        }

        /// <summary>
        /// 生成RDN。OU=***
        /// </summary>
        /// <param name="ou">RDN值。未转义。</param>
        /// <returns></returns>
        public static string GenerateRDNOU(string ou)
        {
            return String.Format("OU={0}", EscapeDN(ou));
        }


        /// <summary>
        /// 未DirectorySearcher查询进行转义。
        /// </summary>
        /// <param name="cn"></param>
        /// <returns></returns>
        public static string Escape4Query(string cn)
        {
            return cn.Replace("(", "\\28").Replace(")", "\\29").Replace("\\", "\\5c");
        }



        /// <summary>
        /// SAMAccountName限制字符。
        /// </summary>
        public static char[] InvalidSAMAccountNameChars = new char[] { 
                '/', '\\', '[', ']', ':', ';', '|', '=', 
                ',', '+',  '*', '?', '<', '>', '"'};

        // 创建DirectoryEntry时，通过UserName和Password提供凭据
        // 如果没有提供，则使用当前进程的凭据
        // 只要凭据有效即可创建DirectoryEntry
        // 但对其进行操作时（如SetPassword），需要对凭据进行访问控制，检验权限。
    }
}
