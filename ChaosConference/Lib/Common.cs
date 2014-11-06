using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace ChaosConference.Lib
{
    public static class Common
    {
        public static readonly string ConferenceNumber = Environment.GetEnvironmentVariable("CONFERENCE_NUMBER") ?? ConfigurationManager.AppSettings["conferenceNumber"];
        public static readonly string Domain = Environment.GetEnvironmentVariable("DOMAIN") ?? ConfigurationManager.AppSettings["domain"];
        public static void SetupClient()
        {
            SetValue("BANDWIDTH_USER_ID", "userId");
            SetValue("BANDWIDTH_API_TOKEN", "apiToken");
            SetValue("BANDWIDTH_API_SECRET", "apiSecret");
            SetValue("BANDWIDTH_API_ENDPOINT", "apiEndPoint");
            SetValue("BANDWIDTH_API_VERSION", "apiVersion");
        }

        private static void SetValue(string environmentVariable, string optionName)
        {
            var value = Environment.GetEnvironmentVariable(environmentVariable);
            var option = ConfigurationManager.AppSettings[optionName];
            if (value == null && option != null)
            {
                Environment.SetEnvironmentVariable(environmentVariable, option);
            }
        }
    }
}