using HueSundowner.Lib;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace HueSundownDaemon {
  public class Worker: BackgroundService {

    public Worker(ILogger<Worker> logger, IConfiguration configuration) {
      this.configuration = configuration;
    }
    public override Task StartAsync(CancellationToken cancellationToken) {
      httpClient = new HttpClient();
      return base.StartAsync(cancellationToken);
    }
    public override Task StopAsync(CancellationToken cancellationToken) {
      httpClient.Dispose();
      return base.StopAsync(cancellationToken);
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
      var hueSettings = configuration.GetSection("HueSettings").Get<HueSettings>();
      logger.Debug("hueSettings: {@Settings}", hueSettings);
      var location = configuration.GetSection("Location").Get<Location>();
      logger.Debug("location: {@Location}", location);
      var schedule = configuration.GetSection("HueSchedule").Get<HueSchedule>();
      logger.Debug("schedule: {@Schedule}", schedule);
      var sundownerJob = new SundownerJob(hueSettings, location, schedule);
      while(!stoppingToken.IsCancellationRequested) {
        await sundownerJob.Execute(httpClient);
        await Task.Delay(schedule.CheckFrequency_m * 60000, stoppingToken);
      }
    }
    private HttpClient httpClient;
    IConfiguration configuration;
    private static readonly Serilog.ILogger logger = Log.Logger.ForContext<Worker>();
  }
}
