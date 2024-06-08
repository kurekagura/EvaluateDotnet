// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Globalization;
using NodaTime;
using YahooQuotesApi;

namespace EvalYahooQuotesApi
{
    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
            //{
            //    var startData = Instant.FromUtc(2024, 1, 1, 0, 0);

            //    YahooQuotes yahooQuotes = new YahooQuotesBuilder()
            //        .WithHistoryStartDate(startData)
            //        .Build();

            //    var security = await yahooQuotes.GetAsync("MSFT", Histories.PriceHistory) ?? throw new ArgumentException("Unknown symbol.");

            //    Console.WriteLine(security.FullExchangeName);

            //    var priceTick = security.PriceHistory.Value;
            //    foreach (var tick in priceTick)
            //    {
            //        Console.WriteLine($"{tick.Date},{tick.Open},{tick.High},{tick.Low},{tick.Close},{tick.Volume}");
            //    }
            //}

            Console.WriteLine();

            {
                var startData = Instant.FromUtc(2024, 1, 1, 0, 0);

                YahooQuotes yahooQuotes = new YahooQuotesBuilder()
                    .WithHistoryStartDate(startData)
                    .Build();

                var security = await yahooQuotes.GetAsync("7202.T", Histories.PriceHistory) ?? throw new ArgumentException("Unknown symbol.");

                Console.WriteLine(security.FullExchangeName);
                Console.WriteLine($"通貨コード={security.Currency}");

                Console.WriteLine($"長い名前={security.LongName}");
                Console.WriteLine($"短い名={security.ShortName}");
                Console.WriteLine($"配当利回り={security.DividendYield}");
                Console.WriteLine($"配当={security.DividendRate}");
                Console.WriteLine($"過去の配当利回り={security.TrailingAnnualDividendYield}");
                Console.WriteLine($"過去の配当={security.TrailingAnnualDividendRate}");

                var priceTick = security.PriceHistory.Value;
                foreach (var tick in priceTick)
                {
                    //移動平均値はtick毎には無い（securityにあるのは最新のものか？）
                    Console.WriteLine($"{tick.Date.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture)},{tick.Open},{tick.High},{tick.Low},{tick.Close},{tick.Volume},調整後終値={tick.AdjustedClose}");
                }
            }

            Console.ReadLine();
            return await Task.FromResult(0);
        }
    }
}
