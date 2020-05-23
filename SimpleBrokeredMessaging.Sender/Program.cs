using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using System;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBrokeredMessaging.Sender
{
    class Program
    {
        const string ServiceBusConnectionString = "Endpoint=sb://bg-servicebus-namespace.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=6nOVJ88EooxWmlHVL/lF4dF86mBmMHgSvPg+jxRamTk=";
        const string QueueName = "";
        static IQueueClient queueClient;

        static async Task Main(string[] args)
        {
            const int numberOfMessages = 10;
            queueClient = new QueueClient(ServiceBusConnectionString, QueueName);

            await SendMessageAsync(numberOfMessages);
            Console.ReadKey();
            await queueClient.CloseAsync();
        }

        static async Task SendMessageAsync(int numberOfMessagesToSend)
        {
            try
            {
                for (int i = 0; i < numberOfMessagesToSend; i++)
                {
                    string messageBody = $"Message {i}";
                    Message message = new Message(Encoding.UTF8.GetBytes(messageBody));
                    await queueClient.SendAsync(message);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"{DateTime.Now} :: Exception: {exception.Message}");
            }
        }
    }
}
