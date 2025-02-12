using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Chat;
using Microsoft.SemanticKernel.Agents.History;
using Microsoft.SemanticKernel.ChatCompletion;
using ServiceBusBot.Agents.Agents;
using ServiceBusBot.Domain.Abstrations;
using ServiceBusBot.Domain.Model;
using System.Text;
using System.Text.Json;

namespace ServiceBusBot.Agents.Services
{
    record SelectionResponse(string name, string reason);
    record TerminationResponse(bool isAnswered, string reason);

#pragma warning disable SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

    public class ChatService : IChatService
    {
        private readonly AgentGroupChat _agentGroupChat = new();

        public ChatService(ServicebusAgent servicebusAgent, StorageAgent storageAgent, CoordinatorAgent coordinatorAgent)
        {
            _agentGroupChat.AddAgent(servicebusAgent.Agent);
            _agentGroupChat.AddAgent(storageAgent.Agent);
            _agentGroupChat.ExecutionSettings = new AgentGroupChatSettings { SelectionStrategy = CreateSelectionStrategy(servicebusAgent, storageAgent, coordinatorAgent), TerminationStrategy= CreateTerminationStrategy(coordinatorAgent) };
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

        private static KernelFunctionSelectionStrategy CreateSelectionStrategy(ServicebusAgent servicebusAgent, StorageAgent storageAgent, CoordinatorAgent coordinatorAgent)
        {
            KernelFunction selectionFunction =
            AgentGroupChat.CreatePromptFunctionForStrategy(
                $$$"""
                Select which participant will take the next turn based on the conversation history.

                Only choose from these participants:
                - ServicebusAssistant
                - StorageAssistant
                
                Choose the next participant according to the action of the most recent participant:
                If the query contains multiple tasks, split them and identify the first task to be done completed and select the agent based on it.
                If the query involves Service Bus operations, select the ServiceBus Assistant.
                If the query involves Storage operations, select the Storage Assistant.
                Once all agents complete their tasks, respond with a single word: Done. Do not provide any additional explanations or details.

                Respond in JSON format.  The JSON schema can include only:
                {
                    "name": "string (the name of the assistant selected for the next turn)",
                    "reason": "string (the reason for the participant was selected)"
                }

                History:
                {{$history}}
                """,
                safeParameterNames: ["history", "participants"]);

            KernelFunctionSelectionStrategy selectionStrategy =
              new(selectionFunction, coordinatorAgent.Agent.Kernel.Clone())
              {
                  HistoryVariableName = "history",
                  HistoryReducer = new ChatHistoryTruncationReducer(1),
                  AgentsVariableName = "participants",
                  ResultParser = (result) => {
                      var textData = (result.GetValue<string>()??string.Empty).Replace("```json", "").Replace("```", "");
                      var jsonData = JsonSerializer.Deserialize<SelectionResponse>(textData);
                      Console.WriteLine($"{jsonData?.reason ?? ""}");
                      return jsonData?.name ?? "ServicebusAgent";
                    }
              };
            return selectionStrategy;
        }

        private static TerminationStrategy CreateTerminationStrategy(CoordinatorAgent coordinatorAgent)
        {
            KernelFunction terminationFunction =
                AgentGroupChat.CreatePromptFunctionForStrategy(
                $$$"""
                Examine the History and determine whether the task has been completed successfully.
                If task has been completed successfully, respond with answered as true.
                If specific tasks are not completed and the agent requires further action to be done by another agent then continue the chat and respond with not answered.
                
                Respond in JSON format.  The JSON schema can include only:
                {
                    "isAnswered": "bool (true if the user request has been fully answered)",
                    "reason": "string (the reason for your determination)"
                }

                History:
                {{$history}}
                """,
              safeParameterNames: "history");

            TerminationStrategy terminationStrategy =
                      new KernelFunctionTerminationStrategy(terminationFunction, coordinatorAgent.Agent.Kernel.Clone())
                      {
                          HistoryReducer = new ChatHistoryTruncationReducer(1),
                          HistoryVariableName = "history",
                          MaximumIterations = 4,
                          ResultParser = (result) =>
                          {
                              var textData = (result.GetValue<string>() ?? string.Empty).Replace("```json", "").Replace("```", "");
                              var jsonData = JsonSerializer.Deserialize<TerminationResponse>(textData);
                              Console.WriteLine($"{jsonData?.reason??""}");
                              return jsonData?.isAnswered ?? false;
                          }
                      };
            return terminationStrategy;
        }

    }

#pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning restore SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
}
