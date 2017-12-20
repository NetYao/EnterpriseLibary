using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Xml.Serialization;
using System.Diagnostics;

namespace Enterprises.Framework.Utility
{
	/// <summary>
	/// 加密工具类
	/// </summary>
	public class EncryptUtility
	{
        /// <summary>
        /// 采用Triple加密时对应的Key,是Base64编码格式的字符串
        /// </summary>
        public static readonly byte[] EncryptTripleKeys = EncodingUtility.ToBytesFromBase64("5aea56uL5bOw5Yqg5a+G5a2X56ym5Liy");
        /// <summary>
        /// 采用Triple加密时对应的IV，是Base64编码格式的字符串
        /// </summary>
        public static readonly byte[] EncryptTripleIVs = EncodingUtility.ToBytesFromBase64("RG9tb1lhb0NoaW5lc2VOYW1lSXNZYW9saWZlbmc=");

        /// <summary>
        /// 采用DES加密时对应的Key，是Base64编码格式的字符串
        /// </summary>
        public static readonly byte[] EncryptDesKeys = EncodingUtility.ToBytesFromBase64("5aea56uL5bOw5Yqg5a+G5a2X56ym5Liy");
        /// <summary>
        /// 采用DES加密时对应的IV，是Base64编码格式的字符串
        /// </summary>
        public static readonly byte[] EncryptDesIVs = EncodingUtility.ToBytesFromBase64("RG9tb1lhb0NoaW5lc2VOYW1lSXNZYW9saWZlbmc=");
        
        /// <summary>
        /// 解密注册信息的公钥密码，通过私钥加密，然后公钥来解密。
        /// </summary>
        public static readonly string RegeditInfoPublicKey = "QVFBQjtyaGUvSlo4QmYvQVEvQ0Z1RUZKOG56TmQ2UXZQTXByUU9ibGNJbjRuQjRtYmtpZnNnbG1GOXVUdVFWK014Ri8rTUlleWsyQ2hkOGFaTDFwZDFDUDczMFNDc0NnL1dJMlA5dTZMMjExMmFRS0dYbGd5UjhsT2tKWmtWSkQ3a2MzczFVbzM3Q3poNHA5NzBhd0YxWllDZW96ZTR5L2hYd3owd01MbnJFOHV1dk09";

        /// <summary>
        /// 获取默认的UnicodeEncoding编码格式
        /// </summary>
        public static readonly Encoding DefaultEncoding = new UnicodeEncoding();

        /// <summary>
        /// 把指定的字符串加密为可以通过Url传递的字符串
        /// </summary>
        /// <param name="rawString"></param>
        /// <returns></returns>
        public static string EncryptUrlString(Guid rawString)
        {
            return EncryptUrlString(rawString.ToString());
        }
        /// <summary>
        /// 把指定的字符串加密为可以通过Url传递的字符串
        /// </summary>
        /// <param name="rawString"></param>
        /// <returns></returns>
        public static string EncryptUrlString(string rawString)
        {
            //追加时间戳，方便后续判断
            string nowTicks = DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture);

            string newParam = string.Format("{0}-{1}", nowTicks, rawString);

            string result = EncryptByTriple(newParam);
            result = EncodingUtility.ToBase64FromGb2312(result);
            result = HttpUtility.UrlEncode(result);
            return result;
        }

	    /// <summary>
	    /// 把指定的已经加密的并且可以通过Url传递的字符串解密
	    /// </summary>
	    /// <param name="encryptedUrlString"></param>
	    /// <returns></returns>
	    public static string DecryptUrlString(string encryptedUrlString)
        {
            const long allowDelayTicks = -1;
            const bool validateTimestamp = false;
            return DecryptUrlString(encryptedUrlString, validateTimestamp, allowDelayTicks);
        }

        /// <summary>
        /// 把指定的已经加密的并且可以通过Url传递的字符串解密，字符串允许延时的Ticks
        /// </summary>
        /// <param name="encryptedUrlString"></param>
        /// <param name="allowDelayTicks"></param>
        /// <returns></returns>
        public static string DecryptUrlString(string encryptedUrlString, long allowDelayTicks)
        {
            const bool validateTimestamp = true;
            return DecryptUrlString(encryptedUrlString, validateTimestamp, allowDelayTicks);
        }

	    /// <summary>
	    /// 把指定的已经加密的并且可以通过Url传递的字符串解密
	    /// </summary>
	    /// <param name="encryptedUrlString"></param>
	    /// <param name="validateTimestamp">是否验证时间戳</param>
	    /// <param name="allowDelayTicks"></param>
	    /// <returns></returns>
	    private static string DecryptUrlString(string encryptedUrlString, bool validateTimestamp, long allowDelayTicks)
        {
            string result = HttpUtility.UrlDecode(encryptedUrlString);
            result = EncodingUtility.ToGb2312StringFromBase64(result);
            result = DecryptByTriple(result);

            //如果用户没有传入数据，则直接忽略
            if (result.Trim() == string.Empty)
            {
                return result;
            }

            //现在都携带了时间戳，需要把时间戳去掉
            if (result.IndexOf("-", StringComparison.Ordinal) == -1)
            {
                throw new Exception("需要解密的字符串格式不正确");
            }
            string rawTicks = result.Substring(0, result.IndexOf("-", StringComparison.Ordinal));
            long lRawTicks;
            if (!long.TryParse(rawTicks, out lRawTicks))
            {
                throw new Exception("需要解密的字符串格式不正确");
            }

            if (validateTimestamp && allowDelayTicks > 0)
            {
                long factDelayTicks = DateTime.Now.Ticks - lRawTicks;
                if (factDelayTicks > allowDelayTicks)
                {
                    throw new Exception("需要解密的字符串数据不正确");
                }
            }

            return result.Remove(0, result.IndexOf("-", StringComparison.Ordinal) + 1);
        }

        /// <summary>
		/// [私钥加密范畴（对称加密）]该加密方式需要双方的就密钥和IV值取得了一致才能进行，否则加密后的东西，对方是解不开的，
		/// 在该方案中，密钥必须保密，而IV可以不保密
		/// 生成新的加密密钥和向量
		/// </summary>
        public static TripleKeyIVEntity GenerateTripleKeyIv()
		{
            var keyIv = new TripleKeyIVEntity();
			var tdes = new TripleDESCryptoServiceProvider();
			tdes.GenerateKey();
			tdes.GenerateIV();
			keyIv.KeyBytes = tdes.Key;
			keyIv.IVBytes = tdes.IV;
            return keyIv;
		}

        /// <summary>
        /// 把字符串解密（字符串必须是base64的），返回的结果字符串为utf8格式的
        /// </summary>
        /// <param name="encryptString"></param>
        /// <returns></returns>
        public static string DecryptByTriple(string encryptString)
        {
            return DecryptByTriple(encryptString,DefaultEncoding, EncryptTripleKeys, EncryptTripleIVs);
        }

        /// <summary>
        /// 把字符串解密（字符串必须是base64的）
        /// </summary>
        /// <param name="base64EncryptString"></param>
        /// <param name="resultEncoding"></param>
        /// <param name="srcKey"></param>
        /// <param name="srcIv"></param>
        /// <returns></returns>
        public static string DecryptByTriple(string base64EncryptString,Encoding resultEncoding, byte[] srcKey, byte[] srcIv)
        {
            if (string.IsNullOrEmpty(base64EncryptString))
            {
                return string.Empty;
            }

            if (resultEncoding == null)
            {
                resultEncoding = DefaultEncoding;
            }

            MemoryStream fin = null;
            MemoryStream fout = null;
            try
            {
                byte[] encryptBytes = Convert.FromBase64String(base64EncryptString);
                using (fin = new MemoryStream(encryptBytes))
                {
                    using (fout = new MemoryStream())
                    {
                        using (var tdes = new TripleDESCryptoServiceProvider())
                        {
                            using (var encStream = new CryptoStream(fout, tdes.CreateDecryptor(srcKey, srcIv), CryptoStreamMode.Write))
                            {
                                encStream.Write(encryptBytes, 0, encryptBytes.Length);
                                encStream.FlushFinalBlock();
                                byte[] outputBytes = fout.ToArray();
                                encStream.Close();
                                string result = resultEncoding.GetString(outputBytes);
                                return result;
                            }
                        }
                    }
                }
            }
            finally
            {
                if (fin != null)
                {
                    fin.Close();
                }
                if (fout != null)
                {
                    fout.Close();
                }
            }
        } 
      
		/// <summary>
		/// [私钥加密范畴（对称加密）]
		/// 解密已经加密的文件，srcKey,srcIV应该与加密的文件采用的key,iv一致,如果没有记录原来加密时的这些数据，则无法解密了
		/// </summary>
		/// <param name="inputFileName">输入已经加密的文件</param>
		/// <param name="outputFileName">输出解密后的文件</param>
		/// <param name="srcKey">对称密钥</param>
		/// <param name="srcIv">初始化向量</param>
        public static void DecryptByTriple(string inputFileName, string outputFileName, byte[] srcKey, byte[] srcIv)
		{
            FileStream fin = null;
            FileStream fout = null;

            try
            {
                //Create the file streams to handle the input and output files.
                //以流的方式来控制输入输出文件

                //源文件，打开，只读
                fin = new FileStream(inputFileName, FileMode.Open, FileAccess.Read);
                //目标文件，打开或者创建，可写
                fout = new FileStream(outputFileName, FileMode.OpenOrCreate, FileAccess.Write);
                fout.SetLength(0);


                //定义每次解密块的大小（私钥加密按块进行）
                const int binLength = 100;
                var bin = new byte[binLength];
                //写入目标文件的流的大小
                long rdlen = 0;
                long inputFileLength = fin.Length;
                //记录每次转换解密的字符长度

                //定义解密服务对象tdes
                var tdes = new TripleDESCryptoServiceProvider();
                //解密提供流
                var encStream = new CryptoStream(fout, tdes.CreateDecryptor(srcKey, srcIv),CryptoStreamMode.Write);

                //根据上面设置的解密方式(encStream)，对源文件按块进行解密
                while (rdlen < inputFileLength)
                {
                    int len = fin.Read(bin, 0, binLength);
                    encStream.Write(bin, 0, len);
                    rdlen = rdlen + len;
                }
                encStream.Close();
            }
            finally
            {
                if (fin != null) fin.Close();
                if (fout != null) fout.Close();
            }
		}

        /// <summary>
        /// 把字符串加密（可以进行解密），并且把加密结果转换为base64编码。输入的字符串要为utf8格式的
        /// </summary>
        /// <param name="rawString"></param>
        /// <returns></returns>
        public static string EncryptByTriple(string rawString)
        {
            return EncryptByTriple(rawString,DefaultEncoding, EncryptTripleKeys, EncryptTripleIVs);
        }

        /// <summary>
        /// 把字符串加密（可以进行解密），并且把加密结果转换为base64编码.
        /// </summary>
        /// <param name="rawString">需要加密的数据</param>
        /// <param name="passwordString">用来加密的密钥</param>
        /// <returns></returns>
        public static string EncryptByTriple(string rawString,string passwordString)
        {
            byte[] passwordBytes = EncodingUtility.ToBytesFromUnicode(passwordString);
            byte[] passwordMd5Bytes = ComputeHashByMD5(passwordBytes);
            return EncryptByTriple(rawString, DefaultEncoding, passwordMd5Bytes, passwordMd5Bytes);
        }

        /// <summary>
        /// 把字符串加密（可以进行解密），并且把加密结果转换为base64编码。其中的Key长度必须为24位，IV必须为8位。这样可以确保跨平台的加密解密结果一致
        /// </summary>
        /// <param name="inputString"></param>
        /// <param name="inputStringEncoding"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static string EncryptByTriple(string inputString, Encoding inputStringEncoding, string key, string iv)
        {
            #region 参数检查

            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            if(key.Trim() == string.Empty)
            {
                throw new Exception("Triple加密的Key不允许为空字符串");
            }
            key = key.Trim();
            if (key.Length != 24)
            {
                throw new Exception("Triple加密的Key长度必须为24位");
            }

            if (iv == null)
            {
                throw new ArgumentNullException("iv");
            }
            if (iv.Trim() == string.Empty)
            {
                throw new Exception("Triple加密的iv不允许为空字符串");
            }
            iv = iv.Trim();
            if (iv.Length != 8)
            {
                throw new Exception("Triple加密的iv长度必须为8位");
            }

            #endregion

            byte[] keyArray = Encoding.UTF8.GetBytes(key);
            byte[] ivArray = Encoding.UTF8.GetBytes(iv);

            return EncryptByTriple(inputString, inputStringEncoding, keyArray, ivArray);
        }

        /// <summary>
        /// 把字符串加密（可以进行解密），并且把加密结果转换为base64编码。
        /// </summary>
        /// <param name="inputString"></param>
        /// <param name="inputStringEncoding"></param>
        /// <param name="srcKey"></param>
        /// <param name="srcIv"></param>
        /// <returns></returns>
        public static string EncryptByTriple(string inputString, Encoding inputStringEncoding, byte[] srcKey, byte[] srcIv)
        {
            if (inputStringEncoding == null)
            {
                inputStringEncoding = DefaultEncoding;
            }

            byte[] inputBytes = inputStringEncoding.GetBytes(inputString);

            return EncryptByTriple(inputBytes, srcKey, srcIv);
        }

        /// <summary>
        /// 把字节数组加密（可以进行解密），并且把加密结果转换为base64编码。
        /// </summary>
        /// <param name="inputData"></param>
        /// <param name="srcKey"></param>
        /// <param name="srcIv"></param>
        /// <returns></returns>
        public static string EncryptByTriple(byte[] inputData, byte[] srcKey, byte[] srcIv)
        {
            MemoryStream fin = null;
            MemoryStream fout = null;
            try
            {
                using (fin = new MemoryStream(inputData))
                {
                    using (fout = new MemoryStream())
                    {
                        using (var tdes = new TripleDESCryptoServiceProvider())
                        {
                            using (var encStream = new CryptoStream(fout, tdes.CreateEncryptor(srcKey, srcIv), CryptoStreamMode.Write))
                            {
                                encStream.Write(inputData, 0, inputData.Length);
                                encStream.FlushFinalBlock();
                                byte[] outputBytes = fout.ToArray();

                                encStream.Flush();
                                encStream.Close();

                                string result = Convert.ToBase64String(outputBytes);
                                return result;
                            }
                        }
                    }
                }
            }
            finally
            {
                if (fin != null)
                {
                    fin.Close();
                }
                if (fout != null)
                {
                    fout.Flush();
                    fout.Close();
                }
            }
        }

        /// <summary>
		/// [私钥加密范畴（对称加密）]
		/// 加密文件
		/// </summary>
		/// <param name="inputFileName">输入文件名(包含路径)，将加密的源文件</param>
		/// <param name="outputFileName">输出文件名(包含路径)，将上面的加密结果输出到该目标文件</param>
		/// <param name="srcKey">Key</param>
		/// <param name="srcIv">指定一个初始化向量IV</param>
        public static void EncryptByTriple(string inputFileName, string outputFileName, byte[] srcKey, byte[] srcIv)
        {
            FileStream fin = null;
            FileStream fout = null;

            try
            {
                //Create the file streams to handle the input and output files.
                //以流的方式来控制输入输出文件

                //源文件，打开，只读
                fin = new FileStream(inputFileName, FileMode.Open, FileAccess.Read);                
                //目标文件，打开或者创建，可写
                fout = new FileStream(outputFileName, FileMode.OpenOrCreate, FileAccess.Write);
                fout.SetLength(0);

                //Create variables to help with read and write.

                //定义每次加密块的大小（私钥加密按块进行）
                int binLength = 100;
                var bin = new byte[binLength];
                //写入目标文件的流的大小
                long rdlen = 0;
                long inputFileLength = fin.Length;
                //记录每次转换加密的字符长度
                int len;

                //定义加密服务对象tdes
                var tdes = new TripleDESCryptoServiceProvider();
                //加密提供流
                var encStream = new CryptoStream(fout, tdes.CreateEncryptor(srcKey, srcIv),
                    CryptoStreamMode.Write);

                //Read from the input file, then encrypt and write to the output file.
                //根据上面设置的加密方式(encStream)，对源文件按块进行加密
                while (rdlen < inputFileLength)
                {
                    len = fin.Read(bin, 0, binLength);
                    encStream.Write(bin, 0, len);
                    rdlen = rdlen + len;
                }
                encStream.Close();
            }
            finally
            {
                if (fin != null) fin.Close();
                if (fout != null) fout.Close();
            }
        }

	    /// <summary>
	    /// 生成Des加密采用的Key和IV（两者都是八位）
	    /// </summary>
	    public static DesKeyIVEntity GenerateDesKeyIv()
        {
            var keys = new byte[8];
            var ivs = new byte[8];
            var rd = new Random();
            rd.NextBytes(keys);
            rd.NextBytes(ivs);

            var keyIv = new DesKeyIVEntity {KeyBytes = keys, IVBytes = ivs};
	        return keyIv;
        }

        /// <summary>
        /// 以Des方式进行加密，key和iv的长度都是8位，返回Base64格式的字符串
        /// </summary>
        /// <param name="inputString"></param>
        /// <param name="inputStringEncoding"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static string EncryptByDES(string inputString,Encoding inputStringEncoding, byte[] key, byte[] iv)
        {
            if (inputStringEncoding == null)
            {
                inputStringEncoding = DefaultEncoding;
            }
            
            byte[] toEncrypt = inputStringEncoding.GetBytes(inputString);
            return EncryptByDES(toEncrypt, key, iv);
        }

        /// <summary>
        /// 以Des方式进行加密，key和iv的长度都是8位，返回Base64格式的字符串
        /// </summary>
        /// <param name="inputData"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static string EncryptByDES(byte[] inputData, byte[] key, byte[] iv)
        {
            using (MemoryStream mStream = new MemoryStream())
            {
                using(DESCryptoServiceProvider desProvider = new DESCryptoServiceProvider())
                {
                    desProvider.Mode = CipherMode.CBC;                    
                    using (CryptoStream cStream = new CryptoStream(mStream, desProvider.CreateEncryptor(key, iv), CryptoStreamMode.Write))
                    {
                        cStream.Write(inputData, 0, inputData.Length);
                        cStream.FlushFinalBlock();
                        byte[] resultBytes = mStream.ToArray();
                        cStream.Close();
                        mStream.Close();
                        return Convert.ToBase64String(resultBytes);
                    }
                }
            }
        }

        /// <summary>
        /// 根据key和iv采用des方式解密字符串，输入字符串为base64格式
        /// </summary>
        /// <param name="inputBase64Data"></param>
        /// <param name="resultEncoding"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static string DecryptByDES(string inputBase64Data,Encoding resultEncoding, byte[] key, byte[] iv)
        {
            if (resultEncoding == null)
            {
                resultEncoding = DefaultEncoding;
            }
            byte[] normalValue = Convert.FromBase64String(inputBase64Data);
            using (var msDecrypt = new MemoryStream(normalValue))
            {
                using (var desProvider = new DESCryptoServiceProvider())
                {
                    desProvider.Mode = CipherMode.CBC;
                    using (var csDecrypt = new CryptoStream(msDecrypt, desProvider.CreateDecryptor(key, iv), CryptoStreamMode.Read))
                    {
                        using (var sr = new StreamReader(csDecrypt, resultEncoding))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
        }

		/// <summary>
		/// 把十进制转换为十六进制
		/// </summary>
		/// <param name="intValue"></param>
		/// <returns></returns>
		public static string ConvertToHexString(int intValue)
		{
			return string.Format("{0,6:x}",intValue);
		}

        /// <summary>
        /// 字节数组转十六进制字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ConvertToHexString(byte[] bytes)
        {
            var sbResult = new StringBuilder(bytes.Length);
            for (int i = 0; i < bytes.Length; i++)
            {
                sbResult.Append(bytes[i].ToString("X2"));
            }

            return sbResult.ToString();
        }
		
		/// <summary>
		/// 根据传递进入的十进制（每个字节用-分开），转换成byte数组
		/// </summary>
        /// <param name="decString"></param>
		/// <returns></returns>
		public static byte[] ConvertToByte(string decString)
		{
			string[] decArray = decString.Split('-');
			int num = decArray.Length;
			var bts = new byte[num];
			int i = 0;
			foreach(string dec in decArray)
			{
				bts.SetValue(Convert.ToByte(dec),i);
				i++;
			}
			return bts;
		}

        /// <summary>
        /// 采用MD5加密，采用UnicodeEncoding编码格式
        /// </summary>
        /// <param name="inputData"></param>
        /// <returns></returns>
        public static string EncryptByMD5(string inputData)
        {
            return EncryptByMD5(inputData, DefaultEncoding);
        }

        /// <summary>
        /// 采用MD5加密，可以指定输入字符串编码的格式
        /// </summary>
        /// <param name="inputData"></param>
        /// <param name="inputDataEncoding"></param>
        /// <returns></returns>
        public static string EncryptByMD5(string inputData, Encoding inputDataEncoding)
        {
            byte[] inputBytes = inputDataEncoding.GetBytes(inputData);
            return EncryptByMD5(inputBytes);   
        }

        /// <summary>
        /// 采用MD5加密字节数组
        /// </summary>
        /// <param name="inputData"></param>
        /// <returns></returns>
        public static string EncryptByMD5(byte[] inputData)
        {
            using (MD5 m = new MD5CryptoServiceProvider())
            {
                byte[] outputs = m.ComputeHash(inputData);
                return EncodingUtility.ToBase64FromBytes(outputs);
            }
        }

        /// <summary>
        /// 把Hash字节数组转换为字符串
        /// </summary>
        /// <param name="hashBytes"></param>
        /// <returns></returns>
        public static string BytesToString(byte[] hashBytes)
        {
            if (hashBytes == null)
            {
                throw new ArgumentNullException("hashBytes");
            }

            string result = string.Empty;
            for (int i = 0; i < hashBytes.Length; i++)
            {
                result += hashBytes[i].ToString("x").PadLeft(2, '0').ToUpper();
            }
            return result;
        }

        /// <summary>
        /// 计算文件的hash结果，可以用来判断客户端和服务器的文件版本是否一致。
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static byte[] ComputeHashByMD5(string fileName)
        {
            FileStream fs = null;
            try
            {
                //一定要加上FileAccess.Read，否则认为是Read/Write了
                fs = new FileStream(fileName, FileMode.Open,FileAccess.Read);
                using (MD5 md5 = new MD5CryptoServiceProvider())
                {
                    byte[] result = md5.ComputeHash(fs);
                    return result;
                }
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }
            }
        }

        /// <summary>
        /// 采用MD5计算二进制序列的Hash结果，并转换成Base64格式的字符串返回
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        public static string ComputeHashStringByMD5(string fileName)
        {
            return EncodingUtility.ToBase64FromBytes(ComputeHashByMD5(fileName));
        }

        /// <summary>
        /// 采用MD5计算二进制序列的Hash结果，并转换成Base64格式的字符串返回
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        public static string ComputeHashStringByMD5(byte[] inputs)
        {
            return EncodingUtility.ToBase64FromBytes(EncryptUtility.ComputeHashByMD5(inputs));
        }

       

        /// <summary>
        /// 采用MD5计算二进制序列的Hash结果
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        public static byte[] ComputeHashByMD5(byte[] inputs)
        {
            if (inputs == null)
            {
                return null;
            }
            try
            {
                using (MD5 md5 = new MD5CryptoServiceProvider())
                {
                    byte[] result = md5.ComputeHash(inputs);
                    return result;
                }
            }
            finally
            {
            }
        }

        /// <summary>
        /// 比较两个二进制序列是否一致，如果一致返回true，否则返回false
        /// </summary>
        /// <param name="bytes1"></param>
        /// <param name="bytes2"></param>
        /// <returns></returns>
        public static bool CompareBytes(byte[] bytes1, byte[] bytes2)
        {
            if (bytes1 == null && bytes2 == null)
            {
                return true;
            }
            if (bytes1 == null || bytes2 == null || bytes1.Length != bytes2.Length)
            {
                return false;
            }
            for (int i = 0; i < bytes1.Length; i++)
            {
                if (bytes1[i] != bytes2[i])
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 采用SHA1加密算法加密指定的字符串inputData（大小写加密的结果不同），
        /// 采用默认的UnicodeEncoding编码格式
        /// </summary>
        /// <param name="inputData"></param>
        /// <returns></returns>
        public static string EncryptBySHA1(string inputData)
        {
            return EncryptBySHA1(inputData, DefaultEncoding);
        }

		/// <summary>
        /// 采用SHA1加密算法加密指定的字符串inputData（大小写加密的结果不同）
		/// </summary>
		/// <param name="inputData"></param>
        /// <param name="inputDataEncoding"></param>
        public static string EncryptBySHA1(string inputData,Encoding inputDataEncoding)
		{
            byte[] inputBytes = inputDataEncoding.GetBytes(inputData);
            return EncryptBySHA1(inputBytes);
		}

        /// <summary>
        /// 采用SHA1加密字节数组
        /// </summary>
        /// <param name="inputData"></param>
        /// <returns></returns>
        public static string EncryptBySHA1(byte[] inputData)
        {
            using (SHA1 sha = new SHA1CryptoServiceProvider())
            {
                byte[] outputs = sha.ComputeHash(inputData);
                return BytesToString(outputs);
            }
        }

        #region AES加密解密
        //AES密钥向量
        private static readonly byte[] _aeskeys = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF, 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="encryptStr">加密字符串</param>
        /// <param name="encryptKey">密钥</param>
        /// <returns></returns>
        public static string AESEncrypt(string encryptStr, string encryptKey)
        {
            if (string.IsNullOrWhiteSpace(encryptStr))
                return string.Empty;

            encryptKey = StringHelper.SubString(encryptKey, 32);
            encryptKey = encryptKey.PadRight(32, ' ');

            //分组加密算法
            SymmetricAlgorithm des = Rijndael.Create();
            byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptStr);//得到需要加密的字节数组 
            //设置密钥及密钥向量
            des.Key = Encoding.UTF8.GetBytes(encryptKey);
            des.IV = _aeskeys;
            byte[] cipherBytes = null;
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(inputByteArray, 0, inputByteArray.Length);
                    cs.FlushFinalBlock();
                    cipherBytes = ms.ToArray();//得到加密后的字节数组
                    cs.Close();
                    ms.Close();
                }
            }
            return Convert.ToBase64String(cipherBytes);
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="decryptStr">解密字符串</param>
        /// <param name="decryptKey">密钥</param>
        /// <returns></returns>
        public static string AESDecrypt(string decryptStr, string decryptKey)
        {
            if (string.IsNullOrWhiteSpace(decryptStr))
                return string.Empty;

            decryptKey = StringHelper.SubString(decryptKey, 32);
            decryptKey = decryptKey.PadRight(32, ' ');

            byte[] cipherText = Convert.FromBase64String(decryptStr);

            SymmetricAlgorithm des = Rijndael.Create();
            des.Key = Encoding.UTF8.GetBytes(decryptKey);
            des.IV = _aeskeys;
            byte[] decryptBytes = new byte[cipherText.Length];
            using (MemoryStream ms = new MemoryStream(cipherText))
            {
                using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    cs.Read(decryptBytes, 0, decryptBytes.Length);
                    cs.Close();
                    ms.Close();
                }
            }
            return Encoding.UTF8.GetString(decryptBytes).Replace("\0", "");//将字符串后尾的'\0'去掉
        }

        #endregion

        #region 非对称加密

        /// <summary>
		/// [非对称加密]获取非对称加密的密钥（此包含公钥和私钥）
		/// </summary>
		/// <returns></returns>
		public static string GetPublicAndPrivateKey()
		{
            var rsaParams = new CspParameters {Flags = CspProviderFlags.UseMachineKeyStore};
            using (var csp = new RSACryptoServiceProvider(1024, rsaParams))
            {
                return csp.ToXmlString(true);
            }
		}
		
		/// <summary>
		/// [非对称加密]从XML格式的Key中获取公钥参数[最终，不能通过xmlKeyString把整个xml都传递过去，而是应该值传递需要的，这样才能有安全保证]
		/// </summary>
		/// <param name="xmlKeyString"></param>
		/// <returns></returns>
		public static RSAParameters GetPublicKeyParam(string xmlKeyString)
		{
            var rsaParams = new CspParameters {Flags = CspProviderFlags.UseMachineKeyStore};
		    using (var csp = new RSACryptoServiceProvider(1024, rsaParams))
            {
                csp.FromXmlString(xmlKeyString);
                return csp.ExportParameters(false);
            }
		}

        /// <summary>
        /// 从XML格式的非对称密钥中获取公钥部分的XML字符串
        /// </summary>
        /// <param name="publicAndPrivateKey"></param>
        /// <returns></returns>
        public static string GetPublicKey(string publicAndPrivateKey)
        {
            var rsaParams = new CspParameters {Flags = CspProviderFlags.UseMachineKeyStore};
            using (var csp = new RSACryptoServiceProvider(1024, rsaParams))
            {
                csp.FromXmlString(publicAndPrivateKey);
                return csp.ToXmlString(false);
            }
        }

		/// <summary>
        /// [非对称加密]返回签名的结果（Base64格式），采用私钥签名输入的数据
        /// 返回加密的结果（Base64格式）
		/// </summary>
        /// <param name="inputData">输入的信息，一般建议这个数据是Base64格式的，即签名时用Base64的数据源。验证时自然也用Base64格式的</param>
        /// <param name="inputDataEncoding"></param>
        /// <param name="xmlPrivateKeyString">xml格式的私钥字符串</param>
		/// <returns>返回加密的结果（Base64格式）</returns>
		public static string SignData(string inputData,Encoding inputDataEncoding ,string xmlPrivateKeyString)
        {
            #region Added By Eric 2012-04-13 

            if (inputData == null)
            {
                throw new ArgumentNullException("inputData");
            }

            if (xmlPrivateKeyString == null)
            {
                throw new ArgumentNullException("xmlPrivateKeyString");
            }

            #endregion


            var rsaParams = new CspParameters {Flags = CspProviderFlags.UseMachineKeyStore};
		    using (var csp = new RSACryptoServiceProvider(1024,rsaParams))
            {                
                csp.FromXmlString(xmlPrivateKeyString);
                return SignData(inputData, inputDataEncoding, csp);
            }
		}

        /// <summary>
        /// [非对称加密]返回签名的结果，采用私钥签名输入的数据
        /// 返回加密的结果（Base64格式）
        /// </summary>
        /// <param name="inputData"></param>
        /// <param name="inputDataEncoding"></param>
        /// <param name="csp"></param>
        /// <returns></returns>
        public static string SignData(string inputData, Encoding inputDataEncoding, RSACryptoServiceProvider csp)
        {
            #region Added By Eric 2012-04-13

            if (inputData == null)
            {
                throw new ArgumentNullException("inputData");
            }

            //计算传入数据的散列值，这样后面的签名会更快
            string inputDataHashValue = EncryptBySHA1(inputData, inputDataEncoding);

            #endregion

            using (var sp = new SHA1CryptoServiceProvider())
            {
                byte[] inputBytes = inputDataEncoding.GetBytes(inputDataHashValue);
                byte[] result = csp.SignData(inputBytes, sp);
                string base64Result = Convert.ToBase64String(result);
                return base64Result;
            }
        }

        /// <summary>
        /// [非对称加密]校验输入的信息和计算出来的信息是否一致（采用公钥来验证）
        /// 如果被签名的数据（即签名前的明文数据）和签名数据对上号，则返回true，否则false
        /// </summary>
        /// <param name="inputData">被签名的数据（即签名前的明文数据）</param>
        /// <param name="inputDataEncoding">被签名的数据的编码格式</param>
        /// /// <param name="signaturedBase64String">需要验证的签名数据</param>
        /// <param name="xmlPublicKeyString">xml格式的公钥字符串</param>
        /// <returns></returns>
        public static bool VerifyData(string inputData, Encoding inputDataEncoding, string signaturedBase64String, string xmlPublicKeyString)
        {
            #region Added By Eric 2012-04-13

            if (inputData == null)
            {
                throw new ArgumentNullException("inputData");
            }

            if (signaturedBase64String == null)
            {
                throw new ArgumentNullException("signaturedBase64String");
            }

            if (xmlPublicKeyString == null)
            {
                throw new ArgumentNullException("xmlPublicKeyString");
            }

            #endregion

            byte[] signature = Convert.FromBase64String(signaturedBase64String);
            return VerifyData(inputData, inputDataEncoding, signature, xmlPublicKeyString);
        }
		
		/// <summary>
		/// [非对称加密]校验输入的信息和计算出来的信息是否一致（采用公钥来验证）
        ///  如果被签名的数据（即签名前的明文数据）和签名数据对上号，则返回true，否则false。
        ///  这里的验证，如果当前机器加入域的话，会通过网络去验证，导致速度很慢。需要注意。
		/// </summary>        
        /// <param name="inputData">被签名的数据（即签名前的明文数据）</param>
        /// <param name="inputDataEncoding">被签名的数据的编码格式</param>
        /// /// <param name="signature">需要验证的签名数据</param>
		/// <param name="xmlPublicKeyString">xml格式的公钥字符串</param>
		/// <returns></returns>
        public static bool VerifyData(string inputData, Encoding inputDataEncoding, byte[] signature, string xmlPublicKeyString)
        {

            if (inputData == null)
            {
                throw new ArgumentNullException("inputData");
            }

            if (signature == null)
            {
                throw new ArgumentNullException("signature");
            }

            if (xmlPublicKeyString == null)
            {
                throw new ArgumentNullException("xmlPublicKeyString");
            }

            //计算传入数据的散列值，这样后面的签名会更快
            string inputDataHashValue = EncryptBySHA1(inputData, inputDataEncoding);

            byte[] inputBytes = inputDataEncoding.GetBytes(inputDataHashValue);
            var rsaParams = new CspParameters {Flags = CspProviderFlags.UseMachineKeyStore};
		    using (var csp = new RSACryptoServiceProvider(1024, rsaParams))
            {
                csp.ImportParameters(GetPublicKeyParam(xmlPublicKeyString));
                using (var sp = new SHA1CryptoServiceProvider())
                {
                    //这里的验证，如果当前机器加入域的话，会通过网络去验证，导致速度很慢。需要注意。
                    //只有无线网：23毫秒；没有任何网络时：156毫秒；有线网络时：15秒。
                    var swTime = new Stopwatch();
                    swTime.Start();
                    var result = csp.VerifyData(inputBytes, sp, signature);
                    swTime.Stop();
                    if (swTime.ElapsedMilliseconds > 1000)
                    {
                       throw new Exception(string.Format("采用非对称密钥进行验证时花费的时间较长，花费了{0}毫秒。（如果当前机器加入域的话，会通过网络去验证，导致速度很慢，甚至要用十几秒。）", swTime.ElapsedMilliseconds));
                    }

                    return result;
                }
            }
        }

        /// <summary>
        /// 采用非对称方式用公钥加密字符串，返回Base64格式的结果
        /// </summary>
        /// <param name="inputData">输入的需要加密的字符串</param>
        /// <param name="inputDataEncoding">输入字符串的编码格式</param>
        /// <param name="publicKey">公钥字符串</param>
        /// <returns></returns>
        public static string EncryptByRSA(string inputData,Encoding inputDataEncoding,string publicKey)
        {
            byte[] inputBytes = inputDataEncoding.GetBytes(inputData);
            var rsaParams = new CspParameters {Flags = CspProviderFlags.UseMachineKeyStore};
            using (var csp = new RSACryptoServiceProvider(1024, rsaParams))
            {
                csp.ImportParameters(GetPublicKeyParam(publicKey));
                byte[] resultBytes = csp.Encrypt(inputBytes, true);
                return Convert.ToBase64String(resultBytes);
            }
        }

        /// <summary>
        /// 采用非对称方式用私钥解密字符串，返回指定编码格式的字符串
        /// </summary>
        /// <param name="inputBase64Data">已经加密的数据(Base64格式）</param>
        /// <param name="resultDataEncoding">解密结果的编码格式</param>
        /// <param name="publicAndPrivateKey">密钥（公、私都有）</param>
        /// <returns></returns>
        public static string DecryptByRSA(string inputBase64Data, Encoding resultDataEncoding, string publicAndPrivateKey)
        {
            byte[] inputBytes = Convert.FromBase64String(inputBase64Data);
            var rsaParams = new CspParameters {Flags = CspProviderFlags.UseMachineKeyStore};
            using (var csp = new RSACryptoServiceProvider(1024, rsaParams))
            {
                csp.FromXmlString(publicAndPrivateKey);
                byte[] resultBytes = csp.Decrypt(inputBytes, true);
                return resultDataEncoding.GetString(resultBytes);
            }
        }

       

        /// <summary>
        /// 采用RAS和DES一起进行加密操作，公钥加密Key/IV，Key和IV来加密源数据
        /// </summary>
        /// <param name="xmlPublicKey"></param>
        /// <param name="srcDataToEncrypted"></param>
        /// <returns></returns>
        public static RSADESEncryptResult EncryptByRSADES(string xmlPublicKey, string srcDataToEncrypted)
        {
            DesKeyIVEntity keyIv = EncryptUtility.GenerateDesKeyIv();
            return EncryptByRSADES(xmlPublicKey, srcDataToEncrypted, keyIv);
        }

        /// <summary>
        /// 采用RAS和DES一起进行加密操作，公钥加密Key/IV，Key和IV来加密源数据，可以指定具体采用的Key / Iv
        /// </summary>
        /// <param name="xmlPublicKey"></param>
        /// <param name="srcDataToEncrypted"></param>
        /// <returns></returns>
        public static RSADESEncryptResult EncryptByRSADES(string xmlPublicKey, string srcDataToEncrypted, DesKeyIVEntity keyIv)
        {
            RSADESEncryptResult result = new RSADESEncryptResult();

            //公钥加密Key
            result.EncryptedDesKeyIV = EncryptUtility.EncryptByRSA(string.Format("{0}.{1}", keyIv.KeyString, keyIv.IVString), Encoding.Unicode, xmlPublicKey);

            //Key和IV来加密源数据
            result.EncryptedSourceData = EncryptUtility.EncryptByDES(srcDataToEncrypted, Encoding.Unicode, keyIv.KeyBytes, keyIv.IVBytes);

            return result;
        }

       

        /// <summary>
        /// 采用RAS和DES一起进行解密操作，私钥解析出来Key/IV，然后用Key/Iv解密业务数据
        /// </summary>
        /// <param name="xmlPrivateKey"></param>
        /// <param name="encryptResult"></param>
        /// <returns></returns>
        public static RSADESDecryptResult DecryptByRSADES(string xmlPrivateKey, RSADESEncryptResult encryptResult)
        {
            string[] encryptedKeyIv = EncryptUtility.DecryptByRSA(encryptResult.EncryptedDesKeyIV, Encoding.Unicode, xmlPrivateKey).Split('.');
            var result = new RSADESDecryptResult();
            result.DecryptedDesKey = encryptedKeyIv[0];
            result.DecryptedDesIv = encryptedKeyIv[1];
            var keyIv = new DesKeyIVEntity() { KeyString = result.DecryptedDesKey, IVString = result.DecryptedDesIv };
            result.DecryptedSourceData = EncryptUtility.DecryptByDES(encryptResult.EncryptedSourceData, Encoding.Unicode, keyIv.KeyBytes, keyIv.IVBytes);
            return result;
        }

       

        #endregion

    }

    /// <summary>
    /// DES加密解密所需要的KEY和IV实体
    /// </summary>
    [Serializable]
    public class DesKeyIVEntity
    {        
        private string fKeyString = string.Empty;        
        private string fIVString = string.Empty;

        [XmlIgnore]
        private byte[] fKeyBytes = null;
        [XmlIgnore]
        private byte[] fIVBytes = null;

        [XmlElement]
        public string KeyString
        {
            get
            {
                return fKeyString;
            }
            set
            {
                fKeyString = value;
                if (!string.IsNullOrEmpty(fKeyString))
                {
                    fKeyBytes = EncodingUtility.ToBytesFromBase64(fKeyString);
                }
            }
        }

        [XmlElement]
        public string IVString
        {
            get
            {
                return fIVString;
            }
            set
            {
                fIVString = value;
                if (!string.IsNullOrEmpty(fIVString))
                {
                    fIVBytes = EncodingUtility.ToBytesFromBase64(fIVString);
                }

            }
        }

        [XmlIgnore]
        public byte[] KeyBytes
        {
            get
            {
                return fKeyBytes;
            }
            set
            {
                fKeyBytes = value;
                if (fKeyBytes != null)
                {
                    fKeyString = EncodingUtility.ToBase64FromBytes(fKeyBytes);
                }
            }
        }

        [XmlIgnore]
        public byte[] IVBytes
        {
            get
            {
                return fIVBytes;
            }
            set
            {
                fIVBytes = value;
                if (fIVBytes != null)
                {
                    fIVString = EncodingUtility.ToBase64FromBytes(fIVBytes);
                }
            }
        }
    }

    /// <summary>
    /// DES加密解密所需要的KEY和IV实体
    /// </summary>
    [Serializable]
    public class TripleKeyIVEntity
    {
        private string fKeyString = string.Empty;
        private string fIVString = string.Empty;


        private byte[] fKeyBytes = null;
        private byte[] fIVBytes = null;

        [XmlElement]
        public string KeyString
        {
            get
            {
                return fKeyString;
            }
            set
            {
                fKeyString = value;
                if (!string.IsNullOrEmpty(fKeyString))
                {
                    fKeyBytes = EncodingUtility.ToBytesFromBase64(fKeyString);
                }
            }
        }

        [XmlElement]
        public string IVString
        {
            get
            {
                return fIVString;
            }
            set
            {
                fIVString = value;
                if (!string.IsNullOrEmpty(fIVString))
                {
                    fIVBytes = EncodingUtility.ToBytesFromBase64(fIVString);
                }

            }
        }

        [XmlIgnore]
        public byte[] KeyBytes
        {
            get
            {
                return fKeyBytes;
            }
            set
            {
                fKeyBytes = value;
                if (fKeyBytes != null)
                {
                    fKeyString = EncodingUtility.ToBase64FromBytes(fKeyBytes);
                }
            }
        }

        [XmlIgnore]
        public byte[] IVBytes
        {
            get
            {
                return fIVBytes;
            }
            set
            {
                fIVBytes = value;
                if (fIVBytes != null)
                {
                    fIVString = EncodingUtility.ToBase64FromBytes(fIVBytes);
                }
            }
        }
    }


    /// <summary>
    /// RSA+DES整合的方式加密后的数据
    /// </summary>
    [Serializable]
    public class RSADESEncryptResult
    {
        /// <summary>
        /// 加密后的Des Key/IV
        /// </summary>
        [System.Xml.Serialization.XmlElement]
        public string EncryptedDesKeyIV { get; set; }

        /// <summary>
        /// 加密后的业务数据
        /// </summary>
        [System.Xml.Serialization.XmlElement]
        public string EncryptedSourceData { get; set; }
    }

    /// <summary>
    /// RSA+DES整合的方式解密后的数据
    /// </summary>
    [Serializable]
    public class RSADESDecryptResult
    {
        /// <summary>
        /// 解密后的Des Key
        /// </summary>
        [System.Xml.Serialization.XmlElement]
        public string DecryptedDesKey { get; set; }

        /// <summary>
        /// 解密后的Des Iv
        /// </summary>
        [System.Xml.Serialization.XmlElement]
        public string DecryptedDesIv { get; set; }

        /// <summary>
        /// 解密后的明文数据
        /// </summary>
        [System.Xml.Serialization.XmlElement]
        public string DecryptedSourceData { get; set; }
    }
}
