using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Enterprises.Framework.MVCTest.Hubs
{
    
    public class SignalTest:Hub
    {
        static SignalTest()
        {
            StringPusher.Init();
        }
    }

    public static class StringPusher

    {

        private static string[] _messages = {"这是从服务器推送的消息。", "使用ASP.NET SignalR技术实现。", "从此不再需要客户端定时发送请求。", "可实现双向实时通信。"};

        private static System.Timers.Timer _timer = new System.Timers.Timer(3000);

        private static IHubConnectionContext _clients = GlobalHost.ConnectionManager.GetHubContext<SignalTest>().Clients;

        private static int _messageIndex = 0;



        public static void Init()
        {
            _timer.Elapsed += (sender, e) => Broadcast();
            _timer.Start();
        }



        public static void Broadcast()

        {
            _messageIndex = (_messageIndex + 1)%_messages.Length;

            _clients.All.showMessage(_messages[_messageIndex]);
        }

    }
}