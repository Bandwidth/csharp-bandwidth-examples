using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Bandwidth.Net;
using Bandwidth.Net.Model;
using Nancy;
using Nancy.TinyIoc;

namespace ConferenceWithAgentApp
{
    public class CallbackModule : NancyModule
    {
        public CallbackModule(TinyIoCContainer container)
        {
            var phoneNumber = container.Resolve<string>("phoneNumber");

            //home page (to show phone number to call)
            Get["/"] = p => string.Format("Call to {0}", phoneNumber);
            
            //catapult callback handler
            Post["/callback", true] = async (p, c) =>
            {
                using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
                {
                    try
                    {
                        dynamic ev = BaseEvent.CreateFromString(await reader.ReadToEndAsync());
                        Trace.WriteLine("Came event " + ((BaseEvent)ev).EventType, "Callback");
                        await ProcessEvent(ev, container);
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(string.Format("{0} - {1}", Request.Url, ex.Message), "CallbackError");
                    }
                    return HttpStatusCode.OK;
                }
            };
        }

        private async Task ProcessEvent(IncomingCallEvent ev, TinyIoCContainer container)
        {
            var baseUrl = container.Resolve<string>("baseUrl"); 
            var phoneNumber = container.Resolve<string>("phoneNumber");
            var agentPhoneNumber = container.Resolve<string>("agentPhoneNumber");
            if (ev.To == phoneNumber)
            {
                //somebody called to application phone number
                var callbackUrl = baseUrl + "/callback";

                //create a conference
                var conference = await Conference.Create(new Dictionary<string, object>
                {
                    {"from", phoneNumber},
                    {"callbackUrl", callbackUrl}
                });

                //create a call to "agent" with enabled recording
                await Call.Create(new Dictionary<string, object>
                {
                    {"recordingEnabled", true},
                    {"to", agentPhoneNumber},
                    {"from", phoneNumber},
                    {"callbackUrl", callbackUrl},
                    {"tag", string.Format("{0}:{1}", ev.CallId, conference.Id)}
                });
            }
        }

        private async Task ProcessEvent(AnswerEvent ev, TinyIoCContainer container)
        {
            var phoneNumber = container.Resolve<string>("phoneNumber");
            var agentPhoneNumber = container.Resolve<string>("agentPhoneNumber");
            if (ev.From == phoneNumber && ev.To == agentPhoneNumber && !string.IsNullOrEmpty(ev.Tag))
            {
                // "agent" answered call
                var ids = ev.Tag.Split(':');
                var incomingCallId = ids[0];
                var conferenceId = ids[1];
                var conference = await Conference.Get(conferenceId);
                var incomingCall = await Call.Get(incomingCallId);
                
                //join it to the conference
                await conference.CreateMember(new Dictionary<string, object>
                {
                    {"callId", ev.CallId}
                });
                
                //answer incoming call (to the app number)
                await incomingCall.AnswerOnIncoming();

                //and join this call to the conference too
                await conference.CreateMember(new Dictionary<string, object>
                {
                    {"callId", incomingCallId}
                });
            }
        }


        private Task ProcessEvent(BaseEvent ev, TinyIoCContainer container)
        {
            //do nothing for other events
            return Task.FromResult(0);
        }
    }
}