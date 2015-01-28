using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Bandwidth.Net.Model;
using SipApp.Lib;

namespace SipApp.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public async Task<ActionResult> Index()
        {
            if (string.IsNullOrEmpty(Common.Caller) || string.IsNullOrEmpty(Common.Domain))
            {
                return Content("Please set up environment variables or fill web.config for the app");
            }
            var callbackUrl = string.Format("http://{0}{1}", Common.Domain, Url.Action("Calls", "Events"));
            var app = (await Application.List()).FirstOrDefault(a => a.Name == Common.ApplicationName) ?? await Application.Create(new Dictionary<string, object> { { "name", Common.ApplicationName }, { "incomingCallUrl", callbackUrl }, {"autoAnswer", false} });
            var domain = (await Domain.List()).FirstOrDefault(a => a.Name == Common.DomainName) ?? await Domain.Create(new Dictionary<string, object> { { "name", Common.DomainName }, {"description", Common.ApplicationName} });
            var endPoint = (await domain.GetEndPoints()).FirstOrDefault(a => a.Name == Common.UserName) ??
                           (await domain.CreateEndPoint(new Dictionary<string, object>
                           {
                               {"name", Common.UserName}, 
                               {"description", Common.UserName + " mobile client"},
                               {"applicationId", app.Id},
                               {"domainId", domain.Id},
                               {"enabled", true},
                               {"credentials", new Dictionary<string, object>{{"password", "1234567890"}}}
                           }));
            return Content(string.Format("This app is ready to use<br/>Please configure your sip phone with account <b>{0}@{1}.bwapp.bwsip.io</b> and password <b>1234567890</b>." +
                " Please check if your sip client is online. Then press this button.<br/><form action=\"{2}\"><input type=\"submit\" value=\"Call me\"></input></form>", Common.UserName, Common.DomainName, Url.Action("CallToSip", "Home")));
        }

        // POST: Call
        [HttpPost]
        public async Task<ActionResult> CallToSip()
        {
            var callbackUrl = string.Format("http://{0}{1}", Common.Domain, Url.Action("Demo", "Events"));
            await Call.Create(string.Format("{0}@{1}.bwapp.bwsip.io", Common.UserName, Common.DomainName), Common.Caller, callbackUrl);
            return Content("Please receive a call from sip account");
        }
  
    }
}
