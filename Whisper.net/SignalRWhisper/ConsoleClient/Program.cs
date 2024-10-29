using System.Diagnostics;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;

namespace ConsoleClient;

internal class Program
{
    private static readonly TaskCompletionSource<bool> s_messageReceivedTcs = new TaskCompletionSource<bool>();

    static async Task Main(string[] args)
    {
        HubConnection? hubConnection = null;
        try
        {
            var config = BuildAppSettings(Directory.GetCurrentDirectory());
            string host = config["whisperApi:host"] ?? throw new Exception();
            int? port = config.GetSection("whisperApi:port").Get<int?>();
            string path = config["whisperApi:path"] ?? string.Empty;
            string? apiKey = config["whisperApi:apiKey"];

            string url = $"http://{host}{(port == null ? string.Empty : $":{port}")}{(string.IsNullOrEmpty(path) ? "" : "/" + path)}";
            hubConnection = new HubConnectionBuilder()
                //.WithUrl(url)
                .WithUrl(url, options =>
                {
                    if (apiKey != null)
                        options.Headers.Add("X-API-KEY", apiKey);
                })
                .WithAutomaticReconnect()
                .Build();

            try
            {
                await hubConnection.StartAsync();
            }
            catch (HttpRequestException)
            {
                throw;
            }

            hubConnection.On<string>(nameof(ReceiveMessage), ReceiveMessage);

            //byte[] messageBytes = Encoding.UTF8.GetBytes("こんにちは");

            //string wavPath = ".input\\60s_16KHz.wav";
            string wavPath = ".input\\03s_16KHz.wav";

            byte[] wavData = File.ReadAllBytes(wavPath);

            var sw = new Stopwatch();
            sw.Start();
            await hubConnection.SendAsync("SendMessage", wavData);
            await s_messageReceivedTcs.Task;  // ReceiveMessageが完了するまで待機
            sw.Stop();
            Console.WriteLine($"{Environment.NewLine}要求・応答時間(秒)：{sw.Elapsed.TotalSeconds}");

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
        }
        finally
        {
            await hubConnection!.DisposeAsync();
        }

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    private static void ReceiveMessage(string message)
    {
        var encodedMsg = $"{message}";
        Console.WriteLine(encodedMsg);
        // メッセージ受信後、TaskCompletionSource を完了させる
        s_messageReceivedTcs.TrySetResult(true);
    }

    public static IConfiguration BuildAppSettings(string basePath)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        return configuration;
    }
}
