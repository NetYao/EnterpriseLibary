using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebHandlerTest;

namespace WebHandlerTest
{
    public partial class TestPerson : System.Web.UI.Page
    {
        public Person person;

        public TestPerson()
        {
            this.PreLoad += new EventHandler(TestPerson_PreLoad);
        }

        void TestPerson_PreLoad(object sender, EventArgs e)
        {
            Person p = this.Session["person"] as Person;

            if (p != null)
            {
                this.tbxName.Text = p.Name;
                this.tbxCity.Text = p.City;
                this.tbxAdd.Text = p.Address;

                this.Session.Remove("person");
            }
        }

 
   
    }
}
