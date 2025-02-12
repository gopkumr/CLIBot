using Microsoft.SemanticKernel;
using ServiceBusBot.Domain.Abstrations;
using ServiceBusBot.Domain.Model;
using ServiceBusBot.Storage.Contract;
using System.ComponentModel;

namespace ServiceBusBot.Agents.Plugins
{
    public class StoragePlugin : IPlugin
    {
        private readonly IStorageOrchastrator _storageTool;

        public StoragePlugin(IStorageOrchastrator storageTool)
        {
            _storageTool = storageTool;
        }

        [KernelFunction("read_file_content_from_path")]
        [Description("Read content of a file using the full path sent")]
        [return: Description("content of the file in string format")]
        public ActionResponse ReadFileContentFromPath(string path)
        {
            return _storageTool.ReadFileContentFromPath(path);
        }

        [KernelFunction("write_content_to_file_at_path")]
        [Description("Write content inti a file using the full path sent")]
        [return: Description("Status of the file write operation")]
        public ActionResponse WriteContentToPath(string path, string? content)
        {
            return _storageTool.WriteContentToPath(path, content);
        }

    }
}
