using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServiceBusBot.Agents;
using ServiceBusBot.CLI;
using ServiceBusBot.Domain.Abstrations;
using ServiceBusBot.ServiceBus;

var configDictionary = CliHelper.GetConfigurationFromUser();

IConfigurationRoot config = new ConfigurationBuilder()
    .AddInMemoryCollection(configDictionary)
    .Build();

var serviceProvider = new ServiceCollection()
   .AddSingleton<IConfiguration>(config)
   .AddLogging(builder => builder.AddConsole().AddFilter((level) => level == LogLevel.Information))
   .RegisterServiceBusTools()
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
    CliHelper.AddBotResponseRowToTable(table, message, response?.Message);
    CliHelper.AddUsageRowToTable(table, response?.TokenUsage);
    CliHelper.RerenderTable(table);

    message = message = CliHelper.PromptUserMessage();
}

