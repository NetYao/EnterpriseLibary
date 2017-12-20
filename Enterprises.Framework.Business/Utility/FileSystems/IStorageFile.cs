namespace Enterprises.Framework.Utility
{
    using System;
    using System.IO;

    /// <summary>
    /// 文件操作
    /// </summary>
    public interface IStorageFile
    {
        /// <summary>
        ///  创建文件
        /// </summary>
        /// <returns></returns>
        Stream CreateFile();
        /// <summary>
        ///  获取文件类型
        /// </summary>
        /// <returns></returns>
        string GetFileType();
        /// <summary>
        /// 最后一次修改时间
        /// </summary>
        /// <returns></returns>
        DateTime GetLastUpdated();
        /// <summary>
        /// 获取文件名
        /// </summary>
        /// <returns></returns>
        string GetName();
        /// <summary>
        /// 获取文件路径
        /// </summary>
        /// <returns></returns>
        string GetPath();
        /// <summary>
        /// 获取文件大小（字节）
        /// </summary>
        /// <returns></returns>
        long GetSize();
        /// <summary>
        /// 读取文件
        /// </summary>
        /// <returns></returns>
        Stream OpenRead();
        /// <summary>
        /// 写入文件
        /// </summary>
        /// <returns></returns>
        Stream OpenWrite();
    }
}

