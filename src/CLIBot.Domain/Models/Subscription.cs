namespace CLIBot.Domain.Models;

public class Subscription(string id, string name)
{
    public string Id { get; set; } = id;
    public string Name { get; set; } = name;
}