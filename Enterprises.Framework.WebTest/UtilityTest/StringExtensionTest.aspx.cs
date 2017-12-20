using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Enterprises.Framework.Utility;

namespace Enterprises.Framework.WebTest.UtilityTest
{
    public partial class StringExtensionTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string mytext = "我是姚立峰 my English Name Is Yaolifeng.";
            Ellipsize.Text = mytext.Ellipsize(14,"...",false);
            RemoveDiacritics.Text = mytext.RemoveDiacritics();
            ToSafeName.Text = mytext.ToSafeName();
            Translate.Text = mytext.Translate(new char[] {'l', 'Y'}, new char[] {'v', 'x'});
            Strip.Text = mytext.Strip('E','N');
            Any_Name.Text = mytext.Any('N').ToString();
            All_Name.Text = "cccCc".All('c').ToString();
        }
    }
}