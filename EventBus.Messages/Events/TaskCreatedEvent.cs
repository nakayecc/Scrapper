namespace EventBus.Messages.Events;

public record TaskCreatedEvent(
    Guid TaskId, 
    string UserId,
    string Url, 
    string CssSelector, 
    string TargetValue,
    int IntervalInMinutes
);