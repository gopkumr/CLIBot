using Microsoft.Extensions.DependencyInjection;
using ServiceBusBot.Domain.Abstrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBusBot.ServiceBus
{
    public static class ServicebusExtension
    {

        public static IServiceCollection RegisterServiceBusTools(this IServiceCollection services)
        {
            services.AddSingleton<ITool, ServiceBusMediator>();
            return services;
        }
    }
}
