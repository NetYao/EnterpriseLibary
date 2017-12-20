namespace Enterprises.Framework.Utility
{
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    ///  存储文件操作
    /// </summary>
    public interface IStorageProvider
    {
        /// <summary>
        ///  合并两个路径
        /// </summary>
        /// <param name="path1"></param>
        /// <param name="path2"></param>
        /// <returns></returns>
        string Combine(string path1, string path2);
        /// <summary>
        /// 创建文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        IStorageFile CreateFile(string path);
        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="path"></param>
        void CreateFolder(string path);
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="path"></param>
        void DeleteFile(string path);
        /// <summary>
        /// 删除文件夹
        /// </summary>
        /// <param name="path"></param>
        void DeleteFolder(string path);
        /// <summary>
        /// 判断文件是否存在
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        bool FileExists(string path);
        /// <summary>
        /// 判断文件夹是否存在
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        bool FolderExists(string path);
        /// <summary>
        /// 获取文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        IStorageFile GetFile(string path);
        
        /// <summary>
        /// 获取文件夹下面的文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        IEnumerable<IStorageFile> ListFiles(string path);

        /// <summary>
        /// 获取文件夹下面的文件夹
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        IEnumerable<IStorageFolder> ListFolders(string path);
        /// <summary>
        ///  重命名文件
        /// </summary>
        /// <param name="oldPath"></param>
        /// <param name="newPath"></param>
        void RenameFile(string oldPath, string newPath);
        /// <summary>
        /// 重命名文件夹
        /// </summary>
        /// <param name="oldPath"></param>
        /// <param name="newPath"></param>
        void RenameFolder(string oldPath, string newPath);
        /// <summary>
        ///  写入文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="inputStream"></param>
        void SaveStream(string path, Stream inputStream);
        /// <summary>
        ///  试图创建文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        bool TryCreateFolder(string path);
        /// <summary>
        /// 试图写入文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="inputStream"></param>
        /// <returns></returns>
        bool TrySaveStream(string path, Stream inputStream);
    }
}

