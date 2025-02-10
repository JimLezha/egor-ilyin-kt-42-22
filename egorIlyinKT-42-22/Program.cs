using NLog;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

try
{
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

// Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseAuthorization();

    app.MapRazorPages();

    app.Run();
}
catch(Exception ex)
{
    logger.Error(ex, "Stopped program because of exception");
}
finally
{
    LogManager.Shutdown();
}

// Add services to the container.

