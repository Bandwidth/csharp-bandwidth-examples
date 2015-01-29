using System.Diagnostics;
using System.Threading.Tasks;
using System.Web.Mvc;
using SipApp.Lib;

namespace SipApp.Controllers
{
    public class EventsController : Controller
    {
        // POST: events/demo
        [HttpPost, ExtractEvent]
        public async Task<ActionResult> Demo()
        {
            Trace.WriteLine("Demo()", "Events");
            await DemoEventsHandler.ProcessEvent(ViewBag.Event, Url, HttpContext);
            return Json(new object());
        }

        // POST: events/calls
        [HttpPost, ExtractEvent]
        public async Task<ActionResult> Calls()
        {
            Trace.WriteLine("Calls()", "Events");
            await CallsEventsHandler.ProcessEvent(ViewBag.Event, Url, HttpContext);
            return Json(new object());
        }

        // POST: events/bridged
        [HttpPost, ExtractEvent]
        public async Task<ActionResult> Bridged()
        {
            Trace.WriteLine("Bridged()", "Events");
            await BridgedEventsHandler.ProcessEvent(ViewBag.Event, Url, HttpContext);
            return Json(new object());
        }
       
    }
}