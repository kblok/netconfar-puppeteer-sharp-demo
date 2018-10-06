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
    public class MediumController : ControllerBase
    {
        [HttpGet("{owner}/{post}")]
        public async Task<FileContentResult> Get(string author, string post)
        {
            var contributorsPage = $"https://medium.com/{author}/{post}";

            using (var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true
            }))
            using (var page = await browser.NewPageAsync())
            {
                await page.GoToAsync(contributorsPage);
                await page.WaitForSelectorAsync("HEADER");
                await page.EvaluateExpressionAsync("document.querySelector('HEADER').remove();");
                var pdf = await page.PdfDataAsync();
                return File(pdf, "application/pdf");
            }
        }
    }
}
