using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Bandwidth.Net;
using Bandwidth.Net.Model;

namespace ChaosConference.Lib
{
    public static class FirstMemberEventsHandler
    {
        public async static Task ProcessEvent(AnswerEvent ev, UrlHelper url, HttpContextBase context)
        {
            var call = await Call.Get(ev.CallId);
            await call.SpeakSentence("Hello! You will be first member of conference");
        }

        public async static Task ProcessEvent(SpeakEvent ev, UrlHelper url, HttpContextBase context)
        {
            if (ev.Status != "done") return;
            var call = await Call.Get(ev.CallId);
            var conferenceUrl = string.Format("http://{0]{1}", url.Action("Conference", "Events"));
            var conference = await Conference.Create(new Dictionary<string, object>
            {
                {"from", Common.ConferenceNumber},
                {"callbackUrl", conferenceUrl}
            });
            await conference.CreateMember(new Dictionary<string, object>
            {
                {"callId", call.Id},
                {"joinTone", true},
                {"leavingTone", true}
            });
        }

        public static Task ProcessEvent(BaseEvent ev, UrlHelper url, HttpContextBase context)
        {
            Trace.WriteLine(string.Format("Unhandled event of type '{0}' for {1} {2}", ev.EventType, context.Request.HttpMethod, context.Request.RawUrl), "Events");
            return Task.Run(() => { });
        }
    }

    public static class CallEventsHandler
    {
        public static async Task ProcessEvent(AnswerEvent ev, UrlHelper url, HttpContextBase context)
        {
            var call = await Call.Get(ev.CallId);
            var conferenceId = context.Application.Get(string.Format("active-conf-{0}", ev.To)) as string;
            if (conferenceId != null)
            {
                await
                    call.SpeakSentence("You will be join to conference.", string.Format("conference:{0}", conferenceId));
            }
            else
            {
                await
                    call.SpeakSentence("We are sorry, the conference is not active.", "terminating");
            }
        }

        public async static Task ProcessEvent(SpeakEvent ev, UrlHelper url, HttpContextBase context)
        {
            if (ev.Status != "done") return;
            var call = await Call.Get(ev.CallId);
            if (ev.Tag == "terminating")
            {
                await call.HangUp();
            }
            else
            {
                var conferenceId = ev.Tag.Split(':').LastOrDefault();
                var conference = await Conference.Get(conferenceId);
                await conference.CreateMember(new Dictionary<string, object>
                {
                    {"callId", call.Id},
                    {"joinTone", true}
                });
            }
        }
        public static Task ProcessEvent(BaseEvent ev, UrlHelper url, HttpContextBase context)
        {
            Trace.WriteLine(string.Format("Unhandled event of type '{0}' for {1} {2}", ev.EventType, context.Request.HttpMethod, context.Request.RawUrl), "Events");
            return Task.Run(() => { });
        }
    }

    public static class ConferenceEventsHandler
    {
        public static async Task ProcessEvent(ConferenceEvent ev, UrlHelper url, HttpContextBase context)
        {
            var conference = await Conference.Get(ev.ConferenceId);
            if (ev.Status == "created")
            {
                context.Application.Set(string.Format("active-conf-{0}", conference.From), conference.Id);
            }
            else
            {
                context.Application.Remove(string.Format("active-conf-{0}", conference.From));
            }
        }
        public static Task ProcessEvent(BaseEvent ev, UrlHelper url, HttpContextBase context)
        {
            Trace.WriteLine(string.Format("Unhandled event of type '{0}' for {1} {2}", ev.EventType, context.Request.HttpMethod, context.Request.RawUrl), "Events");
            return Task.Run(() => { });
        }
    }

}