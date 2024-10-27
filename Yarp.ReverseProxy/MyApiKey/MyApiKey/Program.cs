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

        // API�L�[�����؂���~�h���E�F�A��YARP�̑O�ɒǉ�
        // /api-vv�ւ̃��N�G�X�g�ɑ΂��Ă̂�ApiKeyMiddleware��K�p
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

        // appsettings.json����ExpectedApiKeys���擾���AHashSet�ɕϊ�
        _expectedApiKeys = configuration.GetSection("ExpectedApiKeys")
                                        .Get<HashSet<string>>() ?? new();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // ���N�G�X�g�w�b�_�[��API�L�[���܂܂�A�����ꂽ�L�[���X�g�ɑ��݂��邩���m�F
        if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey) ||
            !_expectedApiKeys.Contains(extractedApiKey!))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Unauthorized");
            return;
        }

        await _next(context); // ���؂��p�X�����ꍇ�A���̃~�h���E�F�A�ɓn��
    }
}
