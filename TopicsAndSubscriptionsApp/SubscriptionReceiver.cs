using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TopicsAndSubscriptionsApp
{
    class SubscriptionReceiver
    {
        private SubscriptionClient SubscriptionClient;
        public SubscriptionReceiver(string connectionString, string topicPath, string subscriptionName)
        {
            SubscriptionClient = new SubscriptionClient(connectionString, topicPath, subscriptionName);
        }

        public void RegisterMessageHandler()
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };
            SubscriptionClient.RegisterMessageHandler(ProcessOrderMessageAsync, messageHandlerOptions);
        }

        private async Task ProcessOrderMessageAsync(Message message, CancellationToken cancellationToken)
        {
            var orderJson = Encoding.UTF8.GetString(message.Body);
            var order = JsonConvert.DeserializeObject<Order>(orderJson);
            Console.WriteLine($"{order.ToString()}");
            await SubscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            return Task.CompletedTask;
        }

        public async Task Close()
        {
            await SubscriptionClient.CloseAsync();
        }
    }
}
