using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
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
            var callbackUrl = string.Format("http://{0}/{1}", Common.Domain, Url.Action("first_member", "Events"));
            await Call.Create(new Dictionary<string, object>
            {
                {"from", Common.ConferenceNumber},
                {"to", payload.To},
                {"callbackUrl", callbackUrl},
                {"recordingEnabled", false}
            });
            return new HttpStatusCodeResult(HttpStatusCode.Created);
        }

        // POST: start/demo
        [HttpPost]
        public async Task<ActionResult> Join(JoinPayload payload)
        {
            if (payload == null || string.IsNullOrEmpty(payload.To) || string.IsNullOrEmpty(payload.From))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "number fields are required");
            }
            var callbackUrl = string.Format("http://{0}/{1}", Common.Domain, Url.Action("other_call_events", "Events"));
            await Call.Create(new Dictionary<string, object>
            {
                {"from", payload.From},
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

    public class JoinPayload
    {
        public string From { get; set; }
        public string To { get; set; }
    }
}
