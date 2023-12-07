using System.Text.Json;

namespace Eval;

internal class Program
{
    static void Main(string[] args)
    {
        {
            var list = new List<KeyValuePair<string, object>>();
            list.Add(new(nameof(MyPluginA), new MyPluginA() { name_SelectedFigureItem = "a1", name_MyInteger = "1", name_MyDouble = "1.1" }));
            list.Add(new(nameof(MyPluginB), new MyPluginB() { name_MyCheckbox = "true" }));
            list.Add(new(nameof(MyPluginA), new MyPluginA() { name_SelectedFigureItem = "a2", name_MyInteger = "2", name_MyDouble = "2.2" }));

            JsonSerializerOptions option = new() { WriteIndented = true };
            var jsonStr = JsonSerializer.Serialize(list, option);
            Console.WriteLine(jsonStr);
        }

        {
            var jsonStr = File.ReadAllText(@".\data\json1.json");
            var seriOpts = new JsonSerializerOptions();
            //seriOpts.Converters.Add(new JsonIntConverter());
            //seriOpts.Converters.Add(new JsonDoubleConverter());
            var objList = JsonSerializer.Deserialize<List<KeyValuePair<string, JsonElement>>>(jsonStr)!;
            foreach (var i in objList)
            {
                Console.WriteLine(i.Value.GetType());
                if (i.Key == nameof(MyPluginA))
                {
                    var a = JsonSerializer.Deserialize<MyPluginA>(i.Value);
                }
                else if (i.Key == nameof(MyPluginB))
                {
                    var b = JsonSerializer.Deserialize<MyPluginB>(i.Value);
                }
            }
        }
    }
}

public class MyPluginA
{
    public string name_SelectedFigureItem { get; set; }
    public string name_MyInteger { get; set; }
    public string name_MyDouble { get; set; }
}

public class MyPluginB
{
    public string name_MyCheckbox { get; set; }
}
