using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using hackingthebrowserapi.Models;
using Microsoft.AspNetCore.Mvc;
using PuppeteerSharp;

namespace hacking_the_browser_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BadgeController : ControllerBase
    {
        [HttpGet("{handle}")]
        public async Task<FileContentResult> Get(string handle)
        {
            var profilePage = $"https://www.twitter.com/{handle}";
            var badgeTemplate = "http://localhost:5000/badge.html";

            TwitterProfile profile = null;

            using (var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = false
            }))
            {
                using (var page = await browser.NewPageAsync())
                {
                    await page.GoToAsync(profilePage);
                    await page.WaitForSelectorAsync(".ProfileHeaderCard-nameLink");
                    profile = await page.EvaluateFunctionAsync<TwitterProfile>(@"() => {
                        return {
                            Name: document.querySelector('.ProfileHeaderCard-nameLink').innerText,
                            Bio: document.querySelector('.ProfileHeaderCard-bio').innerText,
                            ImageUrl: document.querySelector('.ProfileAvatar-image').src
                        };
                    }");

                    profile.Handle = handle;
                }

                using (var page = await browser.NewPageAsync())
                {
                    await page.GoToAsync(badgeTemplate);
                    await page.WaitForSelectorAsync("#name");
                    await page.EvaluateFunctionAsync<TwitterProfile>(@"(profile) => {
                        console.log(profile);
                        document.querySelector('#name').innerText = profile.Name;
                        document.querySelector('#handle').innerText = profile.Handle;
                        document.querySelector('#avatar').src = profile.ImageUrl;
                        document.querySelector('#bio').innerText = profile.Bio;
                    }", profile);

                    var element = await page.QuerySelectorAsync("#card");

                    return File(await element.ScreenshotDataAsync(), "image/png");
                }
            }
        }
    }
}
