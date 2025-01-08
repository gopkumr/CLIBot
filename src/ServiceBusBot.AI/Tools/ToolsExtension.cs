using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using ServiceBusBot.Domain.Abstrations;
using ServiceBusBot.Domain.Attributes;
using System;
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

            // Create a delegate for the method
            var delegateType = Expression.GetDelegateType(
                        method.GetParameters().Select(p => p.ParameterType)
                        .Concat([method.ReturnType])
                        .ToArray());

            var methodDelegate = method.CreateDelegate(delegateType, toolInstance);

            // Assuming AIFunctionFactory.Create takes a delegate, name, and description
            return AIFunctionFactory.Create(methodDelegate, name, description);
        }
        
    }
}
