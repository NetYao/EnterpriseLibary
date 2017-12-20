namespace Enterprises.Framework.Plugin.Excel.Info
{
    public struct CellSpan
    {
        private int _colSpan;
        private int _rowSpan;

        public CellSpan(int rowSpan, int colSpan)
        {
            _rowSpan = rowSpan;
            _colSpan = colSpan;
        }

        public int RowSpan
        {
            get { return _rowSpan; }
            set { _rowSpan = value; }
        }

        public int ColSpan
        {
            get { return _colSpan; }
            set { _colSpan = value; }
        }
    }
}