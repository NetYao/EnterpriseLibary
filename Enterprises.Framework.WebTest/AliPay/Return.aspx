<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Return.aspx.cs"  Inherits="Enterprises.Framework.WebTest.AliPay.Return" %>

    <table>
         <tr>
            <td style="text-align:center; ">
                <img src="../Resources/Images/pay_success.png" /><span style="color:#12C65C;font-weight:bold;"> 您的订单已经支付成功！</span>
            </td>
        </tr>
        <tr>
            <td>
                <table>
                    <tr>
                        <td>
                            <input type="button" value="确定" onclick="javascript: window.location.href = '../PR/BDBUSINESSERPR/List.aspx'"
                                class="mef_Button" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>



