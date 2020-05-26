using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace TopicsAndSubscriptionsApp
{
    class Program
    {
        private static string ServiceBusConnectionString = "";
        private static string OrdersTopicPath = "Orders";
        static async Task Main(string[] args)
        {
            PromptAndWait("Topics and Subscriptions Console");

            //PromptAndWait("Press enter to create topic and subscriptions...");
            //await CreateTopicsAndSubscriptions();

            //PromptAndWait("Press enter to send order messages...");
            //await SendOrderMessages();

            PromptAndWait("Press enter to receive order messages...");
            await ReceiveOrdersFromAllSubscriptions();

            PromptAndWait("Press and Subscriptions Console Complete...");
       }

        private static async Task ReceiveOrdersFromAllSubscriptions()
        {
            var manager = new Manager(ServiceBusConnectionString);
            var subscriptionDescriptions = await manager.GetSubscriptionsForTopic(OrdersTopicPath);
            foreach (var subscriptionDescription in subscriptionDescriptions)
            {
                var receiver = new SubscriptionReceiver(ServiceBusConnectionString, OrdersTopicPath, subscriptionDescription.SubscriptionName);
                receiver.RegisterMessageHandler();
                PromptAndWait($"Receiving order from {subscriptionDescription.SubscriptionName}");
                await receiver.Close();
            }
        
        }

        private static void PromptAndWait(string message)
        {
            Console.Write(message);
            Console.ReadLine();
        }

        private static async Task CreateTopicsAndSubscriptions()
        {
            var manager = new Manager(ServiceBusConnectionString);
            await manager.CreateTopic(OrdersTopicPath);
            await manager.CreateSubscription(OrdersTopicPath, "AllOrders");


            await manager.CreateSubscriptionWithSqlFilter(OrdersTopicPath, "UsaOrders", "region='USA'");//4
            await manager.CreateSubscriptionWithSqlFilter(OrdersTopicPath, "EuOrders", "region='EU'");//0

            await manager.CreateSubscriptionWithSqlFilter(OrdersTopicPath, "LargeOrders", "items > 30");//2
            await manager.CreateSubscriptionWithSqlFilter(OrdersTopicPath, "HighValueOrders", "value > 500");//1

            await manager.CreateSubscriptionWithSqlFilter(OrdersTopicPath, "LoyaltyCardOrders", "loyalty=true AND region='USA'");//2

            await manager.CreateSubscriptionWithCorrelationFilter(OrdersTopicPath, "UkOrders", "UK");//1

        }

        static async Task SendOrderMessages()
        {
            var orders = CreateTestOrders();
            var sender = new TopicSender(ServiceBusConnectionString, OrdersTopicPath);
            foreach (var order in orders)
            {
                await sender.SendOrderMessage(order);
            }
            await sender.Close();
        }

        static List<Order> CreateTestOrders()
        {
            var orders = new List<Order>();
            orders.Add(new Order
            {
                Name = "Loyal Customer",
                Value = 19.99,
                Region = "USA",
                Items = 1,
                HasLoyaltyCard = true
            });
            orders.Add(new Order
            {
                Name = "Large Order",
                Value = 49.99,
                Region = "USA",
                Items = 50,
                HasLoyaltyCard = false
            });
            orders.Add(new Order
            {
                Name = "High Value",
                Value = 749.99,
                Region = "USA",
                Items = 45,
                HasLoyaltyCard = false
            });
            orders.Add(new Order
            {
                Name = "Loyal Europe",
                Value = 49.45,
                Region = "USA",
                Items = 3,
                HasLoyaltyCard = true
            });
            orders.Add(new Order
            {
                Name = "UK Order",
                Value = 49.45,
                Region = "UK",
                Items = 3,
                HasLoyaltyCard = false
            });
            return orders;
        }
    }
}
