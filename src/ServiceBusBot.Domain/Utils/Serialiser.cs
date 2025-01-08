using System.Text.Json;

namespace ServiceBusBot.Domain.Utils
{
    public class Serialiser
    {
        public static string SerialiseJson<T>(T obj)
        {
            return JsonSerializer.Serialize(obj);
        }

        public static T? DeserialiseJson<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json);
        }
    }
}
