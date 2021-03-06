using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Serilog;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace HueSundownerService {
  public class Program {
    public static void Main(string[] args) {

      try {
        var builder = new ConfigurationBuilder()
         .SetBasePath(AppContext.BaseDirectory)
         .AddJsonFile("appSettings.json");
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
          .ConfigureServices((hostContext, services) => {
            services.AddHostedService<Worker>();
          })
          .UseSerilog();
    }
  }
}
