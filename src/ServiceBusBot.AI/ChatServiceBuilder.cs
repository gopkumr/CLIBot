using Microsoft.Extensions.AI;
using ServiceBusBot.AI.Services;
using ServiceBusBot.Domain.Abstrations;

namespace ServiceBusBot.AI
{
    public class ChatServiceBuilder
    {
        readonly ChatClientBuilder _chatClientBuilder;

        private ChatServiceBuilder(ChatClientBuilder chatClientBuilder)
        {
            _chatClientBuilder = chatClientBuilder;
        }

        public static ChatServiceBuilder Initialise(ChatClientBuilder chatClientBuilder)
        {
            return new ChatServiceBuilder(chatClientBuilder);
        }

        public ChatServiceBuilder AddFunctionCalling()
        {
            _chatClientBuilder.UseFunctionInvocation();
            return this;
        }

        public IChatService Build()
        {
            return new ChatService(_chatClientBuilder.Build());
        }

        public IChatService Build(IEnumerable<AITool> tools, ChatToolMode chatToolMode)
        {
            return new ChatService(_chatClientBuilder.Build(), tools, chatToolMode);
        }
    }
}
