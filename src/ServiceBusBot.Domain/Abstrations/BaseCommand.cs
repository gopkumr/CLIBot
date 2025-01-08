using Azure.Core;
using Azure.ResourceManager;

namespace ServiceBusBot.Domain.Abstrations;

public abstract class BaseCommand
{
    ArmClient _client;

    protected ArmClient ARMClient => _client;

    protected BaseCommand(TokenCredential credential)
    {
        _client = new ArmClient(credential);
    }

    protected BaseCommand(TokenCredential credential, string subscriptionId)
    {
        _client = new ArmClient(credential, subscriptionId);
    }
}