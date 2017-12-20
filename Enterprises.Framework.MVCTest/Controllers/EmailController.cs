using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Enterprises.Framework.Email;

namespace Enterprises.Framework.MVCTest.Controllers
{
    public class EmailController : Controller
    {
        //
        // GET: /Email/

        public ActionResult Index()
        {
            return View();
        }

        private bool SendEmail()
        {
            string token = Math.Abs(Guid.NewGuid().GetHashCode()).ToString(CultureInfo.InvariantCulture);
           
            var url = string.Format("{4}/account/chkmail?userid={0}&useremail={1}&stamp={2}&code={3}", "lfyao", HttpUtility.UrlDecode("254949483@qq.com"), DateTime.Now.Ticks, token, "wwww.test.com");
            var body = new StringBuilder();
            body.AppendLine(string.Format("您的用户名：{0}<br>", "姚立峰"));
            body.AppendLine(string.Format("<a href='{0}'>立即验证邮箱</a><br>", url));
            body.AppendLine("您也可以点击以下链接完成验证邮箱(72小时内有效)：<br>");
            body.AppendLine(url);
            body.AppendLine("<br>（若该链接无法点击，请直接复制链接至浏览器地址栏中访问）");

            var emailHelper = new EmailHelper
            {
                MailSubject = "密码激活",
                MailBody = body.ToString(),
                IsbodyHtml = true,
                MailToArray = new string[] { "254949483@qq.com" }
            };

            return emailHelper.Send();
        }


        private bool SendSysEmail()
        {
            string token = Math.Abs(Guid.NewGuid().GetHashCode()).ToString(CultureInfo.InvariantCulture);
            var url = string.Format("{4}/account/chkmail?userid={0}&useremail={1}&stamp={2}&code={3}", "lfyao", HttpUtility.UrlDecode("254949483@qq.com"), DateTime.Now.Ticks, token, "wwww.test.com");
            var body = new StringBuilder();
            body.AppendLine(string.Format("您的用户名：{0}<br>", "姚立峰"));
            body.AppendLine(string.Format("<a href='{0}'>立即验证邮箱</a><br>", url));
            body.AppendLine("您也可以点击以下链接完成验证邮箱(72小时内有效)：<br>");
            body.AppendLine(url);
            body.AppendLine("<br>（若该链接无法点击，请直接复制链接至浏览器地址栏中访问）");

            ISmtpMail mail=new SysMailMessage();
            mail.Subject = "密码激活";
            mail.MailDomain = "smtp.exmail.qq.com";
            mail.MailDomainPort = 25;
            mail.MailServerUserName = "Domo.yao@yibiyi.net";
            mail.MailServerPassWord = "pass@word1";
            mail.From = "Domo.yao@yibiyi.net";
            mail.FromName = "姚立峰";
            mail.Body = body.ToString();
            mail.RecipientName = "yaolifeng001@163.com";
            mail.AddRecipient("254949483@qq.com", "yao-lifeng@163.com");

            return mail.Send();
        }

        public ActionResult ReSendEmail(int typeId)
        {
            bool isSend;
            if (typeId == 1)
            {
                isSend = SendEmail();
            }
            else
            {
                isSend = SendSysEmail();
            }

            if (isSend)
            {
                return Json(new { result = true });
            }

            return Json(new { result = false });
        }
    }
}
