namespace ServiceBusBot.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter, AllowMultiple = false)]
    public class ToolFunctionAttribute : Attribute
    {
        public required string Description { get; set; }
        public string? Name { get; set; }
        public  string? ReturnDescription { get; set; }
    }
}
