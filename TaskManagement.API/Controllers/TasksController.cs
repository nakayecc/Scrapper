using EventBus.Messages.Events;
using Microsoft.AspNetCore.Mvc;
using MassTransit;
using TaskManagement.API.Data;
using TaskManagement.API.Entities;
using TaskManagement.API.Models;

namespace TaskManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController: ControllerBase
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ApplicationDbContext _dbContext;
    
    public TasksController(IPublishEndpoint publishEndpoint, ApplicationDbContext dbContext)
    {
        _publishEndpoint = publishEndpoint;
        _dbContext = dbContext;
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto taskDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var newTask = new ScrapingTask()
        {
            Id = Guid.NewGuid(),
            UserId = "some-user-id", // JWT
            Url = taskDto.Url,
            CssSelector = taskDto.CssSelector,
            TargetValue = taskDto.TargetValue,
            IntervalInMinutes = taskDto.IntervalInMinutes,
            CreatedAt = DateTime.UtcNow
        };
        _dbContext.ScrapingTasks.Add(newTask);
        await _dbContext.SaveChangesAsync();
        
        var eventMessage = new TaskCreatedEvent(
            newTask.Id,
            newTask.UserId,
            newTask.Url,
            newTask.CssSelector,
            newTask.TargetValue,
            newTask.IntervalInMinutes
        );

        await _publishEndpoint.Publish(eventMessage);
        
        return Accepted(new { Id = newTask.Id });
    }
}