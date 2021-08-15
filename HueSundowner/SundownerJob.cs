using Serilog;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace HueSundowner.Lib {
  public class SundownerJob { //: IJob {
    public SundownerJob(HueSettings hueSettings,  
                        Location location,
                        HueSchedule schedule) {
      this.hueSettings = hueSettings;
      this.location = location;
      this.clock = new Clock(); 
      this.schedule = schedule;
    }

    public async Task Execute(HttpClient httpClient) {
      try {
        var sunsetWebService = new SunsetWebService(httpClient, location.Latitude, location.Longitude);
        var hue = new HueController(hueSettings.IpAddress, hueSettings.AppKey);
        var nowUtc = clock.GetNow();

        if(!(sunsetTodayLocal.HasValue && sunsetTodayLocal.Value.Date == nowUtc.Date)) {
          sunsetTodayLocal = await sunsetWebService.GetSundownTimeAsLocal(nowUtc);
        }
        var offtime = GetOfftime(nowUtc.ToLocalTime(), schedule);
        if(nowUtc.ToLocalTime().IsAm()) {
          // If it is AM use the previous day's sunset time
          if(!(sunsetYesterdayLocal.HasValue && sunsetYesterdayLocal.Value.Date == nowUtc.Date.AddDays(1))) {
            sunsetYesterdayLocal = await sunsetWebService.GetSundownTimeAsLocal(nowUtc.Date.AddDays(1));
          }
          await Control(hue, nowUtc.ToLocalTime(), sunsetYesterdayLocal.Value.ToLocalTime(), schedule.DayOfWeekFilter, schedule.SunsetOffsetOn_m, offtime);
        }
        else {
          await Control(hue, nowUtc.ToLocalTime(), sunsetTodayLocal.Value.ToLocalTime(), schedule.DayOfWeekFilter, schedule.SunsetOffsetOn_m, offtime);
        }
      }
      catch(Exception ex) {
        logger.Error(ex, "Error executing SundownerJob");
      }
    }

    private DateTime GetOfftime(DateTime now, HueSchedule schedule) {
      var dayOfWeek = now.DayOfWeek.ToString();
      TimeOfDay todOff;
      if(schedule.TimeOfDayOff.Any(x => string.Compare(x.DayOfWeek, dayOfWeek, true) == 0)) {
        todOff = new TimeOfDay(schedule.TimeOfDayOff.First(x => string.Compare(x.DayOfWeek, dayOfWeek, true) == 0).TimeOfDay);
      }
      else if(schedule.TimeOfDayOff.Any(x => string.Compare(x.DayOfWeek, "Default", true) == 0)) {
        todOff = new TimeOfDay(schedule.TimeOfDayOff.First(x => string.Compare(x.DayOfWeek, "Default", true) == 0).TimeOfDay);
      }
      else {
        todOff =  new TimeOfDay(now.Date.AddDays(1)); // Midnight by default
      }
     
      if(todOff.Hour < 12) {
        // assume tomorrow
        return now.Date.AddDays(1).AddHours(todOff.Hour).AddMinutes(todOff.Minute);
      }
      else {
        // assume today
        return now.Date.AddHours(todOff.Hour).AddMinutes(todOff.Minute);
      }
    }
    // Operates on local time
    private async Task Control(IHueController hue, DateTime now, DateTime sunset, 
      string dayOfWeeFilter, int sunsetOffsetOn_m, DateTime offtime) {
      var turnOn =  IsDayAllowed(dayOfWeeFilter, now) &&
                           now >= sunset.AddMinutes(sunsetOffsetOn_m) &&
                           now < offtime;
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
    HueSchedule schedule;
    HueSettings hueSettings;
    Location location;
    static DateTime? sunsetYesterdayLocal;
    static DateTime? sunsetTodayLocal;
    private static readonly ILogger logger = Log.Logger.ForContext<SundownerJob>();
  }
}
