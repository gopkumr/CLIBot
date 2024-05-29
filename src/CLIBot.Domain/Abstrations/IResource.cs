namespace CLIBot.Domain.Abstractions;

public interface IResource
{
    string Id { get; }
    string Name { get; }
    string Region { get; }
    Dictionary<string, string> Tags {get;} 

}