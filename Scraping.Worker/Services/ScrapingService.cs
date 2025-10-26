namespace Scraping.Worker.Services;
using Microsoft.Playwright;

public class ScrapingService : IScrapingService
{
    public async Task<string?> ScrapeDataAsync(string url, string cssSelector)
    {
        using var playwright = await Playwright.CreateAsync();
        
        await using var browser = await playwright.Chromium.LaunchAsync(new() { Headless = true });
        
        var page = await browser.NewPageAsync();
        await page.GotoAsync(url);

        try
        {
            var priceLocator = page.Locator(cssSelector);
            var priceText = await priceLocator.InnerTextAsync();
        
            return priceText.Trim();
        }
        catch (TimeoutException)
        {
            return null;
        }
        
       
        
        

    }
}