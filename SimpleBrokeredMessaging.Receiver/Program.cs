using Microsoft.Azure.ServiceBus;
using Microsoft.ServiceBus;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleBrokeredMessaging.Receiver
{
    class Program
    {
        const string ServiceBusConnectionstring = "Endpoint=sb://bg-servicebus-namespace.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=6nOVJ88EooxWmlHVL/lF4dF86mBmMHgSvPg+jxRamTk=";
        const string QueueName = "";
        static IQueueClient queueClient;
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }

        static async Task MainAsync()
        {
            queueClient = new QueueClient(ServiceBusConnectionstring, QueueName);
            RegisterOnMessageHandlerAndReceiveMessages();
            Console.ReadKey();
            await queueClient.CloseAsync();

        }

        private static void RegisterOnMessageHandlerAndReceiveMessages()
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };
            queueClient.RegisterMessageHandler(ProcessMessageAsync, messageHandlerOptions);
        }

        private static async Task ProcessMessageAsync(Message message, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{Encoding.UTF8.GetString(message.Body)}");
          await  queueClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        private static async Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            Console.WriteLine("Exception context for troubleshooting:");
            Console.WriteLine($"- Endpoint: {context.Endpoint}");
            Console.WriteLine($"- Entity Path: {context.EntityPath}");
            Console.WriteLine($"- Executing Action: {context.Action}");
        }
    }
}
