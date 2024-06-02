namespace CLIBot.Domain.Abstractions;

using System.ComponentModel;
using Azure.Identity;
using Microsoft.SemanticKernel;

public class ResourceGroupCommand() : BaseCommand(new DefaultAzureCredential())
{

    [KernelFunction]
    [Description("Gets the Identified of the resoure group.")]
    public string GetResourceGroupId(
        [Description("Name of the resource group")] string name){
        var resourceGroup = ARMClient.GetDefaultSubscription().GetResourceGroup(name);

        return resourceGroup?.Value?.Data?.Id??"Cannot find the resource";
    }

}