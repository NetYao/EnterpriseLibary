using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Enterprises.Framework.VerifyImage
{
    /// <summary>
    /// Gif验证码图片类
    /// </summary>
    public class VerifyGifImage:IVerifyImage
    {
        private static readonly byte[] Randb = new byte[4];
        private static readonly RNGCryptoServiceProvider Rand = new RNGCryptoServiceProvider();

        private static readonly Font[] Fonts =
            {
                new Font(new FontFamily("Times New Roman"), 20 + Next(4), FontStyle.Bold),
                new Font(new FontFamily("Georgia"), 20 + Next(4), FontStyle.Bold),
                new Font(new FontFamily("Arial"), 20 + Next(4), FontStyle.Bold),
                new Font(new FontFamily("Comic Sans MS"), 20 + Next(4), FontStyle.Bold)
            };

        private static readonly string[] VerifycodeRange = { "1","2","3","4","5","6","7","8","9",
                                                    "a","b","c","d","e","f","g",
                                                    "h",    "j","k",    "m","n",
                                                        "p","q",    "r","s","t",
                                                    "u","v","w",    "x","y"
                                                    
                                                  };
        private static readonly Random VerifycodeRandom = new Random();
        /// <summary>
        /// 获得下一个随机数
        /// </summary>
        /// <param name="max">最大值</param>
        /// <returns></returns>
        private static int Next(int max)
        {
            Rand.GetBytes(Randb);
            int value = BitConverter.ToInt32(Randb, 0);
            value = value%(max + 1);
            if (value < 0)
                value = -value;
            return value;
        }

        /// <summary>
        /// 获得下一个随机数
        /// </summary>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns></returns>
        private static int Next(int min, int max)
        {
            int value = Next(max - min) + min;
            return value;
        }


        public VerifyImageInfo GenerateImage(int len, bool onlyNum)
        {
            var verifyimage = new VerifyImageInfo { ImageFormat = ImageFormat.Bmp, ContentType = "image/gif" };
            const int width = 155;
            const int height = 40;
            var bgcolor = Color.White;
            var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            var code = CreateVerifyCode(len, onlyNum);
            Graphics g = Graphics.FromImage(bitmap);
            //var rect = new Rectangle(0, 0, width, height);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(bgcolor);

            //int emSize = Next(3) + 15;//(int)((width - 20) * 2 / text.Length);
            //FontFamily family = fonts[Next(fonts.Length - 1)];
            //Font font = new Font(family, emSize, FontStyle.Bold);

            //SizeF measured = new SizeF(0, 0);
            //SizeF workingSize = new SizeF(width, height);
            //while (emSize > 2 && (measured = g.MeasureString(code, font)).Width > workingSize.Width || measured.Height > workingSize.Height)
            //{
            //    font.Dispose();
            //    font = new Font(family, emSize -= 2);
            //}
            const int fixedNumber = 0;

            var drawBrush = new SolidBrush(Color.FromArgb(Next(100), Next(100), Next(100)));
            for (int x = 0; x < 3; x++)
            {
                var linePen =
                    new Pen(Color.FromArgb(Next(150) + fixedNumber, Next(150) + fixedNumber, Next(150) + fixedNumber), 1);
                g.DrawLine(linePen, new PointF(0.0F + Next(20), 0.0F + Next(height)),
                           new PointF(0.0F + Next(width), 0.0F + Next(height)));
            }


            var m = new Matrix();
            for (int x = 0; x < code.Length; x++)
            {
                m.Reset();
                m.RotateAt(Next(30) - 15, new PointF(Convert.ToInt64(width*(0.10*x)), Convert.ToInt64(height*0.5)));
                g.Transform = m;
                drawBrush.Color = Color.FromArgb(Next(150) + fixedNumber + 20, Next(150) + fixedNumber + 20,
                                                 Next(150) + fixedNumber + 20);
                var drawPoint = new PointF(0.0F + Next(4) + x*20, 3.0F + Next(3));
                g.DrawString(Next(1) == 1 ? code[x].ToString(CultureInfo.InvariantCulture) : code[x].ToString(CultureInfo.InvariantCulture).ToUpper(),
                             Fonts[Next(Fonts.Length - 1)], drawBrush, drawPoint);
                g.ResetTransform();
            }

            double distort = Next(5, 10)*(Next(10) == 1 ? 1 : -1);

            using (var copy = (Bitmap) bitmap.Clone())
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int newX = (int) (x + (distort*Math.Sin(Math.PI*y/84.5)));
                        int newY = (int) (y + (distort*Math.Cos(Math.PI*x/54.5)));
                        if (newX < 0 || newX >= width)
                            newX = 0;
                        if (newY < 0 || newY >= height)
                            newY = 0;
                        bitmap.SetPixel(x, y, copy.GetPixel(newX, newY));
                    }
                }
            }

            drawBrush.Dispose();
            g.Dispose();

            verifyimage.Image = bitmap;
            verifyimage.VerifyCode = code;
            return verifyimage;
        }

        /// <summary>
        /// 产生验证码
        /// </summary>
        /// <param name="len">长度</param>
        /// <param name="onlyNum">只是数字</param>
        /// <returns>string</returns>
        private string CreateVerifyCode(int len, bool onlyNum)
        {
            var checkCode = new StringBuilder();
            for (int i = 0; i < len; i++)
            {
                int number = VerifycodeRandom.Next(0, !onlyNum ? VerifycodeRange.Length : 10);
                checkCode.Append(VerifycodeRange[number]);
            }

            return checkCode.ToString();
        }
    }
}
