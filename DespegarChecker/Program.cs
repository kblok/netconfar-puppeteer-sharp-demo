using System;
using System.Threading.Tasks;
using PuppeteerSharp;

namespace DespegarChecker
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);

                while (true)
                {
                    var url = "https://www.despegar.com.ar/shop/flights/results/roundtrip/BUE/MDZ/2018-12-01/2018-12-08/1/0/0/NA/NA/NA/NA/NA?from=SB&di=1-0";
                    using (var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = false }))
                    using (var page = await browser.NewPageAsync())
                    {
                        await page.GoToAsync(url, WaitUntilNavigation.Networkidle0);
                        await page.WaitForSelectorAsync("buy-button");

                        var bestPrice = await page.EvaluateFunctionAsync<string>(@"() => {
                            var elements = document.querySelectorAll('.main-content .price-amount');
                            return elements.length ? elements[0].innerText : '0';
                        }");

                        Console.WriteLine($"Best price for Mendoza {bestPrice}");
                        await Task.Delay(60000);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
            }

        }
    }
}
