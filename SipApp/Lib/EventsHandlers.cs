using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Bandwidth.Net.Model;

namespace SipApp.Lib
{
    // Handle incoming calls to sip client
    public static class DemoEventsHandler
    {
        public static async Task ProcessEvent(AnswerEvent ev, UrlHelper url, HttpContextBase context)
        {
            Trace.WriteLine("AnswerEvent", "Events");
            var call = new Call {Id = ev.CallId};
            Thread.Sleep(2000);
            await call.SpeakSentence("Hello SIP client");
        }

        public static async Task ProcessEvent(SpeakEvent ev, UrlHelper url, HttpContextBase context)
        {
            Trace.WriteLine("SpeakEvent", "Events");
            if (ev.Status != "done") return;
            var call = new Call { Id = ev.CallId };
            await call.HangUp();
        }
    }

    /// <summary>
    /// Handle all calls of the app
    /// </summary>
    public static class CallsEventsHandler
    {
        public static async Task ProcessEvent(IncomingCallEvent ev, UrlHelper url, HttpContextBase context)
        {
            if (ev.From == Common.Caller)
            {
                return;
            }
            Trace.WriteLine("IncomingCallEvent", "Events");
            var sip = string.Format("{0}@{1}.bwapp.bwsip.io", Common.UserName, Common.DomainName);
            var callbackUrl = string.Format("http://{0}{1}", Common.Domain, url.Action("Bridged", "Events"));
            
            if (ev.From == sip)
            {
                await Call.Create(ev.To, Common.Caller, callbackUrl, ev.CallId);
                return;
            }
            if (ev.To == Common.Caller)
            {
                await Call.Create(sip, Common.Caller, callbackUrl, ev.CallId);
                return;
            }
        }
    }

    /// <summary>
    /// Handle call which will be bridged with current
    /// </summary>
    public static class BridgedEventsHandler
    {
        public static async Task ProcessEvent(AnswerEvent ev, UrlHelper url, HttpContextBase context)
        {
            Trace.WriteLine("AnswerEvent", "Events");
            var call = new Call { Id = ev.Tag };
            await call.AnswerOnIncoming();
            await Bridge.Create(new Dictionary<string, object> { { "callIds", new[] { call.Id, ev.CallId } } });
        }
    }

    /// <summary>
    /// Handle incoming call to specific phone and bridge it to sip client
    /// </summary>
    public static class PstnEventsHandler
    {
        public static async Task ProcessEvent(IncomingCallEvent ev, UrlHelper url, HttpContextBase context)
        {
            Trace.WriteLine("IncomingCallEvent", "Events");
            var callbackUrl = string.Format("http://{0}{1}", Common.Domain, url.Action("Bridged", "Events"));
            await Call.Create(string.Format("{0}@{1}.bwapp.bwsip.io", Common.UserName, Common.DomainName), Common.Caller, callbackUrl, ev.CallId);
        }
    }

}