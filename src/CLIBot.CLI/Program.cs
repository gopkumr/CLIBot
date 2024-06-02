using Azure.Identity;
using CLIBot.Domain.Abstractions;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;


var builder = Kernel.CreateBuilder()
                    .AddAzureOpenAIChatCompletion("xxx", "xxxx", "xxxxx");
//builder.Services.AddLogging(c => c.AddDebug().SetMinimumLevel(LogLevel.Trace));
builder.Plugins.AddFromType<ResourceGroupCommand>();
//builder.Plugins.AddFromPromptDirectory("./../../../Plugins/WriterPlugin");
Kernel kernel = builder.Build();


ChatHistory history = [];
Console.Write("User > ");
string? userInput;


while ((userInput = Console.ReadLine()) != null)
{
    // Get user input
    Console.Write("User > ");
    var request = Console.ReadLine();

    // Get chat response
    var poemResult = await kernel.InvokeAsync("ResourceGroupCommand", "GetResourceGroupId", new()
{
    { "name", request }
});
    Console.WriteLine(poemResult);

}
