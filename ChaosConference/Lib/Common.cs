using System;
using System.Configuration;

namespace ChaosConference.Lib
{
    public static class Common
    {
        public static readonly string ConferenceNumber = Environment.GetEnvironmentVariable("CONFERENCE_NUMBER") ?? ConfigurationManager.AppSettings["conferenceNumber"];
        public static readonly string Domain = Environment.GetEnvironmentVariable("DOMAIN") ?? ConfigurationManager.AppSettings["domain"];
        public static readonly string  SecondMemberNumber = Environment.GetEnvironmentVariable("SECOND_MEMBER_NUMBER") ?? ConfigurationManager.AppSettings["secondMemberNumber"];
        public static readonly string ReserveConferenceNumber = Environment.GetEnvironmentVariable("RESERVE_CONFERENCE_NUMBER") ?? ConfigurationManager.AppSettings["reserveConferenceNumber"];
        
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