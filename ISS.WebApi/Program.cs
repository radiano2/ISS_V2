using System;
using ISS.Application;
using ISS.WebApi.Handlers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using ILogger = Microsoft.Extensions.Logging.ILogger;

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


namespace ISS.WebApi
{
    public class Program
    {
        private readonly ISSService _issService;

        public static void Main(string[] args)
        {
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
        }
    }
}