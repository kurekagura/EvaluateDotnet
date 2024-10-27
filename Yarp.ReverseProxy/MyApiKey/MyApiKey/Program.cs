// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using MyApiKey.Client.Pages;
using MyApiKey.Components;

namespace MyApiKey;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveWebAssemblyComponents();

        // Yarp
        builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

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

        // APIキーを検証するミドルウェアをYARPの前に追加
        // /api-vvへのリクエストに対してのみApiKeyMiddlewareを適用
        //app.UseMiddleware<ApiKeyMiddleware>();
        //app.MapWhen(
        //    context => context.Request.Path.StartsWithSegments("/api/vv"),
        //    builder => builder.UseMiddleware<ApiKeyMiddleware>()
        //);
        //Yarp
        app.MapReverseProxy();

        app.MapRazorComponents<App>()
            .AddInteractiveWebAssemblyRenderMode()
            .AddAdditionalAssemblies(typeof(Client._Imports).Assembly);

        app.Run();
    }
}

public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly HashSet<string> _expectedApiKeys;
    private const string ApiKeyHeaderName = "X-API-KEY";

    public ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;

        // appsettings.jsonからExpectedApiKeysを取得し、HashSetに変換
        _expectedApiKeys = configuration.GetSection("ExpectedApiKeys")
                                        .Get<HashSet<string>>() ?? new();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // リクエストヘッダーにAPIキーが含まれ、許可されたキーリストに存在するかを確認
        if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey) ||
            !_expectedApiKeys.Contains(extractedApiKey!))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Unauthorized");
            return;
        }

        await _next(context); // 検証をパスした場合、次のミドルウェアに渡す
    }
}
