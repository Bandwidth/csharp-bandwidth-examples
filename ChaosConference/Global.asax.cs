using System;
using System.Diagnostics;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ChaosConference.Lib;

namespace ChaosConference
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes); 
            Common.SetupClient();
            //Trace.Listeners.Add(new IisTraceListener());
            Trace.Listeners.Add(new ConsoleTraceListener());
        }
    }
}