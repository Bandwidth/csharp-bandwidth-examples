using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Bandwidth.Net.Model;
using Owin;
using TranscriptionApp.Lib;

namespace TranscriptionApp
{
    public partial class Startup
    {
        public const string ApplicationName = "NetTranscriptionApp";
        public async Task ConfigureBandwidth(IAppBuilder app)
        {
            var application = (await Application.List()).FirstOrDefault(a => a.Name == ApplicationName);
            if (application == null)
            {
                application = await Application.Create(new Dictionary<string, object>
                {
                    {"name", ApplicationName},
                    {"incomingCallUrl", ConfigurationManager.AppSettings["baseUrl"] + "/events/externalCall"},
                    {"autoAnswer", false}
                });
            }
            app.Use((t, next) =>
            {
                t.Set("applicationId", application.Id);
                return next();
            });
            app.CreatePerOwinContext(EmailSender.Create);
        }
    }
}