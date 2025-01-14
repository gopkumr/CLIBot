using Microsoft.Extensions.DependencyInjection;
using ServiceBusBot.Domain.Abstrations;
using ServiceBusBot.ServiceBus.Contracts;

namespace ServiceBusBot.ServiceBus
{
    public static class ServicebusExtension
    {

        public static IServiceCollection RegisterServiceBusTools(this IServiceCollection services)
        {
            services.AddSingleton<ITool, ServiceBusOrchastrator>();
            services.AddSingleton<IServiceBusOrchastrator, ServiceBusOrchastrator>();
            return services;
        }
    }
}
