using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using ServiceBusBot.Domain.Abstrations;
using ServiceBusBot.Domain.Attributes;
using ServiceBusBot.Domain.Model;
using ServiceBusBot.ServiceBus.Common;
using ServiceBusBot.ServiceBus.Contracts;
using ServiceBusBot.ServiceBus.Namespace;
using ServiceBusBot.ServiceBus.Queue;

namespace ServiceBusBot.ServiceBus
{
    public class ServiceBusOrchastrator : IAsyncDisposable, ITool, IServiceBusOrchastrator
    {
        ServiceBusClient? _serviceBusClient;
        ServiceBusAdministrationClient? _serviceBusAdminClient;

        public ServiceBusOrchastrator()
        {
            
        }

        [ToolFunction(Description = "Connect to a service bus namespace for the passed name")]
        public async Task<ActionResponse> Connect(string namespaceName)
        {
            if (string.IsNullOrEmpty(namespaceName))
                return new ActionResponse("Please provide a namespace name", false);

            _serviceBusClient = ConnectionService.ConnectClient(namespaceName);
            _serviceBusAdminClient = ConnectionService.ConnectAdminClient(namespaceName);

            try
            {
                await _serviceBusAdminClient.GetNamespacePropertiesAsync();
                _serviceBusClient.CreateReceiver("dummy");
            }
            catch (Exception ex)
            {
                return new ActionResponse(ex.Message, false);
            }

            return new ActionResponse("Connected successfully", true);
        }

        [ToolFunction(Description = "Connect to a service bus namespace for the passed connection string")]
        public async Task<ActionResponse> ConnectUsingConnectionString(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                return new ActionResponse("Please provide a connection string", false);

            _serviceBusClient = ConnectionService.ConnectClientUsingConnectionString(connectionString);
            _serviceBusAdminClient = ConnectionService.ConnectAdminClientUsingConnectionString(connectionString);

            try
            {
                await _serviceBusAdminClient.GetNamespacePropertiesAsync();
                _serviceBusClient.CreateReceiver("dummy");
            }
            catch (Exception ex)
            {
                return new ActionResponse(ex.Message, false);
            }

            return new ActionResponse("Connected successfully", true);
        }

        [ToolFunction(Description = "Get the details of the connected service bus namespace", ReturnDescription = "Returns the properties of namespace in Json Format. This method also can return errors while reading")]
        public async Task<ActionResponse> GetNamespaceDescription()
        {
            if (_serviceBusAdminClient == null)
                return new ActionResponse("ServiceBusClient is null, connect to the service bus before calling this method", false);

            try
            {
                return await NamespaceService.GetNamespaceDescription(_serviceBusAdminClient);
            }
            catch (Exception ex)
            {
                return new ActionResponse(ex.Message, false);
            }
        }

        [ToolFunction(Description = "Create a new queue in the namespace with the passed name", ReturnDescription = "Returns the properties of the new created queue in Json Format. This method also can return errors while reading")]
        public async Task<ActionResponse> CreateNewQueue(string name)
        {
            if (_serviceBusAdminClient == null)
                return new ActionResponse("ServiceBusClient is null, connect to the service bus before calling this method", false);

            try
            {
                return await QueueService.CreateNewQueue(_serviceBusAdminClient, name);
            }
            catch (Exception ex)
            {
                return new ActionResponse(ex.Message, false);
            }
        }

        [ToolFunction(Description = "Send a message to the queue")]
        public async Task<ActionResponse> SendMessageToQueue(string queueName, string content)
        {
            var responseMessage = string.Empty;
            if (_serviceBusClient == null)
                responseMessage = "ServiceBusClient is null, connect to the service bus before calling this method";

            if (string.IsNullOrEmpty(content))
                responseMessage = "Content is empty, please provide a content to send in the message";

            if (!string.IsNullOrEmpty(responseMessage))
                return new ActionResponse(responseMessage, false);

            try
            {
                return await QueueService.SendMessageToQueue(_serviceBusClient, queueName, content);
            }
            catch (Exception ex)
            {
                return new ActionResponse(ex.Message, false);
            }
        }

        [ToolFunction(Description = "Read and delete a number of messages from the queue, if no number is specified single message is read", ReturnDescription = "Returns the content of the message and deletes the message from the queue. This method also can return errors while reading.")]
        public async Task<ActionResponse> ReadMessagesFromQueue(string queueName, int numberOfMessages = 1)
        {
            if (_serviceBusClient == null)
                return new ActionResponse("ServiceBusClient is null, connect to the service bus before calling this method", false);

            try
            {
                return await QueueService.ReadNMessagesFromQueue(_serviceBusClient, queueName, numberOfMessages);
            }
            catch (Exception ex)
            {
                return new ActionResponse(ex.Message, false);
            }
        }

        [ToolFunction(Description = "Peek the oldest message in queue. The message is not removed from the queue.", ReturnDescription="Returns the content of the message and without deleting the message from the queue. This method also can return errors while reading")]
        public async Task<ActionResponse> PeekMessageFromQueue(string queueName)
        {
            if (_serviceBusClient == null)
                return new ActionResponse("ServiceBusClient is null, connect to the service bus before calling this method", false);

            try
            {
                return await QueueService.PeekMessagesFromQueue(_serviceBusClient, queueName);
            }
            catch (Exception ex)
            {
                return new ActionResponse(ex.Message, false);
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_serviceBusClient != null)
                await _serviceBusClient.DisposeAsync();
        }
    }
}
