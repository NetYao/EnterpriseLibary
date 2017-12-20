using System;
using System.Collections.Generic;
using System.Text;

namespace Enterprises.Framework.Email
{
    
    #region .net邮件发送程
    /// <summary>
    /// Net邮件发送程序
    /// </summary>
    [SmtpEmail("Net邮件发送程序", Version = "2.0", Author = "姚立峰", DllFileName = "Enterprises.Framework.dll")]
    public class SysMailMessage : ISmtpMail
    {
        private string _username;

        /// <summary>
        /// 收件人列表
        /// </summary>
        private readonly List<string> _recipient=new List<string>();

        /// <summary>
        /// 邮件主题
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// 邮件正文
        /// </summary>
        public string Body { get; set; }


        /// <summary>
        /// 发件人地址
        /// </summary>
        public string From { get; set; }


        /// <summary>
        /// 发件人姓名
        /// </summary>
        public string FromName { get; set; }


        /// <summary>
        /// 收件人姓名
        /// </summary>
        public string RecipientName
        {
            get { return _recipient.Count > 0 ? _recipient[0] : ""; }
            set { _recipient.Add(value); }
        }

        /// <summary>
        /// 邮箱域
        /// </summary>
        public string MailDomain { get; set; }

        /// <summary>
        /// 邮件服务器端口号
        /// </summary>	
        public int MailDomainPort { get; set; }


        /// <summary>
        /// SMTP认证时使用的用户名
        /// </summary>
        public string MailServerUserName
        {
            set { _username = value.Trim() != "" ? value.Trim() : ""; }
            get { return _username; }
        }

        /// <summary>
        /// SMTP认证时使用的密码
        /// </summary>
        public string MailServerPassWord { get; set; }

        /// <summary>
        ///  是否Html邮件
        /// </summary>
        public bool Html { get; set; }


        //收件人的邮箱地址
        public bool AddRecipient(params string[] recipients)
        {
            if (_recipient.Count<1)
            {
                throw (new ArgumentNullException("recipients"));
            }

            for (int i = 0; i < recipients.Length; i++)
            {
                string recipient = recipients[i].Trim();
                if (recipient == String.Empty)
                {
                    throw new ArgumentNullException("Recipients[" + i + "]");
                }

                if (recipient.IndexOf("@", StringComparison.Ordinal) == -1)
                {
                    throw new ArgumentException("Recipients.IndexOf(\"@\")==-1", "recipients");
                }

                _recipient.Add(recipient);
            }

            return true;
        }

       

        /// <summary>
        /// 将字符串编码为Base64字符串
        /// </summary>
        /// <param name="str">要编码的字符串</param>
        private string Base64Encode(string str)
        {
            byte[] barray = Encoding.Default.GetBytes(str);
            return Convert.ToBase64String(barray);
        }

        /// <summary>
        /// 发送
        /// </summary>
        /// <returns></returns>
        public bool Send()
        {
            var myEmail = new System.Net.Mail.MailMessage();
            Encoding eEncod = Encoding.GetEncoding("utf-8");
            myEmail.From = new System.Net.Mail.MailAddress(From, FromName, eEncod);

            //向收件人地址集合添加邮件地址
            if (_recipient.Count>0)
            {
                foreach (string t in _recipient)
                {
                    myEmail.To.Add(t);
                }
            }
          
            myEmail.Subject = Subject;
            myEmail.IsBodyHtml = true;
            myEmail.Body =  Body;
            myEmail.Priority = System.Net.Mail.MailPriority.Normal;
            myEmail.BodyEncoding = Encoding.GetEncoding("utf-8");
            //myEmail.BodyFormat = this.Html?MailFormat.Html:MailFormat.Text; //邮件形式，.Text、.Html 

           
            var smtp = new System.Net.Mail.SmtpClient
                {
                    Host = MailDomain,
                    Port = MailDomainPort,
                    Credentials = new System.Net.NetworkCredential(MailServerUserName, MailServerPassWord)
                };

            //smtp.UseDefaultCredentials = true;
            //smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;

            //当不是25端口(gmail:587)
            if (MailDomainPort != 25)
            {
                smtp.EnableSsl = true;
            }
            //System.Web.Mail.SmtpMail.SmtpServer = this.MailDomain;

            try
            {
                smtp.Send(myEmail);
            }
            catch (System.Net.Mail.SmtpException e)
            {
                return false;
            }
          
            return true;
        }
    }
    #endregion

}
