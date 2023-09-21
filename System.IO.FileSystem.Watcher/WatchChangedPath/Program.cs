namespace WatchChangedPath;

internal class Program
{
    /// <summary>
    /// ノードのパスの変更のみを検出する。
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    static int Main(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("第一引数に監視対象のディレクトリを指定して下さい。");
            return -1;
        }

        using var watcher = new FileSystemWatcher(args[0]);

        //パスの変更（移動・削除）のみを検出したい。
        watcher.NotifyFilter = NotifyFilters.DirectoryName
                             | NotifyFilters.FileName;

        //【重要】
        // 移動(階層変更)ではDeleted->Createdが発生する。
        // そのため、移動を判断するには、Deleted->Createdイベント間でオブジェクトが同一のものである判定が必要。
        // TODO：判定の方法　MFT (Master File Table)のFileIDをチェックする？
        //watcher.Changed += OnChanged;
        watcher.Deleted += OnDeleted;
        watcher.Created += OnCreated;
        watcher.Renamed += OnRenamed;
        watcher.Error += OnError;

        //watcher.Filter = "*.txt";
        watcher.IncludeSubdirectories = true;
        watcher.EnableRaisingEvents = true; //falseは、VSコンポーネントデザイナで利用する用のようです。

        Console.WriteLine("Press enter to exit.");
        Console.ReadLine();

        return 0;
    }

    private static void OnCreated(object sender, FileSystemEventArgs e)
    {
        Console.WriteLine($"Created: {e.FullPath}");
    }

    private static void OnDeleted(object sender, FileSystemEventArgs e)
    {
        Console.WriteLine($"Deleted: {e.FullPath}");
    }

    private static void OnRenamed(object sender, RenamedEventArgs e)
    {
        Console.WriteLine($"Renamed:");
        Console.WriteLine($"    Old: {e.OldFullPath}");
        Console.WriteLine($"    New: {e.FullPath}");
    }

    //private static void OnChanged(object sender, FileSystemEventArgs e)
    //{
    //    if (e.ChangeType != WatcherChangeTypes.Changed)
    //    {
    //        return;
    //    }
    //    Console.WriteLine($"Changed: {e.FullPath}");
    //}


    private static void OnError(object sender, ErrorEventArgs e) => PrintException(e.GetException());

    private static void PrintException(Exception? ex)
    {
        if (ex != null)
        {
            Console.WriteLine($"Message: {ex.Message}");
            Console.WriteLine("Stacktrace:");
            Console.WriteLine(ex.StackTrace);
            Console.WriteLine();
            PrintException(ex.InnerException); //InnerExceptionまで拾える。
        }
    }
}