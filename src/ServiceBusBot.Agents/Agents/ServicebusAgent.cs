using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel.Agents;
using ServiceBusBot.Agents.Extensions;
using ServiceBusBot.Domain.Abstrations;

namespace ServiceBusBot.Agents.Agents
{
#pragma warning disable SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    public class ServicebusAgent
    {
        readonly string[] _systemPrompts = [
            "You are a helpful AI assistant helping with operations on Azure Servicebus. Respond with proper error message when a command is not found in the functions list. If the requested opteration is not found in the functions list, please respond with a message that you are not capable of performing the operation as of now.",
            "Don't make assumptions about what values to use with functions. Ask for clarification if a user request is ambiguous."
        ];
        private readonly ChatCompletionAgent _servicebusAgent;

        public ServicebusAgent(IConfiguration configuration, IEnumerable<IPlugin> plugins)
        {
            _servicebusAgent = AssistantAgentFactory.ChatAgent("ServicebusAssistant", string.Join(',', _systemPrompts), configuration, plugins);
        }

        //TODO: Replace and Expose agent method abstractions here
        public ChatCompletionAgent Agent => _servicebusAgent;
    }

#pragma warning restore SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
}
