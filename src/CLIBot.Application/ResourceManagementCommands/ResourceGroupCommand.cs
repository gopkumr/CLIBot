namespace CLIBot.Domain.Abstractions;

using System.Collections.Generic;
using CLIBot.Domain.Abstractions;

public class ResourceGroupCommand : ICommand
{
    public required string Name { get; set;}

    public required Dictionary<string, string> Parameters {get;set;}
}