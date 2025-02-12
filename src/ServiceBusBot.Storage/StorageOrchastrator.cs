using ServiceBusBot.Domain.Model;
using ServiceBusBot.Storage.Contract;
using ServiceBusBot.Storage.FileSystem;

namespace ServiceBusBot.Storage
{
    public class StorageOrchastrator : IStorageOrchastrator
    {
        public ActionResponse ReadFileContentFromPath(string path)
        {
            try
            {
                var content = FileService.ReadFileContentFromPath(path);
                return new ActionResponse(content, true);
            }
            catch (Exception ex)
            {
                return new ActionResponse(ex.Message, false);
            }
        }

        public ActionResponse WriteContentToPath(string path, string? content)
        {
            try
            {
                var success = FileService.WriteContentToPath(path, content);
                return new ActionResponse("Content written successfully", success);
            }
            catch (Exception ex)
            {
                return new ActionResponse(ex.Message, false);
            }
        }
    }
}
