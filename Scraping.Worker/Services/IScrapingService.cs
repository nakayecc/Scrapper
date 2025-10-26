namespace Scraping.Worker.Services;

public interface IScrapingService
{
    Task<string?> ScrapeDataAsync(string url, string cssSelector);
}