using System.Diagnostics;
using System.Threading.Tasks;
using System.Web.Mvc;
using ChaosConference.Lib;

namespace ChaosConference.Controllers
{
    public class EventsController : Controller
    {
        // POST: events/first_member
        [HttpPost, ExtractEvent, ActionName("first_member")]
        public async Task<ActionResult> FirstMember()
        {
            Trace.WriteLine("FirstMember()", "Events");
            await FirstMemberEventsHandler.ProcessEvent(ViewBag.Event, Url, HttpContext);
            return Json(new object());
        }

        // POST: events/other_call_events
        [HttpPost, ExtractEvent, ActionName("other_call_events")]
        public async Task<ActionResult> OtherCallEvents()
        {
            Trace.WriteLine("OtherCallEvents()", "Events");
            await CallEventsHandler.ProcessEvent(ViewBag.Event, Url, HttpContext);
            return Json(new object());
        }

        // POST: events/conference
        [HttpPost, ExtractEvent]
        public async Task<ActionResult> Conference()
        {
            Trace.WriteLine("Conference()", "Events");
            await ConferenceEventsHandler.ProcessEvent(ViewBag.Event, Url, HttpContext);
            return Json(new object());
        }
    }
}