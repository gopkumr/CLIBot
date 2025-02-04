﻿using Microsoft.Extensions.Configuration;
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
            "You are a helpful AI assistant helping with operations on differnt types of storage including file system. Execute the part in the request that you can perform using the function and If the requested operation or part of the operation is not found in the functions list, please respond with a request for other agents to perform it.",
            "Don't make assumptions about what values to use with functions. Ask for clarification if a user request is ambiguous."
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
