using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Enterprises.Framework.VerifyImage
{
    /// <summary>
    /// 验证码图片接口
    /// </summary>
    public interface IVerifyImage
    {
        /// <summary>
        /// 产生验证码
        /// </summary>
        /// <param name="len">长度</param>
        /// <param name="onlyNum">只是数字</param>
        /// <returns>验证码对象</returns>
        VerifyImageInfo GenerateImage(int len, bool onlyNum);
    }
}
