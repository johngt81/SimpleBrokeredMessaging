using Microsoft.Azure.Amqp.Framing;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace TopicsAndSubscriptionsApp
{
    class TopicSender
    {
        private TopicClient TopicClient;

        public TopicSender(string connectionString, string topicPath)
        {
            TopicClient = new TopicClient(connectionString, topicPath);
        }

        public async Task SendOrderMessage(Order order)
        {
            Console.WriteLine($"{order.ToString()}");

            var orderJson = JsonConvert.SerializeObject(order);

            var message = new Message(Encoding.UTF8.GetBytes(orderJson));

            message.UserProperties.Add("region", order.Region);
            message.UserProperties.Add("items", order.Items);
            message.UserProperties.Add("value", order.Value);
            message.UserProperties.Add("loyalty", order.HasLoyaltyCard);

            message.CorrelationId = order.Region;
            await TopicClient.SendAsync(message);
        }

        public async Task Close()
        {
            await TopicClient.CloseAsync();
        }
    }
}
