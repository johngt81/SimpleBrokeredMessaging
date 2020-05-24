using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Newtonsoft.Json;
using SimpleBrokeredMessaging.Config;
using SimpleBrokeredMessaging.MessageEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace SimpleBrokeredMessaging.Sender
{
    class Program
    {
        static async Task Main(string[] args)
        {
            WriteLine("Sender Console - Hit enter", ConsoleColor.White);
            Console.ReadLine();

            await RecreateQueueAsync();
            
            
            //Send a list of messages to a queue
            //await SendPizzaOrderList();

            //A queue is a point to point messaging queue
            //await SendTextString("The is a test message");
            WriteLine("Sender Console - Complete", ConsoleColor.White);
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

        static async Task SendTextString(string text)
        {
            WriteLine("SendTextStringAsMessage", ConsoleColor.Cyan);
            var client = new QueueClient(Settings.ConnectionString, Settings.QueueName);

            Message message = new Message(Encoding.UTF8.GetBytes(text));
            await client.SendAsync(message);
            WriteLine("Done!", ConsoleColor.Green);
            Console.WriteLine();
            await client.CloseAsync();
        }

        static async Task SendPizzaOrderList()
        {
            var client = new QueueClient(Settings.ConnectionString, Settings.QueueName);
            var pizzaOrderList = GetPizzaOrderList();
            WriteLine("Sending...", ConsoleColor.Yellow);
            foreach (var pizzaOrder in pizzaOrderList.Take(2))
            {
                var jsonPizzaOrder = JsonConvert.SerializeObject(pizzaOrder);

                var message = new Message(Encoding.UTF8.GetBytes(jsonPizzaOrder))
                {
                    Label = "PizzaOrder",
                    ContentType = "application/json"
                };

                message.UserProperties.Add("SystemId", 123);

                await client.SendAsync(message);
            }
            WriteLine($"Sent {pizzaOrderList.Count} orders!", ConsoleColor.White);
            Console.WriteLine();
            Console.WriteLine();
        }

        static void WriteLine(string message, ConsoleColor consoleColor)
        {
            Console.ForegroundColor = consoleColor;
            Console.WriteLine(message);
        }

        static List<PizzaOrder> GetPizzaOrderList()
        {
            string[] names = { "Alan", "Jennifer", "James" };
            string[] pizzas = { "Huawaiian", "Vegetarian", "Capricciosa", "Napolitana" };
            var pizzaOrderList = new List<PizzaOrder>();
            foreach (var name in names)
            {
                foreach (var pizza in pizzas)
                {
                    PizzaOrder pizzaOrder = new PizzaOrder
                    {
                        CustomerName = name,
                        Type = pizza,
                        Size = "Large"
                    };
                    pizzaOrderList.Add(pizzaOrder);
                }
            }

            return pizzaOrderList;
        }
    }
}
