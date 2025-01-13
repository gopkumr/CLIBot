using Microsoft.Extensions.DependencyInjection;
using ServiceBusBot.Domain.Abstrations;

namespace ServiceBusBot.ServiceBus
{
    public static class ServicebusExtension
    {

        public static IServiceCollection RegisterServiceBusTools(this IServiceCollection services)
        {
            services.AddSingleton<ITool, ServiceBusOrchastrator>();
            return services;
        }
    }
}
