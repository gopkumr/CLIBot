﻿using Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServiceBusBot.Agents;
using ServiceBusBot.CLI;
using ServiceBusBot.Domain.Abstrations;
using ServiceBusBot.Domain.Model;
using ServiceBusBot.ServiceBus;
using ServiceBusBot.Storage;

Console.WriteLine("Initializing...");

IConfigurationRoot config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var serviceProvider = new ServiceCollection()
                       .AddSingleton<IConfiguration>(config)
                       .Configure<AISettings>(config.GetSection(AISettings.SectionName))
                       .AddLogging(builder => builder.AddConsole().AddFilter((level) => level == LogLevel.Information))
                       .RegisterServiceBusTools()
                       .RegisterStorageTools()
                       .RegisterAIServices(config)
                       .BuildServiceProvider();

var chatService = serviceProvider!.GetRequiredService<IChatService>();

CliHelper.RenderHeader();

var table = CliHelper.CreateChatResponseTable();
string message = CliHelper.PromptUserMessage();

while (message != "exit" && !string.IsNullOrEmpty(message))
{
    IEnumerable<ModelResponse>? response = null;
    await CliHelper.ShowSpinnerAndCall("Thinking...", async () =>
    {
        response = await chatService.GetResponseAsync(message);
    });

    CliHelper.RenderHeader();
    response?.ToList().ForEach((item) =>
    {
        var userMessage = message;
        if (response?.ToList().IndexOf(item) > 0)
            userMessage = "";

        CliHelper.AddBotResponseRowToTable(table, userMessage, item.Message, item.Name);
    });

    CliHelper.RerenderTable(table);

    message = message = CliHelper.PromptUserMessage();
}

