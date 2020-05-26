using ErrorHandling.Config;
using Microsoft.Azure.ServiceBus;
using System;
using System.Text;
using System.Threading.Tasks;

namespace ErrorHandling.Sender
{
    class Sender
    {
        private static QueueClient queueClient;
        static async Task Main(string[] args)
        {
            Console.WriteLine("Sender Console");
            Console.WriteLine();

            while (true)
            {
                Console.WriteLine("text/json/poison/unknown/exit?");
                var messageType = Console.ReadLine().ToLower();

                if (messageType == "exit")
                {
                    break;
                }

                switch (messageType)
                {
                    case "text":
                        await SendMessage("Hello", "text/plain");
                        break;
                    case "json":
                        await SendMessage("{\"contact\": {\"name\":\"Demo\", \"twiiter\":\"@testdemo\"} }", "application/json");
                        break;
                    case "poison":
                        await SendMessage("<contact><name>Demo</name><twiiter>@testdemo</twiiter></contact>", "application/json");
                        break;
                    case "unknown":
                        await SendMessage("Unknown message", "application/unknown");
                        break;
                    default:
                        Console.WriteLine("What?");
                        break;
                }
                await queueClient?.CloseAsync();
            }
        }

        private static async Task SendMessage(string text, string contentType)
        {
            try
            {
                queueClient = new QueueClient(Settings.ConnectionString, Settings.QueuePath);
                var message = new Message(Encoding.UTF8.GetBytes(text));
                message.ContentType = contentType;
                Utils.WriteLine($"Created Message: {text}", ConsoleColor.Cyan);

                await queueClient.SendAsync(message);
                Utils.WriteLine("Sent Message", ConsoleColor.Cyan);
            }
            catch (Exception ex)
            {
                Utils.WriteLine(ex.Message, ConsoleColor.Red);
            }
        }
    }
}
