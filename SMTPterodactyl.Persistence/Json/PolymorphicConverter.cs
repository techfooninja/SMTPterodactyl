namespace SMTPterodactyl.Persistence.Json
{
    using SMTPterodactyl.Utilities;
    using System;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    internal class PolymorphicConverter<TInterface> : JsonConverter<TInterface>
    {
        private readonly string typePropertyName;

        public PolymorphicConverter(string typePropertyName = "$type")
        {
            this.typePropertyName = typePropertyName;
        }

        public override TInterface? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            if (!doc.RootElement.TryGetProperty(this.typePropertyName, out var typeProp))
            {
                throw new JsonException("Missing $type property.");
            }

            var typeName = typeProp.GetString();
            var type = TypeExtensions.GetTypeByClassName(typeName) ?? throw new JsonException($"Unknown type: {typeName}");

            return (TInterface?)JsonSerializer.Deserialize(doc.RootElement.GetRawText(), type, options);
        }

        public override void Write(Utf8JsonWriter writer, TInterface value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                return;
            }

            var type = value.GetType();
            var json = JsonSerializer.SerializeToElement(value, type, options);

            writer.WriteStartObject();
            writer.WriteString(this.typePropertyName, type.Name);
            foreach (var prop in json.EnumerateObject())
            {
                prop.WriteTo(writer);
            }

            writer.WriteEndObject();
        }
    }
}
