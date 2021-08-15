using HueSundowner.Lib;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
      _logger = logger;
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
      var location = configuration.GetSection("Location").Get<Location>();
      var schedule = configuration.GetSection("HueSchedule").Get<HueSchedule>();
      var sundownerJob = new SundownerJob(hueSettings, location, schedule);
      while(!stoppingToken.IsCancellationRequested) {
        await sundownerJob.Execute(httpClient);
        await Task.Delay(schedule.CheckFrequency_m * 60000, stoppingToken);
      }
    }
    private readonly ILogger<Worker> _logger;
    private HttpClient httpClient;
    IConfiguration configuration;
  }
}
