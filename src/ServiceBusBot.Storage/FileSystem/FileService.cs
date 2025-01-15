namespace ServiceBusBot.Storage.FileSystem
{
    internal static class FileService
    {
        public static string ReadFileContentFromPath(string path)
        {
            return System.IO.File.ReadAllText(path);
        }
    }
}
