using Aop.Api;
using Aop.Api.Domain;
using Aop.Api.Request;
using Aop.Api.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Enterprises.Pay.AliPay;

namespace Enterprises.Projects.Businessers.AliPay
{
    public partial class Pay : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            AliPayHelper helper = new AliPayHelper();
            helper.PostAlipay("HY20170606055225","0.01","姚立峰测试订单","支付宝web提交支持测试");
        }

       
    }
}