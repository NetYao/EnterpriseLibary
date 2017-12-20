using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enterprises.Framework.Plugin.Excel.ExcelReport
{
    public class SheetRange
    {
        public int? MinRow
        {
            get;
            set;
        }

        public int? MaxRow
        {
            get;
            set;
        }

        public int? MinColumn
        {
            get;
            set;
        }

        public int? MaxColumn
        {
            get;
            set;
        }

        public SheetRange()
        {
        }

        public SheetRange(int minRow, int maxRow, int minColumn, int maxColumn)
        {
            this.MinRow = new int?(minRow);
            this.MaxRow = new int?(maxRow);
            this.MinColumn = new int?(minColumn);
            this.MaxColumn = new int?(maxColumn);
        }

        public override bool Equals(object obj)
        {
            bool flag = base.Equals(obj);
            if (!flag)
            {
                SheetRange sheetRange = obj as SheetRange;
                if (sheetRange != null)
                {
                    flag = (this.MinRow <= sheetRange.MinRow && this.MaxRow >= sheetRange.MaxRow && this.MinColumn <= sheetRange.MinColumn && this.MaxColumn >= sheetRange.MaxColumn);
                }
            }
            return flag;
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("MinRow:{0},MaxRow:{1},MinColumn:{2},MaxColumn:{3}", new object[]
            {
                this.MinRow,
                this.MaxRow,
                this.MinColumn,
                this.MaxColumn
            });
        }
    }
}
