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
            var call = new Call{Id = ev.CallId};
            await call.SpeakSentence("Welcome to the conference");
        }

        public async static Task ProcessEvent(SpeakEvent ev, UrlHelper url, HttpContextBase context)
        {
            if (ev.Status != "done" || ev.Tag == "notification") return;
            var call = new Call{Id = ev.CallId};
            var conferenceUrl = string.Format("http://{0}{1}", Common.Domain, url.Action("Conference", "Events"));
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

    public static class OtherMembersEventsHandler
    {
        public static async Task ProcessEvent(AnswerEvent ev, UrlHelper url, HttpContextBase context)
        {
            var call = new Call{Id = ev.CallId};
            var conferenceId = context.Application.Get(string.Format("active-conf-{0}", ev.Tag)) as string;
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
            var call = new Call{Id = ev.CallId};
            if (ev.Tag == "terminating")
            {
                await call.HangUp();
            }
            else
            {
                if (ev.Tag == "notification")
                {
                    return;
                }
                var conferenceId = ev.Tag.Split(':').LastOrDefault();
                var conference = new Conference{Id = conferenceId};
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

        public static async Task ProcessEvent(ConferenceMemberEvent ev, UrlHelper url, HttpContextBase context)
        {
            if (ev.State != "active" || ev.ActiveMembers < 2) return; //don't speak anything to conference owner (first member)
            var conference = new Conference{Id = ev.ConferenceId};
            var member = await conference.GetMember(ev.MemberId);
            await member.PlayAudio(new Dictionary<string, object>
            {
                {"gender", "female"},
                {"locale", "en_US"},
                {"voice", "kate"},
                {"sentence", string.Format("You are the {0} caller to join the conference.", ToOrdinalNumber(ev.ActiveMembers))},
                {"tag", "notification"}
            });
        }

        private static string ToOrdinalNumber(int count)
        {
            if (count >= Ordinals.Length)
            {
                return string.Format("{0}th", count);
            }
            return Ordinals[count];
        }
        private static readonly string[] Ordinals = {"", "first", "second", "third", "fourth", "fifth"};

        public static Task ProcessEvent(BaseEvent ev, UrlHelper url, HttpContextBase context)
        {
            Trace.WriteLine(string.Format("Unhandled event of type '{0}' for {1} {2}", ev.EventType, context.Request.HttpMethod, context.Request.RawUrl), "Events");
            return Task.Run(() => { });
        }
    }

}