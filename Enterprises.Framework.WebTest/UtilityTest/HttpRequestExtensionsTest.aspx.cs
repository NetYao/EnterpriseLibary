using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Enterprises.Framework.Utility;

namespace Enterprises.Framework.WebTest.UtilityTest
{
    public partial class HttpRequestExtensionsTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ToApplicationRootUrlString.Text = Request.ToApplicationRootUrlString();
            ToRootUrlString.Text = Request.ToRootUrlString();
            ToUrlString.Text = Request.ToUrlString();
        }
    }
}