using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bandwidth.Net;
using Bandwidth.Net.Model;
using ConferenceWithAgentApp.Properties;
using Nancy.Bootstrapper;
using Nancy.Extensions;
using Nancy.TinyIoc;

namespace ConferenceWithAgentApp
{
    using Nancy;

    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            Client.GlobalOptions = new ClientOptions
            {
                UserId = ConfigurationManager.AppSettings.Get("userId"),
                ApiToken = ConfigurationManager.AppSettings.Get("apiToken"),
                ApiSecret = ConfigurationManager.AppSettings.Get("apiSecret"),
            };
            var baseUrl = ConfigurationManager.AppSettings.Get("baseUrl");
            container.Register(baseUrl, "baseUrl");
            //create catapult application with right callback (if need)
            var applicationId = GetApplicationId(baseUrl).Result;
            container.Register(applicationId, "applicationId");
            //and reserve a phone number to handle incoming calls (if need)
            container.Register(GetPhoneNumber(applicationId).Result, "phoneNumber");
            container.Register(ConfigurationManager.AppSettings.Get("agentPhoneNumber"), "agentPhoneNumber");
            
            //log request/response data
            pipelines.BeforeRequest += (ctx) =>
            {
                using (var reader = new StreamReader(ctx.Request.Body, Encoding.UTF8))
                {
                    var text = reader.ReadToEnd();
                    ctx.WriteTraceLog(l => l.AppendLine("Request body: " + text));
                    ctx.Request.Body.Position = 0;
                }
                return null;
            };
            pipelines.AfterRequest += (ctx) =>
            {
                using (var stream = new MemoryStream())
                {
                    ctx.Response.Contents(stream);
                    stream.Position = 0;
                    using (var reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        var text = reader.ReadToEnd();
                        ctx.WriteTraceLog(l => l.AppendLine("Response body: " + text));
                    }
                }
            };
            base.ApplicationStartup(container, pipelines);
        }

        private async Task<string> GetApplicationId(string baseUrl)
        {
            var appId = Settings.Default.ApplicationId;
            if (!string.IsNullOrEmpty(appId)) return appId;
            var app = await Application.Create(new Dictionary<string, object>
                {
                    {"name",  "ConferenceAgentApp for .Net"},
                    {"incomingCallUrl", string.Format("{0}/callback", baseUrl)},
                    {"autoAnswer", false}
                });
            Settings.Default.ApplicationId = app.Id;
            Settings.Default.PhoneNumber = null;
            Settings.Default.Save();
            return app.Id;
        }

        private async Task<string> GetPhoneNumber(string applicationId)
        {
            var phoneNumber = Settings.Default.PhoneNumber;
            if (!string.IsNullOrEmpty(phoneNumber)) return phoneNumber;
            phoneNumber = (await AvailableNumber.SearchLocal(new Dictionary<string, object>
            {
                { "state", "NC" }, { "quantity", 1 }
            })).First().Number;

            await PhoneNumber.Create(new Dictionary<string, object>
                {
                    { "number", phoneNumber },
                    { "applicationId", applicationId }
                });
            Settings.Default.PhoneNumber = phoneNumber;
            Settings.Default.Save();
            return phoneNumber;
        }
    }
}