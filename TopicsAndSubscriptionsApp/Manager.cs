using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace TopicsAndSubscriptionsApp
{
    internal class Manager
    {
        private ManagementClient ManagementClient;

        public Manager(string serviceBusConnectionString)
        {
            ManagementClient = new ManagementClient(serviceBusConnectionString);
        }

        public async Task CreateTopic(string topicPath)
        {
            Console.WriteLine($"Creating Topic {topicPath}");
            if (await ManagementClient.TopicExistsAsync(topicPath))
            {
                await ManagementClient.DeleteTopicAsync(topicPath);
            }
            await ManagementClient.CreateTopicAsync(topicPath);
        }

        public async Task CreateSubscription(string topicPath, string subscriptionName)
        {
            Console.WriteLine($"Creating Subscription {topicPath}/{subscriptionName}");
            await ManagementClient.CreateSubscriptionAsync(topicPath, subscriptionName);
        }

        public async Task<SubscriptionDescription> CreateSubscriptionWithSqlFilter(string topicPath, string subscriptionName, string sqlExpression)
        {
            Console.WriteLine($"Creating Subscription with SQL Filter {topicPath}/{subscriptionName} ({sqlExpression})");

            var subscriptionDescription = new SubscriptionDescription(topicPath, subscriptionName);
            var ruleDescription = new RuleDescription("Default", new SqlFilter(sqlExpression));
            return await ManagementClient.CreateSubscriptionAsync(subscriptionDescription, ruleDescription);
        }

        public async Task<SubscriptionDescription> CreateSubscriptionWithCorrelationFilter(string ordersTopicPath, string subscriptionName, string correlationId)
        {
            Console.WriteLine($"Creating Subscription with Correlation Filter {ordersTopicPath}/{subscriptionName} ({correlationId})");

            var subscriptionDescription = new SubscriptionDescription(ordersTopicPath, subscriptionName);
            var ruleDescription = new RuleDescription("Default", new CorrelationFilter(correlationId));
            return await ManagementClient.CreateSubscriptionAsync(subscriptionDescription, ruleDescription);
        }

        public async Task<IEnumerable<SubscriptionDescription>> GetSubscriptionsForTopic(string ordersTopicPath)
        {
            Console.WriteLine($"Getting subscriptions for topic {ordersTopicPath}");

            return await ManagementClient.GetSubscriptionsAsync(ordersTopicPath);
        }
    }
}