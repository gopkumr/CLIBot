namespace ServiceBusBot.Domain.Model
{
   
    public record ActionResponse(string Message, bool Success);
    public record ModelResponse(string Message, TimeSpan Duration, int TokenUsage);
}
