using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Serilog;
using Microsoft.Extensions.Configuration;
using System.IO;
using HueSundownerService;

namespace HueSundownDaemon {
  public class Program {
    public static void Main(string[] args) {
      try {
        Console.WriteLine(AppContext.BaseDirectory);
        var builder = new ConfigurationBuilder()
         .SetBasePath(AppContext.BaseDirectory)
         .AddJsonFile("appsettings.json");
        var configuration = builder.Build();
        var serilogSettings = configuration.GetSection("SerilogSettings").Get<SerilogSettings>();

        Log.Logger = new LoggerConfiguration()
                        .MinimumLevel.Debug()
                        .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
                        .Enrich.FromLogContext()
                        .WriteTo.File(serilogSettings.LogFile)
#if DEBUG
                        .WriteTo.Console()
#endif
                        .CreateLogger();

        Log.Information("Starting service");
        CreateHostBuilder(args).Build().Run();
        return;
      }
      catch(Exception ex) {
        Log.Fatal(ex, "Error starting service");
      }
      finally {
        Log.CloseAndFlush();
      }
    }

    public static IHostBuilder CreateHostBuilder(string[] args) {
      return Host.CreateDefaultBuilder(args)
          .UseSystemd()
          .ConfigureServices((hostContext, services) => {
            services.AddHostedService<Worker>();
          })
          .UseSerilog();
    }
  }
}
