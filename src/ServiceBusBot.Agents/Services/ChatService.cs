
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using ServiceBusBot.Agents.Agents;
using ServiceBusBot.Domain.Abstrations;
using ServiceBusBot.Domain.Model;
using System.Text;

namespace ServiceBusBot.Agents.Services
{
#pragma warning disable SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

    public class ChatService(ServicebusAgent servicebusAgent) : IChatService
    {
        private readonly ServicebusAgent _servicebusAgent = servicebusAgent;
        private readonly ChatHistory _conversation = [];
        private readonly PromptExecutionSettings settings = new() { FunctionChoiceBehavior = FunctionChoiceBehavior.Required() };

        public async Task<ModelResponse?> GetResponseAsync(string message)
        {
            StringBuilder sb = new();

            await _servicebusAgent.Agent.ReduceAsync(_conversation);

            _conversation.Add(new ChatMessageContent(AuthorRole.User, message));

            await foreach (ChatMessageContent response in _servicebusAgent.Agent.InvokeAsync(_conversation, new KernelArguments(settings)))
            {
                sb.AppendLine(response.Content ?? string.Empty);
            }

            return new ModelResponse(sb.ToString(), 000);
        }

    }

#pragma warning restore SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
}
