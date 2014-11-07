using System;
using System.Configuration;
using System.Web.Mvc;
using DolphinApp.Lib;

namespace DolphinApp.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            if (string.IsNullOrEmpty(Common.Caller) || string.IsNullOrEmpty(Common.Domain) || string.IsNullOrEmpty(Common.BridgeCallee))
            {
                return Content("Please set up environment variables or fill web.config for the app");
            }
            return Content("This app is ready to use");
        }
    }
}
