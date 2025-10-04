using EventBus.Messages.Events;
using Hangfire;
using MassTransit;

namespace Scheduler.Worker.Consumers;

public class TaskCreatedConsumer : IConsumer<TaskCreatedEvent>
{
    private readonly ILogger<TaskCreatedConsumer> _logger;
    private readonly IPublishEndpoint _publishEndpoint; 

    public TaskCreatedConsumer(ILogger<TaskCreatedConsumer> logger, IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Consume(ConsumeContext<TaskCreatedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Scheduler: Odebrano zdarzenie TaskCreatedEvent dla zadania: {TaskId}", message.TaskId);
        
        RecurringJob.AddOrUpdate(
            $"scrape-job-{message.TaskId}",
            () => PublishScrapeTaskScheduledEvent(message.TaskId, message.Url, message.CssSelector),
            $"*/{message.IntervalInMinutes} * * * *",
            new RecurringJobOptions 
            {
                TimeZone = TimeZoneInfo.Utc
            }
        );

        _logger.LogInformation("Scheduler: Zaplanowano zadanie scrapingu {TaskId} dla {Url} co {Interval} minut.", 
            message.TaskId, message.Url, message.IntervalInMinutes);
    }
    
    [AutomaticRetry(Attempts = 3)] 
    public async Task PublishScrapeTaskScheduledEvent(Guid taskId, string url, string cssSelector)
    {
        _logger.LogInformation("Scheduler: Hangfire uruchamia publikację ScrapeTaskScheduledEvent dla zadania: {TaskId}", taskId);
        
        var eventMessage = new ScrapeTaskScheduledEvent(taskId, url, cssSelector);
        await _publishEndpoint.Publish(eventMessage);
        
        _logger.LogInformation("Scheduler: Opublikowano ScrapeTaskScheduledEvent dla zadania: {TaskId}", taskId);
    }
}