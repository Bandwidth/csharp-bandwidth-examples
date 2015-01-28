using System;
using System.Configuration;

namespace SipApp.Lib
{
    public static class Common
    {

        public static readonly string Caller = Environment.GetEnvironmentVariable("CALLER_NUMBER") ?? ConfigurationManager.AppSettings["callerNumber"];
        
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

        public const string ApplicationName = "SipApp Demo";
        public const string DomainName = "sip-app";
        public const string UserName = "test-user";
    }
}