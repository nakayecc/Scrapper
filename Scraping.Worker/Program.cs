using MassTransit;
using Scraping.Worker;
using Scraping.Worker.Consumer;
using Scraping.Worker.Services;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Services.AddScoped<IScrapingService,ScrapingService>();


builder.Services.AddMassTransit(config =>
{
    config.AddConsumer<ScrapeTaskScheduledConsumer>();
    
    config.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMq:Host"], "/", host =>
        {
            host.Username(builder.Configuration["RabbitMq:Username"]);
            host.Password(builder.Configuration["RabbitMq:Password"]);
        });

        cfg.ReceiveEndpoint("scraping-task-scheduled-queue", c =>
        {
            c.ConfigureConsumer<ScrapeTaskScheduledConsumer>(ctx);
        });
    });
});


var host = builder.Build();
host.Run();