using System.CommandLine.NamingConventionBinder;
using System.CommandLine;

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
            int firstSemicolonIndex = str.IndexOf(';');

            if (firstSemicolonIndex >= 0)
            {
                if (int.TryParse(str[..firstSemicolonIndex], out int second))
                {
                    string part2 = str[(firstSemicolonIndex + 1)..];
                }
                else
                    throw new ArgumentException(nameof(timer));
            }
            else
                throw new ArgumentException(nameof(timer));
        }
        return 0;
    }
}