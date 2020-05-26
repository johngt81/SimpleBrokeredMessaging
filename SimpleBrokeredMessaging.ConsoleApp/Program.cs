using System;
using System.Threading.Tasks;

namespace SimpleBrokeredMessaging.ConsoleApp
{
    class Program
    {//bg-servicebus
        private static readonly string serviceBusConnectionString = "";
        static async Task Main(string[] args)
        {
            var managementHelper = new ManagementHelper(serviceBusConnectionString);

            do
            {
                string commandLine = Console.ReadLine();
                string[] commands = commandLine.Split(' ');

                if (commands.Length > 0)
                {
                    switch (commands[0])
                    {
                        case "createqueue":
                        case "cq":
                            if (commands.Length > 1)
                            {
                                managementHelper.CreateQueueAsync(commands[1]).Wait();
                            }
                            else
                            {
                                Console.WriteLine("Queue path not specified");
                            }
                            break;
                        case "listqueues":
                        case "lq":
                            managementHelper.ListQueuesAsync().Wait();
                            break;
                        case "deletequeue":
                        case "dq":
                            managementHelper.DeleteQueueAsync(commands[1]).Wait();
                            break;
                        case "createtopic":
                        case "ct":
                            managementHelper.CreateTopicAsync(commands[1]).Wait();
                            break;
                        case "createsubscription":
                        case "cs":
                            managementHelper.CreateSubscriptionAsync(commands[1], commands[2]).Wait();
                            break;
                        case "listtopicsubscriptions":
                        case "lts":
                            managementHelper.ListTopicsAndSubscriptionsAsync().Wait();
                            break;
                        case "deletetopic":
                        case "dt":
                            managementHelper.DeleteTopicAsync(commands[1]).Wait();
                            break;
                        case "deletesubscription":
                        case "ds":
                            managementHelper.DeleteSubscriptionAsync(commands[1], commands[2]).Wait();
                            break;
                        default:
                            break;
                    }
                }

            } while (true);
        }
    }
}
