using System;
using System.Data;
using System.Drawing.Drawing2D;
using System.IO;
using System.Collections;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Imaging;

namespace Enterprises.Framework.Utility
{
	/// <summary>
	/// 图象处理通用类
	/// </summary>
	public class ImageUtility
	{
        /// <summary>
        /// 图像的格式过滤条件
        /// </summary>
        public const string ImageFilter = "图像文件(*.jpg;*.jpeg;*.gif;*.png;*.bmp)|*.jpg;*.jpeg;*.gif;*.png;*.bmp|所有文件(*.*)|*.*";
        
		/// <summary>
		/// 根据指定的文件的扩展名，获取图像的格式
		/// </summary>
		/// <param name="imageFile"></param>
		/// <returns></returns>
		public static ImageFormat GetImageFormat(string imageFile)
		{
            if (imageFile.IndexOf(".", StringComparison.Ordinal) == -1)
            {
                return ImageFormat.Jpeg; ;
            }

            var extension = Path.GetExtension(imageFile);
            if (extension != null)
		    {
                extension = extension.ToUpper();
		        switch(extension)
		        {
		            case "JPG":
		            case "JPEG":
		                return ImageFormat.Jpeg;
		            case "ICO":
		                return ImageFormat.Icon;
		            case "PNG":
		                return ImageFormat.Png;
		            case "GIF":
		                return ImageFormat.Gif;
		            case "BMP":
		                return ImageFormat.Bmp;
		        }
		    }

		    return ImageFormat.Jpeg;
		}
		/// <summary>
		/// 把图像文件转化为字节数组
		/// </summary>
		/// <param name="imageFile"></param>
		/// <returns></returns>
		public static byte[] Image2Bytes(string imageFile)
		{
            using (var ms = new MemoryStream())
            {
                using (Image img = Image.FromFile(imageFile))
                {
                    img.Save(ms, GetImageFormat(imageFile));
                    return StreamUtility.Stream2Bytes(ms);
                }
            }
		}
		/// <summary>
		/// 把图像转化为字节数组
		/// </summary>
		/// <param name="img"></param>
		/// <returns></returns>
		public static byte[] Image2Bytes(Image img)
		{
            if (img == null)
            {
                return null;
            }

            using (Stream s = new MemoryStream())
            {
                img.Save(s, ImageFormat.Jpeg);
                return StreamUtility.Stream2Bytes(s);
            }
		}

        /// <summary>
        /// 把图片转换成流
        /// </summary>
        /// <param name="img"></param>
        /// <param name="imageFormat"></param>
        /// <returns></returns>
        public static Stream Image2Stream(Image img,ImageFormat imageFormat)
        {
            using (var ms = new MemoryStream())
            {
                img.Save(ms, imageFormat);
                return ms;
            }
        }

        /// <summary>
        /// 把字节数组转化为图像对象
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static Image Bytes2Image(byte[] bytes)
        {
            try
            {
                return Image.FromStream(StreamUtility.Bytes2Stream(bytes));
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 把Image存入指定的文件
        /// </summary>
        /// <param name="img"></param>
        /// <param name="saveFileName"></param>
        public static void SaveImage(Image img, string saveFileName)
        {
            string path = FileUtility.GetFilePath(saveFileName);
            FileUtility.CreatePath(path);
            img.Save(saveFileName);
        }

        /// <summary>
        /// 把//二进制表示的图像存入指定的文件
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="saveFileName"></param>
        public static void SaveImage(byte[] bytes, string saveFileName)
        {            
            Image img = Bytes2Image(bytes);
            if(img != null)
            {
                if (!FileUtility.ExistsFile(saveFileName))
                {
                    FileUtility.CreateFile(saveFileName);
                }
                img.Save(saveFileName);
            }           
        }

        /// <summary>
        /// 从文件获取Image对象
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static Image GetImage(string fileName)
        {
            return Image.FromFile(fileName);
        }

		/// <summary>
		/// 按照给定的长宽格式化图片,
		/// 对于图像的格式化,最好以原始数据为依据来进行,否则容易失真
		/// </summary>
		/// <param name="srcImage"></param>
		/// <param name="dstWidth"></param>
		/// <param name="dstHeight"></param>
		/// <returns></returns>
        public static Image ResizePicture(Image srcImage, int dstWidth, int dstHeight)
		{
			var callBack = new Image.GetThumbnailImageAbort(ThumbnailCallback);
			Image dstImage = srcImage.GetThumbnailImage(dstWidth,dstHeight,callBack,new System.IntPtr());
			return dstImage;
		}

	    /// <summary>
	    /// 格式化图像流（按照默认的最佳尺寸比例进行缩放）
	    /// </summary>
	    /// <param name="srcImage"></param>
	    /// <param name="width"></param>
	    /// <param name="height"></param>
	    public static Image ScaledPicture(Image srcImage, int width=640, int height=480)
		{
			int srcWidth = srcImage.Width;
			int srcHeight = srcImage.Height;

            int dstWidth;
            int dstHeight;

            if (srcWidth == width && srcHeight == height)
            {
                //正好匹配，不用格式化了
                return srcImage;
            }
            else if (srcWidth * height > width * srcHeight)
            {
                //以宽度进行按比例压缩，因为太宽，所以让宽度适应整个图片框
                dstWidth = width;
                dstHeight = srcHeight * dstWidth / srcWidth;
            }
            else
            {
                //以高度进行按比例压缩，因为太高，所以让高度适应整个图片框
                dstHeight = height;
                dstWidth = srcWidth * dstHeight / srcHeight;
            }

            return ResizePicture(srcImage, dstWidth, dstHeight);		
		}


		/// <summary>
		/// 格式化图片为缩略图（按照默认的最佳尺寸比例进行缩放）
		/// </summary>
		/// <param name="fullFileName"></param>
		/// <param name="newFullFileName">新的图片的文件名</param>
        public static void ScaledPicture(string fullFileName, string newFullFileName)
		{
			Image srcImage = null;
			Image dstImage = null;
			srcImage = Image.FromFile(fullFileName);
            dstImage = ScaledPicture(srcImage);			
			dstImage.Save(newFullFileName);
			dstImage.Dispose();
			srcImage.Dispose();
		}

		/// <summary>
		/// 按指定的长宽格式化图片
		/// </summary>
		/// <param name="fullFileName"></param>
		/// <param name="newFullFileName"></param>
		/// <param name="dstWidth"></param>
		/// <param name="dstHeight"></param>
        public static void ResizePicture(string fullFileName, string newFullFileName, int dstWidth, int dstHeight)
		{
			Image srcImage = Image.FromFile(fullFileName);
            Image dstImage = ResizePicture(srcImage, dstWidth, dstHeight);			
			dstImage.Save(newFullFileName);
			dstImage.Dispose();
			srcImage.Dispose();
		}

		/// <summary>
		/// 格式化图片为缩略图,新的图片前面加上Small_前缀,表示为缩略图.
		/// </summary>
		/// <param name="fullFileName"></param>
        public static void ResizePicture(string fullFileName)
		{
			string path = FileUtility.GetFilePath(fullFileName);
			string fileName = FileUtility.GetFileName(fullFileName);
            ScaledPicture(fullFileName, path + "Small_" + fileName);
		}

        /// <summary>
        /// 按照指定的百分比压缩图像
        /// </summary>
        /// <param name="srcBytes"></param>
        /// <param name="percent"></param>
        /// <returns></returns>
        public static byte[] ResizePicture(byte[] srcBytes, int percent)
        {
            Image img = Bytes2Image(srcBytes);
            int width = img.Width * percent / 100;
            int height = img.Height * percent / 100;

            return Image2Bytes(ResizePicture(img, width, height));
        }


		/// <summary>
		/// 内部代理程序
		/// </summary>
		/// <returns></returns>
		private static bool ThumbnailCallback() { return false; }

        /// <summary>
        /// 调整图片
        /// </summary>
        /// <param name="bmp">原始Bitmap</param>
        /// <param name="newW">新的宽度</param>
        /// <param name="newH">新的高度</param>
        /// <param name="mode">保留着，暂时未用</param>
        /// <returns>处理以后的图片</returns>
        public Bitmap ResizeImage(Bitmap bmp, int newW, int newH, int mode)
        {
            try
            {
                Bitmap b = new Bitmap(newW, newH);
                Graphics g = Graphics.FromImage(b);
                // 插值算法的质量
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                //设置高质量,低速度呈现平滑程度
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.DrawImage(bmp, new Rectangle(0, 0, newW, newH), new Rectangle(0, 0, bmp.Width, bmp.Height), GraphicsUnit.Pixel);

                g.Dispose();
                return b;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 剪裁图片 -- 用GDI+
        /// </summary>
        /// <param name="b">原始Bitmap</param>
        /// <param name="startX">开始坐标X</param>
        /// <param name="startY">开始坐标Y</param>
        /// <param name="iWidth">宽度</param>
        /// <param name="iHeight">高度</param>
        /// <returns>剪裁后的Bitmap</returns>
        public Bitmap CutImage(Bitmap b, int startX, int startY, int iWidth, int iHeight)
        {
            if (b == null)
            {
                return null;
            }
            int w = b.Width;
            int h = b.Height;
            if (startX >= w || startY >= h)
            {
                return null;
            }

            if (startX + iWidth > w)
            {
                iWidth = w - startX;
            }
            if (startY + iHeight > h)
            {
                iHeight = h - startY;
            }

            try
            {
                Bitmap bmpOut = new Bitmap(iWidth, iHeight);
                Graphics g = Graphics.FromImage(bmpOut);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                //设置高质量,低速度呈现平滑程度
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.DrawImage(b, new Rectangle(0, 0, iWidth, iHeight), new Rectangle(startX, startY, iWidth, iHeight), GraphicsUnit.Pixel);

                g.Dispose();
                return bmpOut;
            }
            catch
            {
                return null;
            }
        }

        #region  水印,缩略图

        //是否已经加载了JPEG编码解码器
        private static bool _isloadjpegcodec = false;
        //当前系统安装的JPEG编码解码器
        private static ImageCodecInfo _jpegcodec = null;

        /// <summary>
        /// 获得当前系统安装的JPEG编码解码器
        /// </summary>
        /// <returns></returns>
        public static ImageCodecInfo GetJPEGCodec()
        {
            if (_isloadjpegcodec == true)
                return _jpegcodec;

            ImageCodecInfo[] codecsList = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo codec in codecsList)
            {
                if (codec.MimeType.IndexOf("jpeg") > -1)
                {
                    _jpegcodec = codec;
                    break;
                }

            }
            _isloadjpegcodec = true;
            return _jpegcodec;
        }

        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="imagePath">图片路径</param>
        /// <param name="thumbPath">缩略图路径</param>
        /// <param name="width">缩略图宽度</param>
        /// <param name="height">缩略图高度</param>
        /// <param name="mode">生成缩略图的方式</param>   
        public static void GenerateThumb(string imagePath, string thumbPath, int width, int height, string mode)
        {
            Image image = Image.FromFile(imagePath);

            string extension = imagePath.Substring(imagePath.LastIndexOf(".")).ToLower();
            ImageFormat imageFormat = null;
            switch (extension)
            {
                case ".jpg":
                case ".jpeg":
                    imageFormat = ImageFormat.Jpeg;
                    break;
                case ".bmp":
                    imageFormat = ImageFormat.Bmp;
                    break;
                case ".png":
                    imageFormat = ImageFormat.Png;
                    break;
                case ".gif":
                    imageFormat = ImageFormat.Gif;
                    break;
                default:
                    imageFormat = ImageFormat.Jpeg;
                    break;
            }

            int toWidth = width > 0 ? width : image.Width;
            int toHeight = height > 0 ? height : image.Height;

            int x = 0;
            int y = 0;
            int ow = image.Width;
            int oh = image.Height;

            switch (mode)
            {
                case "HW"://指定高宽缩放（可能变形）           
                    break;
                case "W"://指定宽，高按比例             
                    toHeight = image.Height * width / image.Width;
                    break;
                case "H"://指定高，宽按比例
                    toWidth = image.Width * height / image.Height;
                    break;
                case "Cut"://指定高宽裁减（不变形）           
                    if ((double)image.Width / (double)image.Height > (double)toWidth / (double)toHeight)
                    {
                        oh = image.Height;
                        ow = image.Height * toWidth / toHeight;
                        y = 0;
                        x = (image.Width - ow) / 2;
                    }
                    else
                    {
                        ow = image.Width;
                        oh = image.Width * height / toWidth;
                        x = 0;
                        y = (image.Height - oh) / 2;
                    }
                    break;
                default:
                    break;
            }

            //新建一个bmp
            Image bitmap = new Bitmap(toWidth, toHeight);

            //新建一个画板
            Graphics g = Graphics.FromImage(bitmap);

            //设置高质量插值法
            g.InterpolationMode = InterpolationMode.High;

            //设置高质量,低速度呈现平滑程度
            g.SmoothingMode = SmoothingMode.HighQuality;

            //清空画布并以透明背景色填充
            g.Clear(Color.Transparent);

            //在指定位置并且按指定大小绘制原图片的指定部分
            g.DrawImage(image,
                        new Rectangle(0, 0, toWidth, toHeight),
                        new Rectangle(x, y, ow, oh),
                        GraphicsUnit.Pixel);

            try
            {
                bitmap.Save(thumbPath, imageFormat);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (g != null)
                    g.Dispose();
                if (bitmap != null)
                    bitmap.Dispose();
                if (image != null)
                    image.Dispose();
            }
        }

        /// <summary>
        /// 生成图片水印
        /// </summary>
        /// <param name="originalPath">源图路径</param>
        /// <param name="watermarkPath">水印图片路径</param>
        /// <param name="targetPath">保存路径</param>
        /// <param name="position">位置</param>
        /// <param name="opacity">透明度</param>
        /// <param name="quality">质量</param>
        public static void GenerateImageWatermark(string originalPath, string watermarkPath, string targetPath, int position, int opacity, int quality)
        {
            Image originalImage = null;
            Image watermarkImage = null;
            //图片属性
            ImageAttributes attributes = null;
            //画板
            Graphics g = null;
            try
            {

                originalImage = Image.FromFile(originalPath);
                watermarkImage = new Bitmap(watermarkPath);

                if (watermarkImage.Height >= originalImage.Height || watermarkImage.Width >= originalImage.Width)
                {
                    originalImage.Save(targetPath);
                    return;
                }

                if (quality < 0 || quality > 100)
                    quality = 80;

                //水印透明度
                float iii;
                if (opacity > 0 && opacity <= 10)
                    iii = (float)(opacity / 10.0F);
                else
                    iii = 0.5F;

                //水印位置
                int x = 0;
                int y = 0;
                switch (position)
                {
                    case 1:
                        x = (int)(originalImage.Width * (float).01);
                        y = (int)(originalImage.Height * (float).01);
                        break;
                    case 2:
                        x = (int)((originalImage.Width * (float).50) - (watermarkImage.Width / 2));
                        y = (int)(originalImage.Height * (float).01);
                        break;
                    case 3:
                        x = (int)((originalImage.Width * (float).99) - (watermarkImage.Width));
                        y = (int)(originalImage.Height * (float).01);
                        break;
                    case 4:
                        x = (int)(originalImage.Width * (float).01);
                        y = (int)((originalImage.Height * (float).50) - (watermarkImage.Height / 2));
                        break;
                    case 5:
                        x = (int)((originalImage.Width * (float).50) - (watermarkImage.Width / 2));
                        y = (int)((originalImage.Height * (float).50) - (watermarkImage.Height / 2));
                        break;
                    case 6:
                        x = (int)((originalImage.Width * (float).99) - (watermarkImage.Width));
                        y = (int)((originalImage.Height * (float).50) - (watermarkImage.Height / 2));
                        break;
                    case 7:
                        x = (int)(originalImage.Width * (float).01);
                        y = (int)((originalImage.Height * (float).99) - watermarkImage.Height);
                        break;
                    case 8:
                        x = (int)((originalImage.Width * (float).50) - (watermarkImage.Width / 2));
                        y = (int)((originalImage.Height * (float).99) - watermarkImage.Height);
                        break;
                    case 9:
                        x = (int)((originalImage.Width * (float).99) - (watermarkImage.Width));
                        y = (int)((originalImage.Height * (float).99) - watermarkImage.Height);
                        break;
                }

                //颜色映射表
                ColorMap colorMap = new ColorMap();
                colorMap.OldColor = Color.FromArgb(255, 0, 255, 0);
                colorMap.NewColor = Color.FromArgb(0, 0, 0, 0);
                ColorMap[] newColorMap = { colorMap };

                //颜色变换矩阵,iii是设置透明度的范围0到1中的单精度类型
                float[][] newColorMatrix ={ 
                                            new float[] {1.0f,  0.0f,  0.0f,  0.0f, 0.0f},
                                            new float[] {0.0f,  1.0f,  0.0f,  0.0f, 0.0f},
                                            new float[] {0.0f,  0.0f,  1.0f,  0.0f, 0.0f},
                                            new float[] {0.0f,  0.0f,  0.0f,  iii, 0.0f},
                                            new float[] {0.0f,  0.0f,  0.0f,  0.0f, 1.0f}
                                           };
                //定义一个 5 x 5 矩阵
                ColorMatrix matrix = new ColorMatrix(newColorMatrix);

                //图片属性
                attributes = new ImageAttributes();
                attributes.SetRemapTable(newColorMap, ColorAdjustType.Bitmap);
                attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                //画板
                g = Graphics.FromImage(originalImage);
                //绘制水印
                g.DrawImage(watermarkImage, new Rectangle(x, y, watermarkImage.Width, watermarkImage.Height), 0, 0, watermarkImage.Width, watermarkImage.Height, GraphicsUnit.Pixel, attributes);
                //保存图片
                EncoderParameters encoderParams = new EncoderParameters();
                encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, new long[] { quality });
                if (GetJPEGCodec() != null)
                    originalImage.Save(targetPath, _jpegcodec, encoderParams);
                else
                    originalImage.Save(targetPath);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (g != null)
                    g.Dispose();
                if (attributes != null)
                    attributes.Dispose();
                if (watermarkImage != null)
                    watermarkImage.Dispose();
                if (originalImage != null)
                    originalImage.Dispose();
            }
        }

        /// <summary>
        /// 生成文字水印
        /// </summary>
        /// <param name="originalPath">源图路径</param>
        /// <param name="targetPath">保存路径</param>
        /// <param name="text">水印文字</param>
        /// <param name="textSize">文字大小</param>
        /// <param name="textFont">文字字体</param>
        /// <param name="position">位置</param>
        /// <param name="quality">质量</param>
        public static void GenerateTextWatermark(string originalPath, string targetPath, string text, int textSize, string textFont, int position, int quality)
        {
            Image originalImage = null;
            //画板
            Graphics g = null;
            try
            {
                originalImage = Image.FromFile(originalPath);
                //画板
                g = Graphics.FromImage(originalImage);
                if (quality < 0 || quality > 100)
                    quality = 80;

                Font font = new Font(textFont, textSize, FontStyle.Regular, GraphicsUnit.Pixel);
                SizeF sizePair = g.MeasureString(text, font);

                float x = 0;
                float y = 0;

                switch (position)
                {
                    case 1:
                        x = (float)originalImage.Width * (float).01;
                        y = (float)originalImage.Height * (float).01;
                        break;
                    case 2:
                        x = ((float)originalImage.Width * (float).50) - (sizePair.Width / 2);
                        y = (float)originalImage.Height * (float).01;
                        break;
                    case 3:
                        x = ((float)originalImage.Width * (float).99) - sizePair.Width;
                        y = (float)originalImage.Height * (float).01;
                        break;
                    case 4:
                        x = (float)originalImage.Width * (float).01;
                        y = ((float)originalImage.Height * (float).50) - (sizePair.Height / 2);
                        break;
                    case 5:
                        x = ((float)originalImage.Width * (float).50) - (sizePair.Width / 2);
                        y = ((float)originalImage.Height * (float).50) - (sizePair.Height / 2);
                        break;
                    case 6:
                        x = ((float)originalImage.Width * (float).99) - sizePair.Width;
                        y = ((float)originalImage.Height * (float).50) - (sizePair.Height / 2);
                        break;
                    case 7:
                        x = (float)originalImage.Width * (float).01;
                        y = ((float)originalImage.Height * (float).99) - sizePair.Height;
                        break;
                    case 8:
                        x = ((float)originalImage.Width * (float).50) - (sizePair.Width / 2);
                        y = ((float)originalImage.Height * (float).99) - sizePair.Height;
                        break;
                    case 9:
                        x = ((float)originalImage.Width * (float).99) - sizePair.Width;
                        y = ((float)originalImage.Height * (float).99) - sizePair.Height;
                        break;
                }

                g.DrawString(text, font, new SolidBrush(Color.White), x + 1, y + 1);
                g.DrawString(text, font, new SolidBrush(Color.Black), x, y);

                //保存图片
                EncoderParameters encoderParams = new EncoderParameters();
                encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, new long[] { quality });
                if (GetJPEGCodec() != null)
                    originalImage.Save(targetPath, _jpegcodec, encoderParams);
                else
                    originalImage.Save(targetPath);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (g != null)
                    g.Dispose();
                if (originalImage != null)
                    originalImage.Dispose();
            }
        }

        #region 正方型裁剪并缩放  

        /// <summary>  
        /// 正方型裁剪  
        /// 以图片中心为轴心，截取正方型，然后等比缩放  
        /// 用于头像处理  
        /// </summary>  
        /// <remarks>姚立峰 2012-08-08</remarks>  
        /// <param name="fromFile">原图Stream对象</param>  
        /// <param name="fileSaveUrl">缩略图存放地址</param>  
        /// <param name="side">指定的边长（正方型）</param>  
        /// <param name="quality">质量（范围0-100）</param>  
        public static void CutForSquare(System.IO.Stream fromFile, string fileSaveUrl, int side, int quality)
        {
            //创建目录  
            string dir = Path.GetDirectoryName(fileSaveUrl);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            //原始图片（获取原始图片创建对象，并使用流中嵌入的颜色管理信息）  
            System.Drawing.Image initImage = System.Drawing.Image.FromStream(fromFile, true);

            //原图宽高均小于模版，不作处理，直接保存  
            if (initImage.Width <= side && initImage.Height <= side)
            {
                initImage.Save(fileSaveUrl, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            else
            {
                //原始图片的宽、高  
                int initWidth = initImage.Width;
                int initHeight = initImage.Height;

                //非正方型先裁剪为正方型  
                if (initWidth != initHeight)
                {
                    //截图对象  
                    System.Drawing.Image pickedImage = null;
                    System.Drawing.Graphics pickedG = null;

                    //宽大于高的横图  
                    if (initWidth > initHeight)
                    {
                        //对象实例化  
                        pickedImage = new System.Drawing.Bitmap(initHeight, initHeight);
                        pickedG = System.Drawing.Graphics.FromImage(pickedImage);
                        //设置质量  
                        pickedG.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        pickedG.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                        //定位  
                        Rectangle fromR = new Rectangle((initWidth - initHeight) / 2, 0, initHeight, initHeight);
                        Rectangle toR = new Rectangle(0, 0, initHeight, initHeight);
                        //画图  
                        pickedG.DrawImage(initImage, toR, fromR, System.Drawing.GraphicsUnit.Pixel);
                        //重置宽  
                        initWidth = initHeight;
                    }
                    //高大于宽的竖图  
                    else
                    {
                        //对象实例化  
                        pickedImage = new System.Drawing.Bitmap(initWidth, initWidth);
                        pickedG = System.Drawing.Graphics.FromImage(pickedImage);
                        //设置质量  
                        pickedG.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        pickedG.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                        //定位  
                        Rectangle fromR = new Rectangle(0, (initHeight - initWidth) / 2, initWidth, initWidth);
                        Rectangle toR = new Rectangle(0, 0, initWidth, initWidth);
                        //画图  
                        pickedG.DrawImage(initImage, toR, fromR, System.Drawing.GraphicsUnit.Pixel);
                        //重置高  
                        initHeight = initWidth;
                    }

                    //将截图对象赋给原图  
                    initImage = (System.Drawing.Image)pickedImage.Clone();
                    //释放截图资源  
                    pickedG.Dispose();
                    pickedImage.Dispose();
                }

                //缩略图对象  
                System.Drawing.Image resultImage = new System.Drawing.Bitmap(side, side);
                System.Drawing.Graphics resultG = System.Drawing.Graphics.FromImage(resultImage);
                //设置质量  
                resultG.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                resultG.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                //用指定背景色清空画布  
                resultG.Clear(Color.White);
                //绘制缩略图  
                resultG.DrawImage(initImage, new System.Drawing.Rectangle(0, 0, side, side), new System.Drawing.Rectangle(0, 0, initWidth, initHeight), System.Drawing.GraphicsUnit.Pixel);

                //关键质量控制  
                //获取系统编码类型数组,包含了jpeg,bmp,png,gif,tiff  
                ImageCodecInfo[] icis = ImageCodecInfo.GetImageEncoders();
                ImageCodecInfo ici = null;
                foreach (ImageCodecInfo i in icis)
                {
                    if (i.MimeType == "image/jpeg" || i.MimeType == "image/bmp" || i.MimeType == "image/png" || i.MimeType == "image/gif")
                    {
                        ici = i;
                    }
                }
                EncoderParameters ep = new EncoderParameters(1);
                ep.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)quality);

                //保存缩略图  
                resultImage.Save(fileSaveUrl, ici, ep);

                //释放关键质量控制所用资源  
                ep.Dispose();

                //释放缩略图资源  
                resultG.Dispose();
                resultImage.Dispose();

                //释放原始图片资源  
                initImage.Dispose();
            }
        }

        #endregion

        #region 自定义裁剪并缩放  

        /// <summary>  
        /// 指定长宽裁剪  
        /// 按模版比例最大范围的裁剪图片并缩放至模版尺寸  
        /// </summary>  
        /// <remarks>姚立峰 2012-08-08</remarks>  
        /// <param name="fromFile">原图Stream对象</param>  
        /// <param name="fileSaveUrl">保存路径</param>  
        /// <param name="maxWidth">最大宽(单位:px)</param>  
        /// <param name="maxHeight">最大高(单位:px)</param>  
        /// <param name="quality">质量（范围0-100）</param>  
        public static void CutForCustom(System.IO.Stream fromFile, string fileSaveUrl, int maxWidth, int maxHeight, int quality)
        {
            //从文件获取原始图片，并使用流中嵌入的颜色管理信息  
            System.Drawing.Image initImage = System.Drawing.Image.FromStream(fromFile, true);

            //原图宽高均小于模版，不作处理，直接保存  
            if (initImage.Width <= maxWidth && initImage.Height <= maxHeight)
            {
                initImage.Save(fileSaveUrl, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            else
            {
                //模版的宽高比例  
                double templateRate = (double)maxWidth / maxHeight;
                //原图片的宽高比例  
                double initRate = (double)initImage.Width / initImage.Height;

                //原图与模版比例相等，直接缩放  
                if (templateRate == initRate)
                {
                    //按模版大小生成最终图片  
                    System.Drawing.Image templateImage = new System.Drawing.Bitmap(maxWidth, maxHeight);
                    System.Drawing.Graphics templateG = System.Drawing.Graphics.FromImage(templateImage);
                    templateG.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                    templateG.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    templateG.Clear(Color.White);
                    templateG.DrawImage(initImage, new System.Drawing.Rectangle(0, 0, maxWidth, maxHeight), new System.Drawing.Rectangle(0, 0, initImage.Width, initImage.Height), System.Drawing.GraphicsUnit.Pixel);
                    templateImage.Save(fileSaveUrl, System.Drawing.Imaging.ImageFormat.Jpeg);
                }
                //原图与模版比例不等，裁剪后缩放  
                else
                {
                    //裁剪对象  
                    System.Drawing.Image pickedImage = null;
                    System.Drawing.Graphics pickedG = null;

                    //定位  
                    Rectangle fromR = new Rectangle(0, 0, 0, 0);//原图裁剪定位  
                    Rectangle toR = new Rectangle(0, 0, 0, 0);//目标定位  

                    //宽为标准进行裁剪  
                    if (templateRate > initRate)
                    {
                        //裁剪对象实例化  
                        pickedImage = new System.Drawing.Bitmap(initImage.Width, (int)System.Math.Floor(initImage.Width / templateRate));
                        pickedG = System.Drawing.Graphics.FromImage(pickedImage);

                        //裁剪源定位  
                        fromR.X = 0;
                        fromR.Y = (int)System.Math.Floor((initImage.Height - initImage.Width / templateRate) / 2);
                        fromR.Width = initImage.Width;
                        fromR.Height = (int)System.Math.Floor(initImage.Width / templateRate);

                        //裁剪目标定位  
                        toR.X = 0;
                        toR.Y = 0;
                        toR.Width = initImage.Width;
                        toR.Height = (int)System.Math.Floor(initImage.Width / templateRate);
                    }
                    //高为标准进行裁剪  
                    else
                    {
                        pickedImage = new System.Drawing.Bitmap((int)System.Math.Floor(initImage.Height * templateRate), initImage.Height);
                        pickedG = System.Drawing.Graphics.FromImage(pickedImage);

                        fromR.X = (int)System.Math.Floor((initImage.Width - initImage.Height * templateRate) / 2);
                        fromR.Y = 0;
                        fromR.Width = (int)System.Math.Floor(initImage.Height * templateRate);
                        fromR.Height = initImage.Height;

                        toR.X = 0;
                        toR.Y = 0;
                        toR.Width = (int)System.Math.Floor(initImage.Height * templateRate);
                        toR.Height = initImage.Height;
                    }

                    //设置质量  
                    pickedG.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    pickedG.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                    //裁剪  
                    pickedG.DrawImage(initImage, toR, fromR, System.Drawing.GraphicsUnit.Pixel);

                    //按模版大小生成最终图片  
                    System.Drawing.Image templateImage = new System.Drawing.Bitmap(maxWidth, maxHeight);
                    System.Drawing.Graphics templateG = System.Drawing.Graphics.FromImage(templateImage);
                    templateG.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                    templateG.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    templateG.Clear(Color.White);
                    templateG.DrawImage(pickedImage, new System.Drawing.Rectangle(0, 0, maxWidth, maxHeight), new System.Drawing.Rectangle(0, 0, pickedImage.Width, pickedImage.Height), System.Drawing.GraphicsUnit.Pixel);

                    //关键质量控制  
                    //获取系统编码类型数组,包含了jpeg,bmp,png,gif,tiff  
                    ImageCodecInfo[] icis = ImageCodecInfo.GetImageEncoders();
                    ImageCodecInfo ici = null;
                    foreach (ImageCodecInfo i in icis)
                    {
                        if (i.MimeType == "image/jpeg" || i.MimeType == "image/bmp" || i.MimeType == "image/png" || i.MimeType == "image/gif")
                        {
                            ici = i;
                        }
                    }
                    EncoderParameters ep = new EncoderParameters(1);
                    ep.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)quality);

                    //保存缩略图  
                    templateImage.Save(fileSaveUrl, ici, ep);
                    //templateImage.Save(fileSaveUrl, System.Drawing.Imaging.ImageFormat.Jpeg);  

                    //释放资源  
                    templateG.Dispose();
                    templateImage.Dispose();

                    pickedG.Dispose();
                    pickedImage.Dispose();
                }
            }

            //释放资源  
            initImage.Dispose();
        }
        #endregion

        #region 等比缩放  

        /// <summary>  
        /// 图片等比缩放  
        /// </summary>  
        /// <remarks>姚立峰 2012-08-08</remarks>  
        /// <param name="fromFile">原图Stream对象</param>  
        /// <param name="savePath">缩略图存放地址</param>  
        /// <param name="targetWidth">指定的最大宽度</param>  
        /// <param name="targetHeight">指定的最大高度</param>  
        /// <param name="watermarkText">水印文字(为""表示不使用水印)</param>  
        /// <param name="watermarkImage">水印图片路径(为""表示不使用水印)</param>  
        public static void ZoomAuto(System.IO.Stream fromFile, string savePath, System.Double targetWidth, System.Double targetHeight, string watermarkText, string watermarkImage)
        {
            //创建目录  
            string dir = Path.GetDirectoryName(savePath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            //原始图片（获取原始图片创建对象，并使用流中嵌入的颜色管理信息）  
            System.Drawing.Image initImage = System.Drawing.Image.FromStream(fromFile, true);

            //原图宽高均小于模版，不作处理，直接保存  
            if (initImage.Width <= targetWidth && initImage.Height <= targetHeight)
            {
                //文字水印  
                if (watermarkText != "")
                {
                    using (System.Drawing.Graphics gWater = System.Drawing.Graphics.FromImage(initImage))
                    {
                        System.Drawing.Font fontWater = new Font("黑体", 10);
                        System.Drawing.Brush brushWater = new SolidBrush(Color.White);
                        gWater.DrawString(watermarkText, fontWater, brushWater, 10, 10);
                        gWater.Dispose();
                    }
                }

                //透明图片水印  
                if (watermarkImage != "")
                {
                    if (File.Exists(watermarkImage))
                    {
                        //获取水印图片  
                        using (System.Drawing.Image wrImage = System.Drawing.Image.FromFile(watermarkImage))
                        {
                            //水印绘制条件：原始图片宽高均大于或等于水印图片  
                            if (initImage.Width >= wrImage.Width && initImage.Height >= wrImage.Height)
                            {
                                Graphics gWater = Graphics.FromImage(initImage);

                                //透明属性  
                                ImageAttributes imgAttributes = new ImageAttributes();
                                ColorMap colorMap = new ColorMap();
                                colorMap.OldColor = Color.FromArgb(255, 0, 255, 0);
                                colorMap.NewColor = Color.FromArgb(0, 0, 0, 0);
                                ColorMap[] remapTable = { colorMap };
                                imgAttributes.SetRemapTable(remapTable, ColorAdjustType.Bitmap);

                                float[][] colorMatrixElements = {
                                   new float[] {1.0f,  0.0f,  0.0f,  0.0f, 0.0f},
                                   new float[] {0.0f,  1.0f,  0.0f,  0.0f, 0.0f},
                                   new float[] {0.0f,  0.0f,  1.0f,  0.0f, 0.0f},
                                   new float[] {0.0f,  0.0f,  0.0f,  0.5f, 0.0f},//透明度:0.5  
                                   new float[] {0.0f,  0.0f,  0.0f,  0.0f, 1.0f}
                                };

                                ColorMatrix wmColorMatrix = new ColorMatrix(colorMatrixElements);
                                imgAttributes.SetColorMatrix(wmColorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                                gWater.DrawImage(wrImage, new Rectangle(initImage.Width - wrImage.Width, initImage.Height - wrImage.Height, wrImage.Width, wrImage.Height), 0, 0, wrImage.Width, wrImage.Height, GraphicsUnit.Pixel, imgAttributes);

                                gWater.Dispose();
                            }
                            wrImage.Dispose();
                        }
                    }
                }

                //保存  
                initImage.Save(savePath, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            else
            {
                //缩略图宽、高计算  
                double newWidth = initImage.Width;
                double newHeight = initImage.Height;

                //宽大于高或宽等于高（横图或正方）  
                if (initImage.Width > initImage.Height || initImage.Width == initImage.Height)
                {
                    //如果宽大于模版  
                    if (initImage.Width > targetWidth)
                    {
                        //宽按模版，高按比例缩放  
                        newWidth = targetWidth;
                        newHeight = initImage.Height * (targetWidth / initImage.Width);
                    }
                }
                //高大于宽（竖图）  
                else
                {
                    //如果高大于模版  
                    if (initImage.Height > targetHeight)
                    {
                        //高按模版，宽按比例缩放  
                        newHeight = targetHeight;
                        newWidth = initImage.Width * (targetHeight / initImage.Height);
                    }
                }

                //生成新图  
                //新建一个bmp图片  
                System.Drawing.Image newImage = new System.Drawing.Bitmap((int)newWidth, (int)newHeight);
                //新建一个画板  
                System.Drawing.Graphics newG = System.Drawing.Graphics.FromImage(newImage);

                //设置质量  
                newG.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                newG.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                //置背景色  
                newG.Clear(Color.White);
                //画图  
                newG.DrawImage(initImage, new System.Drawing.Rectangle(0, 0, newImage.Width, newImage.Height), new System.Drawing.Rectangle(0, 0, initImage.Width, initImage.Height), System.Drawing.GraphicsUnit.Pixel);

                //文字水印  
                if (watermarkText != "")
                {
                    using (System.Drawing.Graphics gWater = System.Drawing.Graphics.FromImage(newImage))
                    {
                        System.Drawing.Font fontWater = new Font("宋体", 10);
                        System.Drawing.Brush brushWater = new SolidBrush(Color.White);
                        gWater.DrawString(watermarkText, fontWater, brushWater, 10, 10);
                        gWater.Dispose();
                    }
                }

                //透明图片水印  
                if (watermarkImage != "")
                {
                    if (File.Exists(watermarkImage))
                    {
                        //获取水印图片  
                        using (System.Drawing.Image wrImage = System.Drawing.Image.FromFile(watermarkImage))
                        {
                            //水印绘制条件：原始图片宽高均大于或等于水印图片  
                            if (newImage.Width >= wrImage.Width && newImage.Height >= wrImage.Height)
                            {
                                Graphics gWater = Graphics.FromImage(newImage);

                                //透明属性  
                                ImageAttributes imgAttributes = new ImageAttributes();
                                ColorMap colorMap = new ColorMap();
                                colorMap.OldColor = Color.FromArgb(255, 0, 255, 0);
                                colorMap.NewColor = Color.FromArgb(0, 0, 0, 0);
                                ColorMap[] remapTable = { colorMap };
                                imgAttributes.SetRemapTable(remapTable, ColorAdjustType.Bitmap);

                                float[][] colorMatrixElements = {
                                   new float[] {1.0f,  0.0f,  0.0f,  0.0f, 0.0f},
                                   new float[] {0.0f,  1.0f,  0.0f,  0.0f, 0.0f},
                                   new float[] {0.0f,  0.0f,  1.0f,  0.0f, 0.0f},
                                   new float[] {0.0f,  0.0f,  0.0f,  0.5f, 0.0f},//透明度:0.5  
                                   new float[] {0.0f,  0.0f,  0.0f,  0.0f, 1.0f}
                                };

                                ColorMatrix wmColorMatrix = new ColorMatrix(colorMatrixElements);
                                imgAttributes.SetColorMatrix(wmColorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                                gWater.DrawImage(wrImage, new Rectangle(newImage.Width - wrImage.Width, newImage.Height - wrImage.Height, wrImage.Width, wrImage.Height), 0, 0, wrImage.Width, wrImage.Height, GraphicsUnit.Pixel, imgAttributes);
                                gWater.Dispose();
                            }
                            wrImage.Dispose();
                        }
                    }
                }

                //保存缩略图  
                newImage.Save(savePath, System.Drawing.Imaging.ImageFormat.Jpeg);

                //释放资源  
                newG.Dispose();
                newImage.Dispose();
                initImage.Dispose();
            }
        }

        #endregion

        #region 判断文件类型是否为WEB格式图片  

        /// <summary>  
        /// 判断文件类型是否为WEB格式图片  
        /// (注：JPG,GIF,BMP,PNG)  
        /// </summary>  
        /// <param name="contentType">HttpPostedFile.ContentType</param>  
        /// <returns></returns>  
        public static bool IsWebImage(string contentType)
        {
            if (contentType == "image/pjpeg" || contentType == "image/jpeg" || contentType == "image/gif" || contentType == "image/bmp" || contentType == "image/png")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #endregion
    }
}

