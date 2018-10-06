using System;
using System.Threading.Tasks;
using PuppeteerSharp;
using Xunit;

namespace DespegarTests
{
    public class CheckoutTests
    {
        [Fact]
        public async Task ShouldHonorThePrice()
        {
            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);

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

                var clickElement = await page.EvaluateExpressionHandleAsync(@"
                    document.querySelectorAll('.main-content buy-button:first-child A')[0]") as ElementHandle;

                await clickElement.ClickAsync();
                await page.WaitForSelectorAsync(".price-container .amount");
                var checkoutPrice = await page.EvaluateExpressionAsync<string>(@"
                    document.querySelectorAll('.price-container .amount')[0].innerText
                ");

                Assert.Equal(bestPrice, checkoutPrice);
            }
        }
    }
}
