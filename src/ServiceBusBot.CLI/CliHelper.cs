using ServiceBusBot.Domain.Abstrations;
using ServiceBusBot.Domain.Model;
using Spectre.Console;

namespace ServiceBusBot.CLI
{
    internal static class CliHelper
    {
        public static Dictionary<string, string?> GetConfigurationFromUser()
        {
            Dictionary<string, string?> configDictionary = new();
            var aiService = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("What AI Service you want to use?")
                    .AddChoices(new[] {
            "Ollama", "AzureOpenAI", "AzureAI"
                    }));

            var modelId = AnsiConsole.Prompt(
                        new TextPrompt<string>("What's the id of the LLM you want to use?"));

            configDictionary.Add("AI:Service", aiService);
            configDictionary.Add("AI:ModelId", modelId);

            if (aiService == "AzureOpenAI" || aiService == "AzureAI")
            {
                var configPrefix = aiService == "AzureOpenAI" ? "AzureOpenAI" : "AzureAI";
                var azureOpenAIEndpoint = AnsiConsole.Prompt(
                        new TextPrompt<string>($"What's the endpoint of the {configPrefix} service?"));

                configDictionary.Add($"{configPrefix}:Endpoint", azureOpenAIEndpoint);
                configDictionary.Add($"{configPrefix}:AuthMethod", "Identity");

                var apiKeyOption = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("You want to use API Key to connect?")
                        .AddChoices(["Yes", "No"]));


                if (apiKeyOption.ToLower() == "yes")
                {
                    configDictionary[$"{configPrefix}:AuthMethod"] = "APIKey";
                    var azureOpenAIKey = AnsiConsole.Prompt(
                        new TextPrompt<string>($"What's the API Key for the {configPrefix} service?").
                        Secret('-'));

                    configDictionary.Add($"{configPrefix}:APIKey", azureOpenAIKey);
                }
            }
            return configDictionary;
        }

        public static string PromptUserMessage()
        {

            var rule = new Rule("[green]Chat[/]")
            {
                Justification = Justify.Left
            };
            AnsiConsole.Write(rule);
            var message = AnsiConsole.Prompt(
                new TextPrompt<string>("[green]:[/]"));

            return message;
        }

        public static void RenderHeader()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new FigletText("Servicebus Bot")
                            .Centered()
                            .Color(Color.Blue));
            AnsiConsole.MarkupLine("[blue]Enter a message to send to the bot. Type '[white]exit[/]' to quit.[/]");
            Console.WriteLine("");
        }

        public static async Task<ModelResponse?> GetModelResponse(IChatService chatService, string message)
        {
            ModelResponse? response = null;
            await AnsiConsole.Status()
                .Spinner(Spinner.Known.Dots2)
                .StartAsync("Thinking...", async ctx =>
                {
                    response = await chatService.GetResponseAsync(message);
                });

            return response;
        }

        public static Table CreateChatResponseTable()
        {
            return new Table() { ShowRowSeparators = true }
                .Border(TableBorder.Rounded)
                .HideHeaders()
                .AddColumn("")
                .AddColumn("");
        }

        public static void AddBotResponseRowToTable(Table table, string message, string? response)
        {
            table.AddRow($"[green]You:[/] {message}", $"[red]Bot:[/] {(response ?? "None").Replace("[", "{").Replace("]", "}")}");
        }

        public static void AddUsageRowToTable(Table table, int? token)
        {
            table.AddRow($"", $"[red] Total Token:[/] {token?.ToString() ?? ""}");
        }

        public static void RerenderTable(Table table)
        {
            AnsiConsole.Write(table);
        }
    }
}
