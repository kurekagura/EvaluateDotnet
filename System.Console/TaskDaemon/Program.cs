using System.Threading;

namespace TaskDaemon
{
    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, e) =>
            {
                Console.WriteLine("Ctrl+C has been pressed.");
                cts.Cancel();
                e.Cancel = true;
            };

            int errorlevel = await MyTask(cts);
            Console.WriteLine($"MyTaskの戻り値：{errorlevel}");
            Console.WriteLine("Please press any key.");
            Console.ReadKey();
            return errorlevel;
        }

        private static async Task<int> MyTask(CancellationTokenSource cancelTokenSource)
        {
            while (!cancelTokenSource.IsCancellationRequested)
            {
                try
                {
                    Console.WriteLine($"開始日時：{DateTime.Now:yyyy_MM_dd_HH_mm_ss}");
                    Console.WriteLine("キャンセル受付する非同期処理を開始");
                    await Task.Delay(TimeSpan.FromSeconds(5), cancelTokenSource.Token);
                    Console.WriteLine("キャンセル受付しない非同期処理を開始");
                    await Task.Delay(TimeSpan.FromSeconds(5));
                    Console.WriteLine($"終了日時：{DateTime.Now:yyyy_MM_dd_HH_mm_ss}");
                }
                catch (OperationCanceledException)
                {
                    return -1; //loop内の処理中に強制終了で抜ける
                }
            }
            return 0; //loop内の処理完了まで待って抜ける
        }
    }
}