using Microsoft.Azure.ServiceBus;
using RequestResponseMessaging.Configuration;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace RequestResponseMessagingClient
{
    class Program
    {
        static QueueClient RequestQueueClient = new QueueClient(AccountDetails.ConnectionString, AccountDetails.RequestQueueName);
        static QueueClient ResponseQueueClient = new QueueClient(AccountDetails.ConnectionString, AccountDetails.ResponseQueueName);

        static async Task Main(string[] args)
        {
            Console.WriteLine("Client Console");

            while (true)
            {
                Console.WriteLine("Enter text:");
                string text = Console.ReadLine();

                string responseSessionId = Guid.NewGuid().ToString();

                var requestMessage = new Message(Encoding.UTF8.GetBytes(text));
                //SessionId used to correlate request and response
                requestMessage.ReplyToSessionId = responseSessionId;

                var stopwatch = Stopwatch.StartNew();

                await RequestQueueClient.SendAsync(requestMessage);

                //Accept a message session
                var sessionClient = new SessionClient(AccountDetails.ConnectionString, AccountDetails.ResponseQueueName);
                var messageSession = await sessionClient.AcceptMessageSessionAsync(requestMessage.SessionId);

                //Receive the response message
                var responseMessage = await messageSession.ReceiveAsync();
                stopwatch.Stop();

                await sessionClient.CloseAsync();

                var echoTxt = Encoding.UTF8.GetString(responseMessage.Body);

                Console.WriteLine(echoTxt);
                Console.WriteLine($"Time: {stopwatch.ElapsedMilliseconds}");
                Console.WriteLine();
            }
        }
    }
}
