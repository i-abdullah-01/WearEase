using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Collections.Generic; 
namespace WearEase.Models.Extensions
{
    public static class SessionExtensions
    {
        public static void SetObject<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T GetObject<T>(this ISession session, string key)
        {
            var data = session.GetString(key);
            return data == null ? default : JsonSerializer.Deserialize<T>(data);
        }

        public static bool HasKey(this ISession session, string key)
        {
            return session.TryGetValue(key, out _);
        }
    }
}
