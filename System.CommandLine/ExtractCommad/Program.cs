using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Diagnostics;

namespace ExtractCommad;

internal class Program
{
    static void Main(string[] args)
    {
        var rootCmd = new RootCommand
        {
            Description = "検証目的：単なる文字列からコマンド部分と引数部分を分割できるか？"
        };

        rootCmd.AddArgument(new Argument<string>("commandLine", "引数を含む任意のコマンドライン全体"));

        //Invokeの前にセット
        rootCmd.Handler = CommandHandler.Create<string>(Execute);

        rootCmd.Invoke(args);
    }

    private static void Execute(string commandLine)
    {
        {
            //ここからが検証
            var root = new RootCommand();
            root.AddArgument(new Argument<string[]>("commandLineSegments", "引数を含む任意のコマンドライン全体"));
            string commandPart;
            string[] argsList;
            root.Handler = CommandHandler.Create<string[]>((commandLineSegments) =>
            {
                foreach (var part in commandLineSegments)
                {
                    Console.WriteLine(part);
                }
                commandPart = commandLineSegments[0];
                argsList = commandLineSegments[1..];
            });
            var ret = root.Invoke(commandLine); //Invokeで発動するようだ（root.Parse(commandline)メソッドでは発動しない？）
        }

        {
            //部品化（CommandLineParser）したので検証

            var parser = new CommandExtractor();
            {
                if (parser.TryExtract(commandLine, out string command, out string[] args))
                {
                    //commandとargsを分離できている。
                    //ProcessStartInfoに利用できる（はず）
                    var psi = new ProcessStartInfo();
                    psi.FileName = command;
                    Array.ForEach(args, psi.ArgumentList.Add);
                }
            }
            {
                if (parser.TryExtract("cmd.exe", out string command, out string[] args))
                {
                    //commandとargsを分離できている。
                    //ProcessStartInfoに利用できる（はず）
                    var psi = new ProcessStartInfo();
                    psi.FileName = command;
                    Array.ForEach(args, psi.ArgumentList.Add);
                }
            }

            {
                if (parser.TryExtract(null, out string command, out string[] args))
                {
                    //commandとargsを分離できている。
                    //ProcessStartInfoに利用できる（はず）
                    var psi = new ProcessStartInfo();
                    psi.FileName = command;
                    Array.ForEach(args, psi.ArgumentList.Add);
                }
            }
        }
    }

    public class CommandExtractor
    {
        private readonly RootCommand _rootCommand;
        public CommandExtractor()
        {
            _rootCommand = new RootCommand();
            _rootCommand.AddArgument(new Argument<string[]>("commandLineSegments", "引数を含む任意のコマンドライン全体"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandLine"></param>
        /// <param name="command"></param>
        /// <param name="args">成功した場合、argsの要素数は必ず0以上です。</param>
        /// <returns></returns>
        public bool TryExtract(string commandLine, out string command, out string[] args)
        {
            try
            {
                string inner_command = string.Empty;
                string[] inner_args = Array.Empty<string>();
                _rootCommand.Handler = CommandHandler.Create<string[]>((commandLineSegments) =>
                {
                    if (commandLineSegments.Length > 0)
                    {
                        inner_command = commandLineSegments[0];
                        inner_args = commandLineSegments[1..];
                        return 0;
                    }
                    else
                        return -1;
                });

                var ret = _rootCommand.Invoke(commandLine); //Invokeで発動するようだ（Parseメソッドでは発動しない？）
                command = inner_command;
                args = inner_args;
                return ret == 0;
            }
            catch (Exception)
            {
                command = string.Empty;
                args = Array.Empty<string>();
                return false;
            }
        }
    }
}