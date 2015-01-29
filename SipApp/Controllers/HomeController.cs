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
            if (string.IsNullOrEmpty(Common.Domain))
            {
                return Content("Please set up environment variables or fill web.config for the app");
            }
            var callbackUrl = string.Format("http://{0}{1}", Common.Domain, Url.Action("Calls", "Events"));
            var app = (await Application.List()).FirstOrDefault(a => a.Name == Common.ApplicationName) ?? await Application.Create(new Dictionary<string, object> { { "name", Common.ApplicationName }, { "incomingCallUrl", callbackUrl }, {"autoAnswer", false} });
            var numbers = (await PhoneNumber.List()).Where(n=>n.Application.EndsWith(string.Format("/{0}", app.Id))).ToArray();
            if (numbers.Length < 2)
            {
                var availableNumbers = await AvailableNumber.SearchLocal(new Dictionary<string, object>
                {
                    {"city", "Cary"},
                    {"state", "NC"},
                    {"quantity", 2}
                });
                for (var i = 0; i < 2 - numbers.Length; i++)
                {
                    await PhoneNumber.Create(new Dictionary<string, object>
                    {
                        {"number", availableNumbers[i].Number},
                        {"applicationId", app.Id}
                    });
                }
                numbers = await PhoneNumber.List();
            }
            Common.Caller = numbers[0].Number;
            Common.PhoneNumberForIncomingCalls = numbers[1].Number;
            var domain = (await Domain.List()).FirstOrDefault(a => a.Name == Common.DomainName) ?? await Domain.Create(new Dictionary<string, object> { { "name", Common.DomainName }, {"description", Common.ApplicationName} });
            var endPoint = (await domain.GetEndPoints()).FirstOrDefault(a => a.Name == Common.UserName && a.ApplicationId == app.Id) ??
                           (await domain.CreateEndPoint(new Dictionary<string, object>
                           {
                               {"name", Common.UserName}, 
                               {"description", Common.UserName + " mobile client"},
                               {"applicationId", app.Id},
                               {"domainId", domain.Id},
                               {"enabled", true},
                               {"credentials", new Dictionary<string, object>{{"password", "1234567890"}}}
                           }));
            Common.SipUri = endPoint.SipUri;
            return Content(string.Format("This app is ready to use<br/>" + 
                "Please configure your sip phone with account <b>{0}</b>, server <b>{1}</b> and password <b>1234567890</b>." +
                " Please check if your sip client is online.<br/>" +
                "<ol>" +
               "<li>Press this button to check incoming call to sip client directly <form action=\"{2}\" method=\"POST\"><input type=\"submit\" value=\"Call to sip client\"></input></form></li>" +
               "<li>Call from sip client to any number. Outgoing call will be maden from <b>{3}</b></li>" +
               "<li>Call from any phone (except sip client) to <b>{4}</b>. Incoming call will be redirected to sip client.</li>" + 
                "</ol>", endPoint.Credentials["username"], endPoint.Credentials["realm"], Url.Action("CallToSip", "Home"), Common.Caller, Common.PhoneNumberForIncomingCalls));
        }

        // POST: Call
        [HttpPost]
        public async Task<ActionResult> CallToSip()
        {
            var callbackUrl = string.Format("http://{0}{1}", Common.Domain, Url.Action("Demo", "Events"));
            await Call.Create(Common.SipUri, Common.Caller, callbackUrl);
            return Content("Please receive a call from sip account");
        }
  
    }
}
