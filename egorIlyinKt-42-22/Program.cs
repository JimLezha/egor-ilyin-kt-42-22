using Microsoft.AspNetCore.Diagnostics;
//using egorIlyinKT_42_22.Middleware;
using Microsoft.EntityFrameworkCore;
using egorIlyinKT_42_22.ServiceExtensions;
using egorIlyinKT_42_22.Database;
using NLog;
using NLog.Web;


var builder = WebApplication.CreateBuilder(args);

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();


try
{

    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    // Add services to the container.

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddDbContext<UniversityContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    builder.Services.AddServices();
    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    /*app.UseMiddleware<ExceptionHandlerMiddleware>();*/

    app.UseAuthorization();

    app.MapControllers();

    app.Run();


}

catch (Exception ex)
{
    logger.Error(ex, "Stopped program because of exeption");
}

finally
{
    LogManager.Shutdown();
}