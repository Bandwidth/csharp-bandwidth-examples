using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TranscriptionApp.Startup))]
namespace TranscriptionApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            ConfigureBandwidth(app).Wait();
        }
    }
}
