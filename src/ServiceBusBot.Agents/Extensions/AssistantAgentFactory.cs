﻿using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel.Agents;
using ServiceBusBot.Domain.Abstrations;

namespace ServiceBusBot.Agents.Extensions
{
#pragma warning disable SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

    internal static class AssistantAgentFactory
    {
        public static ChatCompletionAgent ChatAgent(string name, string systemPrompt, IConfiguration configuration, IEnumerable<IPlugin>? plugins=null)
        {
            return new ChatCompletionAgent
            {
                Name = name,
                Instructions = systemPrompt,
                Kernel = KernelBuilder
                            .Init()
                            .WithConfiguredModel(configuration)
                            .WithPlugins(plugins)
                            .Build()
            };
        }
    }


#pragma warning restore SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
}
