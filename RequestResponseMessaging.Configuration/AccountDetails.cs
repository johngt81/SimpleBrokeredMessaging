using System;

namespace RequestResponseMessaging.Configuration
{
    public class AccountDetails
    {
        public static string ConnectionString = "Endpoint=sb://bg-servicebus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=H5BpZJ8uCl7VdXZxpCm4WH/gCqDtSDWhwyane+ZWPeQ=";
        public static string RequestQueueName = "requestqueue";
        public static string ResponseQueueName = "responsequeue";
    }
}
