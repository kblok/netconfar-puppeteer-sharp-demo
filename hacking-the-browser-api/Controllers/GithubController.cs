using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PuppeteerSharp;

namespace hacking_the_browser_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GithubController : ControllerBase
    {
        [HttpGet("{owner}/{repo}")]
        public async Task<FileContentResult> Get(string owner, string repo)
        {
            var contributorsPage = $"https://github.com/{owner}/{repo}/graphs/contributors";

            using (var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true
            }))
            using (var page = await browser.NewPageAsync())
            {
                await page.GoToAsync(contributorsPage);
                await page.WaitForSelectorAsync("#contributors");
                var element = await page.QuerySelectorAsync("#contributors");
                var image = await element.ScreenshotDataAsync();

                return File(image, "image/png");
            }
        }
    }
}
