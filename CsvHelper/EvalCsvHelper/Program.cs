using System.Globalization;
using System.Text.Json;
using CsvHelper;

namespace EvalCsvHelper;

internal class Program
{
    static void Main(string[] args)
    {
        string csvRelativePath = @"..\..\..\deposit.csv";
        // 相対パスを絶対パスに変換
        string csvPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), csvRelativePath));
        string dirPath = Path.GetDirectoryName(csvPath) ?? throw new InvalidDataException(nameof(csvPath));
        string jsonPath = Path.Combine(dirPath, Path.GetFileNameWithoutExtension(csvPath) + ".json");

        using (var reader = new StreamReader(csvPath))
        using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            var list = csvReader.GetRecords<Deposit>().ToList();

            foreach (var row in list)
            {
                Console.WriteLine($"{row.RegistrationDate},{row.UserID},{row.Name},{row.Gender},{row.Age},{row.Total},{row.Birthday}");
            }

            string json = JsonSerializer.Serialize(list, new JsonSerializerOptions { WriteIndented = true });
            // JSONデータをファイルに書き込む
            File.WriteAllText(jsonPath, json);
        }

        Console.ReadLine();
    }
}

public class Deposit
{
    public DateTime RegistrationDate { get; set; }
    public required string UserID { get; set; }
    public required string Name { get; set; }
    public string? Gender { get; set; }
    public int Age { get; set; }
    public decimal Total { get; set; }
    public DateTime Birthday { get; set; }
}


