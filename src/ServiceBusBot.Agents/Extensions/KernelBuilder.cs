using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using ServiceBusBot.Domain.Abstrations;

namespace ServiceBusBot.Agents.Extensions
{
#pragma warning disable SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

    internal class KernelBuilder
    {
        private IKernelBuilder _builder;
        private IEnumerable<IPlugin>? plugins = null;
        private const string ollamaEndpoint = "http://localhost:11434/";


        private KernelBuilder()
        {
            _builder = Kernel.CreateBuilder();
        }

        public static KernelBuilder Init()
        {
            return new KernelBuilder();
        }

        public KernelBuilder WithConfiguredModel(IConfiguration configuration)
        {
            _builder = configuration["AI:Service"] switch
            {
                "AzureOpenAI" => _builder.AddAzureOpenAIChatCompletion(configuration["AI:ModelId"]!, configuration["AzureOpenAI:Endpoint"]!, configuration["AzureOpenAI:ApiKey"]!),
                "Ollama" => _builder.AddOllamaChatCompletion(configuration["AI:ModelId"]!, new Uri(ollamaEndpoint)),
                "AzureAI" => throw new ArgumentException("AzureAI service is not implemented yet."),
                _ => throw new ArgumentException("Invalid AI service specified in configuration.")
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
