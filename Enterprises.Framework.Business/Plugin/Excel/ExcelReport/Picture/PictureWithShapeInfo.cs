using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.IO;

namespace Enterprises.Framework.Plugin.Excel.ExcelReport
{
    public class PictureWithShapeInfo
    {
        private SheetRange _SheetRange = new SheetRange();

        public string FilePath
        {
            get;
            set;
        }

        public PictureType PictureType
        {
            get;
            set;
        }

        public SheetRange ShapeRange
        {
            get
            {
                return this._SheetRange;
            }
            set
            {
                if (value != null)
                {
                    this._SheetRange = value;
                }
            }
        }

        public bool AutoSize
        {
            get;
            set;
        }

        public PictureWithShapeInfo()
        {
        }

        public PictureWithShapeInfo(string filePath, SheetRange shapeRange = null, bool autoSize = false)
        {
            this.FilePath = filePath;
            this.ShapeRange = shapeRange;
            this.AutoSize = autoSize;
            this.PictureType = this.GetPictureType(filePath);
        }

        public List<object> GetShapes(ISheet sheet)
        {
            List<object> list = new List<object>();
            IDrawing drawingPatriarch = sheet.DrawingPatriarch;
            if (sheet is HSSFSheet)
            {
                HSSFShapeContainer hSSFShapeContainer = sheet.DrawingPatriarch as HSSFShapeContainer;
                if (drawingPatriarch == null)
                {
                    return list;
                }
                IList<HSSFShape> children = hSSFShapeContainer.Children;
                using (IEnumerator<HSSFShape> enumerator = children.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        HSSFShape current = enumerator.Current;
                        if (current != null && current.Anchor is HSSFClientAnchor)
                        {
                            HSSFClientAnchor hSSFClientAnchor = current.Anchor as HSSFClientAnchor;
                            if (this.IsInternalOrIntersect(this.ShapeRange.MinRow, this.ShapeRange.MaxRow, this.ShapeRange.MinColumn, this.ShapeRange.MaxColumn, hSSFClientAnchor.Row1, hSSFClientAnchor.Row2, hSSFClientAnchor.Col1, hSSFClientAnchor.Col2, true))
                            {
                                list.Add(current);
                            }
                        }
                    }
                    return list;
                }
            }
            List<POIXMLDocumentPart> relations = (sheet as XSSFSheet).GetRelations();
            foreach (POIXMLDocumentPart current2 in relations)
            {
                if (current2 is XSSFDrawing)
                {
                    XSSFDrawing xSSFDrawing = (XSSFDrawing)current2;
                    List<XSSFShape> shapes = xSSFDrawing.GetShapes();
                    foreach (XSSFShape current3 in shapes)
                    {
                        XSSFAnchor anchor = current3.GetAnchor();
                        if (current3 != null && anchor is XSSFClientAnchor)
                        {
                            XSSFClientAnchor xSSFClientAnchor = anchor as XSSFClientAnchor;
                            if (this.IsInternalOrIntersect(this.ShapeRange.MinRow, this.ShapeRange.MaxRow, this.ShapeRange.MinColumn, this.ShapeRange.MaxColumn, xSSFClientAnchor.Row1, xSSFClientAnchor.Row2, xSSFClientAnchor.Col1, xSSFClientAnchor.Col2, true))
                            {
                                list.Add(current3);
                            }
                        }
                    }
                }
            }
            return list;
        }

        private PictureType GetPictureType(string filePath)
        {
            string text = Path.GetExtension(filePath).ToUpper();
            string a;
            if ((a = text) != null)
            {
                if (a == ".JPG")
                {
                    return PictureType.JPEG;
                }
                if (a == ".PNG")
                {
                    return PictureType.PNG;
                }
            }
            return PictureType.None;
        }

        private bool IsInternalOrIntersect(int? rangeMinRow, int? rangeMaxRow, int? rangeMinCol, int? rangeMaxCol, int pictureMinRow, int pictureMaxRow, int pictureMinCol, int pictureMaxCol, bool onlyInternal)
        {
            int num = rangeMinRow ?? pictureMinRow;
            int num2 = rangeMaxRow ?? pictureMaxRow;
            int num3 = rangeMinCol ?? pictureMinCol;
            int num4 = rangeMaxCol ?? pictureMaxCol;
            if (onlyInternal)
            {
                return num <= pictureMinRow && num2 >= pictureMaxRow && num3 <= pictureMinCol && num4 >= pictureMaxCol;
            }
            return Math.Abs(num2 - num) + Math.Abs(pictureMaxRow - pictureMinRow) >= Math.Abs(num2 + num - pictureMaxRow - pictureMinRow) && Math.Abs(num4 - num3) + Math.Abs(pictureMaxCol - pictureMinCol) >= Math.Abs(num4 + num3 - pictureMaxCol - pictureMinCol);
        }
    }
}
