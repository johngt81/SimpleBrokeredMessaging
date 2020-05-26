using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TagReader.Message;

namespace TagReader
{
    class Program
    {
        private static string ConnectionString = "";
        private static string QueueName = "queuesession";

        static async Task Main(string[] args)
        {
            var managementClient = new ManagementClient(ConnectionString);
            if (!await managementClient.QueueExistsAsync(QueueName))
            {
                Console.WriteLine("Queue does not exists");
                Console.ReadKey();
            }

            var tags = GetTags();
            var queueClient = new QueueClient(ConnectionString, QueueName);

            int position = 0;
            string sessionId = Guid.NewGuid().ToString();
            while (position < 5)
            {
                var tag = tags[position];
                var orderJson = JsonConvert.SerializeObject(tag);
                var tagReadMessage = new Microsoft.Azure.ServiceBus.Message(Encoding.UTF8.GetBytes(orderJson));
                tagReadMessage.MessageId = tag.TagId;
                tagReadMessage.SessionId = sessionId;
                await queueClient.SendAsync(tagReadMessage);

                position++;
            }
            Console.ReadKey();
        }

        static RfidTag[] GetTags()
        {
            return new List<RfidTag>
            {
                new RfidTag
                {
                    TagId = "1",
                    Item = "Book",
                    Price = 40
                },
                new RfidTag
                {
                    TagId = "2",
                    Item = "Ruler",
                    Price = 5
                },
                new RfidTag
                {
                    TagId = "3",
                    Item = "Eraser",
                    Price = 2
                },
                new RfidTag
                {
                    TagId = "4",
                    Item = "Pen",
                    Price = 4
                },
                new RfidTag
                {
                    TagId = "5",
                    Item = "Notebook",
                    Price = 15
                }
            }.ToArray();
        }
    }
}
