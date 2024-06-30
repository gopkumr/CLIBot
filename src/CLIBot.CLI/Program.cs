using System.Text;
using Azure.Identity;
using CLIBot.Domain.Abstractions;
using Microsoft.SemanticKernel;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.Plugins.Core;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.Extensions.DependencyInjection;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = Kernel.CreateBuilder()
                            .AddAzureOpenAIChatCompletion(deploymentName: "AzureAI:DeploymentName", 
                                                        endpoint: "AzureAI:APIEndpoint", 
                                                        apiKey: "AzureAI:APIKey");
        
        //builder.Services.AddLogging(c => c.AddDebug().SetMinimumLevel(LogLevel.Trace));
        builder.Plugins.AddFromType<ResourceGroupCommand>();
        //builder.Plugins.AddFromPromptDirectory("./../../../Plugins/WriterPlugin");
        Kernel kernel = builder.Build();
       
        var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
        var openAIPromptExecutionSettings = new OpenAIPromptExecutionSettings
        {
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
        };

        ChatHistory chatMessages = [];
        while (true)
        {
            // Get user input
            Console.Write("User > ");
            var request = Console.ReadLine();
            if (request == null) break;

            chatMessages.AddUserMessage(request);

            var result = await chatCompletionService.GetChatMessageContentAsync(
                chatMessages,
                executionSettings: openAIPromptExecutionSettings,
                kernel: kernel);

            if (result != null)
            {
                chatMessages.AddMessage(result.Role, result.ToString());
                Console.WriteLine(result.ToString());
            }
        }
    }
}