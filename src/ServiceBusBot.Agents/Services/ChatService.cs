
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Chat;
using Microsoft.SemanticKernel.Agents.History;
using Microsoft.SemanticKernel.ChatCompletion;
using ServiceBusBot.Agents.Agents;
using ServiceBusBot.Domain.Abstrations;
using ServiceBusBot.Domain.Model;
using System.Text;

namespace ServiceBusBot.Agents.Services
{
#pragma warning disable SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

    public class ChatService : IChatService
    {
        private readonly AgentGroupChat _agentGroupChat = new();

        public ChatService(ServicebusAgent servicebusAgent, StorageAgent storageAgent)
        {
            _agentGroupChat.AddAgent(servicebusAgent.Agent);
            _agentGroupChat.AddAgent(storageAgent.Agent);
            _agentGroupChat.ExecutionSettings = new AgentGroupChatSettings { SelectionStrategy = CreateSelectionStrategy(servicebusAgent, storageAgent), TerminationStrategy= CreateTerminationStrategy(servicebusAgent) };
        }

        public async Task<IEnumerable<ModelResponse>?> GetResponseAsync(string message)
        {
            StringBuilder sb = new();
            List<ModelResponse> modelResponses = [];

            _agentGroupChat.AddChatMessage(new ChatMessageContent(AuthorRole.User, message));
            _agentGroupChat.IsComplete = false;

            await foreach (ChatMessageContent response in _agentGroupChat.InvokeAsync())
            {
                sb.AppendLine(response.Content ?? string.Empty);
                modelResponses.Add(new ModelResponse(response.Content ?? string.Empty, 000, response.AuthorName ?? "Agents"));
            }

            return modelResponses;
        }

        private static KernelFunctionSelectionStrategy CreateSelectionStrategy(ServicebusAgent servicebusAgent, StorageAgent storageAgent)
        {
            KernelFunction selectionFunction =
            AgentGroupChat.CreatePromptFunctionForStrategy(
                $$$"""
                Determine which participant takes the next turn in a conversation based on History.
                Participants can take more than one turn if the task is to perform same operation.

                Choose only from these participants:
                {{$participants}}

                Follow these rules when choosing the next participant:
                - If the History is from the user, it is the one of the agents' turn.
                - If the History is from an agent and there are no next steps, then the task is completed.
                - If the History has tasks relates to storage and servicebus then split the task and orchastraate between the agents to complete.
                - If the History is from an agent and the next step is to read or write from a storage service, it is {{{storageAgent.Agent.Name}}}'s turn so return the text without explanation: {{{storageAgent.Agent.Name}}}
                - If the History is from an agent and the next step is to perform operations on a servicebus or servicebus namespace services like queues, topics or deadletters, it is {{{servicebusAgent.Agent.Name}}}'s turn so return the text without explanation: {{{servicebusAgent.Agent.Name}}} 

                History:
                {{$history}}
                """,
                safeParameterNames: ["history", "participants"]);

            KernelFunctionSelectionStrategy selectionStrategy =
              new(selectionFunction, servicebusAgent.Agent.Kernel.Clone())
              {
                  HistoryVariableName = "history",
                  HistoryReducer = new ChatHistoryTruncationReducer(1),
                  AgentsVariableName = "participants",
                  ResultParser = (result) => result.GetValue<string>() ?? servicebusAgent.Agent.Name ?? "ServicebusAgent"
              };
            return selectionStrategy;
        }

        private static TerminationStrategy CreateTerminationStrategy(ServicebusAgent servicebusAgent)
        {
            KernelFunction terminationFunction =
                AgentGroupChat.CreatePromptFunctionForStrategy(
                $$$"""
                Examine the History and determine whether the task has been completed successfully.
                If task has been completed successfully, respond with a single word with explanation: DONE.
                If specific tasks are not completed and the agent requires further action to be done by another agent then continue the chat and respond with single word with explanation NOTCOMPLETED.
                If no more actions needs to be done or agent wants inforation from user, respond with a single word with explanation: DONE.
                
                History:
                {{$history}}
                """,
              safeParameterNames: "history");

            TerminationStrategy terminationStrategy =
                      new KernelFunctionTerminationStrategy(terminationFunction, servicebusAgent.Agent.Kernel.Clone())
                      {
                          HistoryReducer = new ChatHistoryTruncationReducer(1),
                          HistoryVariableName = "history",
                          MaximumIterations = 2,
                          ResultParser = (result) => result.GetValue<string>()?.Contains("DONE", StringComparison.OrdinalIgnoreCase) ?? false
                      };
            return terminationStrategy;
        }

    }

#pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning restore SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
}
