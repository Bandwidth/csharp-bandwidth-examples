using System.Collections.Generic;
using System.Configuration;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using TranscriptionApp.Models;

namespace TranscriptionApp.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public async Task<ActionResult> Index()
        {
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            ViewBag.PhoneNumber = user.PhoneNumber;
            return View();
        }

        
        [HttpPost, Authorize]
        public async Task<ActionResult> Call(CallModel callModel)
        {
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            await Bandwidth.Net.Model.Call.Create(new Dictionary<string, object>
            {
                {"from", user.PhoneNumber},
                {"to", callModel.PhoneNumber},
                {"callbackUrl", ConfigurationManager.AppSettings["baseUrl"] + Url.Action("Admin", "Events")}
            });
            return RedirectToAction("Index", new { Info = "Please answer a call" });
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }
    }
}