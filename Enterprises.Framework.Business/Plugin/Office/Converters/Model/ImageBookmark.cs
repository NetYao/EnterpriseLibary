using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Enterprises.Framework.Plugin.Office.Converters.Model;
using Word = Microsoft.Office.Interop.Word;
using System.IO;

namespace DocumentConvert.Infrastructure.Model
{
    /// <summary>
    /// 图片书签
    /// </summary>
    public class ImageBookmark : Bookmark
    {
        /// <summary>
        /// 
        /// </summary>
        public ImageBookmark()
        {

        }

        /// <summary>
        /// 图片书签替换实例的实现
        /// </summary>
        /// <param name="data"></param>
        public override void Instantiate(object data)
        {
            string path = data.ToString();
            //issue: 修正当图片文件不存在时导致整个文档转换失败的BUG
            if (!File.Exists(path))
            {
                return;
            }
            Select();
            object objTrue = true, objFalse = false, objMissing = Type.Missing;
            Word.InlineShape shape = Document.Application.Selection.InlineShapes.AddPicture(path, ref objTrue, ref objTrue, ref objMissing);
            shape.ConvertToShape().WrapFormat.Type = Word.WdWrapType.wdWrapFront;
        }

        protected override object CloneInternal()
        {
            return new ImageBookmark();
        }
    }
}
