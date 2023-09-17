using System.CommandLine;
using System.CommandLine.NamingConventionBinder;

namespace ArrayOption;

internal class Program
{
    static int Main(string[] args)
    {
        var rootCmd = CreateRootCommand();
        var errorLevel = rootCmd.Invoke(args);
        if (errorLevel != 0)
            Console.WriteLine("コマンドは失敗しました。");
        return errorLevel;
    }

    private static RootCommand CreateRootCommand()
    {
        var rootCmd = new RootCommand
        {
            Description = "Launch processes for multiple console applications and aggregate their output into a single console."
        };

        rootCmd.AddArgument(new Argument<string>("sessionId", "Session ID. Used when connecting from a client."));

        var startOpts = new Option<string[]>(aliases: new string[] { "--start", "-s" },
            description: "Specify the commands to start at launch. You can specify multiple commands.")
        { IsRequired = false };
        rootCmd.AddOption(startOpts);

        var endOpts = new Option<string[]>(aliases: new string[] { "--end", "-e" },
            description: "Specify commands to run on shutdown. You can specify multiple commands.")
        { IsRequired = false };
        rootCmd.AddOption(endOpts);

        var timerOpts = new Option<string[]>(aliases: new string[] { "--timer", "-t" },
            description: "Specify commands to run on shutdown. You can specify multiple commands.")
        { IsRequired = false };
        rootCmd.AddOption(timerOpts);

        //Invokeの前にセット
        rootCmd.Handler = CommandHandler.Create<string, string[], string[], string[]>(Execute);

        return rootCmd;
    }

    private static int Execute(string sessionId, string[] start, string[] end, string[] timer)
    {
        foreach (var str in timer)
        {
            if (TryParseTimerArgument(str, out (int, string)? result))
            {
                Console.WriteLine($"second={result!.Value.Item1},command={result!.Value.Item2}");
            }
        }
        return 0;
    }

    private static bool TryParseTimerArgument(string str, out (int second, string command)? result)
    {
        result = null;
        try
        {
            int firstSemicolonIndex = str.IndexOf(';');

            if (firstSemicolonIndex >= 0)
            {
                if (int.TryParse(str[..firstSemicolonIndex], out int second))
                {
                    string command = str[(firstSemicolonIndex + 1)..];
                    result = (second, command);
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }
        catch
        {
            return false;
        }
    }
}