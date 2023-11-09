using Microsoft.Extensions.Configuration;
using MyModels;

namespace ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var connStr = config.GetConnectionString("DefaultConnection");

            Console.WriteLine(connStr);

            var logLevel_a = config["Logging:LogLevel:Default"];
            Console.WriteLine(logLevel_a);

            var logLevel_b = config["Logging:LogLevel:Microsoft.AspNetCore"];
            Console.WriteLine(logLevel_b);

            //Microsoft.Extensions.Configuration.Binder
            var logging = config.GetSection("Logging").Get<Logging>();
        }
    }
}

namespace MyModels
{
    public class Logging
    {
        public LogLevel? LogLevel { get; set; }
    }

    public class LogLevel
    {
        public string? Default { set; get; }
        //キーにドットを含むとモデルバインドできない（？）
        //public string MicrosoftAsp.NetCore { set; get; }
    }
}

