using System.Drawing.Imaging;
using System.Drawing;

namespace Enterprises.Framework.VerifyImage
{
    /// <summary>
    /// 验证码图片信息
    /// </summary>
    public class VerifyImageInfo
    {
        /// <summary>
        /// 生成出的图片
        /// </summary>
        public Bitmap Image { get; set; }

        /// <summary>
        /// 输出的图片类型，如 image/pjpeg
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// 图片的格式
        /// </summary>
        public ImageFormat ImageFormat { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        public string VerifyCode { get; set; }
    }

    /// <summary>
    /// 验证码位数
    /// </summary>
    public enum VerifyLen
    {
        /// <summary>
        /// 4位
        /// </summary>
        Four = 4,
        /// <summary>
        /// 5位
        /// </summary>
        Five = 5,
        /// <summary>
        /// 6位
        /// </summary>
        Six = 6,
    }
}
