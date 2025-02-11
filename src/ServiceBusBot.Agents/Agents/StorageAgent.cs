using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel.Agents;
using ServiceBusBot.Agents.Extensions;
using ServiceBusBot.Domain.Abstrations;
using ServiceBusBot.Domain.Model;

namespace ServiceBusBot.Agents.Agents
{
#pragma warning disable SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    public class StorageAgent: IAgent
    {
        readonly string[] _systemPrompts = [
            "You are a helpful AI assistant helping with operations on differnt types of storage including file system. Execute the part in the request that you can perform using the function and If the requested operation or part of the operation is not found in the functions list, please respond an appropriate message.",
            "Don't make assumptions about what values to use with functions.",
            "Once the task is successfully completed, respond with a success message and do not ask the user any further instructions",
            "If the user query contains instructions that you cannot perform, instruct the Servicebus Assistant to perform it."
        ];
        private readonly ChatCompletionAgent _storageAgent;

        public StorageAgent(IOptions<AISettings> configuration, [FromKeyedServices("StoragePlugin")] IEnumerable<IPlugin> plugins)
        {
            _storageAgent = AssistantAgentFactory.ChatAgent("StorageAssistant", string.Join(',', _systemPrompts), configuration?.Value.StorageAgentSettings!, plugins, configuration?.Value.AzureAIFoundryConnectionString);
        }

        //TODO: Replace and Expose agent method abstractions here
        public ChatCompletionAgent Agent => _storageAgent;
    }

#pragma warning restore SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
}
