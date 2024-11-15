using System.Collections.Frozen;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleApp;

internal class Program
{
    static async Task Main(string[] args)
    {
        var svcCollection = new ServiceCollection();

        //IConfigurationRootは、IConfigurationの拡張版。Reload()、すべての構成プロバイダへアクセスできる、などの機能がある。
        //それらを利用しない場合は、IConfigurationでOK
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .Build();

        svcCollection.AddSingleton<IConfiguration>(configuration);

        //引数が不要なケースは型パラメータのみ指定で対応可能
        //svcCollection.AddSingleton<IAlphabetToKanaService, AlphabetToReadingRomanKatakana>();

        //引数が必要な場合、ファクトリメソッドを渡す
        svcCollection.AddSingleton<IAlphabetToKanaService>(provider =>
        {
            //未登録の場合、InvalidOperationExceptionが発生
            var config = provider.GetRequiredService<IConfiguration>();
            //未登録の場合、nullを戻す。判定後処理を行う場合に使用する
            //var config = provider.GetService<IConfigurationRoot>();
            string dicFile = config["Dictionary"] ?? throw new Exception("appsettings.jsonにDictionaryが定義されていません");
            string jsonStr = File.ReadAllText(dicFile);
            Dictionary<char, string>? dic = JsonSerializer.Deserialize<Dictionary<char, string>>(jsonStr) ?? new();
            return new AlphabetToReadingRomanHirakana(dic);
        });

        var alphabetText = new List<string> { "I", "am", "a", "programmer" };
        using (var sp = svcCollection.BuildServiceProvider())
        {
            var converter = sp.GetRequiredService<IAlphabetToKanaService>();

            var dic = await converter.ToKatakana(alphabetText);

            foreach (var kv in dic)
            {
                Console.WriteLine($"{kv.Key} : {kv.Value}");
            }
        }

        Console.WriteLine("Press any ket to exit.");
        Console.ReadKey();
    }
}

public interface IAlphabetToKanaService
{
    Task<Dictionary<string, string>> ToKatakana(IEnumerable<string> words);
}

public class AlphabetToReadingRomanKatakana : IAlphabetToKanaService
{
    public async Task<Dictionary<string, string>> ToKatakana(IEnumerable<string> words)
    {
        var alphabetToKatakana = new Dictionary<char, string>
        {
            { 'a', "エイ" }, { 'b', "ビー" }, { 'c', "シー" }, { 'd', "ディー" },
            { 'e', "イー" }, { 'f', "エフ" }, { 'g', "ジー" }, { 'h', "エイチ" },
            { 'i', "アイ" }, { 'j', "ジェー" }, { 'k', "ケー" }, { 'l', "エル" },
            { 'm', "エム" }, { 'n', "エヌ" }, { 'o', "オー" }, { 'p', "ピー" },
            { 'q', "キュー" }, { 'r', "アール" }, { 's', "エス" }, { 't', "ティー" },
            { 'u', "ユー" }, { 'v', "ブイ" }, { 'w', "ダブリュー" }, { 'x', "エックス" },
            { 'y', "ワイ" }, { 'z', "ゼット" }
        };

        Dictionary<string, string> someResult = words.Distinct().ToDictionary(
            word => word,
            word => string.Concat(word.Select(c => alphabetToKatakana.GetValueOrDefault(char.ToLower(c)) ?? string.Empty))
        );

        var convertedAlphaWords = words.ToDictionary(w => w, w =>
        {
            return someResult[w];
        });
        return await Task.FromResult(convertedAlphaWords);
    }
}

public class AlphabetToReadingRomanHirakana : IAlphabetToKanaService
{
    private readonly FrozenDictionary<char, string> _dic;

    //private AlphabetToReadingRomanHirakana() { }

    public AlphabetToReadingRomanHirakana(Dictionary<char, string> dic)
    {
        _dic = dic.ToFrozenDictionary<char, string>();
    }

    public async Task<Dictionary<string, string>> ToKatakana(IEnumerable<string> words)
    {
        Dictionary<string, string> someResult = words.Distinct().ToDictionary(
            word => word,
            word => string.Concat(word.Select(c => _dic.GetValueOrDefault(char.ToLower(c)) ?? string.Empty))
        );

        var convertedAlphaWords = words.ToDictionary(w => w, w =>
        {
            return someResult[w];
        });
        return await Task.FromResult(convertedAlphaWords);
    }
}
