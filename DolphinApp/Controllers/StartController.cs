using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Bandwidth.Net.Model;
using DolphinApp.Lib;

namespace DolphinApp.Controllers
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
            var callbackUrl = string.Format("http://{0}{1}", Common.Domain, Url.Action("Demo", "Events"));
            await Call.Create(new Dictionary<string, object>
            {
                {"from", Common.Caller},
                {"to", payload.To},
                {"callbackUrl", callbackUrl}
            });
            return new HttpStatusCodeResult(HttpStatusCode.Created);
        }
    }

    public class DemoPayload
    {
        public string To { get; set; }
    }
}
