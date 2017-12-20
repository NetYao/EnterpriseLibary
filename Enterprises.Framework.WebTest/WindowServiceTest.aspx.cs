using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Enterprises.Test.Bussiness.ServiceClient;

namespace Enterprises.Framework.WebTest
{
    public partial class WindowServiceTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            using (ServiceAgent target = ServiceAgent.Create())
            {
                var a = target.Jiafa(1, 2);
                lbValue.Text = a.ToString();
            }
        }
    }
}