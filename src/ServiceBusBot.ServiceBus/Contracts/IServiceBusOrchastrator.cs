using ServiceBusBot.Domain.Model;

namespace ServiceBusBot.ServiceBus.Contracts
{
    public interface IServiceBusOrchastrator
    {
        Task<ActionResponse> Connect(string namespaceName);
        Task<ActionResponse> ConnectUsingConnectionString(string connectionString);
        Task<ActionResponse> CreateNewQueue(string name);
        Task<ActionResponse> GetNamespaceDescription();
        Task<ActionResponse> PeekMessageFromQueue(string queueName);
        Task<ActionResponse> ReadMessagesFromQueue(string queueName, int numberOfMessages = 1);
        Task<ActionResponse> SendMessageToQueue(string queueName, string content);
    }
}