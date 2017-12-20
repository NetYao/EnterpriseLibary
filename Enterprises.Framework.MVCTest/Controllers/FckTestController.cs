using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Enterprises.Framework.MVCTest.Models;

namespace Enterprises.Framework.MVCTest.Controllers
{
    public class FckTestController : Controller
    {
        //
        // GET: /FckTest/

        public ActionResult Index()
        {
            var model=new FckTestViewModel();
            model.Contents = "ddddd";
            model.OtherTest = "OtherTest";
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Index(FckTestViewModel viewModel)
        {
            var i = 111 + 333;
            viewModel.OtherTest = viewModel.Contents;
            return View(viewModel);
        }
    }
}
