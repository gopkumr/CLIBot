using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceBusBot.Agents.Agents;
using ServiceBusBot.Agents.Plugins;
using ServiceBusBot.Agents.Services;
using ServiceBusBot.Domain.Abstrations;

namespace ServiceBusBot.Agents
{
    public static class AIExtensions
    {
        public static IServiceCollection RegisterAIServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddKeyedSingleton<IPlugin,ServiceBusPlugin>("ServicebusPlugin");
            services.AddKeyedSingleton<IPlugin, StoragePlugin>("StoragePlugin");
            services.AddSingleton<ServicebusAgent>();
            services.AddSingleton<StorageAgent>();
            services.AddSingleton<CoordinatorAgent>();
            services.AddSingleton<IChatService, ChatService>();
            return services;
        }
    }
}
