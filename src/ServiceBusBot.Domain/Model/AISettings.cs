using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBusBot.Domain.Model;

public record AISettings
{
    public const string SectionName = "AISettings";

    public string? AzureAIFoundryConnectionString { get; set; }
    public AgentSettings? ServicebusAgentSettings { get; set; }
    public AgentSettings? StorageAgentSettings { get; set; }
    public AgentSettings? CoordinatorAgentSettings { get; set; }
}

public record AgentSettings {
    public string? Service { get; set; }
    public string? ModelId { get; set; }
}