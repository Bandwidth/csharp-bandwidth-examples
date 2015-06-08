using System.Diagnostics;
using System.Threading.Tasks;
using System.Web.Mvc;
using TranscriptionApp.Lib;

namespace TranscriptionApp.Controllers
{
    public class EventsController : Controller
    {
        [HttpPost, AllowAnonymous, ExtractEvent]
        public async Task<ActionResult> Admin()
        {
            Trace.WriteLine("Admin()", "Events");
            await AdminEventHandler.ProcessEvent(ViewBag.Event, Url, HttpContext);
            return Json(new object());
        }

        [HttpPost, AllowAnonymous, ExtractEvent]
        public async Task<ActionResult> ExternalCall()
        {
            Trace.WriteLine("ExternalCall()", "Events");
            await ExternalCallHandler.ProcessEvent(ViewBag.Event, Url, HttpContext);
            return Json(new object());
        }
    }
}