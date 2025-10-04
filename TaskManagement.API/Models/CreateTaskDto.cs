using System.ComponentModel.DataAnnotations;

namespace TaskManagement.API.Models;

public class CreateTaskDto
{
    [Required]
    public string Url { get; set; }
    [Required]
    public string CssSelector { get; set; }
    [Required]
    public string TargetValue { get; set; }
    [Range(1, 59)] 
    public int IntervalInMinutes { get; set; } = 59; 
}