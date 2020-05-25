using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using RequestResponseMessaging.Configuration;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RequestResponseMessagingServer
{
    class Program
    {
        static QueueClient RequestQueueClient = new QueueClient(AccountDetails.ConnectionString, AccountDetails.RequestQueueName);
        static QueueClient ResponseQueueClient = new QueueClient(AccountDetails.ConnectionString, AccountDetails.ResponseQueueName);

        static async Task Main(string[] args)
        {
            Console.WriteLine("Server Console");

            var managementClient = new ManagementClient(AccountDetails.ConnectionString);

            Console.Write("Creating queues...");

            if (await managementClient.QueueExistsAsync(AccountDetails.RequestQueueName))
            {
                await managementClient.DeleteQueueAsync(AccountDetails.RequestQueueName);
            }

            if (await managementClient.QueueExistsAsync(AccountDetails.ResponseQueueName))
            {
                await managementClient.DeleteQueueAsync(AccountDetails.ResponseQueueName);
            }

            await managementClient.CreateQueueAsync(AccountDetails.RequestQueueName);
            //Create Response queue with Sessions
            QueueDescription responseQueueDescription = new QueueDescription(AccountDetails.ResponseQueueName)
            {
                RequiresSession = true
            };

            await managementClient.CreateQueueAsync(responseQueueDescription);

            Console.WriteLine("Done!");

            RequestQueueClient.RegisterMessageHandler(ProcessRequestMessage, new MessageHandlerOptions(ProcessMessageException));
            Console.WriteLine("Processing, hit Enter to exit.");
            Console.ReadLine();

            //Close the queueClient
            await RequestQueueClient.CloseAsync();
            await ResponseQueueClient.CloseAsync();
        }

        private static Task ProcessMessageException(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine(exceptionReceivedEventArgs.Exception);
            return Task.CompletedTask;
        }

        private static async Task ProcessRequestMessage(Message message, CancellationToken cancellationToken)
        {
            string text = Encoding.UTF8.GetString(message.Body);
            Console.WriteLine($"Received {text}");

            string echoText = $"Echo: {text}";

            var responseMessage = new Message(Encoding.UTF8.GetBytes(echoText));
            responseMessage.SessionId = message.ReplyToSessionId;

            await ResponseQueueClient.SendAsync(responseMessage);
            Console.WriteLine($"Sent: {echoText}");
        }
    }
}
