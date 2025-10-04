namespace EventBus.Messages.Events;

public record ScrapeTaskScheduledEvent(
    Guid TaskId,
    string Url,
    string CssSelector
);