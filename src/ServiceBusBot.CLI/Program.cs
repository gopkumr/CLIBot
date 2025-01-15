using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServiceBusBot.Agents;
using ServiceBusBot.CLI;
using ServiceBusBot.Domain.Abstrations;
using ServiceBusBot.ServiceBus;
using ServiceBusBot.Storage;

var configDictionary = CliHelper.GetConfigurationFromUser();

IConfigurationRoot config = new ConfigurationBuilder()
    .AddInMemoryCollection(configDictionary)
    .Build();

var serviceProvider = new ServiceCollection()
   .AddSingleton<IConfiguration>(config)
   .AddLogging(builder => builder.AddConsole().AddFilter((level) => level == LogLevel.Information))
   .RegisterServiceBusTools()
   .RegisterStorageTools()
   .RegisterAIServices(config)
   .BuildServiceProvider();

var chatService = serviceProvider.GetRequiredService<IChatService>();

CliHelper.RenderHeader();

var table = CliHelper.CreateChatResponseTable();
string message = CliHelper.PromptUserMessage();

while (message != "exit" && !string.IsNullOrEmpty(message))
{
    var response = await CliHelper.GetModelResponse(chatService, message);
    CliHelper.RenderHeader();
    response?.ToList().ForEach((item) =>
    {
        var userMessage = message;
        if (response?.ToList().IndexOf(item) > 0)
            userMessage = "";

        CliHelper.AddBotResponseRowToTable(table, userMessage, item.Message, item.Name);
    });

    //CliHelper.AddUsageRowToTable(table, response?.TokenUsage);
    CliHelper.RerenderTable(table);

    message = message = CliHelper.PromptUserMessage();
}

