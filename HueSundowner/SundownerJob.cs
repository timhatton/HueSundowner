using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HueSundowner.Lib {
  [DisallowConcurrentExecution]
  public class SundownerJob: IJob {
    //public SundownerJob(IHueController hue, ISunsetWebService sunsetWebService, IClock clock, HueSchedule schedule) {
    //  this.hue = hue;
    //  this.sunsetWebService = sunsetWebService;
    //  this.clock = clock;
    //  this.schedule = schedule;
    //}
    public SundownerJob() {
      clock = new Clock();
    }
    public async Task Execute(IJobExecutionContext context) {
      JobDataMap dataMap = context.JobDetail.JobDataMap;

      var longitude = dataMap.GetFloat("Location.Longitude");
      var latitude = dataMap.GetFloat("Location.Latitude");
      var hueIpAddress = dataMap.GetString("HueSettings.IpAddress");
      var hueAppKey = dataMap.GetString("HueSettings.AppKey");
      var dayOfWeeFilter = dataMap.GetString("HueSchedule.DayOfWeekFilter");
      var sunsetOffsetOn_m = dataMap.GetInt("HueSchedule.SunsetOffsetOn_m");
      var sunsetOffsetOff_m = dataMap.GetInt("HueSchedule.SunsetOffsetOff_m");
      var hue = new HueController(hueIpAddress, hueAppKey);
      ISunsetWebService sunsetWebService = new SunsetWebService(latitude, longitude);
      var now = clock.GetNow();
      if(!(sunsetToday.HasValue && sunsetToday.Value.Date == now.Date)) {
        sunsetToday = await sunsetWebService.GetSundownTime(now);
      }
      if(now.IsAm()) {
        // If it is AM use the previous day's sunset time
        if(!(sunsetYesterday.HasValue && sunsetYesterday.Value.Date == now.Date.AddDays(1))) { 
          sunsetYesterday = await sunsetWebService.GetSundownTime(now.Date.AddDays(1));
        }
        await Control(hue, now, sunsetYesterday.Value, dayOfWeeFilter, sunsetOffsetOn_m, sunsetOffsetOff_m);
      }
      else {
        await Control(hue, now, sunsetToday.Value, dayOfWeeFilter, sunsetOffsetOn_m, sunsetOffsetOff_m);
      }
    }

    private async Task Control(IHueController hue, DateTime now, DateTime sunset, 
      string dayOfWeeFilter, int sunsetOffsetOn_m, int sunsetOffsetOff_m) {
      var turnOn =  IsDayAllowed(dayOfWeeFilter, now) &&
                           now >= sunset.AddMinutes(sunsetOffsetOn_m) &&
                           now < sunset.AddMinutes(sunsetOffsetOff_m);
      if(turnOn) {
        await hue.On();
      }
      else {
        await hue.Off();
      }
    }


    public static bool IsDayAllowed(string dayOfWeekFilter, DateTime date) {
      if(string.IsNullOrEmpty(dayOfWeekFilter) || dayOfWeekFilter.Length != 7)
        return false;

      switch(date.DayOfWeek) {
        case DayOfWeek.Monday: return Char.ToUpper(dayOfWeekFilter[0]) == 'M';
        case DayOfWeek.Tuesday: return Char.ToUpper(dayOfWeekFilter[1]) == 'T';
        case DayOfWeek.Wednesday: return Char.ToUpper(dayOfWeekFilter[2]) == 'W';
        case DayOfWeek.Thursday: return Char.ToUpper(dayOfWeekFilter[3]) == 'T';
        case DayOfWeek.Friday: return Char.ToUpper(dayOfWeekFilter[4]) == 'F';
        case DayOfWeek.Saturday: return Char.ToUpper(dayOfWeekFilter[5]) == 'S';
        case DayOfWeek.Sunday: return Char.ToUpper(dayOfWeekFilter[6]) == 'S';
      }
      return false;
    }
    IClock clock;    
    static DateTime? sunsetYesterday;
    static DateTime? sunsetToday;
  }
}
