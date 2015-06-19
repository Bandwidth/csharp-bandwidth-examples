using System.Configuration;
using System.Runtime.Remoting.Channels;
using Bandwidth.Net;

namespace ConferenceWithAgentApp
{
    using System;
    using Nancy.Hosting.Self;

    class Program
    {
        static void Main(string[] args)
        {
            
            var uri = new Uri("http://localhost:9876/");

            using (var host = new NancyHost(uri))
            {
                host.Start();

                Console.WriteLine("Your application is running on " + ConfigurationManager.AppSettings.Get("baseUrl"));
                Console.WriteLine("Press any [Enter] to close the host.");
                Console.ReadLine();
            }
        }
    }
}
