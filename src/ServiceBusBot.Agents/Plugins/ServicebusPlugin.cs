using Microsoft.SemanticKernel;
using ServiceBusBot.Domain.Abstrations;
using ServiceBusBot.Domain.Attributes;
using ServiceBusBot.Domain.Model;
using ServiceBusBot.ServiceBus.Contracts;
using System.ComponentModel;

namespace ServiceBusBot.Agents.Plugins
{
    public class ServiceBusPlugin : IPlugin
    {
        private readonly IServiceBusOrchastrator _servicebusTool;

        public ServiceBusPlugin(IServiceBusOrchastrator servicebusTool)
        {
            _servicebusTool = servicebusTool;
        }

        [KernelFunction("connect_servicebus_namespace")]
        [Description("Connect to a service bus namespace for the passed name using logged in credentials")]
        [return: Description("Properties of the servicebus")]
        public async Task<ActionResponse> Connect(string namespaceName)
        {
           return await _servicebusTool.Connect(namespaceName);
        }

        [KernelFunction("connect_servicebus_namespace_connectionstring")]
        [Description("Connect to a service bus namespace for the passed connection string")]
        [return: Description("Properties of the servicebus")]
        public async Task<ActionResponse> ConnectUsingConnectionString(string connectionString)
        {
            return await _servicebusTool.ConnectUsingConnectionString(connectionString);
        }

        [KernelFunction("get_servicebus_namespace_properties")]
        [Description("Get the details of the connected service bus namespace")]
        [return: Description("Returns the properties of namespace in Json Format. This method also can return errors while reading")]
        public async Task<ActionResponse> GetNamespaceDescription()
        {
            return await _servicebusTool.GetNamespaceDescription();
        }

        [KernelFunction("create_new_queue")]
        [Description("Create a new queue in the namespace with the passed name")]
        [return: Description("Returns the properties of the new created queue in Json Format. This method also can return errors while reading")]
        public async Task<ActionResponse> CreateNewQueue(string name)
        {
            return await _servicebusTool.CreateNewQueue(name);
        }

        [KernelFunction("send_message_to_queue")]
        [Description("Send a message to the queue")]
        [return: Description("Message saying is the send was successfull or an error message if failed")]
        public async Task<ActionResponse> SendMessageToQueue(string queueName, string content)
        {
            return await _servicebusTool.SendMessageToQueue(queueName, content);
        }

        [KernelFunction("read_messages_from_queue")]
        [Description("Read and delete a number of messages from the queue, if no number is specified single message is read")]
        [return: Description("Returns the content of the message and deletes the message from the queue. This method also can return errors while reading.")]
        public async Task<ActionResponse> ReadMessagesFromQueue(string queueName, int numberOfMessages = 1)
        {
            return await _servicebusTool.ReadMessagesFromQueue(queueName, numberOfMessages);
        }

        [KernelFunction("peek_messages_from_queue")]
        [Description("Peek the oldest message in queue. The message is not removed from the queue")]
        [return: Description("Returns the content of the message and without deleting the message from the queue. This method also can return errors while reading")]
        public async Task<ActionResponse> PeekMessageFromQueue(string queueName)
        {
            return await _servicebusTool.PeekMessageFromQueue(queueName);
        }
    }
}
