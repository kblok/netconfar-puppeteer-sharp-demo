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

                var url = "https://twitter.com/search?f=tweets&vertical=default&q=netconfar&src=typd";
                using (var browser = await Puppeteer.LaunchAsync(new LaunchOptions
                {
                    Headless = false,
                    UserDataDir = "/Users/neo/Library/Application Support/Google/Chrome/Default"
                }))
                using (var page = await browser.NewPageAsync())
                {
                    await page.GoToAsync(url, WaitUntilNavigation.Networkidle0);
                    await page.WaitForSelectorAsync(".js-stream-tweet");

                    var linkButtons = await page.QuerySelectorAllAsync(".js-stream-tweet:not(.favorited) .js-actionFavorite");
                    await linkButtons[0].ClickAsync();
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
