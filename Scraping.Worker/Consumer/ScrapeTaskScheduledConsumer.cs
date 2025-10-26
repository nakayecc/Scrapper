using EventBus.Messages.Events;
using MassTransit;
using Scraping.Worker.Services;

namespace Scraping.Worker.Consumer;

public class ScrapeTaskScheduledConsumer :  IConsumer<ScrapeTaskScheduledEvent>
{
    
    private readonly ILogger<ScrapeTaskScheduledEvent> _logger;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IScrapingService _scrapingService;

    public ScrapeTaskScheduledConsumer(ILogger<ScrapeTaskScheduledEvent> logger, IPublishEndpoint publishEndpoint, IScrapingService scrapingService)
    {
        _logger = logger;
        _publishEndpoint = publishEndpoint;
        _scrapingService = scrapingService;
    }


    public async Task Consume(ConsumeContext<ScrapeTaskScheduledEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Start Scraping for task: {TaskId}", message.TaskId);


        try
        {
            var scrapeValue = await _scrapingService.ScrapeDataAsync(context.Message.Url, context.Message.CssSelector);

            if (!string.IsNullOrEmpty(scrapeValue))
            {
                _logger.LogInformation("Scrape task completed: {TaskId}, value find {value}", message.TaskId, scrapeValue);
                var result = new DataScrapedEvent(message.TaskId, message.Url, scrapeValue,DateTime.UtcNow);
                await _publishEndpoint.Publish(result);
            }
            else
            {
                _logger.LogWarning("Value not found for this task: {TaskId}, for url {Url}", message.TaskId, message.Url);
                
            }
            
            
        }
        catch (Exception e)
        {
            _logger.LogError("Error occured while scrape task: {TaskId}, for url {url}", message.TaskId, message.Url);
        }
        
        
        
        
    }
}