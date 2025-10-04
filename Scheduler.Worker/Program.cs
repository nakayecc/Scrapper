using Hangfire;
using Hangfire.PostgreSql;
using MassTransit;
using Scheduler.Worker.Consumers;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMassTransit(config =>
{
    config.AddConsumer<TaskCreatedConsumer>();

    config.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMq:Host"], "/", host =>
        {
            host.Username(builder.Configuration["RabbitMq:Username"]);
            host.Password(builder.Configuration["RabbitMq:Password"]);
        });

        cfg.ReceiveEndpoint("scheduler-task-created-queue", c =>
        {
            c.ConfigureConsumer<TaskCreatedConsumer>(ctx);
        });
    });
});

builder.Services.AddHangfire(configuration => configuration
    .UsePostgreSqlStorage(builder.Configuration.GetConnectionString("HangfireConnection")));

builder.Services.AddHangfireServer();

builder.Services.AddSingleton<IPublishEndpoint>(provider => provider.GetRequiredService<IBusControl>());

var host = builder.Build();
host.Run();