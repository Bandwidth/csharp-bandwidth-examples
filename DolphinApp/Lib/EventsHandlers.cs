using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Bandwidth.Net;
using Bandwidth.Net.Model;

namespace DolphinApp.Lib
{
    public static class DemoEventsHandler
    {
        public async static Task ProcessEvent(AnswerEvent ev, UrlHelper url, HttpContextBase context)
        {
            Trace.WriteLine("AnswerEvent", "Events");
            var call = new Call{Id = ev.CallId};
            Thread.Sleep(3000);
            await call.SpeakSentence("hello flipper", "hello-state");
        }
        public async static Task ProcessEvent(SpeakEvent ev, UrlHelper url, HttpContextBase context)
        {
            if (ev.Status != "done") return;
            
            Trace.WriteLine("SpeakEvent", "Events");
            
            var call = new Call{Id = ev.CallId};
            switch (ev.Tag)
            {
                case "gather_complete":
                    await Call.Create(new Dictionary<string, object>
                    {
                        {"from", Common.Caller},
                        {"to", Common.BridgeCallee},
                        {"callbackUrl", string.Format("http://{0}{1}", Common.Domain, url.Action("Bridged", "Events"))},
                        {"tag", string.Format("other-leg:{0}", call.Id)}
                    });
                    break;
                case "terminating":
                    await call.HangUp();
                    break;
                case "hello-state":
                    await call.PlayAudio(new Dictionary<string, object>
                    {
                        {"fileUrl", string.Format("http://{0}/content/dolphin.mp3", Common.Domain)},
                        {"tag", "dolphin-state"}
                    });
                    break;
            }
        }

        public static async Task ProcessEvent(DtmfEvent ev, UrlHelper url, HttpContextBase context)
        {
            Trace.WriteLine(string.Format("DtmfEvent {0}", ev.DtmfDigit), "Events");
            var call = new Call { Id = ev.CallId };
            if (ev.DtmfDigit.StartsWith("1"))
            {
                await call.SpeakSentence("Please stay on the line. Your call is being connected.", "gather_complete");
            }
            else
            {
                await call.SpeakSentence("This call will be terminated", "terminating");
            }
        }

        public static async Task ProcessEvent(PlaybackEvent ev, UrlHelper url, HttpContextBase context)
        {
            if (ev.Status != "done") return;
            Trace.WriteLine("PlaybackEvent", "Events");
            var call = new Call{Id = ev.CallId};
            if (ev.Tag == "dolphin-state")
            {
                await call.CreateGather(new Dictionary<string, object>
                {
                    {"maxDigits", "5"},
                    {"terminatingDigits", "*"},
                    {"interDigitTimeout", "3"},
                    {"prompt", new Dictionary<string, object>{
                        {"sentence", "Press 1 to speak with the fish, press 2 to let it go"},
                        {"loopEnabled", false},
                        {"voice", "Kate"}
                    }},
                    {"tag", "gather_started"}
                });
            }
        }

        


        public static Task ProcessEvent(BaseEvent ev, UrlHelper url, HttpContextBase context)
        {
            Trace.WriteLine(string.Format("Unhandled event of type '{0}' for {1} {2}", ev.EventType, context.Request.HttpMethod, context.Request.RawUrl), "Events");
            return Task.Run(() => { });
        }
    }

    public static class BridgedEventsHandler
    {
        public static async Task ProcessEvent(AnswerEvent ev, UrlHelper url, HttpContextBase context)
        {
            Trace.WriteLine("Bridged-AnswerEvent", "Events");
            var call = new Call{Id = ev.CallId};
            var otherCallId = ev.Tag.Split(':').LastOrDefault();
            Thread.Sleep(3000);
            await
                call.SpeakSentence("You have a dolphin on line 1. Watch out, he's hungry!",
                    string.Format("warning:{0}", otherCallId));
        }

        public async static Task ProcessEvent(SpeakEvent ev, UrlHelper url, HttpContextBase context)
        {
            if (ev.Status != "done") return;
            Trace.WriteLine("Bridged-SpeakEvent", "Events");
            if (ev.Tag.StartsWith("warning"))
            {
                var otherCallId = ev.Tag.Split(':').LastOrDefault();
                await Bridge.Create(new Dictionary<string, object>
                {
                    {"callIds", new[]{otherCallId}}
                });
            }
        }

        public async static Task ProcessEvent(HangupEvent ev, UrlHelper url, HttpContextBase context)
        {
            Trace.WriteLine("Bridged-HangupEvent", "Events");
            var otherCallId = ev.Tag.Split(':').LastOrDefault();
            var call = new Call{Id = otherCallId};
            if (ev.Cause == "CALL_REJECTED")
            {
                await call.SpeakSentence("We are sorry, the user is reject your call", "terminating");
            }
            else
            {
                await call.HangUp();
            }
        }

        public static Task ProcessEvent(BaseEvent ev, UrlHelper url, HttpContextBase context)
        {
            Trace.WriteLine(string.Format("Unhandled event of type '{0}' for {1} {2}", ev.EventType, context.Request.HttpMethod, context.Request.RawUrl), "Events");
            return Task.Run(() => { });
        }
    }
}