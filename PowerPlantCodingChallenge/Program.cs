using System.Text.Json.Serialization;
using PowerPlantCodingChallenge.Config;
using PowerPlantCodingChallenge.Domain;
using PowerPlantCodingChallenge.Exceptions;
using PowerPlantCodingChallenge.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

{
    // OpenApi
    builder.Services.AddOpenApi();

    // Enable Controllers
    builder.Services.AddControllers()
    
    // Controller options
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

    // Expose Config
    builder.Services.Configure<AppConfig>(
        builder.Configuration.GetSection(AppConfig.SectionName));

    // Dependency Injection
    builder.Services.AddSingleton<ProductionPlanService>();
    builder.Services.AddSingleton<ProductionPlanSolver>();

    // Exception Handling
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();
    
    // Logging
    builder.Host.UseSerilog((context, config) =>
    {
        var logPath = Environment.GetEnvironmentVariable("LOG_PATH")
                      ?? Path.Combine(context.HostingEnvironment.ContentRootPath, "logs");
        
        Directory.CreateDirectory(logPath);

        config
            .WriteTo.Console()
            .WriteTo.File(
                Path.Combine(logPath, "log-.txt"),
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7);
    });
}


var app = builder.Build();

{
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.UseExceptionHandler();
}

app.Run();