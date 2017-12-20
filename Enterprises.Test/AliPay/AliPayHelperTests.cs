using NUnit.Framework;
using Enterprises.Pay.AliPay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Enterprises.Pay.AliPay.Tests
{
    [TestFixture()]
    public class AliPayHelperTests
    {
        [Test()]
        public void AcquirePageCreateandpayTest()
        {
            AliPayHelper svc=new AliPayHelper();
            var url= svc.AcquirePageCreateandpay();
            Console.WriteLine(url);
            url = "https://mapi.alipay.com/gateway.do?" + url;
            //url = "alipays://platformapi/startapp?appId="+AlipayConfig.AppId+"&url=" + HttpUtility.UrlEncode(url);
            //url= "https://ds.alipay.com/?scheme="+ HttpUtility.UrlEncode(url);
            Console.WriteLine(url);
        }
    }
}