using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using Microsoft.Exchange.WebServices.Data;

namespace Enterprises.Framework.Plugin.Domain.AD
{
    public class ExchangeEwsService
    {
        /// <summary>
        /// 获取exchange 会议室记录
        /// </summary>
        public static List<MeetRoomEntity> GetRoomList(ExchangeAdminConfig config, DateTime startTime,
            DateTime endDateTime)
        {
            ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2013_SP1);

            service.Credentials = new NetworkCredential(config.AdminAccount, config.AdminPwd);
            service.Url = new Uri($"http://{config.ServerIpOrDomain}/ews/exchange.asmx");
            service.UseDefaultCredentials = false;
            List<MeetRoomEntity> result = new List<MeetRoomEntity>();
            EmailAddressCollection listOfRoomList = service.GetRoomLists();
            foreach (EmailAddress address in listOfRoomList)
            {
                var roomAddresses = service.GetRooms(address);

                foreach (EmailAddress roomAddress in roomAddresses)
                {
                    MeetRoomEntity roomEntity = new MeetRoomEntity()
                    {
                        StartDate = startTime,
                        EndDate = endDateTime,
                        MeetEmail = roomAddress.Address,
                        Name = roomAddress.Name
                    };
                    
                    CalendarView calendarView = new CalendarView(startTime, endDateTime);
                    FolderId folderId = new FolderId(WellKnownFolderName.Calendar, roomAddress.Address);
                    var roomAppts = service.FindAppointments(folderId, calendarView);
                    List<MeetInfoEntity> meets = new List<MeetInfoEntity>();
                    if (roomAppts.Items.Count > 0)
                    {
                        foreach (Appointment appt in roomAppts)
                        {
                            meets.Add(new MeetInfoEntity
                            {
                                StartDate = appt.Start,
                                EndDate = appt.End,
                                Subject = appt.Subject
                            });
                        }

                        roomEntity.Meetings = meets;
                    }

                    result.Add(roomEntity);
                }
            }

            return result;

            // listOfRoomList==0 执行下面语句 把会议室归类
            // $members=Get-Mailbox -Filter {(RecipientTypeDetails -eq "RoomMailbox")} 
            //New - DistributionGroup - Name "Poole-Rooms" - RoomList - Members $Members

            // 最好可能还需要设置权限
            // Add-MailboxFolderPermission -Identity quyuanfenghe@ops.net:\Calendar -AccessRights Owner -User Administrator@ops.net
        }


        /// <summary>  
        /// ExchangeService对象  
        /// </summary>  
        private static readonly ExchangeService Service = new ExchangeService(ExchangeVersion.Exchange2013_SP1);



        /// <summary>  
        /// 初始化ExchangeService对象  
        /// </summary>  
        private static void InitializeEws(ExchangeAdminConfig config)
        {
            Service.Credentials = new WebCredentials(config.AdminAccount, config.AdminPwd, config.Ldap);
            Service.Url = new Uri($"http://{config.ServerIpOrDomain}/ews/exchange.asmx");
            Service.UseDefaultCredentials = false;
        }

        #region 会议相关操作  

        /// <summary>  
        /// 获取会议室列表  
        /// </summary>  
        /// <returns>会议室列表</returns>  
        public static EmailAddressCollection GetRoomList(ExchangeAdminConfig config)
        {
            InitializeEws(config);
            EmailAddressCollection roomList = Service.GetRoomLists();
            return roomList;
        }

        /// <summary>  
        /// 保存会议  
        /// </summary>  
        /// <param name="ap">会议属性</param>
        /// <param name="config"></param>
        /// <returns>保存结果</returns>  
        public static bool CreateAppointment(AppointmentProperty ap, ExchangeAdminConfig config)
        {
            try
            {
                InitializeEws(config);
                //初始化会议对象  
                Appointment appointment = new Appointment(Service);
                //会议主题  
                appointment.Subject = ap.Subject;
                //会议内容  
                appointment.Body = ap.Body;
                //会议开始  
                appointment.Start = ap.Start;
                //会议结束  
                appointment.End = ap.End;
                //会议的位置  
                appointment.Location = ap.Location;
                //添加与会者  
                foreach (string attendee in ap.Attendees)
                {
                    appointment.RequiredAttendees.Add(new Attendee(attendee));
                }
                //保存会议  
                appointment.Save();

                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }
        }

        /// <summary>
        /// 委托创建会议
        /// </summary>
        /// <param name="ap">会议属性</param>
        /// <param name="config"></param>
        /// <param name="senderEmailAddress">发送者邮箱地址</param>
        public static bool DelegateAccessCreateMeeting(AppointmentProperty ap, ExchangeAdminConfig config,string senderEmailAddress)
        {
            InitializeEws(config);
            Appointment meeting = new Appointment(Service);
           

            // Set the properties on the meeting object to create the meeting.
            meeting.Subject = ap.Subject;
            meeting.Body = ap.Body;
            meeting.Start =ap.Start;
            meeting.End =ap.End;
            meeting.Location =ap.Location;
            //添加与会者  
            foreach (string attendee in ap.Attendees)
            {
                meeting.RequiredAttendees.Add(new Attendee(attendee));
            }

            meeting.ReminderMinutesBeforeStart = 60;

            // Save the meeting to the Calendar folder for 
            // the mailbox owner and send the meeting request.
            // This method call results in a CreateItem call to EWS.
            meeting.Save(new FolderId(WellKnownFolderName.Calendar,senderEmailAddress),SendInvitationsMode.SendToAllAndSaveCopy);

            // Verify that the meeting was created.
            Item item = Item.Bind(Service, meeting.Id, new PropertySet(ItemSchema.Subject));
            return true;
        }

        /// <summary>  
        /// 获取当前用户指定时间段的会议列表  
        /// </summary>  
        /// <param name="start">开始时间</param>  
        /// <param name="end">结束时间</param>
        /// <param name="config"></param>
        /// <returns>会议列表</returns>  
        public static List<AppointmentProperty> GetAppointment(DateTime start, DateTime end, ExchangeAdminConfig config)
        {
            InitializeEws(config);
            //要返回的会议列表  
            List<AppointmentProperty> appointments = new List<AppointmentProperty>();
            //要获取的时间段  
            CalendarView cv = new CalendarView(start, end);
            FindItemsResults<Appointment> aps = Service.FindAppointments(WellKnownFolderName.Calendar, cv);
            foreach (Appointment ap in aps)
            {
                //定义需要的会议属性  
                PropertyDefinitionBase[] bas = new PropertyDefinitionBase[]
                {
                    ItemSchema.Id, AppointmentSchema.Start, AppointmentSchema.End, ItemSchema.Subject,
                    ItemSchema.Body, AppointmentSchema.RequiredAttendees, AppointmentSchema.Location
                };

                PropertySet props = new PropertySet(bas);
                Appointment email = Appointment.Bind(Service, ap.Id, props);
                AppointmentProperty appointment = new AppointmentProperty();
                appointment.Start = email.Start;
                appointment.End = email.End;
                appointment.Body = email.Body;
                appointment.Subject = email.Subject;
                appointment.Location = email.Location;
                appointment.Attendees = email.RequiredAttendees.Select(p=>p.Address).ToList();
                appointment.ID = email.Id;
               
                appointments.Add(appointment);
            }
            return appointments;
        }

        /// <summary>
        /// 取消会议室指定时间段的会议
        /// </summary>
        /// <param name="config"></param>
        /// <param name="roomName">会议室名称</param>
        /// <param name="startTime">会议开始时间</param>
        /// <param name="endDateTime">会议结束时间</param>
        /// <returns></returns>
        public static void CancelMeeting(ExchangeAdminConfig config,string roomName, DateTime startTime,DateTime endDateTime)
        {
            InitializeEws(config);
            //Service.ImpersonatedUserId = new ImpersonatedUserId(ConnectingIdType.SmtpAddress, "lfyao2@ops.net");

            EmailAddressCollection listOfRoomList = Service.GetRoomLists();
            foreach (EmailAddress address in listOfRoomList)
            {
                var roomAddresses = Service.GetRooms(address);
                foreach (EmailAddress roomAddress in roomAddresses)
                {
                    if (roomAddress.Name != roomName && roomAddress.Address != roomName)
                    {
                        continue;
                    }

                    CalendarView calendarView = new CalendarView(startTime, endDateTime);
                    FolderId folderId = new FolderId(WellKnownFolderName.Calendar, roomAddress.Address);
                    var roomAppts = Service.FindAppointments(folderId, calendarView);
                    if (roomAppts.Items.Count > 0)
                    {
                        foreach (Appointment appt in roomAppts)
                        {
                            Appointment meeting = Appointment.Bind(Service, appt.Id, new PropertySet());
                            meeting.CancelMeeting();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 根据主题查询会议
        /// </summary>
        /// <param name="senderAppointmentSubject"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static MeetingMessage GetSenderMeeting(string senderAppointmentSubject,ExchangeAdminConfig config)
        {
            InitializeEws(config);
            SearchFilter.IsEqualTo senderMeetingMessageSearchBySubject = new SearchFilter.IsEqualTo(MeetingMessageSchema.Subject, senderAppointmentSubject);
            FindItemsResults<Item> senderMeetingRequestResult = Service.FindItems(WellKnownFolderName.Inbox, senderMeetingMessageSearchBySubject, new ItemView(1));

            MeetingMessage senderMeetingMessage = senderMeetingRequestResult.First() as MeetingMessage;
            senderMeetingMessage.Load();

            return senderMeetingMessage;
        }

        /// <summary>  
        /// 导出会议到文件  
        /// </summary>  
        /// <param name="path">目的物理路径</param>  
        /// <param name="count">要导出的数量</param>
        /// <param name="config"></param>
        /// <returns>导出结果</returns>  
        public static bool ExportMimeAppointment(string path, int count, ExchangeAdminConfig config)
        {
            try
            {
                InitializeEws(config);
                Folder inbox = Folder.Bind(Service, WellKnownFolderName.Calendar);
                ItemView view = new ItemView(count);
                view.PropertySet = new PropertySet(BasePropertySet.IdOnly);
                FindItemsResults<Item> results = inbox.FindItems(view);
                foreach (var item in results)
                {
                    PropertyDefinitionBase[] bas = new PropertyDefinitionBase[]
                    {
                        ItemSchema.Id, AppointmentSchema.Start, AppointmentSchema.End, ItemSchema.Subject,
                        ItemSchema.Body, AppointmentSchema.RequiredAttendees, AppointmentSchema.Location,
                        ItemSchema.MimeContent
                    };
                    PropertySet props = new PropertySet(bas);
                    Appointment email = Appointment.Bind(Service, item.Id, props);
                    string iCalFileName = path + email.Start.ToString("yyyy-MM-dd") + "_" + email.Subject + ".ics";
                    using (FileStream fs = new FileStream(iCalFileName, FileMode.Create, FileAccess.Write))
                    {
                        fs.Write(email.MimeContent.Content, 0, email.MimeContent.Content.Length);
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        /// <summary>
        /// 获取建议的会议时间和忙/闲信息
        /// </summary>
        /// <param name="config"></param>
        /// <param name="startTime"></param>
        /// <param name="endDateTime"></param>
        public static void GetSuggestedMeetingTimesAndFreeBusyInfo(ExchangeAdminConfig config, DateTime startTime,
            DateTime endDateTime)
        {
            InitializeEws(config);
            // Create a collection of attendees. 
            List<AttendeeInfo> attendees = new List<AttendeeInfo>();

            attendees.Add(new AttendeeInfo()
            {
                SmtpAddress = "quyuanfenghe@ops.net",
                AttendeeType = MeetingAttendeeType.Room
            });

            attendees.Add(new AttendeeInfo()
            {
                SmtpAddress = "santanyingyue@ops.net",
                AttendeeType = MeetingAttendeeType.Room
            });

            // Specify options to request free/busy information and suggested meeting times.
            //AvailabilityOptions availabilityOptions = new AvailabilityOptions();
            //availabilityOptions.GoodSuggestionThreshold = 49;
            //availabilityOptions.MaximumNonWorkHoursSuggestionsPerDay = 0;
            //availabilityOptions.MaximumSuggestionsPerDay = 2;
            // Note that 60 minutes is the default value for MeetingDuration, but setting it explicitly for demonstration purposes.
            //availabilityOptions.MeetingDuration = 30;
            //availabilityOptions.MinimumSuggestionQuality = SuggestionQuality.Good;
            //availabilityOptions.DetailedSuggestionsWindow = new TimeWindow(startTime, endDateTime);
            //availabilityOptions.RequestedFreeBusyView = FreeBusyViewType.FreeBusy;

            // Return free/busy information and a set of suggested meeting times. 
            // This method results in a GetUserAvailabilityRequest call to EWS.
            GetUserAvailabilityResults results = Service.GetUserAvailability(attendees,
                                                                             new TimeWindow(startTime, endDateTime),
                                                                             AvailabilityData.FreeBusy);
            // Display suggested meeting times. 
            Console.WriteLine("Availability for {0} and {1}", attendees[0].SmtpAddress, attendees[1].SmtpAddress);
            Console.WriteLine();

            //foreach (Suggestion suggestion in results.Suggestions)
            //{
            //    Console.WriteLine("Suggested date: {0}\n", suggestion.Date.ToString("yyyy-MM-dd HH:mm:ss"));
            //    Console.WriteLine("Suggested meeting times:\n");
            //    foreach (TimeSuggestion timeSuggestion in suggestion.TimeSuggestions)
            //    {
            //        Console.WriteLine("\t{0} - {1}\n",
            //                          timeSuggestion.MeetingTime.ToString("yyyy-MM-dd HH:mm:ss"),
            //                          timeSuggestion.MeetingTime.Add(TimeSpan.FromMinutes(availabilityOptions.MeetingDuration)).ToString("yyyy-MM-dd HH:mm:ss"));



            //    }
            //}

            int i = 0;

            // Display free/busy times.
            foreach (AttendeeAvailability availability in results.AttendeesAvailability)
            {
                Console.WriteLine("Availability information for {0}:\n", attendees[i].SmtpAddress);

                foreach (CalendarEvent calEvent in availability.CalendarEvents)
                {
                    Console.WriteLine("\tBusy from {0} to {1} \n", calEvent.StartTime.ToString("yyyy-MM-dd HH:mm:ss"), calEvent.EndTime.ToString("yyyy-MM-dd HH:mm:ss"));
                }

                i++;
            }
        }

        #endregion

        #region 邮件相关操作  

        /// <summary>  
        /// 获取邮件列表  
        /// </summary>  
        /// <param name="isRead">已读true/未读false</param>  
        /// <param name="count">数量</param>
        /// <param name="config"></param>
        /// <returns>邮件列表</returns>  
        public static List<EmailMessage> GetEmailList(bool isRead, int count, ExchangeAdminConfig config)
        {
            InitializeEws(config);
            List<EmailMessage> emails = new List<EmailMessage>();
            //创建过滤器  
            SearchFilter sf = new SearchFilter.IsEqualTo(EmailMessageSchema.IsRead, isRead);
            FindItemsResults<Item> findResults = null;
            try
            {
                findResults = Service.FindItems(WellKnownFolderName.Inbox, sf, new ItemView(count));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            if (findResults != null)
            {
                foreach (Item item in findResults.Items)
                {
                    EmailMessage email = EmailMessage.Bind(Service, item.Id);
                    emails.Add(email);
                }
            }

            return emails;
        }

        

        public static bool DeleteMeeting(ItemId meetingId)
        {
            Appointment meeting = Appointment.Bind(Service, meetingId, new PropertySet());

            // Delete the meeting by using the Delete method. 
            meeting.CancelMeeting();

            // Verify that the meeting has been deleted by looking for a matching subject in the Deleted Items folder's first entry. 
            ItemView itemView = new ItemView(1);
            itemView.Traversal = ItemTraversal.Shallow;
            // Just retrieve the properties you need. 
            itemView.PropertySet = new PropertySet(ItemSchema.Id, ItemSchema.ParentFolderId, ItemSchema.Subject);

            // Note that the FindItems method results in a call to EWS. 
            FindItemsResults<Item> deletedItems = Service.FindItems(WellKnownFolderName.DeletedItems, itemView);
            Item deletedItem = deletedItems.First();

            Folder parentFolder = Folder.Bind(Service, deletedItem.ParentFolderId, new PropertySet(FolderSchema.DisplayName));

            Console.WriteLine("The meeting " + "\"" + deletedItem.Subject + "\"" + " is now in the " + parentFolder.DisplayName + " folder.");
            return true;
        }

        /// <summary>  
        /// 发送邮件  
        /// </summary>  
        /// <param name="subject">邮件标题</param>  
        /// <param name="body">邮件正文</param>  
        /// <param name="emails">收件人列表</param>
        /// <param name="config"></param>
        /// <returns>发送结果</returns>  
        public static bool SendEmail(string subject, string body, List<EmailAddress> emails, ExchangeAdminConfig config)
        {
            try
            {
                InitializeEws(config);
                EmailMessage message = new EmailMessage(Service);
                // 邮件主题  
                message.Subject = subject;
                message.Body = new MessageBody();
                // 指定发送邮件的格式，可以是Text和Html格式  
                message.Body.BodyType = BodyType.Text;
                // 邮件内容  
                message.Body.Text = body;
                // 可以添加多个邮件人.也可以添加一个集合，用  
                foreach (EmailAddress email in emails)
                {
                    message.ToRecipients.Add(email);
                }
                // 保存草稿  
                //message.save();  
                // 只发送不保存邮件  
                // message.Send();  
                // 发送并保存已发送邮件  
                message.SendAndSaveCopy();
               
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        #endregion

        /// <summary>
        /// 授权代理人
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Collection<DelegateUserResponse> AddDelegates(ExchangeAdminConfig config)
        {
            InitializeEws(config);
            // Create a list to hold the new delegates to add.
            List<DelegateUser> newDelegates = new List<DelegateUser>();

            //创建一个新的代理，该代理具有邮箱所有者日历文件夹的编辑器访问权限。.
            DelegateUser calendarDelegate = new DelegateUser("lfyao2@ops.net");
            calendarDelegate.Permissions.CalendarFolderPermissionLevel = DelegateFolderPermissionLevel.Editor;

            // Add the delegate to the list of new delegates.
            newDelegates.Add(calendarDelegate);

            // 创建具有邮箱所有者的“联系人”文件夹的编辑者访问权限的新代理。
            DelegateUser contactDelegate = new DelegateUser("lfyao2@ops.net");
            contactDelegate.Permissions.ContactsFolderPermissionLevel = DelegateFolderPermissionLevel.Editor;

            // Add the delegate to the list of new delegates.
            newDelegates.Add(contactDelegate);

            // 创建具有对邮箱所有者的收件箱文件夹的编辑者访问权限的新代理。
            DelegateUser emailDelegate = new DelegateUser("lfyao2@ops.net");
            emailDelegate.Permissions.InboxFolderPermissionLevel = DelegateFolderPermissionLevel.Editor;

            // Add the delegate to the list of new delegates.
            newDelegates.Add(emailDelegate);

            // 创建表示邮箱所有者的邮箱对象。
            Mailbox mailbox = new Mailbox("administrator@ops.net");

            // Call the AddDelegates method to add the delegates to the target mailbox.
            Collection<DelegateUserResponse> response = Service.AddDelegates(mailbox, MeetingRequestsDeliveryScope.DelegatesAndSendInformationToMe, newDelegates);


            foreach (DelegateUserResponse resp in response)
            {
                // Print out the result and the last eight characters of the item ID.
                Console.WriteLine("For delegate " + resp.DelegateUser.UserId.PrimarySmtpAddress.ToString());
                Console.WriteLine("Result: {0}", resp.Result);
                Console.WriteLine("Error Code: {0}", resp.ErrorCode);
                Console.WriteLine("ErrorMessage: {0}\r\n", resp.ErrorMessage);
                Console.WriteLine("\r\n");
            }

            return response;
        }
        
        
       
    }
}
