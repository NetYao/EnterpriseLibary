using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Aop.Api;
using Aop.Api.Domain;
using Aop.Api.Request;
using Aop.Api.Response;
using Aop.Api.Util;
using Enterprises.Framework.Plugin.Logging;

namespace Enterprises.Pay.AliPay
{
    public class AliPayHelper
    {
        public string AcquirePageCreateandpay()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("service", "alipay.dut.customer.agreement.page.sign");
            dic.Add("partner", AlipayConfig.Pid);
            dic.Add("product_code", "GENERAL_WITHHOLDING_P");
            //dic.Add("access_info", "{\"channel\":\"ALIPAYAPP\"}");
            dic.Add("access_info", "{\"channel\":\"PC\"}");
            dic.Add("_input_charset", "utf-8");
           
            //dic.Add("scene", "INDUSTRY|CARRENTAL");
            //dic.Add("agreement_sign_parameters", Newtonsoft.Json.JsonConvert.SerializeObject(dic));
            //dic.Add("notify_url", AlipayConfig.NotifyUrl);
            //dic.Add("out_trade_no", "201601010001x");
            //dic.Add("request_from_url", "test");
            //dic.Add("seller_id", AlipayConfig.SellerId);
            //var sign = AlipaySignature.RSASign(dic,AlipayConfig.AliPayHzhbPrivateKey, AlipayConfig.Charset,false, AliPaySignType.RSA.ToString());
            var @params = AlipaySignature.GetSignContent(dic);
            var sign = Md5Sign(@params, AlipayConfig.Md5Key, "utf-8");
            dic.Add("sign", sign);
            dic.Add("sign_type", "MD5");
            var content = AlipaySignature.GetSignContent(dic);
            return content;
        }

        /// <summary>
        /// 统一下单并支付页面接口-alipay.acquire.page.createandpay(首次扣款和签约合并)
        /// </summary>
        /// <returns></returns>
        public string AlipayAcquirePageCreateandpay()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            // 基本参数
            dic.Add("service", "alipay.acquire.page.createandpay");
            dic.Add("partner", AlipayConfig.Pid);
            dic.Add("product_code", "GENERAL_WITHHOLDING");
            dic.Add("integration_type", "ALIAPP");
            dic.Add("_input_charset", "utf-8");
            // 业务参数
            dic.Add("out_trade_no", "DX" + DateTime.Now.ToString("yyyyMMddHHssmmffff"));  //商户网站唯一订单号
            dic.Add("subject", "TestOrder");  //订单标题
            dic.Add("total_fee", "0.01"); // 订单金额
            dic.Add("agreement_sign_parameters", "{\"productCode\":\"GENERAL_WITHHOLDING_P\",\"scene\":\"INDUSTRY|APPSTORE\",\"notifyUrl\":\"https://www.gingergo.cn/api/alipay/signed\",\"externalAgreementNo\":\"\"}");
            dic.Add("notify_url", "https://www.gingergo.cn/api/alipay/signed");
            dic.Add("request_from_url", "https://www.gingergo.cn/api/alipay/signed");
            dic.Add("return_url", "https://www.gingergo.cn/api/alipay/signed");
            dic.Add("seller_id", AlipayConfig.Pid);


            var @params = AlipaySignature.GetSignContent(dic);
            var sign = Md5Sign(@params, AlipayConfig.Md5Key, "utf-8");
            dic.Add("sign", sign);
            dic.Add("sign_type", "MD5");
            var content = AlipaySignature.GetSignContent(dic);
            return content;
        }

        public string Md5Sign(string prestr, string md5Key, string charset)
        {
            StringBuilder sb = new StringBuilder(32);
            prestr = $"{prestr}{md5Key}";
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] t = md5.ComputeHash(Encoding.GetEncoding(charset).GetBytes(prestr));
            for (int i = 0; i < t.Length; i++)
            {
                sb.Append(t[i].ToString("x").PadLeft(2, '0'));
            }

            return sb.ToString();
        }

        /// <summary>
        /// 网页版 提交支付即可
        /// </summary>
        public void PostAlipay(string orderNo,string totalFee,string subject,string body)
        {
            // pr单号，即为订单编号;
            //var url = AliPayHelper.CreateRSASignedOrderString(entity.BPR_CODE, 0.01, entity.BPR_CODE);
            //FileLogHelper.WriteLog("signal:"+ url);

            AlipayTradePagePayRequest request = new AlipayTradePagePayRequest();
            request.SetReturnUrl(AlipayConfig.ReturnUrl);
            request.SetNotifyUrl(AlipayConfig.NotifyUrl);
            AlipayTradePayModel model = new AlipayTradePayModel();
            model.Body = body;
            model.Subject = subject;
            model.TotalAmount = totalFee;
            model.ProductCode = "FAST_INSTANT_TRADE_PAY";
            model.OutTradeNo = orderNo;

            request.SetBizModel(model);
            AlipayTradePagePayResponse response = null;
            try
            {
                System.Web.HttpContext.Current.Response.Write("正在跳转支付宝支付");
                IAopClient client = new DefaultAopClient(AlipayConfig.GatewayUrl, AlipayConfig.AppId,
                    AlipayConfig.MerchantPrivateKey, "json", "1.0",
                     AlipayConfig.SignType, AlipayConfig.AlipayPublicKey, "utf-8");
               
                response = client.pageExecute(request);
                System.Web.HttpContext.Current.Response.Clear();
                System.Web.HttpContext.Current.Response.ContentType = "text/html";
                System.Web.HttpContext.Current.Response.Write(response.Body);
            }
            catch (Exception e)
            {
                
                throw e;
            }
        }

        /// <summary>
        /// 获取短链接( 生成调用接口错误，待调通)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="appid"></param>
        /// <returns></returns>
        public string AlipayMobileShortLinkApply(string url, string appid)
        {
            var furl = string.Format("https://mapi.alipay.com/gateway.do?service=alipay.mobile.short.link.apply&partner={0}&_input_charset=utf-8&timestamp={1}&open_way=WEB&real_url=", appid, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            furl = furl + HttpUtility.UrlEncode(url);
            var res = new WebUtils().DoGet(furl, null, "utf-8");
            return res;
        }
    }
}
