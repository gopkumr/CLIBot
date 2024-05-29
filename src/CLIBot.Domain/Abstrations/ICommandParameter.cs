namespace CLIBot.Domain.Abstractions;

public interface ICommandParameter
{
    string Name { get; }
    string Value { get; }

}