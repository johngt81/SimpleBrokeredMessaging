using System;
using System.Data.Common;

namespace ErrorHandling.Config
{
    public class Settings
    { 
        public static string ConnectionString= "";
        public static string QueuePath = "errorhandling";
        public static string ForwardingQueuePath = "errorhandingforwarding";
    }
}
