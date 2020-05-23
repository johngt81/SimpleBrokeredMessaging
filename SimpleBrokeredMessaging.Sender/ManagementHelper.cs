using Microsoft.Azure.ServiceBus.Management;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SimpleBrokeredMessaging.Sender
{
    class ManagementHelper
    {
        private ManagementClient managementClient;
        public ManagementHelper(string connectionString)
        {//All management operating
            managementClient = new ManagementClient(connectionString);
        }
    }
}
