using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI;
using Aop.Api.Util;
using Enterprises.Framework.Plugin.Logging;
using Enterprises.Pay.AliPay;
using Newtonsoft.Json;

namespace Enterprises.Framework.WebTest.AliPay
{
    public partial class Notify : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string returnStr = "fail";
            ILogger logger = NullLogger.Instance;
            try
            {
                SortedDictionary<string, string> sPara = GetRequestPost();
                logger.Debug("支付提交参数：" + JsonConvert.SerializeObject(sPara));
                if (sPara.Count > 0)//判断是否有带返回参数
                {
                    bool verifyResult = AlipaySignature.RSACheckV1(sPara, AlipayConfig.AlipayPublicKey, "utf-8", AlipayConfig.SignType, false);
                    logger.Debug("验证结果：" + verifyResult);
                    if(verifyResult)
                    {
                        try
                        {
                            #region 保存支付信息
                            ////商户订单号
                            //string out_trade_no = Request.Form["out_trade_no"];
                            ////支付宝交易号
                            //string trade_no = Request.Form["trade_no"];
                            ////支付金额
                            //decimal total_amount = Request.Form["total_amount"].ParseDecimal().GetValueOrDefault();
                            ////交易状态
                            //string trade_status = Request.Form["trade_status"];
                            ////卖家支付宝账号
                            //string seller_id = Request.Form["seller_id"];
                            ////商品描述
                            //string body = Request.Form["body"];
                            ////交易创建时间
                            //DateTime gmt_create = DateTime.Parse(Request.Form["gmt_create"]);
                            ////交易付款时间
                            //DateTime gmt_payment = DateTime.Parse(Request.Form["gmt_payment"]);
                            //string appid = Request.Form["app_id"];
                            //string buyerId = Request.Form["buyer_id"];

                            //BD_BUSINESSER_PRBusiness svc = new BD_BUSINESSER_PRBusiness();
                            //bool dataValidity = svc.CheckOrderInfo(out_trade_no, total_amount, seller_id, appid);//商家判断参数时候是否匹配if (DataValidity)
                            //{
                            //    if (Request.Form["trade_status"] == "TRADE_FINISHED")
                            //    {
                            //        //退款日期超过可退款期限后（如三个月可退款），支付宝系统发送该交易状态通知
                            //    }
                            //    else if (Request.Form["trade_status"] == "TRADE_SUCCESS")
                            //    {
                            //        // 支付成功
                            //        BD_PAYMENT_STREAMEntity model = new BD_PAYMENT_STREAMEntity() {
                            //            PMS_PAY_ACCOUNT= buyerId,
                            //            PMS_PAY_MONEY= total_amount,
                            //            PMS_ORDER_PAY_TIME = gmt_payment,
                            //            PMS_ORDER_CRETATE_TIME= gmt_create,
                            //            PMS_BUSINESSER_ACCOUNT= seller_id,
                            //            PMS_PAY_TYPE = PayType.AliPay.GetHashCode(),
                            //            PMS_PAY_ORDER_NUMBER= out_trade_no,
                            //            PMS_TRANSACTION_NUMBER= trade_no,
                            //            PMS_TRANSACTION_STATUS=PayStatus.Success.GetHashCode(),
                            //            PMS_MEMO= body
                            //        };
                            //        svc.PaySuccess(out_trade_no, model);//修改订单
                            //    }

                            //    returnStr = "success"; //请不要修改或删除
                            //}

                            #endregion
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex.ToString());
                            returnStr = "fail";
                        }
                    }
                }
                else
                {
                    //验证失败
                    returnStr = "fail";
                }

                Response.Write(returnStr);
            }
            catch (Exception ex)
            {
                logger.Error("支付失败" + ex);
                Response.Write("error");
            }
        }

        /// <summary>
        /// 获取支付宝POST过来通知消息，并以“参数名=参数值”的形式组成数组
        /// </summary>
        /// <returns>request回来的信息组成的数组</returns>
        public SortedDictionary<string, string> GetRequestPost()
        {
            int i = 0;
            SortedDictionary<string, string> sArray = new SortedDictionary<string, string>();
            NameValueCollection coll;
            //Load Form variables into NameValueCollection variable.
            coll = Request.Form;

            // Get names of all forms into a string array.
            String[] requestItem = coll.AllKeys;

            for (i = 0; i < requestItem.Length; i++)
            {
                sArray.Add(requestItem[i], Request.Form[requestItem[i]]);
            }

            return sArray;
        }
    }
}