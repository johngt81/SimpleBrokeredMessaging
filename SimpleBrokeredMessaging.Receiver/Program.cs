using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.ServiceBus;
using Newtonsoft.Json;
using SimpleBrokeredMessaging.Config;
using SimpleBrokeredMessaging.MessageEntities;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleBrokeredMessaging.Receiver
{
    class Program
    {
        static IQueueClient queueClient;

        static async Task Main(string[] args)
        {
            await RecreateQueueAsync();
            ReceiveAndProcessPizzaOrders(1);
            Console.ReadLine();
        }

        static async Task RecreateQueueAsync()
        {
            var managementClient = new ManagementClient(Settings.ConnectionString);
            if (!await managementClient.QueueExistsAsync(Settings.QueueName))
            {
                await managementClient.CreateQueueAsync(Settings.QueueName);
            }
        }

        static void ReceiveAndProcessPizzaOrders(int threads)
        {
            WriteLine($"ReceiveAndProcessPizzaOrders({threads})", ConsoleColor.Cyan);
            queueClient = new QueueClient(Settings.ConnectionString, Settings.QueueName);
            var options = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                AutoComplete = false,
                MaxConcurrentCalls = threads,
                MaxAutoRenewDuration = TimeSpan.FromSeconds(30)
            };

            queueClient.RegisterMessageHandler(ProcessPizzaMessageAsync, options);
            WriteLine("Receiving, hit enter to exit", ConsoleColor.White);
            Console.ReadLine();
            StopReceivingAsync().Wait();
        }

        static async Task ProcessPizzaMessageAsync(Message message, CancellationToken cancellationToken)
        {
            var messageBodyText = Encoding.UTF8.GetString(message.Body);
            var pizzaOrder = JsonConvert.DeserializeObject<PizzaOrder>(messageBodyText);

            CookPizza(pizzaOrder);

            //Completes message
            await queueClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        private static void CookPizza(PizzaOrder pizzaOrder)
        {
            WriteLine($"Cooking {pizzaOrder.Type} for {pizzaOrder.CustomerName} ", ConsoleColor.Yellow);
            Thread.Sleep(5000);

            WriteLine($"Cooking {pizzaOrder.Type} pizza for {pizzaOrder.CustomerName} is ready!", ConsoleColor.Yellow);
        }

        private static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            WriteLine(exceptionReceivedEventArgs.Exception.Message, ConsoleColor.Red);
            return Task.CompletedTask;
        }

        private static async Task StopReceivingAsync()
        {
            await queueClient.CloseAsync();
        }

        static void WriteLine(string message, ConsoleColor consoleColor)
        {
            Console.ForegroundColor = consoleColor;
            Console.WriteLine(message);
        }
    }
}
