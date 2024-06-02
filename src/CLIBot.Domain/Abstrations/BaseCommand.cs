using System.Collections.Generic;
using System.Security.Authentication.ExtendedProtection;
using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;

namespace CLIBot.Domain.Abstractions;

public abstract class BaseCommand
{
    ArmClient _client;

    protected ArmClient ARMClient => _client;

    protected BaseCommand(TokenCredential credential)
    {
        _client = new ArmClient(credential);
    }

    protected BaseCommand(TokenCredential credential,  string subscriptionId)
    {
        _client = new ArmClient(credential, subscriptionId);
    }
}