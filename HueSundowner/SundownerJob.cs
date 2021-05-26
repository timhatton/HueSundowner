using System;
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
      var sunsetWebService = new SunsetWebService(httpClient, location.Latitude, location.Longitude);
      var hue = new HueController(hueSettings.IpAddress, hueSettings.AppKey);
      var now = clock.GetNow();
      if(!(sunsetToday.HasValue && sunsetToday.Value.Date == now.Date)) {
        sunsetToday = await sunsetWebService.GetSundownTime(now);
      }
      if(now.IsAm()) {
        // If it is AM use the previous day's sunset time
        if(!(sunsetYesterday.HasValue && sunsetYesterday.Value.Date == now.Date.AddDays(1))) { 
          sunsetYesterday = await sunsetWebService.GetSundownTime(now.Date.AddDays(1));
        }
        await Control(hue, now, sunsetYesterday.Value, schedule.DayOfWeekFilter, schedule.SunsetOffsetOn_m, schedule.SunsetOffsetOff_m);
      }
      else {
        await Control(hue, now, sunsetToday.Value, schedule.DayOfWeekFilter, schedule.SunsetOffsetOn_m, schedule.SunsetOffsetOff_m);
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
    HueSchedule schedule;
    HueSettings hueSettings;
    Location location;
    static DateTime? sunsetYesterday;
    static DateTime? sunsetToday;
  }
}
