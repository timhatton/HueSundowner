using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Serilog;

namespace HueSundownerService {
  public class Program {
    public static void Main(string[] args) {
      Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Debug()
        .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
        .Enrich.FromLogContext()
        .WriteTo.File(@"c:\HueSundownerLogs\huesundowner.log")
#if DEBUG
        .WriteTo.Console()
#endif
        .CreateLogger();

      try {
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
          .ConfigureServices((hostContext, services) => {
            services.AddHostedService<Worker>();
          })
          .UseSerilog();
    }
  }
}
