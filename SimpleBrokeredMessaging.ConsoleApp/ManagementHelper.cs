using Microsoft.Azure.ServiceBus.Management;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBrokeredMessaging.ConsoleApp
{
    class ManagementHelper
    {
        private ManagementClient managementClient;
        public ManagementHelper(string connectionString)
        {
            managementClient = new ManagementClient(connectionString);
        }

        public async Task ListQueuesAsync()
        {
            Console.Write($"Listing queues ...");
            IEnumerable<QueueDescription> queueDescriptions = await managementClient.GetQueuesAsync();
            foreach (var queueDescription in queueDescriptions)
            {
                Console.WriteLine("\t{0}", queueDescription.Path);
            }
            Console.WriteLine("Done!");
        }

        public async Task CreateQueueAsync(string queuePath)
        {
            Console.Write($"Creating queue {queuePath}...");
            QueueDescription description = GetQueueDescription(queuePath);
            await managementClient.CreateQueueAsync(description);
            Console.WriteLine("Done!");
        }

        public async Task DeleteQueueAsync(string queuePath)
        {
            Console.Write($"Deleting queue {queuePath}...");
            await managementClient.DeleteQueueAsync(queuePath);
            Console.WriteLine("Done!");
        }

        private QueueDescription GetQueueDescription(string queuePath)
        {
            return new QueueDescription(queuePath)
            {
                RequiresDuplicateDetection = true,
                RequiresSession = true,
                MaxDeliveryCount = 20
            };
        }

        public async Task ListTopicsAndSubscriptionsAsync()
        {
            IEnumerable<TopicDescription> topicDescriptions = await managementClient.GetTopicsAsync();
            Console.WriteLine("Listing topics and subscriptions");
            foreach (TopicDescription topicDescription in topicDescriptions)
            {
                IEnumerable<SubscriptionDescription> subscriptionDescriptions = await managementClient.GetSubscriptionsAsync(topicDescription.Path);
                foreach (SubscriptionDescription subscriptionDescription in subscriptionDescriptions)
                {
                    Console.WriteLine($"\t\t{subscriptionDescription.SubscriptionName}");
                }
            }
            Console.WriteLine("Done!");
        }

        public async Task CreateTopicAsync(string topicPath)
        {
            Console.Write($"Creating topic ...{topicPath}");
            await managementClient.CreateTopicAsync(topicPath);
            Console.WriteLine("Done!");
        }

        public async Task CreateSubscriptionAsync(string topicPath, string subscriptionName)
        {
            Console.Write($"Creating subscription {topicPath}/subscriptions/{subscriptionName}...");
            await managementClient.CreateSubscriptionAsync(topicPath, subscriptionName);
            Console.WriteLine("Done!");
        }

        public async Task DeleteTopicAsync(string topicPath)
        {
            Console.Write($"Deleting topic ...{topicPath}");
            await managementClient.DeleteTopicAsync(topicPath);
            Console.WriteLine("Done!");
        }

        public async Task DeleteSubscriptionAsync(string topicPath, string subscriptionName)
        {
            Console.Write($"Deleting subscription {subscriptionName} in topic {topicPath} ");
            await managementClient.DeleteSubscriptionAsync(topicPath, subscriptionName);
            Console.WriteLine("Done!");
        }
    }
}
