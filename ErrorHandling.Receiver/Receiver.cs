using ErrorHandling.Config;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ErrorHandling.Receiver
{
    class Receiver
    {
        private static IQueueClient queueClient;
        static async Task Main(string[] args)
        {
            await CreateQueue();
            queueClient = new QueueClient(Settings.ConnectionString, Settings.QueuePath);
            ReceiveMessages();
        }

        static void ReceiveMessages()
        {
            var options = new MessageHandlerOptions(ExRcvHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };
            queueClient.RegisterMessageHandler(ProcessMessage, options);
            Utils.WriteLine("Receiving messages", ConsoleColor.Cyan);
            Console.ReadLine();
        }

        static async Task ProcessMessage(Message message, CancellationToken arg2)
        {
            Utils.WriteLine($"Received: {message.Label}", ConsoleColor.Cyan);
            switch (message.ContentType)
            {
                case "text/plain":
                    await ProcessTextMessage(message);
                    break;
                case "application/json":
                    await ProcessJsonMessage(message);
                    break;
                default:
                    Console.WriteLine("Received unknown message: " + message.ContentType);
                    break;
            }
        }

        private static async Task ProcessJsonMessage(Message message)
        {
            var body = Encoding.UTF8.GetString(message.Body);
            Utils.WriteLine($"JSON message {body}", ConsoleColor.Yellow);
            try
            {
                dynamic data = JsonConvert.DeserializeObject(body);

                Utils.WriteLine($"Name: {data.contact.name}", ConsoleColor.Yellow);
                Utils.WriteLine($"Twiiter: {data.contact.twiiter}", ConsoleColor.Yellow);


                await queueClient.CompleteAsync(message.SystemProperties.LockToken);
                Utils.WriteLine("Processed message", ConsoleColor.Cyan);
            }
            catch (Exception mex)
            {
                Utils.WriteLine($"Exception: {mex.Message}", ConsoleColor.Red);
            }
        }

        private static async Task ProcessTextMessage(Message message)
        {
            var body = Encoding.UTF8.GetString(message.Body);
            
            Utils.WriteLine($"Text message {body} Delivery Count: {message.SystemProperties.DeliveryCount}", ConsoleColor.Yellow);
            try
            {
                var forwardingQueueClient = new QueueClient(Settings.ConnectionString, Settings.ForwardingQueuePath);
                await forwardingQueueClient.SendAsync(new Message(Encoding.UTF8.GetBytes("test")));
                await forwardingQueueClient.CloseAsync();

                await queueClient.CompleteAsync(message.SystemProperties.LockToken);
                Utils.WriteLine("Processed message", ConsoleColor.Cyan);
            }
            catch (Exception mex)
            {
                Utils.WriteLine($"Exception: {mex.Message}", ConsoleColor.Red);
                //explicit deadlettering

                if (message.SystemProperties.DeliveryCount > 5)
                {
                    await queueClient.DeadLetterAsync(message.SystemProperties.LockToken,
                        deadLetterReason: mex.Message,
                        deadLetterErrorDescription: mex.ToString());
                }
            }
        }

        private static Task ExRcvHandler(ExceptionReceivedEventArgs arg)
        {
            Utils.WriteLine($"Exception: {arg.Exception.Message}", ConsoleColor.Red);
            return Task.CompletedTask;
        }

        private static async Task CreateQueue()
        {
            var managementClient = new ManagementClient(Settings.ConnectionString);

            if (!await managementClient.QueueExistsAsync(Settings.QueuePath))
            {
                await managementClient.CreateQueueAsync(new QueueDescription(Settings.QueuePath)
                {//Locks expiring time
                    LockDuration = TimeSpan.FromSeconds(5)
                });
            }

            if (!await managementClient.QueueExistsAsync(Settings.ForwardingQueuePath))
            {
                await managementClient.CreateQueueAsync(Settings.ForwardingQueuePath);
            }
        }
    }
}
