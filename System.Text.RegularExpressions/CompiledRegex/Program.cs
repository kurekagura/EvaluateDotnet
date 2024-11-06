using System.Text.Json;
using System.Text.RegularExpressions;

namespace CompiledRegex;

internal class Program
{
    static void Main(string[] args)
    {
        //var text = ".NET C#のregExやExpressionTreeは便利。Win32も使える。.NETの有名な言語はC#";
        string text = File.ReadAllText("Text.txt", System.Text.Encoding.UTF8);

        var keywords = new List<string>(File.ReadAllLines("辞書.txt"));
        var myRegex = new MyRegex(keywords);

        string replacedText;
        List<string> dictWords;
        myRegex.ReplaceWithDictionary(text, out replacedText, out dictWords);

        string replacedText2;
        List<string> alphaWords;
        myRegex.ExtractAlphabeticWords(replacedText, dictWords.Count, out replacedText2, out alphaWords);
        var usedDict = alphaWords.Distinct().ToDictionary(word => word, word => string.Empty);
        string usedDictJsonStr = JsonSerializer.Serialize(usedDict);
        File.WriteAllText("ExtractedWords.json", usedDictJsonStr);

        //仮
        Dictionary<string, string> someResult = alphaWords.Distinct().ToDictionary(word => word, word => word.ToUpperInvariant());
        //someResult.Remove("Win"); //テスト

        IEnumerable<string> convertedAlphaWords = alphaWords.Select<string, string>(w =>
        {
            return someResult.GetValueOrDefault(w) ?? string.Empty; //存在しない場合、空文字
        });

        IEnumerable<string> ite = dictWords.Concat(convertedAlphaWords);
        //var array = ite.ToArray();
        string reconstruct = string.Format(replacedText2, ite.ToArray());
        Console.WriteLine(reconstruct);
        Console.WriteLine("Hit any key to exit.");
        Console.ReadKey();
    }
}

public class MyRegex
{
    private readonly Regex _regexDict;
    private readonly Regex _regexAlpha;

    public MyRegex(List<string> keywords)
    {
        // 辞書単語を正規表現して結合、事前コンパイル
        var pattern = string.Join("|", keywords);
        _regexDict = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        _regexAlpha = new Regex(@"[A-Z]?[a-z]+|[A-Z]+(?![a-z])", RegexOptions.Compiled); //アッパー・ローワーキャメルケースで分割
    }

    public void ReplaceWithDictionary(string text, out string replacedText, out List<string> words)
    {
        var localWords = new List<string>();
        words = new List<string>();
        replacedText = _regexDict.Replace(text, match =>
        {
            localWords.Add(match.Value);
            return $"{{{localWords.Count - 1}}}";
        });
        words = localWords;
    }

    public void ExtractAlphabeticWords(string text, int baseIndex, out string replacedText, out List<string> words)
    {
        var localWords = new List<string>();
        words = new List<string>();
        replacedText = _regexAlpha.Replace(text, match =>
        {
            localWords.Add(match.Value);
            return $"{{{baseIndex + localWords.Count - 1}}}";
        });
        words = localWords;
    }
}
