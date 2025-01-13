using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using ServiceBusBot.Domain.Model;
using ServiceBusBot.Domain.Utils;

namespace ServiceBusBot.ServiceBus.Queue
{
    public static class QueueService
    {
        public static async Task<ActionResponse> GetNamespaceDescription(ServiceBusAdministrationClient client)
        {
            var properties = await client.GetNamespacePropertiesAsync();
            return new ActionResponse(Serialiser.SerialiseJson(properties), true);
        }

        public static async Task<ActionResponse> CreateNewQueue(ServiceBusAdministrationClient client, string name)
        {
            var options = new CreateQueueOptions(name);
            var response = await client.CreateQueueAsync(options);
            return new ActionResponse(Serialiser.SerialiseJson(response), true);
        }

        public static async Task<ActionResponse> SendMessageToQueue(ServiceBusClient client, string queueName, string content)
        {
            ServiceBusSender sender = client.CreateSender(queueName);
            var message = new ServiceBusMessage(content);
            await sender.SendMessageAsync(message);
            return new ActionResponse("Message sent successfully", true);
        }

        public static async Task<ActionResponse> ReadNMessagesFromQueue(ServiceBusClient client, string queueName, int numberOfMessages = 10)
        {
            ServiceBusReceiver receiver = client.CreateReceiver(queueName);
            var messages = await receiver.ReceiveMessagesAsync(numberOfMessages);
            var messagebody = messages.Select(m => m.Body.ToString());

            messages.ToList().ForEach(async message => await receiver.CompleteMessageAsync(message));

            return new ActionResponse(Serialiser.SerialiseJson(messagebody), true);
        }

        public static async Task<ActionResponse> PeekMessagesFromQueue(ServiceBusClient client, string queueName, int numberOfMessages = 10)
        {
            ServiceBusReceiver receiver = client.CreateReceiver(queueName);
            var messages = await receiver.PeekMessagesAsync(numberOfMessages);
            var messagebody = messages.Select(m => m.Body.ToString());
            return new ActionResponse(Serialiser.SerialiseJson(messagebody), true);
        }

        public static async Task<ActionResponse> PeekMessagesFromDeadletterQueue(ServiceBusClient client, string queueName, int numberOfMessages = 10)
        {
            ServiceBusReceiver receiver = client.CreateReceiver(queueName, new ServiceBusReceiverOptions
            {
                SubQueue = SubQueue.DeadLetter
            });

            var messages = await receiver.PeekMessagesAsync(numberOfMessages);
            var messagebody = messages.Select(m => new { SeqNo=m.SequenceNumber, Content = m.Body.ToString() });

            return new ActionResponse(Serialiser.SerialiseJson(messagebody), true);
        }

        public static async Task<ActionResponse> RequeueFromDeadletter(ServiceBusClient client, string queueName, long sequenceNumber)
        {
            ServiceBusReceiver receiver = client.CreateReceiver(queueName, new ServiceBusReceiverOptions
            {
                SubQueue = SubQueue.DeadLetter
            });

            var message = await receiver.PeekMessageAsync(fromSequenceNumber: sequenceNumber);
            ServiceBusSender sender = client.CreateSender(queueName);
            await sender.SendMessageAsync(new ServiceBusMessage(message));
            await receiver.CompleteMessageAsync(message);

            return new ActionResponse("Message successfully requeued and removed from deadletter", true);
        }
    }
}
