// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.ResponseCompression;
using SignalRWhisper.Components;
using SignalRWhisper.Hubs;

namespace SignalRWhisper;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        #region SignalR
        //builder.Services.AddSingleton<WhisperWrapper>();
        //GlobalHost.DependencyResolver.Register(typeof(ChatHub),() => new ChatHub(new ChatMessageRepository()));
        builder.Services.AddSingleton<IWhisperService, WhisperWrapper>(sp =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();  // IConfigurationを取得
            string modelPath = configuration["whisperModelPath"] ?? throw new Exception();
            return new WhisperWrapper(modelPath, "ja");
        });

        builder.Services.AddSignalR(config =>
        {
            config.MaximumReceiveMessageSize = 1024 * 1024 * 10;
        });
        builder.Services.AddResponseCompression(opts =>
        {
            opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                ["application/octet-stream"]);
        });
        #endregion

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveWebAssemblyComponents();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseWebAssemblyDebugging();
        }
        else
        {
            app.UseExceptionHandler("/Error");
        }

        app.UseStaticFiles();
        app.UseAntiforgery();

        app.MapRazorComponents<App>()
            .AddInteractiveWebAssemblyRenderMode()
            .AddAdditionalAssemblies(typeof(Client._Imports).Assembly);

        // SignalRハブのエンドポイントを追加
        app.MapHub<WhisperHub>("/api/whisper");

        app.Run();
    }
}
