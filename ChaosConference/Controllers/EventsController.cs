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
            await FirstMemberEventsHandler.ProcessEvent(ViewBag.Event, Url, HttpContext);
            return Json(new object());
        }

        // POST: events/other_call_events
        [HttpPost, ExtractEvent, ActionName("other_call_events")]
        public async Task<ActionResult> OtherCallEvents()
        {
            await CallEventsHandler.ProcessEvent(ViewBag.Event, Url, HttpContext);
            return Json(new object());
        }

        // POST: events/other_call_events
        [HttpPost, ExtractEvent]
        public async Task<ActionResult> Conference()
        {
            await ConferenceEventsHandler.ProcessEvent(ViewBag.Event, Url, HttpContext);
            return Json(new object());
        }
    }
}