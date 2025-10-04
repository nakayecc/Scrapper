namespace EventBus.Messages.Events;

public record DataScrapedEvent(
    Guid TaskId,
    string Url,
    string ScrapedValue,
    DateTime ScrapedAt
);