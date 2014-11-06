using System;
using System.Web.Mvc;
using Bandwidth.Net.Model;
using ChaosConference.Lib;

namespace ChaosConference.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            if (string.IsNullOrEmpty(Common.ConferenceNumber) || string.IsNullOrEmpty(Common.Domain))
            {
                return Content("Please set up environment variables or web.config for the app");
            }
            return Content("This app is ready to use");
        }
    }
}
