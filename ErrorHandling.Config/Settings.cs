using System;
using System.Data.Common;

namespace ErrorHandling.Config
{
    public class Settings
    { 
        public static string ConnectionString= "Endpoint=sb://bg-servicebus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=H5BpZJ8uCl7VdXZxpCm4WH/gCqDtSDWhwyane+ZWPeQ=";
        public static string QueuePath = "errorhandling";
        public static string ForwardingQueuePath = "errorhandingforwarding";
    }
}
