using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;

namespace ServiceBusBot.ServiceBus.Common
{
    public static class ConnectionService
    {
        public static ServiceBusClient ConnectClient(string? namespaceName)
        {
            var fullyQualifiedNamespace = $"{namespaceName}.servicebus.windows.net";

            var credentialOption = new DefaultAzureCredentialOptions
            {
                ExcludeInteractiveBrowserCredential = false,
            };

            return new(fullyQualifiedNamespace, new DefaultAzureCredential(credentialOption));
        }

        public static ServiceBusAdministrationClient ConnectAdminClient(string? namespaceName)
        {
            var fullyQualifiedNamespace = $"{namespaceName}.servicebus.windows.net";

            var credentialOption = new DefaultAzureCredentialOptions
            {
                ExcludeInteractiveBrowserCredential = false,
            };

            return new(fullyQualifiedNamespace, new DefaultAzureCredential(credentialOption));
        }

        public static ServiceBusClient ConnectClientUsingConnectionString(string? connectionString)
        {
            return new(connectionString);
        }

        public static ServiceBusAdministrationClient ConnectAdminClientUsingConnectionString(string? connectionString)
        {

            return new(connectionString);
        }
    }
}
