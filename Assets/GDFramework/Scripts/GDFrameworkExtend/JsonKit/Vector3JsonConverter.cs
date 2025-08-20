using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace GDFrameworkExtend.JsonKit
{
    public sealed class Vector3JsonConverter : JsonConverter<Vector3>
    {
        public override void WriteJson(JsonWriter w, Vector3 v, JsonSerializer s)
        { w.WriteStartObject(); w.WritePropertyName("x"); w.WriteValue(v.x); w.WritePropertyName("y"); w.WriteValue(v.y); w.WritePropertyName("z"); w.WriteValue(v.z); w.WriteEndObject(); }
        public override Vector3 ReadJson(JsonReader r, System.Type t, Vector3 e, bool h, JsonSerializer s)
        {
            if (r.TokenType == JsonToken.Null) return default;
            if (r.TokenType == JsonToken.StartArray){ var a = JArray.Load(r); return new Vector3(a.Count>0?a[0]!.Value<float>():0, a.Count>1?a[1]!.Value<float>():0, a.Count>2?a[2]!.Value<float>():0); }
            var o = JObject.Load(r); return new Vector3(o.GetValue("x", System.StringComparison.OrdinalIgnoreCase)?.Value<float>() ?? 0f,
                o.GetValue("y", System.StringComparison.OrdinalIgnoreCase)?.Value<float>() ?? 0f,
                o.GetValue("z", System.StringComparison.OrdinalIgnoreCase)?.Value<float>() ?? 0f);
        }
    }
}