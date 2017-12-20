namespace Enterprises.Framework.Utility
{
    using System;

    /// <summary>
    /// 文件夹操作
    /// </summary>
    public interface IStorageFolder
    {
        /// <summary>
        /// 获取最后更改时间
        /// </summary>
        /// <returns></returns>
        DateTime GetLastUpdated();
        /// <summary>
        /// 获取文件夹名称
        /// </summary>
        /// <returns></returns>
        string GetName();
        /// <summary>
        /// 获取父级文件夹
        /// </summary>
        /// <returns></returns>
        IStorageFolder GetParent();
        /// <summary>
        /// 获取路径
        /// </summary>
        /// <returns></returns>
        string GetPath();
        /// <summary>
        /// 文件夹大小
        /// </summary>
        /// <returns></returns>
        long GetSize();
    }
}

