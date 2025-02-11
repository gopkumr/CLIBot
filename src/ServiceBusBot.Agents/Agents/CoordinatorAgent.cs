using Azure.AI.Projects;
using Azure;
using Microsoft.Azure.Amqp.Framing;
using Microsoft.Azure.Amqp.Transaction;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client.Extensions.Msal;
using Microsoft.SemanticKernel.Agents;
using OllamaSharp.Models;
using OpenAI.Assistants;
using ServiceBusBot.Agents.Extensions;
using ServiceBusBot.Domain.Abstrations;
using ServiceBusBot.Domain.Model;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.ComponentModel;
using System.Data;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System;
namespace ServiceBusBot.Agents.Agents
{
#pragma warning disable SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    public class CoordinatorAgent: IAgent
    {
        readonly string[] _systemPrompts = [""];
        private readonly ChatCompletionAgent _coordinatorAgent;

        public CoordinatorAgent(IOptions<AISettings> settings)
        {
            _coordinatorAgent = AssistantAgentFactory.ChatAgent("CoordinatorAgent", string.Join(',', _systemPrompts), settings?.Value?.CoordinatorAgentSettings!, aiFoundryConnectionString: settings?.Value.AzureAIFoundryConnectionString);
        }

        //TODO: Replace and Expose agent method abstractions here
        public ChatCompletionAgent Agent => _coordinatorAgent;
    }

#pragma warning restore SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
}
