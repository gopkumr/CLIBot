using ServiceBusBot.Domain.Model;

namespace ServiceBusBot.Domain.Abstrations
{
    public interface IChatService
    {
        Task<ModelResponse?> GetResponseAsync(string message);
    }
}
