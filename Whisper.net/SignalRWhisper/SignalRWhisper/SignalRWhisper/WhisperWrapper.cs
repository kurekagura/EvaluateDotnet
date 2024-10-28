// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Whisper.net;

namespace SignalRWhisper;

public interface IWhisperService
{
    IAsyncEnumerable<string> ProcessAsync(MemoryStream ms);
}

public class WhisperWrapper : IWhisperService, IDisposable
{
    private WhisperFactory? wFactory;
    private WhisperProcessor? wProcessor;
    private bool disposedValue;

    //public WhisperWrapper()

    public WhisperWrapper(string modelFilePath, string language)
    {
        //ここでWhisperFactoryをDisposeするとNG。
        wFactory = WhisperFactory.FromPath(modelFilePath);
        wProcessor = wFactory.CreateBuilder().WithLanguage(language).Build();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // マネージド状態を破棄します (マネージド オブジェクト)
                wProcessor?.Dispose();
                wFactory?.Dispose();
            }

            // TODO: アンマネージド リソース (アンマネージド オブジェクト) を解放し、ファイナライザーをオーバーライドします
            //大きなフィールドを null に設定します
            wProcessor = null;
            wFactory = null;
            disposedValue = true;
        }
    }

    // // TODO: 'Dispose(bool disposing)' にアンマネージド リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします
    // ~WhiperWrapper()
    // {
    //     // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public async IAsyncEnumerable<string> ProcessAsync(MemoryStream ms)
    {
        await foreach (var segData in wProcessor!.ProcessAsync(ms))
        {
            yield return segData.Text;
        }
    }
}
