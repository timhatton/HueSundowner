using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Serilog;
using Microsoft.Extensions.Configuration;
using System.IO;
using HueSundownerService;
using HueSundowner.Lib;

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
        Log.Debug("Serilog settings: {@SerilogSettings}", serilogSettings);
        var hueSettings = configuration.GetSection("HueSettings").Get<HueSettings>();
        Log.Debug("hueSettings: {@Settings}", hueSettings);
        CreateHostBuilder(args, configuration).Build().Run();
        return;
      }
      catch(Exception ex) {
        Log.Fatal(ex, "Error starting service");
      }
      finally {
        Log.CloseAndFlush();
      }
    }

    public static IHostBuilder CreateHostBuilder(string[] args, IConfiguration configuration) {
      return Host.CreateDefaultBuilder(args)
          .UseSystemd()
          .ConfigureServices((hostContext, services) => {
            services.AddSingleton<IConfiguration>(configuration);
            services.AddHostedService<Worker>();
          })
          .UseSerilog();
    }
  }
}
