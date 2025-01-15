using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Order_Processing_Worker_Service.EFCore.Data;
using Order_Processing_Worker_Service.Infrastructure;
using Order_Processing_Worker_Service.Infrastructure.Services;
using Quartz;
using Quartz.Impl;
using Serilog;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        // Load configuration from appsettings.json and environment-specific files
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
              .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true)
              .AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        // Get connection string from appsettings.json
        var connectionString = context.Configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
        }

        // Configure EF Core to use SQL Server
        services.AddDbContext<OrderDbContext>(options =>
            options.UseSqlServer(connectionString));

        // Register application services
        services.AddHttpClient<IApiOrderService, ApiOrderService>();
        services.AddScoped<DbOrderService>();

        // Configure Quartz for job scheduling
        services.AddQuartz(q =>
        {
            q.UseMicrosoftDependencyInjectionJobFactory();

            // Schedule the SchedulerJob
            q.ScheduleJob<SchedulerJob>(trigger => trigger
                .WithIdentity("OrderProcessingJob", "Group1")
                .StartNow()
                .WithSimpleSchedule(schedule => schedule
                    .WithIntervalInMinutes(15)
                    .RepeatForever()));
        });

        services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });

        services.AddTransient<SchedulerJob>();
    })
    .UseSerilog((context, config) =>
    {
        config.ReadFrom.Configuration(context.Configuration)
              .WriteTo.Console()
              .WriteTo.File("Logs/myapp-.log", rollingInterval: RollingInterval.Day);
    })
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
        logging.AddDebug();
    })
    .Build();

await host.RunAsync();
