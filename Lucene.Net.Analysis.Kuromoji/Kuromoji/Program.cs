using System.Text;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Ja;
using Lucene.Net.Analysis.Ja.Dict;
using Lucene.Net.Analysis.Ja.TokenAttributes;
using Lucene.Net.Analysis.TokenAttributes;

namespace Kuromoji;

internal class Program
{
    static async Task Main(string[] args)
    {
        try
        {
            var text = await File.ReadAllTextAsync(@"..\..\..\PrivateJunkData\20241121_kuromoji_text.txt");
            var dict = GetUserDictionaryFromFile(@"..\..\..\PrivateJunkData\20241121_kuromoji_dict.txt");
            //var dict = GetUserDictionaryFromCode();

            using var reader = new StringReader(text);
            //トークナイザ作成
            using var tokenizer = new JapaneseTokenizer(reader, dict, false, JapaneseTokenizerMode.NORMAL);
            var tsc = new TokenStreamComponents(tokenizer);

            var ts = tsc.TokenStream;

            ts.Reset();

            var sb = new StringBuilder();
            var replaced = new StringBuilder();

            while (ts.IncrementToken())
            {
                var term = ts.GetAttribute<ICharTermAttribute>();
                var partOfSpeech = ts.AddAttribute<IPartOfSpeechAttribute>();
                var reading = ts.AddAttribute<IReadingAttribute>();

                string strTerm = term.ToString();
                string strPartOfSpeech = partOfSpeech.GetPartOfSpeech();
                string strReading = reading.GetReading();

                Console.WriteLine($"用語:{strTerm} 品詞:{strPartOfSpeech} 読み:{strReading}");

                sb.Append(strTerm);
                replaced.Append(strPartOfSpeech != "UNKNOWN" ? strTerm : strReading);
            }
            Console.WriteLine();

            Console.WriteLine($"原文:{Environment.NewLine}{text}{Environment.NewLine}");
            Console.WriteLine($"再構築:{Environment.NewLine}{sb}{Environment.NewLine}");
            Console.WriteLine($"置換後:{Environment.NewLine}{replaced}{Environment.NewLine}");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
        }
        finally
        {
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }

    private static UserDictionary GetUserDictionaryFromCode()
    {
        string dict =
    @"
.NET,.NET,ドットネット,カスタム名詞
C#,C#,シーシャープ,カスタム名詞
";

        using var reader = new StringReader(dict);
        return new UserDictionary(reader);
    }

    private static UserDictionary GetUserDictionaryFromFile(string dictFilePath)
    {
        using var reader = new StreamReader(dictFilePath, Encoding.UTF8);
        return new UserDictionary(reader);
    }
}
