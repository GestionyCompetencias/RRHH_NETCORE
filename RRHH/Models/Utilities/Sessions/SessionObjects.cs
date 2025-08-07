using Newtonsoft.Json;

namespace RRHH.Models.Utilities.Sessions
{
    /// <summary>
    /// Clase estática que tiene por objetivo almacenar objetos en las sesiones de usuario.
    /// </summary>
    public static class SessionObjects
    {
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
}
