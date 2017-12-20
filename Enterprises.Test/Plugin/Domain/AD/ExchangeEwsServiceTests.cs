using Microsoft.VisualStudio.TestTools.UnitTesting;
using Enterprises.Framework.Plugin.Domain.AD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Exchange.WebServices.Data;

namespace Enterprises.Framework.Plugin.Domain.AD.Tests
{
    [TestClass()]
    public class ExchangeEwsServiceTests
    {
        ExchangeAdminConfig exconfig = new ExchangeAdminConfig().GetDefaultInstance("ops.net");

        [TestMethod()]
        public void GetAppointmentTest()
        {
            List<AppointmentProperty> list = ExchangeEwsService.GetAppointment(DateTime.Now, DateTime.Now.AddDays(20), exconfig);
            var result = list.Select(p => new {p.Location, p.Subject, p.Start, p.End});
            Console.Write(Newtonsoft.Json.JsonConvert.SerializeObject(result));
        }

        [TestMethod()]
        public void GetRoomListTest()
        {
            var result = ExchangeEwsService.GetRoomList(exconfig, DateTime.Now, DateTime.Now.AddDays(20));
            Console.Write(Newtonsoft.Json.JsonConvert.SerializeObject(result));
        }


        [TestMethod()]
        public void GetSuggestedMeetingTimesAndFreeBusyInfoTest()
        {
             ExchangeEwsService.GetSuggestedMeetingTimesAndFreeBusyInfo(exconfig, DateTime.Now, DateTime.Now.AddDays(1));
            //Console.Write(result.Count);
        }

        [TestMethod()]
        public void ExportMimeAppointmentTest()
        {
             ExchangeEwsService.ExportMimeAppointment(@"E:\",10,exconfig);
            //Console.Write(result.Count);
        }

        [TestMethod()]
        public void CancelMeetingTest()
        {
             ExchangeEwsService.CancelMeeting(exconfig, "quyuanfenghe@ops.net", DateTime.Now, DateTime.Now.AddDays(20));
        }

        [TestMethod()]
        public void CreateMeetingTest()
        {
            var app=new AppointmentProperty();
            app.Body = "测试会议";
            app.End=DateTime.Now.AddDays(3);
            app.Location = "每天站会";
            app.Start=DateTime.Now;
            app.Subject = "每天站会";
           app.Attendees=new List<string>() {"lfyao@ops.net"};
            ExchangeEwsService.CreateAppointment(app, exconfig);
        }

        [TestMethod()]
        public void DelegateAccessCreateMeetingTest()
        {
            var app = new AppointmentProperty();
            app.Body = "测试会议";
            app.End = DateTime.Now.AddDays(3);
            app.Location = "每天站会";
            app.Start = DateTime.Now;
            app.Subject = "每天站会";
            app.Attendees = new List<string>() { "lfyao@ops.net","lfyao@pphbh.com" };
            ExchangeEwsService.DelegateAccessCreateMeeting(app, exconfig,"administrator@ops.net");
        }

        [TestMethod()]
        public void GetUnReadMailCountByUserMailAddress()
        {

            int unRead = 0;

            try
            {

                Microsoft.Exchange.WebServices.Data.ExchangeService service = new Microsoft.Exchange.WebServices.Data.ExchangeService(Microsoft.Exchange.WebServices.Data.ExchangeVersion.Exchange2013_SP1);
                service.Credentials = new NetworkCredential(exconfig.AdminAccount,exconfig.AdminPwd, exconfig.Ldap);
                service.Url = new Uri($"http://{exconfig.ServerIpOrDomain}/ews/exchange.asmx");
                service.ImpersonatedUserId = new Microsoft.Exchange.WebServices.Data.ImpersonatedUserId(Microsoft.Exchange.WebServices.Data.ConnectingIdType.SmtpAddress, "lfyao@ops.net");
                unRead = Microsoft.Exchange.WebServices.Data.Folder.Bind(service, Microsoft.Exchange.WebServices.Data.WellKnownFolderName.Inbox).UnreadCount;
            }
            catch(Exception ex)
            {
                Console.Write(ex);
            }

            Console.Write(unRead); ;
        }

    }
}