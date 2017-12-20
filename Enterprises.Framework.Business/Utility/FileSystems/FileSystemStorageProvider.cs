namespace Enterprises.Framework.Utility
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// 存储文件操作类
    /// </summary>
    public class FileSystemStorageProvider : IStorageProvider
    {
        /// <summary>
        /// 合并文件路径
        /// </summary>
        /// <param name="path1"></param>
        /// <param name="path2"></param>
        /// <returns></returns>
        public string Combine(string path1, string path2)
        {
            return Path.Combine(path1, path2);
        }

        /// <summary>
        /// 创建文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public IStorageFile CreateFile(string path)
        {
            
                var fileInfo = new FileInfo(path);
                if (fileInfo.Exists)
                {
                    throw new ArgumentException(string.Format("文件 {0} 已经存在", fileInfo.Name));
                }

                string directoryName = Path.GetDirectoryName(fileInfo.FullName);
                if (directoryName != null && !Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }

                File.WriteAllBytes(fileInfo.FullName, new byte[0]);
                return new FileSystemStorageFile(Fix(path), fileInfo);
           
        }


        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="path"></param>
        /// <exception cref="ArgumentException"></exception>
        public void CreateFolder(string path)
        {
            var info = new DirectoryInfo(path);
            if (info.Exists)
            {
                throw new ArgumentException(string.Format("文件夹 {0} 已经存在", path ));
            }

            Directory.CreateDirectory(info.FullName);
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="path"></param>
        public void DeleteFile(string path)
        {
            var info = new FileInfo(path);
            if (!info.Exists)
            {
                throw new ArgumentException(string.Format("文件 {0} 已经不存在", path));
            }
            info.Delete();
        }

        /// <summary>
        /// 删除文件夹
        /// </summary>
        /// <param name="path"></param>
        public void DeleteFolder(string path)
        {
            var info = new DirectoryInfo(path);
            if (!info.Exists)
            {
                throw new ArgumentException(string.Format("文件夹 {0} 已经不存在", path));
            }

            info.Delete(true);
        }

        /// <summary>
        /// 文件是否存在
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        private static string Fix(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return "";
            }
            if (Path.DirectorySeparatorChar == '/')
            {
                return path;
            }
            return path.Replace('/', Path.DirectorySeparatorChar);
        }

        /// <summary>
        /// 文件夹是否存在
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool FolderExists(string path)
        {
            return new DirectoryInfo(path).Exists;
        }

        public IStorageFile GetFile(string path)
        {
            var fileInfo = new FileInfo(path);
            if (!fileInfo.Exists)
            {
                throw new ArgumentException(string.Format("File {0} does not exist",path));
            }
            return new FileSystemStorageFile(Fix(path), fileInfo);
        }


        private static bool IsHidden(FileSystemInfo di)
        {
            return ((di.Attributes & FileAttributes.Hidden) != 0);
        }

        public IEnumerable<IStorageFile> ListFiles(string path)
        {
            var info = new DirectoryInfo(path);
            if (!info.Exists)
            {
                throw new ArgumentException(string.Format("Directory {0} does not exist", path ));
            }

            return info.GetFiles().Where(fi => !IsHidden(fi)).Select<FileInfo, IStorageFile>(fi => new FileSystemStorageFile(Path.Combine(Fix(path), fi.Name), fi)).ToList<IStorageFile>();
        }

        public IEnumerable<IStorageFolder> ListFolders(string path)
        {
            var info = new DirectoryInfo(path);
            if (!info.Exists)
            {
                try
                {
                    info.Create();
                }
                catch (Exception exception)
                {
                    throw new ArgumentException(string.Format("The folder could not be created at path: {0}. {1}",  path, exception ));
                }
            }
            return info.GetDirectories().Where(di => !IsHidden(di)).Select<DirectoryInfo, IStorageFolder>(di => new FileSystemStorageFolder(Path.Combine(Fix(path), di.Name), di)).ToList<IStorageFolder>();
        }

      

        /// <summary>
        /// 验证第二个路径是否是第一个路径的子级
        /// </summary>
        /// <param name="basePath"></param>
        /// <param name="mappedPath"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static string ValidatePath(string basePath, string mappedPath)
        {
            bool flag;
            try
            {
                flag = Path.GetFullPath(mappedPath).StartsWith(Path.GetFullPath(basePath), StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                flag = false;
            }
            if (!flag)
            {
                throw new ArgumentException("无效路径");
            }

            return mappedPath;
        }

        public void RenameFile(string oldPath, string newPath)
        {
            var info = new FileInfo(oldPath);
            if (!info.Exists)
            {
                throw new ArgumentException(string.Format("File {0} does not exist", oldPath));
            }
            var info2 = new FileInfo(newPath);
            if (info2.Exists)
            {
                throw new ArgumentException(string.Format("File {0} already exists", oldPath));
            }

            File.Move(info.FullName, info2.FullName);
        }

        public void RenameFolder(string oldPath, string newPath)
        {
            var info = new DirectoryInfo(oldPath);
            if (!info.Exists)
            {
                throw new ArgumentException(string.Format("Directory {0} does not exist", oldPath));
            }
            var info2 = new DirectoryInfo(newPath);
            if (info2.Exists)
            {
                throw new ArgumentException(string.Format("Directory {0} already exists", newPath));
            }
            Directory.Move(info.FullName, info2.FullName);
        }

        public void SaveStream(string path, Stream inputStream)
        {
            Stream stream = this.CreateFile(path).OpenWrite();
            byte[] buffer = new byte[0x2000];
            while (true)
            {
                int count = inputStream.Read(buffer, 0, buffer.Length);
                if (count <= 0)
                {
                    break;
                }
                stream.Write(buffer, 0, count);
            }
            stream.Dispose();
        }

        public bool TryCreateFolder(string path)
        {
            try
            {
                var info = new DirectoryInfo(path);
                if (info.Exists)
                {
                    return false;
                }
                CreateFolder(path);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool TrySaveStream(string path, Stream inputStream)
        {
            try
            {
                SaveStream(path, inputStream);
            }
            catch
            {
                return false;
            }
            return true;
        }

       

        private class FileSystemStorageFile : IStorageFile
        {
            private readonly FileInfo _fileInfo;
            private readonly string _path;

            public FileSystemStorageFile(string path, FileInfo fileInfo)
            {
                _path = path;
                _fileInfo = fileInfo;
            }

            public Stream CreateFile()
            {
                return new FileStream(_fileInfo.FullName, FileMode.Truncate, FileAccess.ReadWrite);
            }

            public string GetFileType()
            {
                return _fileInfo.Extension;
            }

            public DateTime GetLastUpdated()
            {
                return _fileInfo.LastWriteTime;
            }

            public string GetName()
            {
                return _fileInfo.Name;
            }

            public string GetPath()
            {
                return _path;
            }

            public long GetSize()
            {
                return _fileInfo.Length;
            }

            public Stream OpenRead()
            {
                return new FileStream(_fileInfo.FullName, FileMode.Open, FileAccess.Read);
            }

            public Stream OpenWrite()
            {
                return new FileStream(_fileInfo.FullName, FileMode.Open, FileAccess.ReadWrite);
            }
        }

        private class FileSystemStorageFolder : IStorageFolder
        {
            private readonly DirectoryInfo _directoryInfo;
            private readonly string _path;
           

            public FileSystemStorageFolder(string path, DirectoryInfo directoryInfo)
            {
                _path = path;
                _directoryInfo = directoryInfo;
               
            }

            private static long GetDirectorySize(DirectoryInfo directoryInfo)
            {
                long num = 0L;
                foreach (FileInfo info in directoryInfo.GetFiles())
                {
                    if (!FileSystemStorageProvider.IsHidden(info))
                    {
                        num += info.Length;
                    }
                }
                foreach (DirectoryInfo info2 in directoryInfo.GetDirectories())
                {
                    if (!FileSystemStorageProvider.IsHidden(info2))
                    {
                        num += GetDirectorySize(info2);
                    }
                }
                return num;
            }

            public DateTime GetLastUpdated()
            {
                return _directoryInfo.LastWriteTime;
            }

            public string GetName()
            {
                return _directoryInfo.Name;
            }

            public IStorageFolder GetParent()
            {
                if (_directoryInfo.Parent == null)
                {
                    throw new ArgumentException(string.Format("Directory {0} does not have a parent directory", _directoryInfo.Name));
                }
                return new FileSystemStorageProvider.FileSystemStorageFolder(Path.GetDirectoryName(this._path), this._directoryInfo.Parent);
            }

            public string GetPath()
            {
                return _path;
            }

            public long GetSize()
            {
                return GetDirectorySize(_directoryInfo);
            }

           
        }
    }
}

