using System.Web.Mvc;
using Enterprises.Framework.MVCTest.Models;

namespace Enterprises.Framework.MVCTest.Controllers
{
    public class SignalController : Controller
    {
        public ActionResult Index()
        {
            var model=new SignalModel();
            return View(model);
        }

        public ActionResult Index2()
        {
            return View();
        }
    }

    
}