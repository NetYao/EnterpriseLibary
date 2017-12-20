using System;
using System.Configuration;
using System.Net.Mail;
using System.Text;
using Enterprises.Framework.Utility;

namespace Enterprises.Framework.Email
{

    /// <summary>
    /// 微软自带简单发邮件帮助类
    /// </summary>
    public class EmailHelper
    {
        /// <summary>
        /// 发送者
        /// </summary>
        public string MailFrom = ConfigurationManager.AppSettings["EmailFrom"];

        /// <summary>
        /// 发件人密码
        /// </summary>
        public string MailPwd = ConfigurationManager.AppSettings["EmailPwd"];

        /// <summary>
        /// SMTP邮件服务器
        /// </summary>
        public string Host = ConfigurationManager.AppSettings["EmailHost"];

        /// <summary>
        /// 收件人
        /// </summary>
        public string[] MailToArray { get; set; }

        /// <summary>
        /// 抄送
        /// </summary>
        public string[] MailCcArray { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string MailSubject { get; set; }

        /// <summary>
        /// 正文
        /// </summary>
        public string MailBody { get; set; }

       

        /// <summary>
        /// 正文是否是html格式
        /// </summary>
        public bool IsbodyHtml { get; set; }

        /// <summary>
        /// 附件
        /// </summary>
        public string[] AttachmentsPath { get; set; }

        /// <summary>
        /// 邮件发送
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool Send()
        {
            //使用指定的邮件地址初始化MailAddress实例
            var maddr = new MailAddress(MailFrom);
            //初始化MailMessage实例
            var myMail = new MailMessage();


            //向收件人地址集合添加邮件地址
            if (MailToArray != null)
            {
                foreach (string t in MailToArray)
                {
                    myMail.To.Add(t);
                }
            }

            //向抄送收件人地址集合添加邮件地址
            if (MailCcArray != null)
            {
                foreach (string t in MailCcArray)
                {
                    myMail.CC.Add(t);
                }
            }
            //发件人地址
            myMail.From = maddr;

            //电子邮件的标题
            myMail.Subject = MailSubject;

            //电子邮件的主题内容使用的编码
            myMail.SubjectEncoding = Encoding.UTF8;

            //电子邮件正文
            myMail.Body = MailBody;

            //电子邮件正文的编码
            myMail.BodyEncoding = Encoding.Default;

            myMail.Priority = MailPriority.High;

            myMail.IsBodyHtml = IsbodyHtml;

            //在有附件的情况下添加附件
            try
            {
                if (AttachmentsPath != null && AttachmentsPath.Length > 0)
                {
                    foreach (string path in AttachmentsPath)
                    {
                        var attachFile = new Attachment(path);
                        myMail.Attachments.Add(attachFile);
                    }
                }
            }
            catch (Exception err)
            {
                throw new Exception("在添加附件时有错误:" + err);
            }

            //指定发件人的邮件地址和密码以验证发件人身份
            //设置SMTP邮件服务器
            var smtp = new SmtpClient
                {
                    Credentials = new System.Net.NetworkCredential(MailFrom, MailPwd),
                    Host = Host
                };

            try
            {
                //将邮件发送到SMTP邮件服务器
                smtp.Send(myMail);
                return true;
            }
            catch (SmtpException ex)
            {
                LogHelper.WriteErrorLog("发送邮件失败",ex);
                return false;
            }
        }
    }

}