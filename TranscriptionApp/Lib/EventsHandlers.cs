using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Bandwidth.Net;
using Bandwidth.Net.Model;
using Microsoft.AspNet.Identity.Owin;
using TranscriptionApp.Models;

namespace TranscriptionApp.Lib
{
    public static class AdminEventHandler
    {
        public static Task Menu(Call call, string prompt, string tag)
        {
            return call.CreateGather(new Dictionary<string, object>
            {
                {"tag", tag},
                {"maxDigits", 1},
                {
                    "prompt", new Dictionary<string, object>
                    {
                        {"locale", "en_US"},
                        {"gender", "female"},
                        {"sentence", prompt},
                        {"voice", "kate"},
                        {"bargeable", true},
                        {"loopEnabled", false}
                    }
                }
            });
        }

        public static Task MainMenu(Call call)
        {
            return Menu(call, "Press 1 to listen to your voicemail. Press 2 to record a new greeting.", "main-menu");
        }

        public static Task VoiceMailMenu(Call call)
        {
            return Menu(call,
                "Press 1 to record a greeting.  Press 2 to listen to the greeting. Press 3 to use the default greeting. Press 0 to go back.",
                "voice-mail-menu");
        }

        public static Task VoiceMessageMenu(Call call, int index)
        {
            return Menu(call,
                "Press 1 to go to the next voice mail. Press 2 to delete this voice mail and go to the next one." +
                "Press 3 to repeat this voice mail again. Press 0 to go back to main menu.",
                "voice-message-menu:" + index);
        }

        public static Task PlayVoiceMailMessage(ApplicationUser user, Call call, int index)
        {
            var message = user.VoiceMessages[index];
            if (message == null)
            {
                return call.SpeakSentence("You have no voice messages", "main-menu");
            }
            return call.SpeakSentence(
                message.StartTime.ToLongDateString() + " " + message.StartTime.ToShortTimeString(),
                "voice-message-date:" + index);
        }

        public static async Task ProcessEvent(AnswerEvent ev, UrlHelper url, HttpContextBase context, dynamic viewBag)
        {
            var call = (Call) viewBag.Call;
            var dbContext = context.GetOwinContext().Get<ApplicationDbContext>();
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.PhoneNumber == ev.From);
            if (user == null)
            {
                await call.HangUp();
                return;
            }
            var activeCall = new ActiveCall
            {
                Id = ev.CallId,
                User = user
            };
            dbContext.ActiveCalls.Add(activeCall);
            await dbContext.SaveChangesAsync();
            RunAsyncWithDelay(TimeSpan.FromSeconds(2), async () => await MainMenu(call));
        }

        public static async Task ProcessEvent(HangupEvent ev, UrlHelper url, HttpContextBase context, dynamic viewBag)
        {
            var dbContext = context.GetOwinContext().Get<ApplicationDbContext>();
            var activeCall = dbContext.ActiveCalls.Find(ev.CallId);
            if (activeCall != null)
            {
                dbContext.ActiveCalls.Remove(activeCall);
                await dbContext.SaveChangesAsync();
            }
        }

        public static async Task ProcessEvent(GatherEvent ev, UrlHelper url, HttpContextBase context, dynamic viewBag)
        {
            var call = (Call) viewBag.Call;
            var user = (ApplicationUser) viewBag.User;
            var dbContext = context.GetOwinContext().Get<ApplicationDbContext>();
            if (ev.State != "completed") return;
            var tags = ev.Tag.Split(':');
            var tag = tags.FirstOrDefault();
            if (string.IsNullOrEmpty(tag)) return;
            switch (tag)
            {
                case "recording":
                    await call.RecordingOff();
                    await call.SpeakSentence("Your greeting has been recorded. Thank you.", "stop-recording");
                    break;
                case "main-menu":
                    switch (ev.Digits)
                    {
                        case "1":
                            await PlayVoiceMailMessage(user, call, user.VoiceMessages.Count - 1);
                            break;
                        case "2":
                            await VoiceMailMenu(call);
                            break;
                        default:
                            await MainMenu(call);
                            break;
                    }
                    break;
                case "voice-mail-menu":
                    switch (ev.Digits)
                    {
                        case "0":
                            await MainMenu(call);
                            break;
                        case "1":
                            await
                                call.SpeakSentence("Say your greeting now. Press # to stop recording.",
                                    "start-recording");
                            break;
                        case "2":
                            await call.SpeakSentence("Your greating", "listen-to-recording");
                            break;
                        case "3":
                            await call.SpeakSentence("Your greating will be set to default", "remove-recording");
                            break;
                        default:
                            await VoiceMailMenu(call);
                            break;
                    }
                    break;
                case "voice-message-menu":
                    var index = int.Parse(tags[1]);
                    switch (ev.Digits)
                    {
                        case "0":
                            await MainMenu(call);
                            break;
                        case "1":
                            await PlayVoiceMailMessage(user, call, index - 1);
                            break;
                        case "2":
                            dbContext.VoiceMessages.Remove(user.VoiceMessages[index]);
                            await dbContext.SaveChangesAsync();
                            await PlayVoiceMailMessage(user, call, index - 1);
                            break;
                        case "3":
                            await PlayVoiceMailMessage(user, call, index);
                            break;
                        default:
                            await VoiceMessageMenu(call, index);
                            break;
                    }
                    break;
            }
        }

        public static async Task ProcessEvent(SpeakEvent ev, UrlHelper url, HttpContextBase context, dynamic viewBag)
        {
            if (ev.Status != "done") return;
            await ProcessPlaybackEvent(context, viewBag, ev.Tag);
        }

        public static async Task ProcessEvent(PlaybackEvent ev, UrlHelper url, HttpContextBase context, dynamic viewBag)
        {
            if (ev.Status != "done") return;
            await ProcessPlaybackEvent(context, viewBag, ev.Tag);
        }

        public static async Task ProcessEvent(RecordingEvent ev, UrlHelper url, HttpContextBase context, dynamic viewBag)
        {
            if (ev.State != RecordingState.Complete) return;
            var user = (ApplicationUser) viewBag.User;
            var dbContext = context.GetOwinContext().Get<ApplicationDbContext>();
            var recording = await Recording.Get(ev.RecordingId);
            user.Greeting = recording.Media;
            await dbContext.SaveChangesAsync();
        }

        public static Task ProcessEvent(BaseEvent ev, UrlHelper url, HttpContextBase context, dynamic viewBag)
        {
            Trace.WriteLine(
                string.Format("Unhandled event of type '{0}' for {1} {2}", ev.EventType, context.Request.HttpMethod,
                    context.Request.RawUrl), "Events");
            return Task.Run(() => { });
        }

        private static async Task ProcessPlaybackEvent(HttpContextBase context, dynamic viewBag, string tag)
        {
            var tags = tag.Split(':');
            var call = (Call) viewBag.Call;
            var user = (ApplicationUser) viewBag.User;
            var dbContext = context.GetOwinContext().Get<ApplicationDbContext>();

            switch (tags[0])
            {
                case "start-recording":
                    await call.RecordingOn();
                    await call.CreateGather(new Dictionary<string, object>
                    {
                        {"tag", "recording"},
                        {"interDigitTimeout", 30},
                        {"maxDigits", 30},
                        {"terminatingDigits", "#"}
                    });
                    break;
                case "listen-to-recording":
                    await user.PlayGreeting(call);
                    break;
                case "remove-recording":
                    user.Greeting = null;
                    await dbContext.SaveChangesAsync();
                    break;
                case "stop-recording":
                    await VoiceMailMenu(call);
                    break;
                case "main-menu":
                    await MainMenu(call);
                    break;
                case "voice-message-date":
                    var index = int.Parse(tags[1]);
                    await call.PlayAudio(new Dictionary<string, object>
                    {
                        {"fileUrl", user.VoiceMessages[index].Url},
                        {"tag", "voice-message-url:" + index}
                    });
                    break;
                case "voice-message-url":
                    index = int.Parse(tags[1]);
                    RunAsyncWithDelay(TimeSpan.FromSeconds(1.5), async () => await VoiceMessageMenu(call, index));
                    break;
            }
        }

        public static void RunAsyncWithDelay(TimeSpan delay, Func<Task> action)
        {
            Task.Delay(delay).ContinueWith(t => action().Wait());
        }
    }

    public static class ExternalCallHandler
    {
        public static async Task ProcessEvent(IncomingCallEvent ev, UrlHelper url, HttpContextBase context,
            dynamic viewBag)
        {
            var dbContext = context.GetOwinContext().Get<ApplicationDbContext>();
            var call = (Call) viewBag.Call;
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.PhoneNumber == ev.To);
            if (user == null)
            {
                await call.RejectIncoming();
                return;
            }
            var activeCall = new ActiveCall
            {
                Id = ev.CallId,
                User = user
            };
            dbContext.ActiveCalls.Add(activeCall);
            await dbContext.SaveChangesAsync();
            await call.AnswerOnIncoming();
        }

        public static async Task ProcessEvent(AnswerEvent ev, UrlHelper url, HttpContextBase context, dynamic viewBag)
        {
            var user = (ApplicationUser) viewBag.User;
            var call = (Call) viewBag.Call;
            if (user == null)
            {
                await call.HangUp();
                return;
            }
            await user.PlayGreeting(call);
        }

        public static Task ProcessEvent(HangupEvent ev, UrlHelper url, HttpContextBase context, dynamic viewBag)
        {
            AdminEventHandler.RunAsyncWithDelay(TimeSpan.FromMinutes(15), async () =>
            {
                using (var dbContext = new ApplicationDbContext())
                {
                    var activeCall = dbContext.ActiveCalls.Find(ev.CallId);
                    if (activeCall != null)
                    {
                        dbContext.ActiveCalls.Remove(activeCall);
                        await dbContext.SaveChangesAsync();
                    }
                }
            });
            return Task.FromResult(0);
        }

        public static async Task ProcessEvent(SpeakEvent ev, UrlHelper url, HttpContextBase context, dynamic viewBag)
        {
            if (ev.Status != "done") return;
            await ProcessPlaybackEvent(viewBag, ev.Tag);
        }

        public static async Task ProcessEvent(PlaybackEvent ev, UrlHelper url, HttpContextBase context, dynamic viewBag)
        {
            if (ev.Status != "done") return;
            await ProcessPlaybackEvent(viewBag, ev.Tag);
        }

        public static async Task ProcessEvent(GatherEvent ev, UrlHelper url, HttpContextBase context, dynamic viewBag)
        {
            if (ev.State != "completed") return;
            var call = (Call)viewBag.Call;
            
              //make hangup on press any key
              if(ev.Tag == "stop-recording"){
                await call.HangUp();
              }
        }

        public static async Task ProcessEvent(TranscriptionEvent ev, UrlHelper url, HttpContextBase context, dynamic viewBag)
        {
            if (ev.State != "completed") return;
            //call was recorded and transcription was completed here
            var recording = await Recording.Get(ev.RecordingId);
            var index = recording.Call.LastIndexOf('/');
            var callId = recording.Call.Substring(index + 1);
            var call = await Call.Get(callId);
            var dbContext = context.GetOwinContext().Get<ApplicationDbContext>();
            var activeCall = dbContext.ActiveCalls.Find(callId);
            if (activeCall == null)
            {
                throw new Exception("Missing active call with id " + callId);
            }
            var user = activeCall.User;
            var from = call.From;

            //add new voice message to user
            dbContext.VoiceMessages.Add(new VoiceMessage
            {
                Url = recording.Media,
                StartTime = recording.StartTime,
                EndTime = recording.EndTime,
                User = user
            });
            await dbContext.SaveChangesAsync();

            //and send email notification
            await context.GetOwinContext().Get<EmailSender>().SendMessage(user.Email, "TranscriptionApp - new voice message from " + from,
                string.Format("<p>You received a new voice message from <b>{0}</b> at {1} {2}:</p>" 
                + "<p><pre>{3}</pre></p>", @from, recording.StartTime.ToLongDateString(), recording.StartTime.ToShortTimeString(), ev.Text)
            );
            
        }

        public static Task ProcessEvent(BaseEvent ev, UrlHelper url, HttpContextBase context, dynamic viewBag)
        {
            Trace.WriteLine(
                string.Format("Unhandled event of type '{0}' for {1} {2}", ev.EventType, context.Request.HttpMethod,
                    context.Request.RawUrl), "Events");
            return Task.Run(() => { });
        }

        private static async Task ProcessPlaybackEvent(dynamic viewBag, string tag)
        {
            var call = (Call) viewBag.Call;
            switch (tag)
            {
                case "greeting":
                    //play beep before recording
                    await call.PlayAudio(new Dictionary<string, object>
                    {
                        {"fileUrl", ConfigurationManager.AppSettings["baseUrl"] + "/Content/beep.mp3"},
                        {"tag", "start-recording"}
                    });
                    break;
                case "start-recording":
                    //start recording of call after 'beep' (with transcription of result)
                    await
                        call.Update(new Dictionary<string, object>
                        {
                            {"transcriptionEnabled", true},
                            {"recordingEnabled", true}
                        });
                    //press any key to stop recording (and call too)
                    await call.CreateGather(new Dictionary<string, object>
                    {
                        {"tag", "stop-recording"},
                        {"interDigitTimeout", 30},
                        {"maxDigits", 1}
                    });
                    break;
            }
        }
    }
}