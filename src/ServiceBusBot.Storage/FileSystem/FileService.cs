namespace ServiceBusBot.Storage.FileSystem
{
    internal static class FileService
    {
        public static string ReadFileContentFromPath(string path)
        {
            return System.IO.File.ReadAllText(path);
        }

        public static bool WriteContentToPath(string path, string? content)
        {
            System.IO.File.WriteAllText(path, content);

            return true;
        }
    }
}
