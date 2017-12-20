using System;
using System.Text;
using System.Collections;
using System.Net.Sockets;

namespace Enterprises.Framework.Email
{
	
	#region 邮件发送程序

    /// <summary>
    /// 邮件发送程序
    /// </summary>
    [SmtpEmail("邮件发送程序", Version = "2.0", Author = "姚立峰", DllFileName = "Enterprises.Framework.dll")]
	public class SmtpMail: ISmtpMail
	{
        private const string Enter = "\r\n";

        /// <summary>
		/// 设定语言代码,默认设定为GB2312,如不需要可设置为""
		/// </summary>
		private string _charset = "GB2312";

        /// <summary>
		/// 发件人姓名
		/// </summary>
		private string _fromName = "";

		/// <summary>
		/// 回复邮件地址
		/// </summary>
		///public string ReplyTo="";

		/// <summary>
		/// 收件人姓名
		/// </summary>	
		private string _recipientName = "";

		/// <summary>
		/// 收件人列表
		/// </summary>
		private readonly Hashtable _recipient = new Hashtable();

		/// <summary>
		/// 邮件服务器域名
		/// </summary>	
		private string _mailserver = "";

		/// <summary>
		/// 邮件服务器端口号
		/// </summary>	
		private int _mailserverport = 25;

		/// <summary>
		/// SMTP认证时使用的用户名
		/// </summary>
		private string _username = "";

		/// <summary>
		/// SMTP认证时使用的密码
		/// </summary>
		private string _password = "";

		/// <summary>
		/// 是否需要SMTP验证
		/// </summary>		
		private bool _eSmtp;


        /// <summary>
		/// 邮件附件列表
		/// </summary>
		private readonly IList _attachments;

		/// <summary>
		/// 邮件发送优先级,可设置为"High","Normal","Low"或"1","3","5"
		/// </summary>
		private string _priority = "Normal";

        /// <summary>
		/// 密送收件人列表
		/// </summary>
		///private Hashtable RecipientBCC=new Hashtable();

		/// <summary>
		/// 收件人数量
		/// </summary>
		private int _recipientNum;

		/// <summary>
		/// 最多收件人数量
		/// </summary>
		private int _recipientmaxnum = 5;

		/// <summary>
		/// 密件收件人数量
		/// </summary>
		///private int RecipientBCCNum=0;

		/// <summary>
		/// 错误消息反馈
		/// </summary>
		private string _errmsg;

		/// <summary>
		/// TcpClient对象,用于连接服务器
		/// </summary>	
		private TcpClient _tc;

		/// <summary>
		/// NetworkStream对象
		/// </summary>	
		private NetworkStream _ns;
		
		/// <summary>
		/// 服务器交互记录
		/// </summary>
		private string _logs = "";

		/// <summary>
		/// SMTP错误代码哈希表
		/// </summary>
		private readonly Hashtable _errCodeHt = new Hashtable();

		/// <summary>
		/// SMTP正确代码哈希表
		/// </summary>
		private readonly Hashtable _rightCodeHt = new Hashtable();
		

		/// <summary>
		/// </summary>
		public SmtpMail()
		{
		    From = "";
		    _attachments = new ArrayList();
			SmtpCodeAdd();
		}

		#region Properties

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
		/// 设定语言代码,默认设定为GB2312,如不需要可设置为""
		/// </summary>
		public string Charset
		{
			get
			{
				return _charset;
			}
			set
			{
				_charset = value;
			}
		}

		/// <summary>
		/// 发件人姓名
		/// </summary>
		public string FromName
		{
			get
			{
				return _fromName;
			}
			set
			{
				_fromName = value;
			}
		}

		/// <summary>
		/// 收件人姓名
		/// </summary>
		public string RecipientName
		{
			get
			{
				return _recipientName;
			}
			set
			{
				_recipientName = value;
			}
		}
		
		/// <summary>
		/// 邮件服务器域名和验证信息
		/// 形如："user:pass@www.server.com:25",也可省略次要信息。如"user:pass@www.server.com"或"www.server.com"
		/// </summary>	
		public string MailDomain
		{
			set
			{
				string maidomain = value.Trim();

			    if(maidomain != "")
				{
					int tempint = maidomain.IndexOf("@", StringComparison.Ordinal);
					if(tempint != -1)
					{
						string str = maidomain.Substring(0,tempint);
						MailServerUserName = str.Substring(0,str.IndexOf(":", StringComparison.Ordinal));
						MailServerPassWord = str.Substring(str.IndexOf(":", StringComparison.Ordinal) + 1,str.Length - str.IndexOf(":", StringComparison.Ordinal) - 1);
						maidomain = maidomain.Substring(tempint + 1,maidomain.Length - tempint - 1);
					}

					tempint = maidomain.IndexOf(":", StringComparison.Ordinal);
					if(tempint != -1)
					{
						_mailserver = maidomain.Substring(0,tempint);
						_mailserverport = Convert.ToInt32(maidomain.Substring(tempint + 1,maidomain.Length - tempint - 1));
					}
					else
					{
						_mailserver = maidomain;

					}

					
				}

			}
		}



		/// <summary>
		/// 邮件服务器端口号
		/// </summary>	
		public int MailDomainPort
		{
			set
			{
				_mailserverport = value;
			}
		}



		/// <summary>
		/// SMTP认证时使用的用户名
		/// </summary>
		public string MailServerUserName
		{
			set
			{
				if(value.Trim() != "")
				{
					_username = value.Trim();
					_eSmtp = true;
				}
				else
				{
					_username = "";
					_eSmtp = false;
				}
			}
		}

		/// <summary>
		/// SMTP认证时使用的密码
		/// </summary>
		public string MailServerPassWord
		{
			set
			{
				_password = value;
			}
		}	


		/// <summary>
		/// 
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		public string ErrCodeHtMessage(string code)
		{
			return _errCodeHt[code].ToString();
		}
		/// <summary>
		/// 邮件发送优先级,可设置为"High","Normal","Low"或"1","3","5"
		/// </summary>
		public string Priority
		{
			set
			{
				switch(value.ToLower())
				{
					case "high":
						_priority = "High";
						break;

					case "1":
						_priority = "High";
						break;

					case "normal":
						_priority = "Normal";
						break;

					case "3":
						_priority = "Normal";
						break;

					case "low":
						_priority = "Low";
						break;

					case "5":
						_priority = "Low";
						break;

					default:
						_priority = "High";
						break;
				}
			}
		}

        /// <summary>
        ///  是否Html邮件
        /// </summary>
        public bool Html { get; set; }


        /// <summary>
		/// 错误消息反馈
		/// </summary>		
		public string ErrorMessage
		{
			get
			{
				return _errmsg;
			}
		}

		/// <summary>
		/// 服务器交互记录
		/// </summary>
		public string Logs
		{
			get
			{
				return _logs;
			}
		}

		/// <summary>
		/// 最多收件人数量
		/// </summary>
		public int RecipientMaxNum
		{
			set
			{
				_recipientmaxnum = value;
			}
		}

		
		#endregion

		#region Methods


		/// <summary>
		/// 添加邮件附件
		/// </summary>
		/// <param name="filePath">附件绝对路径</param>
		public void AddAttachment(params string[] filePath)
		{
			if(filePath == null)
			{
				throw(new ArgumentNullException("filePath"));
			}
			for(int i = 0; i < filePath.Length; i++)
			{
				_attachments.Add(filePath[i]);
			}
		}
		
		/// <summary>
		/// 添加一组收件人(不超过recipientmaxnum个),参数为字符串数组
		/// </summary>
		/// <param name="recipients">保存有收件人地址的字符串数组(不超过recipientmaxnum个)</param>	
		public bool AddRecipient(params string[] recipients)
		{
			if(_recipient == null)
			{
				Dispose();
				throw(new ArgumentNullException("recipients"));
			}

			for(int i = 0; i < recipients.Length; i++)
			{
				string recipient = recipients[i].Trim();
				if(recipient == String.Empty)
				{
					Dispose();
					throw new ArgumentNullException("Recipients["+ i +"]");
				}
				if(recipient.IndexOf("@", StringComparison.Ordinal) == -1)
				{
					Dispose();
					throw new ArgumentException("Recipients.IndexOf(\"@\")==-1","recipients");
				}

				if(!AddRecipient(recipient))
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// 发送邮件方法,所有参数均通过属性设置。
		/// </summary>
		public bool Send()
		{
			if(_mailserver.Trim() == "")
			{
                throw (new ArgumentNullException("_mailserver", "必须指定SMTP服务器"));
			}

			return SendEmail();
			
		}


		/// <summary>
		/// 发送邮件方法
		/// </summary>
		/// <param name="smtpserver">smtp服务器信息,如"username:password@www.smtpserver.com:25",也可去掉部分次要信息,如"www.smtpserver.com"</param>
		public bool Send(string smtpserver)
		{			
			MailDomain = smtpserver;
			return Send();
		}


		/// <summary>
		/// 发送邮件方法
		/// </summary>
		/// <param name="smtpserver">smtp服务器信息,如"username:password@www.smtpserver.com:25",也可去掉部分次要信息,如"www.smtpserver.com"</param>
		/// <param name="from">发件人mail地址</param>
		/// <param name="fromname">发件人姓名</param>
		/// <param name="to">收件人地址</param>
		/// <param name="toname">收件人姓名</param>
		/// <param name="html">是否HTML邮件</param>
		/// <param name="subject">邮件主题</param>
		/// <param name="body">邮件正文</param>
		public bool Send(string smtpserver,string from,string fromname,string to,string toname,bool html,string subject,string body)
		{
			MailDomain = smtpserver;
			From = from;
			FromName = fromname;
			AddRecipient(to);
			RecipientName = toname;
			Html = html;
			Subject = subject;
			Body = body;
			return Send();
		}
		

		#endregion

		#region Private Helper Functions

		/// <summary>
		/// 添加一个收件人
		/// </summary>	
		/// <param name="recipients">收件人地址</param>
		private bool AddRecipient(string recipients)
		{
			//修改等待邮件验证的用户重复发送的问题
			_recipient.Clear();
			_recipientNum = 0;
			if(_recipientNum<_recipientmaxnum)
			{
			    _recipient.Add(_recipientNum, recipients);
				_recipientNum++;				
				return true;
			}

		    Dispose();
		    throw(new ArgumentOutOfRangeException("recipients","收件人过多不可多于 "+ _recipientmaxnum  +" 个"));
		}

		void Dispose()
		{
			if(_ns != null)
				_ns.Close();
			if(_tc != null)
				_tc.Close();
		}

		/// <summary>
		/// SMTP回应代码哈希表
		/// </summary>
		private void SmtpCodeAdd()
		{
			_errCodeHt.Add("500","邮箱地址错误");
			_errCodeHt.Add("501","参数格式错误");
			_errCodeHt.Add("502","命令不可实现");
			_errCodeHt.Add("503","服务器需要SMTP验证");
			_errCodeHt.Add("504","命令参数不可实现");
			_errCodeHt.Add("421","服务未就绪,关闭传输信道");
			_errCodeHt.Add("450","要求的邮件操作未完成,邮箱不可用(例如,邮箱忙)");
			_errCodeHt.Add("550","要求的邮件操作未完成,邮箱不可用(例如,邮箱未找到,或不可访问)");
			_errCodeHt.Add("451","放弃要求的操作;处理过程中出错");
			_errCodeHt.Add("551","用户非本地,请尝试<forward-path>");
			_errCodeHt.Add("452","系统存储不足, 要求的操作未执行");
			_errCodeHt.Add("552","过量的存储分配, 要求的操作未执行");
			_errCodeHt.Add("553","邮箱名不可用, 要求的操作未执行(例如邮箱格式错误)");
			_errCodeHt.Add("432","需要一个密码转换");
			_errCodeHt.Add("534","认证机制过于简单");
			_errCodeHt.Add("538","当前请求的认证机制需要加密");
			_errCodeHt.Add("454","临时认证失败");
			_errCodeHt.Add("530","需要认证");

			_rightCodeHt.Add("220","服务就绪");
			_rightCodeHt.Add("250","要求的邮件操作完成");
			_rightCodeHt.Add("251","用户非本地, 将转发向<forward-path>");
			_rightCodeHt.Add("354","开始邮件输入, 以<enter>.<enter>结束");
			_rightCodeHt.Add("221","服务关闭传输信道");
			_rightCodeHt.Add("334","服务器响应验证Base64字符串");
			_rightCodeHt.Add("235","验证成功");
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
		/// 将Base64字符串解码为普通字符串
		/// </summary>
		/// <param name="str">要解码的字符串</param>
		private string Base64Decode(string str)
        {
            byte[] barray = Convert.FromBase64String(str);
            return Encoding.Default.GetString(barray);
        }


        /// <summary>
		/// 得到上传附件的文件流
		/// </summary>
		/// <param name="filePath">附件的绝对路径</param>
		private string GetStream(string filePath)
		{
			//建立文件流对象
			var fileStr = new System.IO.FileStream(filePath,System.IO.FileMode.Open);
			var by = new byte[Convert.ToInt32(fileStr.Length)];
			fileStr.Read(by,0,by.Length);
			fileStr.Close();
			return(Convert.ToBase64String(by));
		}

		/// <summary>
		/// 发送SMTP命令
		/// </summary>	
		private bool SendCommand(string str)
		{
		    if(str == null || str.Trim() == String.Empty)
			{
				return true;
			}
			_logs += str;
			byte[] writeBuffer = Encoding.Default.GetBytes(str);
			try
			{
				_ns.Write(writeBuffer,0,writeBuffer.Length);
			}
			catch
			{
				_errmsg = "网络连接错误";
				return false;
			}
			return true;
		}

		/// <summary>
		/// 接收SMTP服务器回应
		/// </summary>
		private string RecvResponse()
		{
			int streamSize;
			string returnValue = String.Empty;
			var  readBuffer = new byte[1024] ;
			try
			{
				streamSize = _ns.Read(readBuffer,0,readBuffer.Length);
			}
			catch
			{
				_errmsg = "网络连接错误";
				return "false";
			}

			if (streamSize == 0)
			{
				return returnValue ;
			}
		    returnValue = Encoding.Default.GetString(readBuffer).Substring(0,streamSize);
		    _logs += returnValue + Enter;
		    return returnValue;
		}

		/// <summary>
		/// 与服务器交互,发送一条命令并接收回应。
		/// </summary>
		/// <param name="str">一个要发送的命令</param>
		/// <param name="errstr">如果错误,要反馈的信息</param>
		private bool Dialog(string str,string errstr)
		{
			if(str == null||str.Trim() == "")
			{
				return true;
			}
			if(SendCommand(str))
			{
				string rr = RecvResponse();
				if(rr == "false")
				{
					return false;
				}
				string rrCode = rr.Substring(0,3);
				if(_rightCodeHt[rrCode] != null)
				{
					return true;
				}
			    if(_errCodeHt[rrCode] != null)
			    {
			        _errmsg += (rrCode+_errCodeHt[rrCode]);
			        _errmsg += Enter;
			    }
			    else
			    {
			        _errmsg += rr;
			    }
			    _errmsg += errstr;
			    return false;
			}

		    return false;
		}


		/// <summary>
		/// 与服务器交互,发送一组命令并接收回应。
		/// </summary>
		private bool Dialog(string[] str,string errstr)
		{
			for(int i = 0; i < str.Length; i++)
			{
				if(!Dialog(str[i],""))
				{
					_errmsg += Enter;
					_errmsg += errstr;
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// SendEmail
		/// </summary>
		/// <returns></returns>
		private bool SendEmail()
		{
			//连接网络
			try
			{
				_tc = new TcpClient(_mailserver,_mailserverport);
			}
			catch(Exception e)
			{
				_errmsg = e.ToString();
				return false;
			}

			_ns = _tc.GetStream();
		

			//验证网络连接是否正确
			if(_rightCodeHt[RecvResponse().Substring(0,3)] == null)
			{
				_errmsg = "网络连接失败";
				return false;
			}


			string[] sendBuffer;
			string sendBufferstr;

			//进行SMTP验证
			if(_eSmtp)
			{
				sendBuffer = new String[4];
				sendBuffer[0] = "EHLO " + _mailserver + Enter;
				sendBuffer[1] = "AUTH LOGIN" + Enter;
				sendBuffer[2] = Base64Encode(_username) + Enter;
				sendBuffer[3] = Base64Encode(_password) + Enter;
				if(!Dialog(sendBuffer,"SMTP服务器验证失败,请核对用户名和密码。"))
					return false;
			}
			else
			{
				sendBufferstr = "HELO " + _mailserver + Enter;
				if(!Dialog(sendBufferstr,""))
					return false;
			}

			//
			sendBufferstr = "MAIL FROM:<" + From + ">" + Enter;
			if(!Dialog(sendBufferstr,"发件人地址错误,或不能为空"))
				return false;

			//
			sendBuffer = new string[_recipientmaxnum];
			for(int i = 0; i < _recipient.Count; i++)
			{
				sendBuffer[i] = "RCPT TO:<" + _recipient[i] + ">" + Enter;

			}
			if(!Dialog(sendBuffer,"收件人地址有误"))
				return false;

			/*
						SendBuffer=new string[10];
						for(int i=0;i<RecipientBCC.Count;i++)
						{

							SendBuffer[i]="RCPT TO:<" + RecipientBCC[i].ToString() +">" + enter;

						}

						if(!Dialog(SendBuffer,"密件收件人地址有误"))
							return false;
			*/
			sendBufferstr = "DATA" + Enter;
			if(!Dialog(sendBufferstr,""))
				return false;

			sendBufferstr = "From:" + FromName + "<" + From + ">" +Enter;

			//if(ReplyTo.Trim()!="")
			//{
			//	SendBufferstr+="Reply-To: " + ReplyTo + enter;
			//}

            //SendBufferstr += "To:" + (Discuz.Common.Utils.StrIsNullOrEmpty(RecipientName)?"":Base64Encode(RecipientName)) + "<" + Recipient[0] + ">" + enter;
            sendBufferstr += "To:=?" + Charset.ToUpper() + "?B?" + (string.IsNullOrEmpty(RecipientName) ? "" : Base64Encode(RecipientName)) + "?=" + "<" + _recipient[0] + ">" + Enter;
			
            //注释掉抄送代码
            sendBufferstr += "CC:";
            for (int i = 1; i < _recipient.Count; i++)
            {
                sendBufferstr += _recipient[i] + "<" + _recipient[i] + ">,";
            }
			sendBufferstr += Enter;

			sendBufferstr += (string.IsNullOrEmpty(Subject)?"Subject:":((Charset=="")?("Subject:" + Subject):("Subject:" + "=?" + Charset.ToUpper() + "?B?" + Base64Encode(Subject) +"?="))) + Enter;
			sendBufferstr += "X-Priority:" + _priority + Enter;
			sendBufferstr += "X-MSMail-Priority:" + _priority + Enter;
			sendBufferstr += "Importance:" + _priority + Enter;
			sendBufferstr += "X-Mailer: Lion.Web.Mail.SmtpMail Pubclass [cn]" + Enter;
			sendBufferstr += "MIME-Version: 1.0" + Enter;
			if(_attachments.Count != 0)
			{
				sendBufferstr += "Content-Type: multipart/mixed;" + Enter;
				sendBufferstr += "	boundary=\"=====" + (Html?"001_Dragon520636771063_":"001_Dragon303406132050_") + "=====\"" + Enter + Enter;
			}

			if(Html)
			{
				if(_attachments.Count == 0)
				{
					sendBufferstr += "Content-Type: multipart/alternative;" + Enter;//内容格式和分隔符
					sendBufferstr += "	boundary=\"=====003_Dragon520636771063_=====\"" + Enter + Enter;

					sendBufferstr += "This is a multi-part message in MIME format." + Enter + Enter;
				}
				else
				{
					sendBufferstr += "This is a multi-part message in MIME format." + Enter + Enter;
					sendBufferstr += "--=====001_Dragon520636771063_=====" + Enter;
					sendBufferstr += "Content-Type: multipart/alternative;" + Enter;//内容格式和分隔符
					sendBufferstr += "	boundary=\"=====003_Dragon520636771063_=====\"" + Enter + Enter;					
				}
				sendBufferstr += "--=====003_Dragon520636771063_=====" + Enter;
				sendBufferstr += "Content-Type: text/plain;" + Enter;
				sendBufferstr += ((Charset=="")?("	charset=\"iso-8859-1\""):("	charset=\"" + Charset.ToLower() + "\"")) + Enter;
				sendBufferstr += "Content-Transfer-Encoding: base64" + Enter + Enter;
				sendBufferstr += Base64Encode("邮件内容为HTML格式,请选择HTML方式查看") + Enter + Enter;

				sendBufferstr += "--=====003_Dragon520636771063_=====" + Enter;

				

				sendBufferstr += "Content-Type: text/html;" + Enter;
				sendBufferstr += ((Charset=="")?("	charset=\"iso-8859-1\""):("	charset=\"" + Charset.ToLower() + "\"")) + Enter;
				sendBufferstr += "Content-Transfer-Encoding: base64" + Enter + Enter;
				sendBufferstr += Base64Encode(Body) + Enter + Enter;
				sendBufferstr += "--=====003_Dragon520636771063_=====--" + Enter;
			}
			else
			{
				if(_attachments.Count!=0)
				{
					sendBufferstr += "--=====001_Dragon303406132050_=====" + Enter;
				}
				sendBufferstr += "Content-Type: text/plain;" + Enter;
				sendBufferstr += ((Charset=="")?("	charset=\"iso-8859-1\""):("	charset=\"" + Charset.ToLower() + "\"")) + Enter;
				sendBufferstr += "Content-Transfer-Encoding: base64" + Enter + Enter;
				sendBufferstr += Base64Encode(Body) + Enter;
			}
			
			//SendBufferstr += "Content-Transfer-Encoding: base64"+enter;

			

			
			if(_attachments.Count != 0)
			{
				for(int i = 0; i < _attachments.Count; i++)
				{
					var filepath = (string)_attachments[i];
					sendBufferstr += "--=====" + (Html?"001_Dragon520636771063_":"001_Dragon303406132050_") + "=====" + Enter;
					//SendBufferstr += "Content-Type: application/octet-stream"+enter;
					sendBufferstr += "Content-Type: text/plain;" + Enter;
					sendBufferstr += "	name=\"=?" + Charset.ToUpper() + "?B?" + Base64Encode(filepath.Substring(filepath.LastIndexOf("\\", StringComparison.Ordinal)+1)) + "?=\"" + Enter;
					sendBufferstr += "Content-Transfer-Encoding: base64" + Enter;
					sendBufferstr += "Content-Disposition: attachment;" + Enter;
					sendBufferstr += "	filename=\"=?" + Charset.ToUpper() + "?B?" + Base64Encode(filepath.Substring(filepath.LastIndexOf("\\", StringComparison.Ordinal)+1)) + "?=\"" + Enter + Enter;
					sendBufferstr += GetStream(filepath) + Enter + Enter;
				}
				sendBufferstr += "--=====" + (Html?"001_Dragon520636771063_":"001_Dragon303406132050_") + "=====--" + Enter + Enter;
			}
			
			
			
			sendBufferstr += Enter + "." + Enter;

			if(!Dialog(sendBufferstr,"错误信件信息"))
				return false;


			sendBufferstr = "QUIT" + Enter;
			if(!Dialog(sendBufferstr,"断开连接时错误"))
				return false;


			_ns.Close();
			_tc.Close();
			return true;
		}


		#endregion

		#region
		/*
		/// <summary>
		/// 添加一个密件收件人
		/// </summary>
		/// <param name="str">收件人地址</param>
		public bool AddRecipientBCC(string str)
		{
			if(str==null||str.Trim()=="")
				return true;
			if(RecipientBCCNum<10)
			{
				RecipientBCC.Add(RecipientBCCNum,str);
				RecipientBCCNum++;
				return true;
			}
			else
			{
				errmsg+="收件人过多";
				return false;
			}
		}


		/// <summary>
		/// 添加一组密件收件人(不超过10个),参数为字符串数组
		/// </summary>	
		/// <param name="str">保存有收件人地址的字符串数组(不超过10个)</param>
		public bool AddRecipientBCC(string[] str)
		{
			for(int i=0;i<str.Length;i++)
			{
				if(!AddRecipientBCC(str[i]))
				{
					return false;
				}
			}
			return true;
		}

		*/			
		#endregion	

		
	}
	#endregion

}