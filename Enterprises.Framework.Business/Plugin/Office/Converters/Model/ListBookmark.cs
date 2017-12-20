using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Enterprises.Framework.Plugin.Office.Converters.Model;
using Word = Microsoft.Office.Interop.Word;
using System.Data;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DocumentConvert.Infrastructure.Model
{
    /// <summary>
    /// 表格书签
    /// </summary>
    public class ListBookmark : Bookmark
    {
        /// <summary>
        /// 
        /// </summary>
        public ListBookmark()
        {

        }

        /// <summary>
        /// 表格书签替换实现
        /// </summary>
        /// <param name="data"></param>
        public override void Instantiate(object data)
        {
            var dataTable = data as DataTable;
            if (dataTable == null)
            {
                return;
            }
            Word.Table table = Document.GetClosestTable(WordBookmark);
            if (table.Rows.Count < 2)
            {
                return;
            }

            Word.Row templateRow = table.Rows[2];
            object beforeRow = templateRow;
            if (templateRow.Range.Bookmarks.Count == 0)
            {
                int columnCount = templateRow.Cells.Count + 1, length = dataTable.Rows.Count;
                for (int index = length - 1; index >= 0; index--)
                {
                    
                    Word.Row newRow = table.Rows.Add(beforeRow);
                    for (int i = 1; i < columnCount; i++)
                    {
                        newRow.Cells[i].Range.Text = dataTable.Rows[index][i - 1].ToString();
                    }
                    beforeRow = newRow;
                }
            }
            else
            {
                var titles = new List<string>();
                
                for (int i = 1; i <= templateRow.Cells.Count; i++)
                {
                    Word.Cell item = templateRow.Cells[i];
                    Word.Bookmark hasBookmark = null;
                    foreach (Word.Bookmark bookmark in templateRow.Range.Bookmarks)
                    {
                        if (item.Range.Start <= bookmark.Range.Start && item.Range.End >= bookmark.Range.End)
                        {
                            hasBookmark = bookmark;
                            break;
                        }
                    }

                    titles.Add(hasBookmark != null ? hasBookmark.Name : string.Empty);
                }

                int columnCount = templateRow.Cells.Count + 1, length = dataTable.Rows.Count;
                for (int index = 0; index < length; index++)
                {

                    Word.Row newRow = table.Rows.Add(ref beforeRow);
                    for (int i = 1; i < columnCount; i++)
                    {
                        if (!string.IsNullOrEmpty(titles[i - 1]))
                        {
                            newRow.Cells[i].Range.Text = dataTable.Rows[index][titles[i - 1]].ToString();
                        }
                    }

                    //beforeRow = newRow;
                }
            }

            templateRow.Delete();
        }

        protected override object CloneInternal()
        {
            return new ListBookmark();
        }


    }
}
