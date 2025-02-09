using OllamaSharp.Models.Chat;
using ServiceBusBot.Domain.Abstrations;
using ServiceBusBot.Domain.Model;
using Spectre.Console;

namespace ServiceBusBot.CLI
{
    internal static class CliHelper
    {
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

        public static async Task ShowSpinnerAndCall(string message, Func<Task> modelTriggerFunction)
        {
           await AnsiConsole.Status()
                .Spinner(Spinner.Known.Dots2)
                .StartAsync(message, async ctx =>
                {
                   await modelTriggerFunction();
                });
        }

        public static Table CreateChatResponseTable()
        {
            return new Table() { ShowRowSeparators = true }
                .Border(TableBorder.Rounded)
                .HideHeaders()
                .AddColumn("")
                .AddColumn("");
        }

        public static void AddBotResponseRowToTable(Table table, string message, string? response, string? agentNames)
        {
            table.AddRow($"[green]You:[/] {message}", $"[red]{agentNames??"Agents"}:[/] {(response ?? "None").Replace("[", "{").Replace("]", "}")}");
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
