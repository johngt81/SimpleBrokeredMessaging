using System;

namespace RequestResponseMessaging.Configuration
{
    public class AccountDetails
    {
        public static string ConnectionString = "";
        public static string RequestQueueName = "requestqueue";
        public static string ResponseQueueName = "responsequeue";
    }
}
