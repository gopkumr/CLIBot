using Microsoft.Extensions.DependencyInjection;
using ServiceBusBot.Storage.Contract;

namespace ServiceBusBot.Storage
{
    public static class StorageExtensions
    {
        public static IServiceCollection RegisterStorageTools(this IServiceCollection services)
        {
            services.AddSingleton<IStorageOrchastrator, StorageOrchastrator>();
            return services;
        }
    }
}
