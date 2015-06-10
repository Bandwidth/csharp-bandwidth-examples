using System.Collections.Generic;
using System.Configuration;
using System.Security.Claims;
using System.Text;
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
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                await Bandwidth.Net.Model.Call.Create(new Dictionary<string, object>
                {
                    {"from", user.PhoneNumber},
                    {"to", callModel.PhoneNumber},
                    {"callbackUrl", ConfigurationManager.AppSettings["baseUrl"] + Url.Action("Admin", "Events")}
                });
                TempData["Info"] = "Please answer a call";
            }
            else
            {
                var message = new StringBuilder();
                foreach (var state in ModelState.Values)
                {
                    foreach (var error in state.Errors)
                    {
                        message.AppendLine(error.ErrorMessage);
                    }
                }
                TempData["Error"] = message.ToString();
            }
            return RedirectToAction("Index");
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