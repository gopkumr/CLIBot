using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using ServiceBusBot.Domain.Abstrations;
using ServiceBusBot.Domain.Attributes;
using System.Linq.Expressions;
using System.Reflection;

namespace ServiceBusBot.AI.Tools
{
    public static class ToolsExtension
    {
        public static List<AITool> ScanAndExtractToolFunctions(this IServiceCollection serviceCollection)
        {
            var aiTools = new List<AITool>();
            var toolFunctionAttributeType = typeof(ToolFunctionAttribute);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            // Retrieve all registered tools
            var tools = serviceProvider.GetServices<ITool>();
            if (tools == null || !tools.Any()) return aiTools;

            foreach (var tool in tools)
            {
                var methods = tool.GetType().GetMethods()
                    .Where(m => m.GetCustomAttributes(toolFunctionAttributeType, false).Length > 0);

                foreach (var method in methods)
                {
                    // Register the method to the AI
                    var aiTool = MethodToAITool(method, tool);
                    if (aiTool != null) aiTools.Add(aiTool);
                }
            }

            return aiTools;
        }

        private static AITool? MethodToAITool(MethodInfo method, ITool toolInstance)
        {
            var attribute = method.GetCustomAttribute<ToolFunctionAttribute>();
            if (attribute == null) return null;

            var name = attribute.Name ?? method.Name;
            var description = attribute.Description ?? "No description provided";
            var returnDescription = attribute.ReturnDescription ?? "";

            var creationOptions = new AIFunctionFactoryCreateOptions
            {
                Name = name,
                Description = description,
                Parameters = method.GetParameters().Select(p => new AIFunctionParameterMetadata(p.Name!)
                    {
                        ParameterType = p.ParameterType,
                        IsRequired = !p.IsOptional,
                        DefaultValue = p.DefaultValue,
                        Description = GetParameterDescription(p),
                        Name = GetParameterName(p),
                        HasDefaultValue = p.HasDefaultValue,
                }).ToList(),
                ReturnParameter = method.ReturnParameter != null ? new AIFunctionReturnParameterMetadata()
                    {
                        ParameterType = method.ReturnParameter.ParameterType,
                        Description = returnDescription
                    } : AIFunctionReturnParameterMetadata.Empty
            };

            return AIFunctionFactory.Create(method, toolInstance, creationOptions);
        }

        private static string GetParameterDescription(ParameterInfo parameter)
        {
            var attribute = parameter.GetCustomAttribute<ToolFunctionAttribute>();
            return attribute?.Description ?? parameter.Name ?? "";
        }

        private static string GetParameterName(ParameterInfo parameter)
        {
            var attribute = parameter.GetCustomAttribute<ToolFunctionAttribute>();
            return attribute?.Name ?? parameter.Name ?? "";
        }
    }
}
