using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Enterprises.Framework.Utility;
using Newtonsoft.Json;

namespace Enterprises.Framework.WebTest.UtilityTest
{
    public partial class EntityUtilTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var myClass = new MyClass
                {
                    ID = Guid.NewGuid(),
                    Mobile = "13606806123",
                    MobileIsValid = true,
                    Users = new List<UserEntity> { new UserEntity { ID = Guid.NewGuid(), UserName = "姚立峰" }, new UserEntity { ID = Guid.NewGuid(), UserName = "尹华燕" } }
                };

            var myClass2=new MyClass2();
            EntityUtil.CloneData(myClass, myClass2);
            //TextBox1.Text = JsonConvert.SerializeObject(myClass);
            //TextBox2.Text = JsonConvert.SerializeObject(myClass2);
            TextBox1.Text = EntityUtil.CloneDataToString(myClass);
            TextBox2.Text = EntityUtil.CloneDataToString(myClass2);
        }

        public class MyClass
        {
            public Guid ID { get; set; }
            public string Mobile { get; set; }
            public bool? MobileIsValid { get; set; }
            public List<UserEntity> Users { get; set; }
        }

        public class UserEntity
        {
            public Guid ID { get; set; }
            public string UserName { get; set; }
        }

        public class MyClass2
        {
            public Guid ID { get; set; }
            public string Mobile { get; set; }
            public bool? MobileIsValid { get; set; }
            public string Email { get; set; }
            public List<UserEntity> Users { get; set; }
        }
    }
}