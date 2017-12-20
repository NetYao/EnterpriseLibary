using System;

namespace Enterprises.Framework.Plugin.Office.Converters
{
    /// <summary>
    /// 文档转换状态
    /// </summary>
    [Flags]
    public enum DocumentStatus
    {
        /// <summary>
        /// 无状态
        /// </summary>
        None = 0,
        /// <summary>
        /// 转换成功
        /// </summary>
        Processed = 1,
        /// <summary>
        /// 新文档
        /// </summary>
        New = 2,
        /// <summary>
        /// 修改
        /// </summary>
        Modified = 4,
        /// <summary>
        /// 失败
        /// </summary>
        Failed = 8
    }

    /// <summary>
    /// 转换文件类型
    /// </summary>
    [Flags]
    public enum OutputType
    {
        /// <summary>
        /// 无
        /// </summary>
        None =0,
        /// <summary>
        /// Word文件
        /// </summary>
        Word = 1,
        /// <summary>
        /// PDF文件
        /// </summary>
        PDF = 2
    }

    /// <summary>
    /// 书签模式
    /// </summary>
    public enum RenderMode
    {
        /// <summary>
        /// 文本书签
        /// </summary>
        Plain = 0,
        /// <summary>
        /// 图片书签
        /// </summary>
        Image = 1,
        /// <summary>
        /// 表格书签
        /// </summary>
        List = 2
    }
}
