using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using ServiceBusBot.Domain.Abstrations;
using ServiceBusBot.Domain.Model;

namespace ServiceBusBot.Agents.Extensions
{
#pragma warning disable SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

    internal static class AssistantAgentFactory
    {
        public static ChatCompletionAgent ChatAgent(string name, string systemPrompt, AgentSettings configuration, IEnumerable<IPlugin>? plugins=null, string? aiFoundryConnectionString=null)
        {
            return new ChatCompletionAgent
            {
                Name = $"{name}",
                Instructions = systemPrompt,
                Kernel =  KernelBuilder
                            .Init(aiFoundryConnectionString)
                            .WithConfiguredModel(configuration)
                            .WithPlugins(plugins)
                            .Build(),
                Arguments = new KernelArguments(
                    new OpenAIPromptExecutionSettings()
                    {
                        FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
                    })
            };
        }
    }


#pragma warning restore SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
}
