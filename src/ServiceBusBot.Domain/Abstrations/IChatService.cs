using ServiceBusBot.Domain.Model;

namespace ServiceBusBot.Domain.Abstrations
{
    public interface IChatService
    {
        Task<IEnumerable<ModelResponse>?> GetResponseAsync(string message);
    }
}
