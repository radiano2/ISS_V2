using System;
using ISS.Application;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;


Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithExceptionDetails()
    .WriteTo.Console()
    .WriteTo.File("logs/ISS-.log", rollingInterval: RollingInterval.Day,
        shared: false, restrictedToMinimumLevel: LogEventLevel.Information, retainedFileCountLimit: 7,
        fileSizeLimitBytes: 15728640 //15 MB
    )
    .CreateLogger();


try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddTransient<ISSService>();

    var app = builder.Build();

    app.MapControllers();
    app.Run();


}
catch (Exception ex)
{
    Log.Error(ex, "Something went wrong");
}
finally
{
    Log.CloseAndFlush();
}