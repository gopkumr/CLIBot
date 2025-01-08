using Azure;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceBusBot.AI.Tools;
using ServiceBusBot.Domain.Abstrations;

namespace ServiceBusBot.AI
{
    public static class AIExtensions
    {
        const string ollamaEndpoint = "http://localhost:11434/";

        public static IServiceCollection RegisterAIServices(this IServiceCollection services, IConfiguration configuration)
        {
            var chatService = BuildChatService(services, configuration);

            services.AddSingleton(chatService!);
            return services;
        }

        private static IChatService BuildChatService(IServiceCollection services, IConfiguration configuration)
        {
            var tools = services.ScanAndExtractToolFunctions();
            var chatService = configuration["AI:Service"] switch
            {
                "AzureOpenAI" => BuildAzureAIServices(tools, configuration["AzureOpenAI:Endpoint"]!, configuration["AI:ModelId"]!, configuration["AzureOpenAI:ApiKey"]!),
                "Ollama" => BuildOllamaAIServices(tools, configuration["AI:ModelId"]!),
                _ => throw new ArgumentException("Invalid AI service specified in configuration.")
            };
            return chatService;
        }

        private static IChatService BuildOllamaAIServices(IEnumerable<AITool> tools, string modelId) =>
            ChatServiceBuilder.Initialise(new OllamaChatClient(ollamaEndpoint, modelId).AsBuilder())
                              .AddFunctionCalling()
                              .Build(tools);

        private static IChatService BuildAzureAIServices(IEnumerable<AITool> tools, string endpoint, string modelId, string? apiKey = null)
        {
            var azOpenAIClient = new AzureOpenAIClient(new Uri(endpoint), new DefaultAzureCredential());
            if (apiKey != null)
            {
                azOpenAIClient = new AzureOpenAIClient(new Uri(endpoint), new AzureKeyCredential(apiKey));
            }

            return ChatServiceBuilder.Initialise(azOpenAIClient.AsChatClient(modelId).AsBuilder())
                                     .AddFunctionCalling()
                                     .Build(tools);
        }
    }
}
