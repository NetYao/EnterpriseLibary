using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Enterprises.Framework.VerifyImage;

namespace Enterprises.Framework.MVCTest.Controllers
{
    public class VerifyImageController : Controller
    {
        //
        // GET: /VerifyImage/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetValidateCode()
        {
            var vimg = VerifyImageProvider.GetInstance("VerifyImage").GenerateImage(4, false);
            //var vimg = VerifyImageProvider.GetInstance("VerifyGifImage").GenerateImage(4, false);
            var image = vimg.Image;
            var ms = new MemoryStream();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            Session["CheckCode"] = vimg.VerifyCode;
            return File(ms.GetBuffer(), @"image/jpeg");
        }
    }
}
