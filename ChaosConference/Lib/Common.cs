using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChaosConference.Lib
{
    public static class Common
    {
        public static readonly string ConferenceNumber = Environment.GetEnvironmentVariable("CONFERENCE_NUMBER");
        public static readonly string Domain = Environment.GetEnvironmentVariable("DOMAIN");
    }
}