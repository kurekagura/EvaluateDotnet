using System.Drawing;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace DeSeri;

internal class Program
{
    static void Main(string[] args)
    {
        var rectF = new RectangleF(1.1f,2.2f,3.3f,4.4f);

        // カスタムオプションにコンバーターを追加
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        options.Converters.Add(new RectangleFJsonConverter());

        // シリアライズ
        string json = JsonSerializer.Serialize(rectF, options);
        Console.WriteLine("Serialized JSON:");
        Console.WriteLine(json);

        // デシリアライズ
        var deserializedRectF = JsonSerializer.Deserialize<RectangleF>(json, options);
        Console.WriteLine($"Deserialized RectangleF: {deserializedRectF}");

        Console.WriteLine("Press any key to exit..");
        Console.ReadKey();
    }
}

public class RectangleFJsonConverter : JsonConverter<RectangleF>
{
    public override RectangleF Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // JSONをデシリアライズしてRectangleFを復元
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException();

        float x = 0, y = 0, width = 0, height = 0;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                break;

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                string propertyName = reader.GetString();
                reader.Read();

                switch (propertyName)
                {
                    case "X": x = reader.GetSingle(); break;
                    case "Y": y = reader.GetSingle(); break;
                    case "Width": width = reader.GetSingle(); break;
                    case "Height": height = reader.GetSingle(); break;
                }
            }
        }

        return new RectangleF(x, y, width, height);
    }

    public override void Write(Utf8JsonWriter writer, RectangleF value, JsonSerializerOptions options)
    {
        // RectangleFをJSON形式で出力
        writer.WriteStartObject();
        writer.WriteNumber("X", value.X);
        writer.WriteNumber("Y", value.Y);
        writer.WriteNumber("Width", value.Width);
        writer.WriteNumber("Height", value.Height);
        writer.WriteEndObject();
    }
}
