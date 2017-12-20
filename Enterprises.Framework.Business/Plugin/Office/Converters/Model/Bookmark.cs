using System;
using System.Collections.Generic;
using DocumentConvert.Infrastructure.Model;

namespace Enterprises.Framework.Plugin.Office.Converters.Model
{
    /// <summary>
    /// 自定义书签对象
    /// </summary>
    public class Bookmark : ICloneable
    {
        private Microsoft.Office.Interop.Word.Bookmark _bookmark;
        private Document _document;
        private static readonly Dictionary<int, Bookmark> Cache;

        static Bookmark()
        {
            Cache = new Dictionary<int, Bookmark>
                {
                    {(int) RenderMode.Plain, new PlainBookmark()},
                    {(int) RenderMode.Image, new ImageBookmark()},
                    {(int) RenderMode.List, new ListBookmark()}
                };
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public Bookmark()
        {
        }

        protected Document Document
        {
            get
            {
                return _document;
            }
        }

        protected Microsoft.Office.Interop.Word.Bookmark WordBookmark
        {
            get
            {
                return _bookmark;
            }
        }

        public Microsoft.Office.Interop.Word.Range Range
        {
            get
            {
                return _bookmark.Range;
            }
        }

        /// <summary>
        /// 虚方法子类继承实现
        /// </summary>
        /// <param name="data"></param>
        public virtual void Instantiate(object data)
        {
 
        }

        /// <summary>
        /// 
        /// </summary>
        public void Select()
        {
            _bookmark.Select();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return CloneInternal();
        }

        protected virtual object CloneInternal()
        {
            throw new NotSupportedException("不支持直接克隆书签！");
        }

        /// <summary>
        /// 创建书签
        /// </summary>
        /// <param name="bookmark"></param>
        /// <param name="document"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public static Bookmark CreateBookmark(Microsoft.Office.Interop.Word.Bookmark bookmark, Document document, RenderMode mode)
        {
            Bookmark value;
            if (Cache.TryGetValue((int)mode, out value))
            {
                value = (Bookmark)value.Clone();
                value._bookmark = bookmark;
                value._document = document;
                return value;
            }
            throw new NotSupportedException("不支持该类型的书签！");
        }
    }
}
