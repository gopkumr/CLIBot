using System.Collections.Generic;

namespace CLIBot.Domain.Abstractions;

public interface ICommand
{
    string Name { get; }
    Dictionary<string,string> Parameters { get; }
}