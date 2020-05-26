using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;
using TagReader.Message;

namespace SimpleBrokeredMessaging.Checkout
{
    class Program
    {
        private static string ConnectionString = "";
        private static string QueueName = "queuesession";

        static async Task Main(string[] args)
        {
            var managementClient = new ManagementClient(ConnectionString);
            if (await managementClient.QueueExistsAsync(QueueName))
            {
                await managementClient.DeleteQueueAsync(QueueName);
            }

            QueueDescription queueDescription = new QueueDescription(QueueName)
            {
                RequiresDuplicateDetection = true,
                DuplicateDetectionHistoryTimeWindow = TimeSpan.FromMinutes(10),
                RequiresSession = true
            };
            await managementClient.CreateQueueAsync(queueDescription);


            var sessionClient = new SessionClient(ConnectionString, QueueName);

            while (true)
            {
                Console.WriteLine("Accepting a message session...");
                Console.ForegroundColor = ConsoleColor.White;
                var messageSession = await sessionClient.AcceptMessageSessionAsync();

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"Accepted session: {messageSession.SessionId}");
                Console.ForegroundColor = ConsoleColor.Yellow;

                int receivedCount = 0;
                double billTotal = 0.0;

                while (true)
                {//Receive Mode a message
                    var message = await messageSession.ReceiveAsync();
                    if (message == null)
                    {
                        Console.WriteLine("Closing session...!");
                        await messageSession.CloseAsync();
                        break;
                    }
                    else
                    {
                        var json = Encoding.UTF8.GetString(message.Body);
                        var tag = JsonConvert.DeserializeObject<RfidTag>(json);
                        Console.WriteLine($"{tag.ToString()}");

                        receivedCount++;
                        billTotal += tag.Price;
                        await messageSession.CompleteAsync(message.SystemProperties.LockToken);
                    }
                }
                //Total bill
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Bill customer ${billTotal} for {receivedCount} items.");
            }
        }
    }
}
