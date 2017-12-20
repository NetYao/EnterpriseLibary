namespace Enterprises.Framework.Plugin.Excel.Info
{
    
    public class MergedRegionInfo
    {
        public MergedRegionInfo(int index, int firstRow, int lastRow, int firstCol, int lastCol)
        {
            Index = index;
            FirstRow = firstRow;
            LastRow = lastRow;
            FirstCol = firstCol;
            LastCol = lastCol;
        }

        public int Index { get; set; }
        public int FirstRow { get; set; }
        public int LastRow { get; set; }
        public int FirstCol { get; set; }
        public int LastCol { get; set; }
    }
}