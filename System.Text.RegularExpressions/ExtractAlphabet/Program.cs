using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ExtractAlphabet;

internal class Program
{
    //TODO:コンパイル時にして高速化
    //[GeneratedRegex(@"[A-Z]?[a-z]+|[A-Z]+(?![a-z])")]
    //private static partial Regex AlphabetAndCamelCaseRegex();

    static void Main(string[] args)
    {
        var text = "私はAppleとOrangeが好き。Appleが一番、Orangeが二番。AppleOrageもOrangeAppleもappleOrangeは嫌い。";

        // アルファベット単語の検出
        //var matches = Regex.Matches(text, @"[A-Za-z]+");
        //var list = new List<string>();
        //foreach (Match match in matches)
        //{
        //    list.Add(match.Value);
        //}

        // 置換処理で {0}, {1}, ... に変換
        //var result = Regex.Replace(text, @"[A-Za-z]+", match =>
        //{
        //    int index = list.IndexOf(match.Value);
        //    return "{" + index + "}";
        //});

        //string regex = @"[A-Za-z]+";
        string regex = @"[A-Z]?[a-z]+|[A-Z]+(?![a-z])";　//アッパー・ローワーキャメルを分割
        var list = new List<string>();
        string textTemplate = Regex.Replace(text, regex, match =>
        {
            list.Add(match.Value);
            return $"{{{list.Count - 1}}}";
        });

        string reText = string.Format(textTemplate, list.ToArray());

        Console.WriteLine($"text = \"{text}\"");
        Console.WriteLine($"result = \"{textTemplate}\"");
        Console.WriteLine($"list = {string.Join(",", list)}");
        Console.WriteLine($"再構成 = \"{reText}\"");

        Console.ReadKey();
    }
}
