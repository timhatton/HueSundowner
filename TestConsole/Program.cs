using HueSundowner.Lib;
using Microsoft.Extensions.Configuration;
using Quartz;
using Quartz.Impl;
using Serilog;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TestConsole {
  class Program {
    static async Task Main(string[] args) {
      Log.Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .CreateLogger();
      var builder = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile("appSettings.json");
      var configuration = builder.Build();
      var hueSettings = configuration.GetSection("HueSettings").Get<HueSettings>();
      var location = configuration.GetSection("Location").Get<Location>();
      var schedule = configuration.GetSection("HueSchedule").Get<HueSchedule>();
      //var hue = new HueController(hueSettings);
      //var sunsetService = new SunsetWebService(location);
      //var job = new SundownerJob(hue, sunsetService, new Clock(), schedule);
      //await job.Execute(null);      

      //StdSchedulerFactory factory = new StdSchedulerFactory();
      //IScheduler scheduler = await factory.GetScheduler();
      //IJobDetail job = JobBuilder.Create<SundownerJob>()
      //  .WithIdentity("SundownerJob")
      //  .UsingJobData("Location.Longitude", location.Longitude)
      //  .UsingJobData("Location.Latitude", location.Latitude)
      //  .UsingJobData("HueSettings.AppKey", hueSettings.AppKey)
      //  .UsingJobData("HueSettings.IpAddress", hueSettings.IpAddress)
      //  .UsingJobData("HueSchedule.DayOfWeekFilter", schedule.DayOfWeekFilter)
      //  .UsingJobData("HueSchedule.SunsetOffsetOff_m", schedule.SunsetOffsetOff_m)
      //  .UsingJobData("HueSchedule.SunsetOffsetOn_m", schedule.SunsetOffsetOn_m)
      //  .Build();
   

      //ITrigger trigger = TriggerBuilder.Create()
      //  .WithIdentity("SimpleTrigger")
      //  .StartNow()
      //  .WithSimpleSchedule(x => x
      //      .WithIntervalInMinutes(schedule.CheckFrequency_m)
      //      .RepeatForever())
      //  .Build();

      //await scheduler.ScheduleJob(job, trigger);
      //await scheduler.Start();

      //Console.ReadLine();

      //await scheduler.Shutdown();
      //Log.CloseAndFlush();
    }    
  }
}
