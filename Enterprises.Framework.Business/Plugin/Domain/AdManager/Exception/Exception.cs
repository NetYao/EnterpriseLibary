using System;

namespace Enterprises.Framework.Plugin.Domain.AdManager.Exception
{
    [Serializable]
    public class ADException : ApplicationException
    {
        public ADException()
            : base()
        { }
        public ADException(string message)
            : base(message)
        { }
        public ADException(string message, System.Exception innerException)
            : base(message, innerException)
        { }
    }

    /// <summary>
    /// DirectoryEntry不存在
    /// </summary>
    [Serializable]
    public class EntryNotExistException : ADException
    {
        public EntryNotExistException()
            : base()
        { }
        public EntryNotExistException(string message)
            : base(message)
        { }
        public EntryNotExistException(string message, System.Exception innerException)
            : base(message, innerException)
        { }
    }

    /// <summary>
    /// 相同名称的DirectoryEntry已存在
    /// </summary>
    [Serializable]
    public class SameRDNException : ADException
    {
        public SameRDNException()
            : base()
        { }
        public SameRDNException(string message)
            : base(message)
        { }
        public SameRDNException(string message, System.Exception innerException)
            : base(message, innerException)
        { }
    }

    /// <summary>
    /// 不是容器
    /// </summary>
    [Serializable]
    public class NotContainerException : ADException
    {
        public NotContainerException()
            : base()
        { }
        public NotContainerException(string message)
            : base(message)
        { }
        public NotContainerException(string message, System.Exception innerException)
            : base(message, innerException)
        { }
    }

    /// <summary>
    /// 类型错误
    /// </summary>
    [Serializable]
    public class SchemaClassException : ADException
    {
        public SchemaClassException()
            : base()
        { }
        public SchemaClassException(string message)
            : base(message)
        { }
        public SchemaClassException(string message, System.Exception innerException)
            : base(message, innerException)
        { }
    }

    /// <summary>
    /// 存在子对象
    /// </summary>
    [Serializable]
    public class ExistChildException : ADException
    {
        public ExistChildException()
            : base()
        { }
        public ExistChildException(string message)
            : base(message)
        { }
        public ExistChildException(string message, System.Exception innerException)
            : base(message, innerException)
        { }
    }
}
