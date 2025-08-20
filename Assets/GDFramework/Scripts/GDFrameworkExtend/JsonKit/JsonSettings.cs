using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GDFrameworkExtend.JsonKit
{
    public static class JsonSettings
    {
        public static JsonSerializerSettings Make()
        {
            var s = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            s.Converters.Add(new Vector2JsonConverter());
            s.Converters.Add(new Vector3JsonConverter());
            s.Converters.Add(new StringEnumConverter()); 
            return s;
        }
    }
}