using System;
using System.Collections.Generic;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Enterprises.Framework.Plugin.Excel.ExcelReport
{
    public class PictureWithShapeFormatter : ElementFormatter
    {
        private PictureWithShapeInfo PictureInfo;

        public PictureWithShapeFormatter(PictureWithShapeInfo pictureInfo)
        {
            this.PictureInfo = pictureInfo;
        }

        public override void Format(SheetAdapter sheetAdapter)
        {
            ISheet currentSheet = sheetAdapter.CurrentSheet;
            List<object> shapes = this.PictureInfo.GetShapes(currentSheet);
            bool flag = false;
            if (currentSheet is HSSFSheet)
            {
                flag = true;
            }
            if (shapes == null || shapes.Count <= 0)
            {
                throw new Exception(string.Format("未能获取到工作薄[{0}]指定区域的图形对象列表！", currentSheet.SheetName));
            }
            byte[] pictureData = File.ReadAllBytes(this.PictureInfo.FilePath);
            IClientAnchor anchor;
            IDrawing drawing;
            if (flag)
            {
                HSSFShape hSSFShape = shapes[0] as HSSFShape;
                anchor = (hSSFShape.Anchor as IClientAnchor);
                drawing = hSSFShape.Patriarch;
                hSSFShape.LineStyle = LineStyle.None;
            }
            else
            {
                XSSFShape xSSFShape = shapes[0] as XSSFShape;
                anchor = (xSSFShape.GetAnchor() as IClientAnchor);
                drawing = xSSFShape.GetDrawing();
                xSSFShape.LineStyle = LineStyle.None;
            }
            int pictureIndex = currentSheet.Workbook.AddPicture(pictureData, this.PictureInfo.PictureType);
            IPicture picture = drawing.CreatePicture(anchor, pictureIndex);
            if (this.PictureInfo.AutoSize)
            {
                picture.Resize();
            }
        }
    }
}
