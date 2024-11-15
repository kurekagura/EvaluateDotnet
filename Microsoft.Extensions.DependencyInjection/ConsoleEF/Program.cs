using System.Diagnostics;
using ConsoleEF.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleEF;

internal class Program
{
    static async Task Main(string[] args)
    {
        var services = new ServiceCollection();

        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .Build();

        services.AddSingleton<IConfiguration>(configuration);
        services.AddDbContextFactory<SampleDbContext>(
            (spProvider, optBuilder) =>
            {
                var config = spProvider.GetRequiredService<IConfiguration>();
                string connectionString = config.GetConnectionString("DefaultConnection") ?? throw new Exception();
                //string dbFilePath = Path.Combine(Directory.GetCurrentDirectory(), "SampleLocalDB.mdf");
                //connectionString = connectionString.Replace("SampleLocalDB.mdf", dbFilePath);
                optBuilder.UseSqlServer(connectionString);
            });

        var procId = Process.GetCurrentProcess().Id;
        var text = $"プロセスIDは{procId}です";

        using var spProvider = services.BuildServiceProvider();

        var newLog = new Mylog
        {
            Id = Guid.NewGuid(),
            ProcessId = procId.ToString(),
            TxtData = text,
            BinData = System.Text.Encoding.UTF8.GetBytes(text),
            Crt = DateTime.UtcNow,
            Upd = DateTime.UtcNow
        };

        try
        {
            //using var db = spProvider.GetRequiredService<SampleDbContext>();
            var dbFactory = spProvider.GetRequiredService<IDbContextFactory<SampleDbContext>>();
            using var db = dbFactory.CreateDbContext();
            //await db.Database.MigrateAsync();

            db.Mylogs.Add(newLog);
            var count = await db.SaveChangesAsync();
            Console.WriteLine($"{count}件 Insertしました");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
        }
        finally
        {
            Console.WriteLine("Hit any key to exit.");
            Console.ReadKey();
        }

    }
}

/// <summary>
/// これを実装しておくとマイグレーションが機能する
/// dotnet ef migrations add InitialCreate
/// 接続文字列はAddDbContextFactoryから取得している？
/// </summary>
public class SampleDbContextFactory : IDesignTimeDbContextFactory<SampleDbContext>
{
    public SampleDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<SampleDbContext>();
        optionsBuilder.UseSqlServer("YourConnectionString");

        return new SampleDbContext(optionsBuilder.Options);
    }
}
