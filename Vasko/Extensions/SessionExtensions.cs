using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace Vasko.Extensions
{
    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            var jsonData = JsonSerializer.Serialize(value);
            session.SetString(key, jsonData);
        }

        public static T? Get<T>(this ISession session, string key)
        {
            var jsonData = session.GetString(key);
            if (string.IsNullOrEmpty(jsonData))
                return default;
            return JsonSerializer.Deserialize<T>(jsonData);
        }
    }
}