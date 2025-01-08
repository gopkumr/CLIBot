using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using ServiceBusBot.Domain.Abstrations;
using ServiceBusBot.Domain.Attributes;
using ServiceBusBot.Domain.Model;
using ServiceBusBot.ServiceBus.Common;
using ServiceBusBot.ServiceBus.Namespace;
using ServiceBusBot.ServiceBus.Queue;

namespace ServiceBusBot.ServiceBus
{
    public class ServiceBusMediator : IAsyncDisposable, ITool
    {
        ServiceBusClient? _serviceBusClient;
        ServiceBusAdministrationClient? _serviceBusAdminClient;

        [ToolFunction(Description = "Connect to a service bus namespace for the passed name")]
        public async Task<ActionResponse> Connect(string? namespaceName = null)
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
        public async Task<ActionResponse> ConnectUsingConnectionString(string? connectionString = null)
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

        [ToolFunction(Description = "Get the details of the connected service bus namespace")]
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

        [ToolFunction(Description = "Create a new queue in the namespace with the passed name")]
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
            if (_serviceBusClient == null)
                return new ActionResponse("ServiceBusClient is null, connect to the service bus before calling this method", false);

            try
            {
                return await QueueService.SendMessageToQueue(_serviceBusClient, queueName, content);
            }
            catch (Exception ex)
            {
                return new ActionResponse(ex.Message, false);
            }
        }

        [ToolFunction(Description = "Read and delete a number of messages from the queue, if no number is specified single message is read")]
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

        [ToolFunction(Description = "Peek the oldest message in queue. The message is not removed from the queue")]
        public async Task<ActionResponse> PeekMessageFromQueue(string queueName)
        {
            if (_serviceBusClient == null)
                return new ActionResponse("ServiceBusClient is null, connect to the service bus before calling this method", false);

            try
            {
                return await QueueService.PeekMessageFromQueue(_serviceBusClient, queueName);
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
