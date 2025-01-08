using Azure.Messaging.ServiceBus.Administration;
using ServiceBusBot.Domain.Model;
using ServiceBusBot.Domain.Utils;

namespace ServiceBusBot.ServiceBus.Namespace
{
    public static class NamespaceService
    {
        public static async Task<ActionResponse> GetNamespaceDescription(ServiceBusAdministrationClient client)
        {
            var properties = await client.GetNamespacePropertiesAsync();
            return new ActionResponse(Serialiser.SerialiseJson(properties), true);
        }
    }
}
