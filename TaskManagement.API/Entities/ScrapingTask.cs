namespace TaskManagement.API.Entities;

public class ScrapingTask
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public string Url { get; set; }
    public string CssSelector { get; set; }
    public string TargetValue { get; set; }
    public int IntervalInMinutes { get; set; }
    public DateTime CreatedAt { get; set; }
    
}