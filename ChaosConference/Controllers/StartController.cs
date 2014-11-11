using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Bandwidth.Net;
using Bandwidth.Net.Model;
using ChaosConference.Lib;

namespace ChaosConference.Controllers
{
    public class StartController : Controller
    {
        // POST: start/demo
        [HttpPost]
        public async Task<ActionResult> Demo(DemoPayload payload)
        {
            if (payload == null || string.IsNullOrEmpty(payload.To))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "number field is required");
            }
            var conferenceId = HttpContext.Application.Get(string.Format("active-conf-{0}", Common.ConferenceNumber)) as string;
            var callbackUrl = string.Format("http://{0}{1}", Common.Domain, Url.Action((conferenceId == null) ? "first_member" : "other_call_events", "Events"));
            await Call.Create(new Dictionary<string, object>
            {
                {"from", Common.ConferenceNumber},
                {"to", payload.To},
                {"callbackUrl", callbackUrl},
                {"recordingEnabled", false}
            });
            return new HttpStatusCodeResult(HttpStatusCode.Created);
        }

    }

    public class DemoPayload
    {
       public string To { get; set; }
    }


}
