using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Enterprises.Framework.MVCTest.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";
            //IWcfTest wcf=new WcfTestClient();
            //var d = wcf.GetNum();
            //ViewBag.Message = "wcf 返回值："+d.ToString();
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult GridView()
        {
            return View();
        }

      
        public ActionResult Grid()
        {
            //排序的字段名
             string sortname = Request.Params["sortname"];
             //排序的方向
             string sortorder = Request.Params["sortorder"];
             //当前页
             int page = Convert.ToInt32(Request.Params["page"]);
             //每页显示的记录数
             int pagesize = Convert.ToInt32(Request.Params["pagesize"]);


            List<User> users = new List<User>();
            int count = 100;
            for (int i = 1; i <= count; i++)
            {
                users.Add(new User{
                    Id=i,
                    Name="User Name"+i,
                    CreateDate=DateTime.Now
                });
            }

            //这里模拟排序操作
            if (sortorder == "desc")
                users = users.OrderByDescending(c => c.Id).ToList();
            else
                users = users.OrderBy(c => c.Id).ToList();

            IList<User> targetList = new List<User>();
            //这里模拟分页操作        
            for (var i = 0; i < count; i++)
            {
                if (i >= (page - 1) * pagesize && i < page * pagesize)
                {
                    targetList.Add(users[i]);
                }
            }


            var grid = new
            {
                Rows = targetList,
                Total = count
            };

            return Json(grid);
        }


        public class User
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public DateTime CreateDate { get; set; }
        }
    }
}
