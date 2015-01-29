using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Bandwidth.Net;
using Bandwidth.Net.Model;

namespace SipApp.Lib
{
    // Handle incoming calls to sip client directly
    public static class DemoEventsHandler
    {
        public static async Task ProcessEvent(AnswerEvent ev, UrlHelper url, HttpContextBase context)
        {
            Trace.WriteLine("AnswerEvent", "Events");
            var call = new Call {Id = ev.CallId};
            await call.SpeakSentence("Hello SIP client");
        }

        public static async Task ProcessEvent(SpeakEvent ev, UrlHelper url, HttpContextBase context)
        {
            Trace.WriteLine("SpeakEvent", "Events");
            if (ev.Status != "done") return;
            var call = new Call { Id = ev.CallId };
            await call.HangUp();
        }
        public static Task ProcessEvent(BaseEvent ev, UrlHelper url, HttpContextBase context)
        {
            Trace.WriteLine(string.Format("Unhandled event of type '{0}' for {1} {2}", ev.EventType, context.Request.HttpMethod, context.Request.RawUrl), "Events");
            return Task.Run(() => { });
        }
    }

    /// <summary>
    /// Handle all calls of the app
    /// </summary>
    public static class CallsEventsHandler
    {
        public static async Task ProcessEvent(IncomingCallEvent ev, UrlHelper url, HttpContextBase context)
        {
            Trace.WriteLine("IncomingCallEvent", "Events");
            var call = new Call { Id = ev.CallId };
            var callbackUrl = string.Format("http://{0}{1}", Common.Domain, url.Action("Bridged", "Events"));
                
            if (ev.From == Common.SipUri)
            {
                await call.AnswerOnIncoming();
                await Call.Create(ev.To, Common.Caller, callbackUrl, ev.CallId);
                return;
            }
            if (ev.To == Common.PhoneNumberForIncomingCalls)
            {
                await call.AnswerOnIncoming();
                await Call.Create(Common.SipUri, Common.Caller, callbackUrl, ev.CallId);
            }
        }

        public static Task ProcessEvent(BaseEvent ev, UrlHelper url, HttpContextBase context)
        {
            Trace.WriteLine(string.Format("Unhandled event of type '{0}' for {1} {2}", ev.EventType, context.Request.HttpMethod, context.Request.RawUrl), "Events");
            return Task.Run(() => { });
        }
    }

    /// <summary>
    /// Handle call which will be bridged with current call
    /// </summary>
    public static class BridgedEventsHandler
    {
        public static async Task ProcessEvent(AnswerEvent ev, UrlHelper url, HttpContextBase context)
        {
            Trace.WriteLine("AnswerEvent", "Events");
            await Bridge.Create(new Dictionary<string, object> { { "callIds", new[] { ev.CallId, ev.Tag } } });
        }
        public static Task ProcessEvent(BaseEvent ev, UrlHelper url, HttpContextBase context)
        {
            Trace.WriteLine(string.Format("Unhandled event of type '{0}' for {1} {2}", ev.EventType, context.Request.HttpMethod, context.Request.RawUrl), "Events");
            return Task.Run(() => { });
        }
    }
}