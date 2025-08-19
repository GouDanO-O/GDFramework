using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace GDFrameworkExtend.JsonKit
{
    public sealed class Vector2JsonConverter : JsonConverter<Vector2>
    {
        public override void WriteJson(JsonWriter writer, Vector2 value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("x"); writer.WriteValue(value.x);
            writer.WritePropertyName("y"); writer.WriteValue(value.y);
            writer.WriteEndObject();
        }

        public override Vector2 ReadJson(JsonReader reader, Type objectType, Vector2 existingValue,
            bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return default;

            // 既兼容对象也兼容数组格式（可选）
            if (reader.TokenType == JsonToken.StartArray)
            {
                var arr = JArray.Load(reader);
                float x = arr.Count > 0 ? arr[0]!.Value<float>() : 0f;
                float y = arr.Count > 1 ? arr[1]!.Value<float>() : 0f;
                return new Vector2(x, y);
            }

            var jo = JObject.Load(reader);
            float vx = jo["x"] != null ? jo["x"]!.Value<float>() : 0f;
            float vy = jo["y"] != null ? jo["y"]!.Value<float>() : 0f;
            return new Vector2(vx, vy);
        }
    }
}