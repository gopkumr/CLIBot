using Azure.AI.Projects;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using ServiceBusBot.Domain.Abstrations;
using ServiceBusBot.Domain.Model;

namespace ServiceBusBot.Agents.Extensions
{
#pragma warning disable SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

    internal class KernelBuilder
    {
        private IKernelBuilder _builder;
        private const string ollamaEndpoint = "http://localhost:11434/";
        private readonly AIProjectClient? _aiProjectClient = null;

        private KernelBuilder(string? aiFoundryConnectionString = null)
        {
            _builder = Kernel.CreateBuilder();
            if (aiFoundryConnectionString != null)
                _aiProjectClient = new AIProjectClient(aiFoundryConnectionString!, new DefaultAzureCredential());
        }

        public static KernelBuilder Init(string? aiFoundryConnectionString = null)
        {
            return new KernelBuilder(aiFoundryConnectionString);
        }

        public KernelBuilder WithConfiguredModel(AgentSettings configuration)
        {
            var connections = _aiProjectClient?.GetConnectionsClient();

            switch (configuration.Service)
            {
                case "AzureOpenAI":
                    if (connections == null) throw new ArgumentException("Invalid configuration, missing AI Foundry project connection string.");
                    ConnectionResponse connection = connections.GetDefaultConnection(ConnectionType.AzureOpenAI, withCredential: true);
                    var properties = connection.Properties as ConnectionPropertiesApiKeyAuth;
                    _builder = _builder.AddAzureOpenAIChatCompletion(configuration.ModelId!, properties!.Target, properties!.Credentials.Key);
                    break;
                case "Ollama":
                    _builder = _builder.AddOllamaChatCompletion(configuration.ModelId!, new Uri(ollamaEndpoint));
                    break;
                case "AzureAI":
                    throw new ArgumentException("AzureAI service is not implemented yet.");
                default:
                    throw new ArgumentException("Invalid AI service specified in configuration.");
            };

            return this;
        }

        public KernelBuilder WithPlugins(IEnumerable<IPlugin>? plugins)
        {
            plugins?.ToList().ForEach(q => _builder.Plugins.AddFromObject(q));
            return this;
        }
        public Kernel Build()
        {
            return _builder.Build();
        }


    }

#pragma warning restore SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
}
