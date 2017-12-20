using System;
using System.Globalization;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;


namespace Enterprises.Framework.Utility
{
	/// <summary>
	/// 处理文件业务
	/// </summary>
    public class FileUtility
	{
        private static string _fCurrentPath = string.Empty;
        
		/// <summary>
		/// [获取]系统日志文件（包括路径）
		/// </summary>
		private static string LogPathFile 
		{
			get
			{
				return "c:/debug.txt";
			}	
		}

	    /// <summary>
	    /// 输出调试信息
	    /// </summary>
	    /// <param name="result"></param>		
	    public static void Write2File(object result)
		{
			if(result == null)
				result = "null";
			StreamWriter sw = null;
			try
			{				
				sw = new StreamWriter(LogPathFile,true);
				sw.WriteLine(DateTime.Now.ToString(CultureInfo.InvariantCulture) + " :  " + result);
			}			
			finally
			{
				if(sw != null)
				{
					sw.Flush();
					sw.Close();
				}
			}
		}


        /// <summary>
        /// 创建文件,并且覆盖写入新的内容,如果没有文件，则创建相应的文件，以UTF8方式创建
        /// </summary>
        /// <param name="fileName"></param>
        public static void CreateFile(string fileName)
        {
            CreateFile(fileName, string.Empty);
        }

        /// <summary>
        /// 创建文件,并且写入新的内容,如果没有文件，则创建相应的文件，以UTF8方式创建
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="text"></param>
        /// <param name="append"></param>
        public static void CreateFile(string fileName,string text,bool append)
        {
            CreateFile(fileName, text, Encoding.UTF8, append);
        }

		/// <summary>
		/// 创建文件,并且覆盖写入新的内容，如果文件没有权限，则对文件进行user用户的完全控制权限
        /// ,如果没有文件，则创建相应的文件，以UTF8方式创建
		/// </summary>
		/// <param name="fileName">文件名，包括绝对路径</param>
		/// <param name="text">需要写入的内容</param>
		public static void CreateFile(string fileName,string text)
		{
            CreateFile(fileName, text, Encoding.UTF8, false);
		}	
		
		/// <summary>
		/// 创建指定的目录,如果有则不需要创建.
		/// </summary>
		/// <param name="path"></param>
		public static string CreatePath(string path)
		{
			if(!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			return path;
		}
		/// <summary>
		/// 追加文件内容，如果文件不存在，则先创建新的文件并追加内容
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="text"></param>
		public static void AppendFile(string fileName,string text)
		{
			if(File.Exists(fileName))
			{
                StreamWriter sw = null;
                try
                {
                    sw = new StreamWriter(fileName, true);
                    sw.WriteLine(text);
                }
                finally
                {
                    if (sw != null)
                    {
                        sw.Flush();
                        sw.Close();
                    }
                }
			}
			else
			{
				CreateFile(fileName,text);
			}
		}
      
        /// <summary>
        /// 获取格式化后的文件名，把异常字符替换为下划线
        /// </summary>
        /// <param name="rawFileName"></param>
        /// <returns></returns>
        public static string FormatFileName(string rawFileName)
        {
            var reg = new Regex("[/\\:* ]+");
            string newFileName = reg.Replace(rawFileName, "_");
            return newFileName;
        }

		/// <summary>
		/// 删除指定的文件
		/// </summary>
		/// <param name="fileName"></param>
		public static void DeleteFile(string fileName)
		{
            if (ExistsFile(fileName))
            {
                File.Delete(fileName);
            }
		}

        /// <summary>
        /// 删除指定目录中的所有文件，包括所有子目录
        /// </summary>
        /// <param name="filesPath"></param>
        public static void DeleteFiles(string filesPath)
        {
            if (!ExistsPath(filesPath))
                return;

            string[] files = Directory.GetFiles(filesPath);
            foreach (string file in files)
            {
                SetReadOnly(file, false);
                File.Delete(file);
            }
        }

        /// <summary>
        /// 删除指定的路径,如果不存在，则返回
        /// </summary>
        /// <param name="path"></param>
        public static void DeletePath(string path)
        {
            try
            {
                if (ExistsPath(path))
                {
                    DeleteFiles(path);
                    Directory.Delete(path, true);
                }
            }
            catch 
            {
            }
        }


	    /// <summary>
	    /// 格式化文件路径地址，以\结尾
	    /// </summary>
	    /// <param name="filePath"></param>
	    /// <returns></returns>
	    public static string FormatPath(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return string.Empty;
            
            filePath = filePath.Trim();
            filePath = filePath.Replace("/", "\\");

            if (!filePath.EndsWith("\\"))
            {
                return filePath + "\\";
            }
            return filePath;
        }

	    /// <summary>
	    /// 对文件的路径进行格式化，确保其中只有\
	    /// </summary>
	    /// <param name="fullFileName"></param>
	    /// <returns></returns>
	    public static string FormatFile(string fullFileName)
        {
            return FormatPath(GetFilePath(fullFileName)) + GetFileName(fullFileName);
        }

		/// <summary>
		/// 根据提供的包括路径的文件名，返回其对应的文件目录，以\结尾
		/// </summary>
		/// <param name="fullFileName"></param>
		/// <returns></returns>
		public static string GetFilePath(string fullFileName)
		{
			string path = Path.GetDirectoryName(fullFileName);
            return FormatPath(path);
		}


		/// <summary>
        /// 根据全路径文件名，只返回文件名，包括扩展名， 但不包括路径。
		/// </summary>
		/// <param name="fullFileName"></param>
		/// <returns></returns>
		public static string GetFileName(string fullFileName)
		{
            var fi = new FileInfo(fullFileName);
            return fi.Name;
		}

        /// <summary>
        /// 根据全路径文件名，只返回文件名，不包括路径和扩展名
        /// </summary>
        /// <param name="fullFileName"></param>
        /// <returns></returns>
        public static string GetOnlyFileName(string fullFileName)
        {
            string fileName = string.Empty;
            if (fileName.IndexOf(".", StringComparison.Ordinal) == -1)
            {
                fileName = fullFileName;
            }
            else
            {
                fileName = GetFileName(fullFileName);
            }

            if (fileName.IndexOf("\\", StringComparison.Ordinal) != -1)
            {
                fileName = fileName.Remove(0, fileName.LastIndexOf("\\", StringComparison.Ordinal) + 1);
            }

            if (fileName.IndexOf("/", StringComparison.Ordinal) != -1)
            {
                fileName = fileName.Remove(0, fileName.LastIndexOf("/", StringComparison.Ordinal) + 1);
            }
            if (fileName.IndexOf(".", StringComparison.Ordinal) != -1)
            {
                fileName = fileName.Substring(0, fileName.LastIndexOf('.'));
            }

            return fileName;
        }	
		
		


		/// <summary>
		/// 判断是否存在指定的文件
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public static bool ExistsFile(string fileName)
		{
            return File.Exists(fileName);
		}

		/// <summary>
		/// 判断是否存在指定的目录
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static bool ExistsPath(string path)
		{
			return Directory.Exists(path);
		}


		/// <summary>
		/// 把源文件夹下的文件全部复制到目标文件夹
		/// </summary>
		/// <param name="fromPath"></param>
		/// <param name="toPath"></param>
		public static void CopyFiles(string fromPath,string toPath)
		{
            if (string.IsNullOrEmpty(fromPath))
            {
                throw new Exception("fromPath不能为空");
            }
            if (string.IsNullOrEmpty(toPath))
            {
                throw new Exception("toPath不能为空");
            }

            fromPath = FormatFile(fromPath.Trim());            
            toPath = FormatFile(toPath.Trim());
            CreatePath(toPath);
            var diSrc = new DirectoryInfo(fromPath);

            //Copy Directories
            DirectoryInfo[] diSubs = diSrc.GetDirectories("*.*", SearchOption.AllDirectories);
            foreach (DirectoryInfo diSub in diSubs)
            {
                string newPath = toPath + diSub.FullName.Remove(0, fromPath.Length);
                CreatePath(newPath);
            }

            //Copy Files
            FileInfo[] srcFiles = diSrc.GetFiles("*.*",SearchOption.AllDirectories);
            foreach(FileInfo fiSrc in srcFiles)
            {
                string newFilePath = toPath + fiSrc.FullName.Remove(0, fromPath.Length);
                var fiNew = new FileInfo(newFilePath);
                if (!fiNew.Directory.Exists)
                {
                    fiNew.Directory.Create();
                }

                fiSrc.CopyTo(newFilePath, true);
            }
		}
		

		/// <summary>
		/// 判断两个流是否相等
		/// </summary>
		/// <param name="s1"></param>
		/// <param name="s2"></param>
		/// <returns></returns>
		public static bool CheckEqual(Stream s1,Stream s2)
		{			
			if(s1 == null || s2 == null || s1.Length != s2.Length)
				return false;

			byte[] b1 = new byte[s1.Length];
			s1.Read(b1,0,(int)s1.Length);
			
			byte[] b2 = new byte[s2.Length];
			s1.Read(b2,0,(int)s2.Length);
			
			for(int i = 0; i < b1.Length; i++)
			{
				if(b1[i] != b2[i])
					return false;
			}

			return true;			
		}


		/// <summary>
		/// 获取流对应的字符串
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static string GetStreamValue(Stream s)
		{
            if (s.CanSeek)
            {
                s.Position = 0;
            }

			var b = new byte[s.Length];
			s.Read(b,0,(int)s.Length);
            if (s.CanSeek)
            {
                s.Position = 0;
            }

            return Encoding.ASCII.GetString(b);			
		}


		/// <summary>
		/// 复制文件到指定的文件夹,文件名相同，返回拷贝后的文件名（包括全路径）
		/// </summary>
		/// <param name="srcFile">源文件路径</param>
		/// <param name="dstPath">复制目标路径</param>
		public static string CopyFile(string srcFile,string dstPath)
		{
            if (!File.Exists(srcFile))
            {
                throw new FileNotFoundException(srcFile);
            }

            srcFile = FormatFile(srcFile);
            dstPath = FormatPath(dstPath);
			string fileName = GetFileName(srcFile);
            CreatePath(dstPath);
            fileName = Path.Combine(dstPath,fileName);
			File.Copy(srcFile,fileName,true);            
            return fileName;
		}

        /// <summary>
        /// 复制文件到指定的文件夹
        /// </summary>
        /// <param name="srcFile">源文件路径</param>
        /// <param name="dstPath">复制目标路径</param>
        /// <param name="dstFileName">新的文件名</param>
        /// <param name="overWrite">存在同名文件是否覆盖</param>
        /// <returns></returns>
        public static string CopyFile(string srcFile, string dstPath, string dstFileName, bool overWrite)
        {
            string dstFullName = Path.Combine(dstPath, dstFileName);
            CreatePath(dstPath);
            File.Copy(srcFile, dstFullName, overWrite);
            return dstFullName;
		}

	    /// <summary>
	    /// 把文件拷贝到目标文件，覆盖目标文件
	    /// </summary>
	    /// <param name="srcFile"></param>
	    /// <param name="dstFile"></param>
	    /// <param name="overWrite"></param>
	    public static void CopyToFile(string srcFile, string dstFile, bool overWrite)
        {
            CopyFile(srcFile, GetFilePath(dstFile), GetFileName(dstFile),overWrite);
        }

	    /// <summary>
	    /// 通过加后缀的方式复制文件，如原文件为：C:\X.doc，加上后缀1变为：C:\X1.doc。
	    /// 返回新文件的全路径
	    /// </summary>
	    /// <param name="srcFile"></param>
	    /// <param name="newFilePostfix"></param>
	    /// <param name="overWrite"></param>
	    public static string CopyToFileWithPostfix(string srcFile, string newFilePostfix,bool overWrite)
        {
            string newFile = string.Format("{0}_{1}{2}",
                srcFile.Substring(0, srcFile.LastIndexOf(".", StringComparison.Ordinal)),
                newFilePostfix,
                srcFile.Remove(0, srcFile.LastIndexOf(".", StringComparison.Ordinal)));
            if (overWrite)
            {
                CopyToFile(srcFile, newFile, true);
            }

	        return newFile;
        }
		
        /// <summary>
        /// 读取文件中的内容，以UTF8方式读取
		/// </summary>
		/// <param name="fullFileName"></param>
		public static string ReadFile(string fullFileName)
		{
            using (var sr = new StreamReader(fullFileName, Encoding.UTF8))
            {
                return sr.ReadToEnd();
            }
		}

        /// <summary>
        /// 读取文件中的内容，以UTF8方式读取
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string ReadFile(FileInfo file)
        {
            return ReadFile(file.FullName);
        }

        /// <summary>
        /// 读取文件中的内容，可以指定编码格式
        /// </summary>
        /// <param name="fullFileName"></param>
        /// <param name="fileEncoding"></param>
        /// <returns></returns>
        public static string ReadFile(string fullFileName,Encoding fileEncoding)
        {
            using (var sr = new StreamReader(fullFileName, fileEncoding))
            {
                return sr.ReadToEnd();
            }
        }

        /// <summary>
        /// Unicode读取文件中的内容
        /// </summary>
        /// <param name="fullFileName"></param>
        /// <returns></returns>
        public static string ReadUnicodeFile(string fullFileName)
        {
            using (var sr = new StreamReader(fullFileName, Encoding.Unicode))
            {
                return sr.ReadToEnd();
            }
        }

        /// <summary>
        /// GB2312读取文件中的内容
        /// </summary>
        /// <param name="fullFileName"></param>
        /// <returns></returns>
        public static string ReadGb2312File(string fullFileName)
        {
            using (var sr = new StreamReader(fullFileName, Encoding.GetEncoding("gb2312")))
            {
                return sr.ReadToEnd();
            }
        }


	    /// <summary>
	    /// 创建指定格式的文件，并改写原来的内容
	    /// </summary>
	    /// <param name="fileName"></param>
	    /// <param name="text"></param>
	    /// <param name="encoding"></param>
	    public static void CreateFile(string fileName,string text,Encoding encoding)
		{
            CreateFile(fileName, text, encoding, false);
		}

        /// <summary>
        /// 创建指定格式的文件，并改写原来的内容
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="text"></param>
        /// <param name="encoding"></param>
        /// <param name="append">是否追加修改</param>
        public static void CreateFile(string fileName, string text, Encoding encoding,bool append)
        {
            StreamWriter sw = null;
            try
            {
                string path = GetFilePath(fileName);
                if (path.Trim() != string.Empty && !Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                sw = new StreamWriter(fileName, append, encoding);
                sw.WriteLine(text);
            }
            finally
            {
                if (sw != null)
                {
                    sw.Flush();
                    sw.Close();
                }
            }
        }		
       
		/// <summary>
		/// 获取指定目录中所有的文件的版本信息对于 Excel，不要考虑版本了
		/// </summary>
		/// <param name="filePath"></param>
		/// <returns></returns>
		public static Hashtable GetFilesVersion(string filePath)
		{
			CreatePath(filePath);
			string[] files = Directory.GetFiles(filePath);
			var ht = new Hashtable();
			foreach(string f in files)
			{
				string fileName = GetFileName(f);
				FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(f);
				string fileVersion = fvi.FileVersion;
				ht.Add(fileName,fileVersion);				
			}

			return ht;
		}
        
		/// <summary>
		/// 获取文件的扩展名,包含分隔符.(eg .xls)
		/// </summary>
		/// <param name="fullFileName"></param>
		/// <returns></returns>
		public static string GetExtension(string fullFileName)
		{
            if (fullFileName.IndexOf(".", StringComparison.Ordinal) == -1)
            {
                return string.Empty;
            }
			return Path.GetExtension(fullFileName);
		}

        /// <summary>
        /// 获取指定路径下的所有文件列表
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string[] GetFiles(string filePath)
        {
            CreatePath(filePath);
            return Directory.GetFiles(filePath);
        }

        /// <summary>
        /// 获取指定路径下的所有文件列表
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="filter">过滤文件</param>
        /// <param name="includeSubDirectories">是否包含子文件夹</param>
        /// <returns></returns>
        public static string[] GetFiles(string filePath, string filter, bool includeSubDirectories)
        {
            CreatePath(filePath);
            if (includeSubDirectories)
            {
                return Directory.GetFiles(filePath, filter, SearchOption.AllDirectories);
            }

            return Directory.GetFiles(filePath,filter);
        }

        /// <summary>
        /// 获取指定路径下的所有文件列表
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="includeSubDirectories"></param>
        /// <returns></returns>
        public static string[] GetFiles(string filePath,bool includeSubDirectories)
        {
            return GetFiles(filePath, "*.*", includeSubDirectories);
        }

        /// <summary>
        /// 设置路径是否隐藏
        /// </summary>
        /// <param name="path"></param>
        /// <param name="hidden"></param>
        public static void SetPathHidden(string path, bool hidden)
        {
            if (!ExistsPath(path))
            {
                CreatePath(path);
            }

            var di = new DirectoryInfo(path);
            FileAttributes fab = di.Attributes;
            if (hidden)
            {
                fab = fab | FileAttributes.Hidden;
                File.SetAttributes(path, fab);
            }
            else
            {
                if((fab | FileAttributes.Hidden) != fab)
                {
                    fab = fab ^ FileAttributes.Hidden;
                    File.SetAttributes(path, fab);
                }
            }            
        }

        /// <summary>
        /// 设置文件是否隐藏
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="hidden"></param>
        public static void SetFileHidden(string fileName, bool hidden)
        {
            if (!ExistsFile(fileName))
            {
                return;
            }

            FileAttributes fab = File.GetAttributes(fileName);
            if (hidden)
            {
                fab = fab | FileAttributes.Hidden;
                File.SetAttributes(fileName, fab);
            }
            else
            {
                if ((fab | FileAttributes.Hidden) != fab)
                {
                    fab = fab ^ FileAttributes.Hidden;
                    File.SetAttributes(fileName, fab);
                }
            }
        }

        /// <summary>
        /// 设置文件的只读属性,readOnly来标记如何设置readOnly
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="readOnly"></param>
        public static void SetReadOnly(string fileName, bool readOnly)
        {
            if (!ExistsFile(fileName))
            {
                return;
            }

            FileAttributes fab = File.GetAttributes(fileName);
            if (readOnly)
            {
                //设置为只读
                if ((fab | FileAttributes.ReadOnly) != fab)
                {
                    //Set ReadOnly
                    fab = fab | FileAttributes.ReadOnly;
                    File.SetAttributes(fileName, fab);
                }
            }
            else
            {
                //设置为可写
                if ((fab | FileAttributes.ReadOnly) == fab)
                {
                    //Set Not ReadOnly
                    fab = fab ^ FileAttributes.ReadOnly;
                    File.SetAttributes(fileName, fab);
                }
            }
        }

        /// <summary>
        /// 获取指定文件的内容行数
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static int GetFileLineNumber(string fileName)
        {
            if (!ExistsFile(fileName))
                return 0;

            int lineNumber = 0;
            using (var sr = new StreamReader(fileName))
            {
                while (!sr.EndOfStream)
                {
                    sr.ReadLine();
                    lineNumber++;
                }
            }
            return lineNumber;
        }


        /// <summary>
        /// 比较两个路径,通过引用返回结果
        /// </summary>
        /// <param name="path1"></param>
        /// <param name="path2"></param>
        /// <param name="inPath1AndNotInPath2"></param>
        /// <param name="inPath2AndNotInPath1"></param>
        public void ComparePath(string path1, string path2,ref StringCollection inPath1AndNotInPath2,ref StringCollection inPath2AndNotInPath1)
        {            
            if (ExistsPath(path1) && ExistsPath(path2))
            {
                path1 = FormatPath(path1);
                path2 = FormatPath(path2);
                
                foreach (string f in GetFiles(path1, false))
                {
                    inPath1AndNotInPath2.Add(f.ToUpper());
                }

                foreach (string f in GetFiles(path2, false))
                {                    
                    inPath2AndNotInPath1.Add(f.ToUpper());
                }

                foreach (string f in inPath1AndNotInPath2)
                {
                    if (inPath2AndNotInPath1.Contains(f))
                    {
                        inPath2AndNotInPath1.Remove(f);
                    }
                }

                foreach (string f in inPath2AndNotInPath1)
                {
                    if (inPath1AndNotInPath2.Contains(f))
                    {
                        inPath1AndNotInPath2.Remove(f);
                    }
                }
            }            
        }

        /// <summary>
        /// 保存二进制数组到物理文件
        /// </summary>
        /// <param name="fileBytes"></param>
        /// <param name="fileName"></param>
        public static void SaveBitFile(Byte[] fileBytes, string fileName)
        {
            if (fileBytes == null)
            {
                throw new ArgumentNullException("fileBytes");
            }
            int actBytesLength = fileBytes.Length;
            SaveBitFile(fileBytes,actBytesLength, fileName, false);
        }

	    /// <summary>
	    /// 保存二进制数组到物理文件
	    /// </summary>
	    /// <param name="fileBytes"></param>
	    /// <param name="actBytesLength"></param>
	    /// <param name="fileName"></param>
	    /// <param name="append"></param>
	    public static void SaveBitFile(Byte[] fileBytes,int actBytesLength, string fileName,bool append)
        {
            if (fileBytes == null)
            {
                throw new ArgumentNullException("fileBytes");
            }
            string path = GetFilePath(fileName);
            CreatePath(path);
            if (!append)
            {
                DeleteFile(fileName);
            }
            FileStream fs = null;
            try
            {
                fs = append ? new FileStream(fileName, FileMode.Append, FileAccess.Write) : new FileStream(fileName, FileMode.Create, FileAccess.Write);
                fs.Position = fs.Length;
                int count = actBytesLength;

                fs.Write(fileBytes, 0, count);
                fs.Flush();
            }
            finally
            {
                if (fs != null)
                {
                    fs.Flush();
                    fs.Dispose();
                    fs.Close();
                }
            }
        }

        /// <summary>
        /// 获取文件对应的二进制数组，以方便以二进制格式发送文件（可以用于WebService），或者保存到数据库
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static Byte[] GetBitFile(string fileName)
        {
            FileStream fs = null;
            try
            {
                //打开文件
                fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                //文件长度
                var fileLength = (int)fs.Length;
                var results = new Byte[fileLength];
                //读入数组
                fs.Read(results, 0, fileLength);
                return results;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Flush();
                    fs.Dispose();
                    fs.Close();
                }
            }
        }

        /// <summary>
        /// 获取指定文件的大小(Byte为单位）
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static long GetFileSize(string fileName)
        {
            FileStream fs = null;
            try
            {
                //打开文件
                fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                //文件长度
                long fileLength = fs.Length;
                return fileLength;
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
        /// 根据计算单位返回文件大小
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="sizeType"></param>
        /// <returns></returns>
        public static string GetFileSize(string fileName,ByteSizeType sizeType)
        {
            long fileSize = GetFileSize(fileName);           
            
            double result = ((fileSize * 1.0) / ((int)sizeType));            
            switch (sizeType)
            {
                case ByteSizeType.KB:
                    if (result == 0)
                    {
                        result = 1;
                    }
                    return string.Format("{0:F0}{1}", result,sizeType);
                case ByteSizeType.MB:
                case ByteSizeType.GB:
                    return string.Format("{0:F2}{1}", result, sizeType);
            }
            return string.Empty;
        }

        /// <summary>
        /// 合并两个文件路径
        /// </summary>
        /// <param name="partPath1"></param>
        /// <param name="partPath2"></param>
        /// <returns></returns>
        public static string Combine(string partPath1, string partPath2)
        {
            return Path.Combine(partPath1, partPath2);
        }

        /// <summary>
        /// 获取文件写入的时间
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static DateTime GetLastWriteTime(string file)
        {
            var fi = new FileInfo(file);
            return fi.LastWriteTime;
        }

        /// <summary>
        /// 获取文件创建的时间
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static DateTime GetCreateTime(string file)
        {
            var fi = new FileInfo(file);
            return fi.CreationTime;
        }

        /// <summary>
        /// 设置文件写入的时间
        /// </summary>
        /// <param name="file"></param>
        /// <param name="dt"></param>
        public static void SetLastWriteTime(string file,DateTime dt)
        {
            new FileInfo(file) {LastWriteTime = dt};
        }

	    /// <summary>
        /// 设置文件创建的时间
        /// </summary>
        /// <param name="file"></param>
        /// <param name="dt"></param>
        public static void SetCreateTime(string file, DateTime dt)
	    {
	        new FileInfo(file) {CreationTime = dt};
	    }

	    /// <summary>
        /// 重命名文件
        /// </summary>
        /// <param name="rawFullFileName"></param>
        /// <param name="newFullFileName"></param>
        public static void ReNameFile(string rawFullFileName, string newFullFileName)
        {
            File.Move(rawFullFileName, newFullFileName);
        }

        /// <summary>
        /// 判断是否全路径（如果包含冒号，则认为是全路径）
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsUncPath(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            return path.IndexOf(":", StringComparison.Ordinal) != -1;
        }

      


        /// <summary>
        /// 两个目录同步
        /// </summary>
        /// <param name="sourcedir"></param>
        /// <param name="destdir"></param>
        /// <returns></returns>
        public SyncResult StartSync(string sourcedir, string destdir)
        {
            int createDirCount = 0; // 记录创建的数量
            int copyFileCount = 0; // 记录覆盖数量
            //传入目录名保护
            sourcedir = sourcedir.Trim();
            destdir = destdir.Trim();
            //保证目录最后一个字符不是斜杠
            if (sourcedir[sourcedir.Length - 1] == '\\')
                sourcedir = sourcedir.Remove(sourcedir.Length - 1);
            if (destdir[destdir.Length - 1] == '\\')
                destdir = destdir.Remove(destdir.Length - 1);
            //判断目录是否存在
            if (!Directory.Exists(sourcedir)) return SyncResult.SourceDirNotExists;
            if (!Directory.Exists(destdir)) return SyncResult.DestDirNotExists;

            //获取源、目的目录内的目录信息
            var sDirInfo = GetDirectories(sourcedir);
            var dDirInfo = GetDirectories(destdir);
            //
            //      开始同步两个目录，但只先同步源目录信息
            //------比较两目录中的子目录信息---------------------
            foreach (KeyValuePair<string, string> kvp in sDirInfo) //在查找有无源目录存在而在目标目录中不存在的目录
            {
                if (!dDirInfo.ContainsKey(kvp.Key)) //如果目标目录中不存在目录，则马上建立
                {
                    string dirname = destdir + "\\" + kvp.Key;
                    Directory.CreateDirectory(dirname);
                   

                    createDirCount++;
                }
                //递归调用目录同步函数，实现嵌套目录一次性全同步
                StartSync(sourcedir + "\\" + kvp.Key, destdir + "\\" + kvp.Key);
            }

            //取得源目录下所有文件的列表
            string[] sFiles = Directory.GetFiles(sourcedir);
            //string[] DFiles = Directory.GetFiles(destdir);
            //------比较两目录中的文件信息（本层目录）--------------
            foreach (string sfilename in sFiles)
            {
                string dfilename = destdir + "\\" + Path.GetFileName(sfilename);
                if (File.Exists(dfilename)) //如果目的目录中已经存在同名文件，则比较其最后修改时间，取最新的为准
                {
                    //取得源、目的目录中同名文件的文件信息
                    FileInfo sfi = new FileInfo(sfilename);
                    FileInfo dfi = new FileInfo(dfilename);
                    //如果源文件大于目的文件修改时间5秒以上才拷贝覆盖过去，主要是考虑到操作系统的一些差异，对于修改时间相同而文件大小不同的文件则暂不处理
                    if (sfi.LastWriteTime > dfi.LastWriteTime.AddSeconds(5))
                    {
                        //拷贝源目录下的同名文件到目的目录
                        File.Copy(sfilename, dfilename, true);
                      
                        copyFileCount++;
                    }
                }
                else //如果目的目录中不存在同名文件，则拷贝过去
                {
                    //拷贝源目录下的同名文件到目的目录
                    File.Copy(sfilename, dfilename, true);
                  
                    copyFileCount++;
                }
            }

            LogHelper.WriteLog($"创建{createDirCount}个，覆盖{copyFileCount}个");
            return SyncResult.Success;
        }

        public static Dictionary<string, string> GetDirectories(string dirname)
        {
            Dictionary<string, string> dirs = new Dictionary<string, string>();
            string[] strDirs = Directory.GetDirectories(dirname);
            foreach (string str in strDirs)
            {
                string[] result = str.Split('\\');
                dirs.Add(result[result.Length - 1], result[result.Length - 1]);
            }
            return dirs;
        }

    }

    /// <summary>
    /// 同步文件枚举
    /// </summary>
    public enum SyncResult
    {
        Success, SourceDirNotExists, DestDirNotExists
    }

    /// <summary>
    /// 文件大小
    /// </summary>
    public enum ByteSizeType
    {
        /// <summary>
        /// 
        /// </summary>
        KB = 1024,

        /// <summary>
        /// 
        /// </summary>
        MB = 1048576,

        /// <summary>
        /// 
        /// </summary>
        GB = 1073741824
    }
}

