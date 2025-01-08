using Azure;
using Microsoft.Extensions.AI;
using Microsoft.VisualBasic;
using ServiceBusBot.Domain.Abstrations;
using ServiceBusBot.Domain.Model;
using System.Text;

namespace ServiceBusBot.AI.Services
{
    public class ChatService : IChatService
    {
        private readonly IChatClient _chatClient;
        private readonly IEnumerable<AITool>? _tools;

        private List<ChatMessage> conversation =
                [
                    new(ChatRole.System, "You are a helpful AI assistant helping with operations on Azure Servicebus. Respond with proper error message when a command is not found in the functions list. If the requested opteration is not found in the functions list, please respond with a message that you are not capable of performing the operation as of now."),
                    new(ChatRole.System, "Don't make assumptions about what values to use with functions. Ask for clarification if a user request is ambiguous.")
               ];

        internal ChatService(IChatClient chatClient, IEnumerable<AITool> tools)
        {
            _chatClient = chatClient;
            _tools = tools;
        }

        internal ChatService(IChatClient chatClient)
        {
            _chatClient = chatClient;
        }

        public async Task<ModelResponse?> GetResponseAsync(string message)
        {
            var chatOptions = new ChatOptions
            {
                Tools = _tools?.ToList(),
                ToolMode = ChatToolMode.RequireAny
            };

            CondenseChatHistory();

            conversation.Add(new ChatMessage(ChatRole.User, message));

            var response = await _chatClient.CompleteAsync(conversation, chatOptions);

            conversation.Add(new ChatMessage(ChatRole.Assistant, response.Message.Text));

            return new ModelResponse(response?.Message?.Text??string.Empty, 
                                    TimeSpan.Parse(response?.AdditionalProperties?["total_duration"]?.ToString()??TimeSpan.MinValue.ToString()), 
                                    response?.Usage?.TotalTokenCount??000);
        }

        private void CondenseChatHistory()
        {
            // Remove all historical user and assistant messages from the conversation except the last two
            var lastTwoUserMessages = conversation
                .Where(m => m.Role == ChatRole.User)
                .TakeLast(2)
                .ToList();

            // Keep system messages and the last two user/assistant messages
            conversation = conversation
                .Where(m => m.Role != ChatRole.User)
                .Concat(lastTwoUserMessages)
                .ToList();
        }
    }
}
